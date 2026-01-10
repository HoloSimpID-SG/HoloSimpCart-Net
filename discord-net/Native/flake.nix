{
  inputs = {
    self.submodules = true;
    nixpkgs.url = "github:NixOS/nixpkgs-unstable";
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
            zig
            zls
            clang-tools

            nixd
            alejandra
          ];
        };
        devShells = pkgs.mkShellNoCC self.devShellFragments.${system}.default;
      }
    );
}
