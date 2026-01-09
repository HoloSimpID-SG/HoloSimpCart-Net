{
  inputs = {
    self.submodules = true;
    nixpkgs.url = "github:NixOS/nixpkgs";
    flake-utils.url = "github:numtide/flake-utils";
  };

  outputs =
    {
      self,
      nixpkgs,
      flake-utils,
      ...
    }:
    flake-utils.lib.eachDefaultSystem (
      system:
      let
        pkgs = import nixpkgs { inherit system; };
      in
      {
        devShellFragments.default = {
          packages = with pkgs; [
            python3
            pixi
            ruff
            basedpyright

            podman
            docker-language-server
            hadolint

            nixd
            nixfmt
          ];
        };
        devShells = pkgs.mkShellNoCC self.devShellFragments.${system}.default;
      }
    );
}
