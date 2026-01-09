{
  inputs = {
    self.submodules = true;
    nixpkgs.url = "github:NixOS/nixpkgs";
    flake-utils.url = "github:numtide/flake-utils";
    nuget-packageslock2nix = {
      url = "github:mdarocha/nuget-packageslock2nix";
      inputs.nixpkgs.follows = "nixpkgs";
    };

    native = {
      url = "path:./Native";
      inputs.nixpkgs.follows = "nixpkgs";
      inputs.flake-utils.follows = "flake-utils";
    };
  };

  outputs =
    {
      self,
      nixpkgs,
      flake-utils,
      nuget-packageslock2nix,
      native,
      ...
    }:
    flake-utils.lib.eachDefaultSystem (
      system:
      let
        project = "RuTakingTooLong";
        pkgs = import nixpkgs { inherit system; };
        fragments = [
          native.devShellFragments.${system}.default
        ];
        mergedPackages = builtins.concatLists (map (f: f.packages or [ ]) fragments);
        mergedEnv = builtins.foldl' (a: b: a // (b.env or { })) { } fragments;
        mergedShellHook = builtins.concatStringsSep "\n" (map (f: f.shellHook or "") fragments);
      in
      {
        packages.default = pkgs.buildDotnetModule {
          pname = project;
          version = "1.0.0";
          src = ./.;

          nativeBuildInputs = with pkgs; [
            swig
            just
          ];

          preConfigure = ''
            just swig
          '';

          projectFile = "${project}/${project}.csproj";

          nugetDeps = nuget-packageslock2nix.lib {
            inherit system;
            name = project;
            lockfiles = [ ./packages.lock.json ];
          };

          installPhase = ''
            just install OUTDIR=$out
          '';
        };

        apps.default = {
          type = "app";
          program = "${self.packages.${system}.default}/${project}";
        };

        devShellFragments.default = {
          packages =
            self.packages.${system}.default.nativeBuildInputs
            ++ (with pkgs; [
              dotnet-sdk_9
              dotnet-ef
              roslyn-ls
              # swig

              xmlformat
              clang-tools

              # just
              just-lsp
              nushell

              podman
              hadolint

              nixd
              nixfmt
            ])
            ++ mergedPackages;
          env = mergedEnv;
          shellHook = mergedShellHook;
        };
        devShells = pkgs.mkShellNoCC self.devShellFragments.${system}.default;
      }
    );
}
