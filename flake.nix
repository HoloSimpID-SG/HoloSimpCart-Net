{
  inputs = {
    self.submodules = true;
    nixpkgs.url = "github:NixOS/nixpkgs/nixpkgs-unstable";
    flake-utils.url = "github:numtide/flake-utils";
    flake-compat = {
      url = "github:NixOS/flake-compat";
      flake = false;
    };

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
        packages.discord-net = inputs.discord-net.packages.${system}.default;
        devShells.default = pkgs.mkShellNoCC {
          inputsFrom =
            (builtins.attrValues self.packages.${system})
            ++ [
              inputs.discord-net.devShells.${system}.default
              # inputs.python-uvicorn.devShells.${system}.default
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
