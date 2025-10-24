{
  inputs = {
    self.submodules = true;
    nixpkgs.url = "github:NixOS/nixpkgs";
    flake-utils.url = "github:numtide/flake-utils";
    nuget-packageslock2nix = {
      url = "github:mdarocha/nuget-packageslock2nix";
      inputs.nixpkgs.follows = "nixpkgs";
    };
  };

  outputs = { self, nixpkgs, flake-utils, nuget-packageslock2nix, ... }:
    flake-utils.lib.eachDefaultSystem (system:
      let
        project = "RuTakingTooLong";
        pkgs = import nixpkgs { inherit system; };
      in {
        packages.default = pkgs.buildDotnetModule {
          pname = project;
          version = "1.0.0";
          src = ./.;

          nativeBuildInputs = with pkgs; [ swig zig_0_14 gnumake ];

          preConfigure = ''
            make native
          '';

          projectFile = "${project}/${project}.csproj";

          nugetDeps = nuget-packageslock2nix.lib {
            inherit system;
            name = project;
            lockfiles = [ ./packages.lock.json ];
          };

          installPhase = ''
            make install OUTDIR=$out
          '';
        };

        apps.default = {
          type = "app";
          program = "${self.packages.${system}.default}/${project}";
        };
      });
}
