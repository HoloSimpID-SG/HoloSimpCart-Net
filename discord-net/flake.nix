{
  inputs = {
    self.submodules = true;
    nixpkgs.url = "github:NixOS/nixpkgs/nixpkgs-unstable";
    flake-utils.url = "github:numtide/flake-utils";
    nuget-packageslock2nix = {
      url = "github:mdarocha/nuget-packageslock2nix";
      inputs.nixpkgs.follows = "nixpkgs";
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
        pkgs = import nixpkgs {inherit system;};
        project = "RuTakingTooLong";
        # container_name = pkgs.lib.toLower project;
        container_name = "discord-bot";
      in {
        packages = {
          default = pkgs.buildDotnetModule (finalAttrs: {
            pname = project;
            version = "1.0.0";
            src = pkgs.lib.cleanSource ./.;

            zonDeps = pkgs.callPackage ./Native/build.zig.zon.nix {};
            projectFile = "${project}/${project}.csproj";
            dotnet-sdk = pkgs.dotnetCorePackages.sdk_9_0;
            dotnet-runtime = pkgs.dotnetCorePackages.runtime_9_0;

            nativeBuildInputs = with pkgs; [
              zig
              swig
              just
            ];
            DOTNET_FLAGS = "--self-contained true";
            ZIG_FLAGS = "-Dtarget=${system}-gnu --system ${finalAttrs.zonDeps}";
            buildPhase = ''
              just build
            '';
            nugetDeps = inputs.nuget-packageslock2nix.lib {
              inherit system;
              name = project;
              lockfiles = [
                ./MMOR.NET/packages.lock.json
                ./${project}/packages.lock.json
              ];
            };
            installPhase = ''
              just OUTDIR=$out install
            '';
          });
          container = pkgs.dockerTools.buildLayeredImage {
            name = container_name;
            tag = "latest";
            contents = [
              pkgs.dotnetCorePackages.runtime_9_0
              self.packages.${system}.default
              pkgs.cacert
            ];
            config = {
              Env = [
                "HOME=/root"
                "SSL_CERT_FILE=${pkgs.cacert}/etc/ssl/certs/ca-bundle.crt"
              ];
              Entrypoint = [
                "${self.packages.${system}.default}/${project}"
              ];
            };
          };
        };

        apps = {
          default = {
            type = "app";
            program = "${self.packages.${system}.default}/${project}";
          };
          container = {
            type = "app";
            program = "${pkgs.writeShellScriptBin "container" ''
              podman load < $(nix build .#container --print-out-paths)
              podman run -it --rm localhost/${container_name}:latest
            ''}/bin/container";
          };
        };

        devShells.default = pkgs.mkShellNoCC {
          inputsFrom =
            (builtins.attrValues self.packages.${system})
            ++ [inputs.mmor-net.devShells.${system}.default];
          packages =
            [self.formatter.${system}]
            ++ (with pkgs; [
              dotnet-ef
              roslyn-ls
              swig

              zls
              zon2nix

              xmlformat
              clang-tools

              podman
              hadolint

              nixd
            ]);
        };
        formatter = pkgs.alejandra;
      }
    );
}
