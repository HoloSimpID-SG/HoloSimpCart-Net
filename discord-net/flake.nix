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

    mmor-net = {
      url = "path:./MMOR.NET/src";
      inputs = {
        nixpkgs.follows = "nixpkgs";
        flake-utils.follows = "flake-utils";
      };
    };
  };

  outputs = {
    self,
    nixpkgs,
    ...
  } @ inputs:
    inputs.flake-utils.lib.eachDefaultSystem (
      system: let
        project = "RuTakingTooLong";
        pkgs = import nixpkgs {inherit system;};
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

          # runtimeDeps = [
          #   inputs.native.packages.${system}.default
          # ];

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

          installPhase = ''
            just OUTDIR=$out FLAGS="--sc" install-dotnet
          '';
        };

        apps.default = {
          type = "app";
          program = "${self.packages.${system}.default}/${project}";
        };

        devShells.default = pkgs.mkShellNoCC {
          inputsFrom =
            (builtins.attrValues self.packages.${system})
            ++ [
              inputs.native.devShells.${system}.default
              inputs.mmor-net.devShells.${system}.default
            ];
          packages =
            [self.formatter.${system}]
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
            ]);
        };
        formatter = pkgs.alejandra;
      }
    );
}
