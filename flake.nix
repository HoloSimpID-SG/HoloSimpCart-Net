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
              project.name = "holo-simp-cart";
              services = {
                database = {
                  service = {
                    image = "postgres:16";
                    environment = {
                      POSTGRES_HOST = "$POSTGRES_HOST";
                      POSTGRES_PORT = "$POSTGRES_PORT";
                      POSTGRES_USER = "$POSTGRES_USER";
                      POSTGRES_PASSWORD = "$POSTGRES_PASSWORD";
                      POSTGRES_DB = "$POSTGRES_DB";
                    };
                    # user = "root";
                    ports = ["5432:5432"];
                    command = ["-p" "5432"];
                    volumes = [
                      "db_data:/var/lib/postgresql/data"
                    ];
                    healthcheck = {
                      test = [
                        "CMD-SHELL"
                        "pg_isready -U $${POSTGRES_USER} -d $${POSTGRES_DB} -p $${POSTGRES_PORT}"
                      ];
                      interval = "5s";
                      timeout = "5s";
                      retries = 5;
                    };
                  };
                };
                discord-bot = {
                  build.image = pkgs.lib.mkForce inputs.discord-net.packages.${system}.container;
                  service = {
                    depends_on.database.condition = "service_healthy";
                    environment = {
                      DISCORD_TOKEN = "$DISCORD_TOKEN";
                      GUILD_ID = "$GUILD_ID";
                      THREAD_ID = "$THREAD_ID";

                      UVICORN_PORT = "$UVICORN_PORT";

                      POSTGRES_HOST = "$POSTGRES_HOST";
                      POSTGRES_PORT = "$POSTGRES_PORT";
                      POSTGRES_USER = "$POSTGRES_USER";
                      POSTGRES_PASSWORD = "$POSTGRES_PASSWORD";
                      POSTGRES_DB = "$POSTGRES_DB";
                    };
                    volumes = ["shared:/shared"];
                  };
                };
                python-uvicorn = {
                  build.image = pkgs.lib.mkForce inputs.python-uvicorn.packages.${system}.container;
                  service = {
                    environment = {
                      UVICORN_PORT = "$UVICORN_PORT";
                    };
                    ports = ["8000:80"];
                    volumes = ["shared:/shared"];
                  };
                };
              };
              docker-compose.volumes = {
                shared = {};
                db_data = {};
              };
            })
          ];
        };
        apps.default = {
          type = "app";
          program = "${pkgs.writeShellScriptBin "up" ''
            podman load < ${inputs.discord-net.packages.${system}.container}
            podman load < ${inputs.python-uvicorn.packages.${system}.container}
            # cd ${toString ./.}
            podman-compose --env-file .env --file ${self.packages.${system}.default} up "$@"
            # podman-compose --file ${self.packages.${system}.default} up "$@"
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
