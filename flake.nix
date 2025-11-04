{
  description = "Development Shell with Nix if you don't know what to install";

  inputs = {
    self.submodules = true;
    nixpkgs.url = "github:NixOS/nixpkgs/nixpkgs-unstable";
    flake-utils.url = "github:numtide/flake-utils";
  };

  outputs = { self, nixpkgs, flake-utils, ... }:
    flake-utils.lib.eachDefaultSystem (system:
      let
        pkgs = import nixpkgs { inherit system; };
      in {
        devShells.default = pkgs.mkShell {
          packages = with pkgs; [
            # C#
            dotnet-sdk_10
            dotnet-ef
            roslyn-ls

            # Zig + C++
            zig
            swig
            zls
            clang-tools

            python3
            ruff
            basedpyright
            # Manim Requirements
            cairo
            pango
            gobject-introspection

            gnumake
            podman
            podman-compose

            nixd # LSP for Nix
            nixfmt
          ];

          shellHook = ''
          echo "You are using pre-configured Nix Shell for HoloSimpCart-NET"
          '';
        };
      }
    );
}
