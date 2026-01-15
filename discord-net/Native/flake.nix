{
  inputs = {
    self.submodules = true;
    nixpkgs.url = "github:NixOS/nixpkgs/nixpkgs-unstable";
    flake-utils.url = "github:numtide/flake-utils";

    # zon2nix = {
    #   url = "github:jcollie/zon2nix";
    #   inputs.nixpkgs.follows = "nixpkgs";
    # };
  };

  outputs = {
    self,
    nixpkgs,
    flake-utils,
    # zon2nix,
    ...
  } @ inputs:
    flake-utils.lib.eachDefaultSystem (
      system: let
        pkgs = import nixpkgs {inherit system;};
        # a = pkgs.callPackage ./third_party/boost-zig.zon.nix {};
        # b = pkgs.callPackage ./build.zig.zon.nix {};
      in {
        packages.default = pkgs.stdenvNoCC.mkDerivation {
          buildInputs = [
            (pkgs.callPackage ./third_party/boozt-zig.zon.nix {})
            (pkgs.callPackage ./build.zig.zon.nix {})
          ];

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
            just native
            # zig build --release=fast \
            #   -Dtarget=x86_64-linux-musl --verbose \
            #   --search-prefix ${pkgs.boost.out}
          '';
          # installPhase = ''
          #   just OUTDIR=$out install-native
          # '';
        };
        devShellFragments.default = {
          packages =
            self.packages.${system}.default.nativeBuildInputs
            ++ self.packages.${system}.default.buildInputs
            ++ (with pkgs; [
              zls
              clang-tools
              zon2nix

              nixd
              alejandra
            ]);
          # ++ zon2nix;
        };
        devShells = pkgs.mkShellNoCC self.devShellFragments.${system}.default;
      }
    );
}
