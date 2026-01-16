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
        packages.default = pkgs.stdenvNoCC.mkDerivation (finalAttrs: {
          pname = "libNative";
          version = "1.0";
          src = pkgs.lib.cleanSource ./..;
          zon = pkgs.callPackage ./build.zig.zon.nix {};
          nativeBuildInputs = with pkgs; [
            zig
            just
            swig
          ];
          configurePhase = ''
            just swig
          '';
          buildPhase = ''
            just ZIG_FLAGS="--system ${finalAttrs.zon}" native
          '';
          installPhase = ''
            just ZIG_FLAGS="--system ${finalAttrs.zon}" TRIPLE="${system}-gnu" OUTDIR=$out install-native
          '';
        });
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
        };
        formatter = pkgs.alejandra;
      }
    );
}
