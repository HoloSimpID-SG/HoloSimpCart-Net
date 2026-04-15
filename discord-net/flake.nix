{
  inputs = {
    self.submodules = true;
    nixpkgs.url = "github:NixOS/nixpkgs";
    flake-utils.url = "github:numtide/flake-utils";
    nuget-packageslock2nix = {
      url = "github:mdarocha/nuget-packageslock2nix";
      inputs.nixpkgs.follows = "nixpkgs";
    };

    mmor-net = {
      url = "path:./MMOR.NET";
      inputs = {
        nixpkgs.follows = "nixpkgs";
        flake-utils.follows = "flake-utils";
      };
    };

    zig = {
      url = "github:mitchellh/zig-overlay";
      inputs.nixpkgs.follows = "nixpkgs";
    };

    zon2nix = {
      url = "github:jcollie/zon2nix";
      inputs.nixpkgs.follows = "nixpkgs";
    };
  };

  outputs = {self, ...} @ inputs:
    inputs.flake-utils.lib.eachDefaultSystem (system: let
      project = "RuTakingTooLong";
      pkgs = import inputs.nixpkgs {
        inherit system;
        overlays = [
          inputs.zig.overlays.default
        ];
      };
    in {
      packages.default = pkgs.buildDotnetModule (finalAttrs: {
        pname = project;
        version = "1.0.0";
        src = pkgs.lib.cleanSource ./.;

        zonDeps = pkgs.callPackage ./Native/build.zig.zon.nix {};
        dotnet-sdk = pkgs.dotnetCorePackages.sdk_9_0;
        dotnet-runtime = pkgs.dotnetCorePackages.runtime_9_0;

        ZigFlags = "--system ${finalAttrs.zonDeps}";
        TargetFramework = "net9.0";

        nativeBuildInputs = with pkgs; [
          zig
          swig
        ];

        buildPhase = ''
          dotnet build RuTakingTooLong \
            --configuration Release
        '';

        nugetDeps = inputs.nuget-packageslock2nix.lib {
          inherit system;
          name = project;
          lockfiles = [
            ./MMOR.NET/packages.lock.json
            ./RuTakingTooLong/packages.lock.json
          ];
        };

        installPhase = ''
          dotnet publish RuTakingTooLong --no-build --output "$out"
        '';
      });

      apps.default = {
        type = "app";
        program = "${self.packages.${system}.default}/${project}";
      };

      devShells.default = pkgs.mkShellNoCC {
        inputsFrom =
          (builtins.attrValues self.packages.${system})
          ++ [inputs.mmor-net.devShells.${system}.default];
        packages =
          [inputs.zon2nix.packages.${system}.zon2nix]
          ++ (with pkgs; [
            dotnet-ef
            roslyn-ls

            zls
            clang-tools

            lemminx
            xmlformat
          ]);
      };
    });
}
