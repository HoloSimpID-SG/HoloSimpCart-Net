{
  inputs = {
    self.submodules = true;
    nixpkgs.url = "github:NixOS/nixpkgs/nixpkgs-unstable";
    flake-utils.url = "github:numtide/flake-utils";
    flake-compat = {
      url = "github:NixOS/flake-compat";
      flake = false;
    };
    arion.url = "github:hercules-ci/arion";

    discord-net = {
      url = "path:./discord-net";
      inputs = {
        nixpkgs.follows = "nixpkgs";
        flake-utils.follows = "flake-utils";
      };
    };
    python-uvicorn = {
      url = "path:./python-uvicorn";
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
      in {
        packages.default = inputs.arion.lib.build {
          inherit pkgs;
          modules = [
            ({pkgs, ...}: {
              project.name = "HoloSimpCart-Net";
              services = {
                database = {
                  service = {
                    image = "postgres:16";
                  };
                  # service.env_file = [./.env];
                };
                discord-bot = {
                  build.image = pkgs.lib.mkForce inputs.discord-net.packages.${system}.container;
                  # service.env_file = [./.env];
                  service.depends_on = ["db" "python-uvicorn"];
                };
                python-uvicorn = {
                  build.image = pkgs.lib.mkForce inputs.python-uvicorn.packages.${system}.container;
                  # service.env_file = [./.env];
                  service.ports = ["8000:80"];
                };
              };
            })
          ];
        };
        apps.default = {
          type = "app";
          program = "${pkgs.writeShellScriptBin "up" ''
            podman load < ${inputs.discord-net.packages.${system}.container}
            podman load < ${inputs.python-uvicorn.packages.${system}.container}
            podman-compose --env-file .env --file ${self.packages.${system}.default} up "$@"
          ''}/bin/up";
        };
        devShells.default = pkgs.mkShellNoCC {
          inputsFrom =
            (builtins.attrValues self.packages.${system})
            ++ [
              inputs.discord-net.devShells.${system}.default
              inputs.python-uvicorn.devShells.${system}.default
            ];
          packages =
            [self.formatter.${system}]
            ++ (with pkgs; [
              just
              just-lsp

              postgresql
              podman
              podman-compose

              prek
              nixd
            ]);
        };
        formatter = pkgs.alejandra;
      }
    );
}
