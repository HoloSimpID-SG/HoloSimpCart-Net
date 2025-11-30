{
  description = "Development Shell with Nix if you don't know what to install";

  inputs = {
    self.submodules = true;
    nixpkgs.url = "github:NixOS/nixpkgs/nixpkgs-unstable";
    flake-utils.url = "github:numtide/flake-utils";
  };

  outputs =
    {
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
        devShells.default = pkgs.mkShell {
          packages = with pkgs; [
            # C#
            dotnet-sdk_9
            dotnet-ef
            roslyn-ls
            xmlformat
            # Zig + C++
            zig
            swig
            zls
            clang-tools
            # Python
            python3
            ruff
            basedpyright
            # Others
            just
            nushell
            postgresql
            podman
            podman-compose
            # flake management
            nixd # LSP for Nix
            nixfmt
          ];
        };
      }
    );
}
