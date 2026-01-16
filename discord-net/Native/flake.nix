{
  inputs = {
    self.submodules = true;
    nixpkgs.url = "github:NixOS/nixpkgs/nixpkgs-unstable";
    flake-utils.url = "github:numtide/flake-utils";
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
        packages.default = pkgs.stdenvNoCC.mkDerivation {
          deps = pkgs.callPackage ./build.zig.zon.nix {};

          pname = "libNative";
          version = "1.0";
          src = pkgs.lib.cleanSource ./..;
          nativeBuildInputs = with pkgs; [
            just
            swig
            zig
          ];
          zigPreferMusl = true;
          # buildInputs =
          #   (pkgs.callPackage ./build.zig.zon.nix {})
          #   ++ (with pkgs; [
          #     boost
          #   ]);
          # postPatch = ''
          #   ln -s ${pkgs.callPackage ./build.zig.zon.nix {}} $ZIG_GLOBAL_CACHE_DIR/p
          # '';
          buildPhase = ''
            # just native
            zig build --release=fast \
              -Dtarget=x86_64-linux-musl --verbose \
              --search-prefix ${pkgs.boost.out}
          '';
          # installPhase = ''
          #   just OUTDIR=$out install-native
          # '';
        };
        devShells.default = pkgs.mkShellNoCC {
          inputsFrom = builtins.attrValues self.packages.${system};
          packages =
            [self.formatter.${system}]
            ++ (with pkgs; [
              zls
              clang-tools
              zon2nix

              nix-prefetch-git
              jq

              nixd
            ]);
          # ++ zon2nix;
        };
        formatter = pkgs.alejandra;
      }
    );
}
