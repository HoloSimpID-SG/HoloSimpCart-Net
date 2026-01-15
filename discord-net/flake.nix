{
  inputs = {
    self.submodules = true;
    nixpkgs.url = "github:NixOS/nixpkgs/nixpkgs-unstable";
    flake-utils.url = "github:numtide/flake-utils";
    nuget-packageslock2nix = {
      url = "github:mdarocha/nuget-packageslock2nix";
      inputs.nixpkgs.follows = "nixpkgs";
    };

    native = {
      url = "path:./Native";
      inputs = {
        nixpkgs.follows = "nixpkgs";
        flake-utils.follows = "flake-utils";
      };
    };

    # mmor-net = {
    #   url = "path:./MMOR.NET/src";
    #   inputs = {
    #     nixpkgs.follows = "nixpkgs";
    #     flake-utils.follows = "flake-utils";
    #   };
    # };
  };

  outputs = {
    self,
    nixpkgs,
    flake-utils,
    # nuget-packageslock2nix,
    # native,
    # mmor-net,
    ...
  } @ inputs:
    flake-utils.lib.eachDefaultSystem (
      system: let
        project = "RuTakingTooLong";
        pkgs = import nixpkgs {inherit system;};
        fragments = [
          inputs.native.devShellFragments.${system}.default
          # inputs.mmor-net.devShellFragments.${system}.default
        ];
        mergedPackages = builtins.concatLists (map (f: f.packages or []) fragments);
        mergedEnv = builtins.foldl' (a: b: a // (b.env or {})) {} fragments;
        mergedShellHook = builtins.concatStringsSep "\n" (map (f: f.shellHook or "") fragments);
      in {
        packages.default = pkgs.buildDotnetModule {
          pname = project;
          version = "1.0.0";
          src = nixpkgs.lib.cleanSource ./.;

          dotnet-sdk = pkgs.dotnetCorePackages.sdk_9_0;
          dotnet-runtime = pkgs.dotnetCorePackages.runtime_9_0;

          nativeBuildInputs = with pkgs; [
            swig
            just
          ];

          runtimeDeps = [
            inputs.native.packages.${system}.default
          ];

          buildPhase = ''
            just FLAGS="--sc" build
          '';

          projectFile = "${project}/${project}.csproj";

          nugetDeps = inputs.nuget-packageslock2nix.lib {
            inherit system;
            name = project;
            lockfiles = [
              ./MMOR.NET/packages.lock.json
              ./${project}/packages.lock.json
            ];
          };

          installphase = ''
            just outdir=$out flags="--sc" install-dotnet
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
              dotnet-ef
              roslyn-ls
              swig

              xmlformat
              clang-tools

              just-lsp

              podman
              hadolint

              nixd
              alejandra
            ])
            ++ mergedPackages;
          env = mergedEnv;
          shellHook = mergedShellHook;
        };
        devShells = pkgs.mkShellNoCC self.devShellFragments.${system}.default;
      }
    );
}
