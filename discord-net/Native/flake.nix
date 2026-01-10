{
  inputs = {
    self.submodules = true;
    nixpkgs.url = "github:NixOS/nixpkgs/nixpkgs-unstable";
    flake-utils.url = "github:numtide/flake-utils";

    zig = {
      url = "github:mitchellh/zig-overlay";
      inputs = {
        nixpkgs.follows = "nixpkgs";
        flake-utils.follows = "flake-utils";
        flake-compat.follows = "flake-compat";
      };
    };
    zon2nix = {
      url = "github:jcollie/zon2nix?rev=bf983aa90ff169372b9fa8c02e57ea75e0b42245";
      inputs = {
        nixpkgs.follows = "nixpkgs";
      };
    };
  };

  outputs = {
    self,
    nixpkgs,
    flake-utils,
    zig,
    zon2nix,
    ...
  }:
    flake-utils.lib.eachDefaultSystem (
      system: let
        pkgs = import nixpkgs {inherit system;};
      in {
        packages.default = pkgs.stdenv.mkDerivation {
          pname = "libNative";
          version = "1.0";
          src = ./.;
          postPatch = ''
            ln -s ${pkgs.callPackage ./build.zig.zon.nix {}} $ZIG_GLOBAL_CACHE_DIR/p
          '';
          buildPhase = ''
            just native
          '';
          installPhase = ''
            just install-native
          '';
        };
        devShellFragments.default = {
          packages =
            (with pkgs; [
              zig
              zls
              clang-tools

              nixd
              alejandra
            ])
            ++ zon2nix.packages.${system}.zon2nix;
        };
        devShells = pkgs.mkShellNoCC self.devShellFragments.${system}.default;
      }
    );
}
