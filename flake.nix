{
  inputs = {
    self.submodules = true;
    nixpkgs.url = "github:NixOS/nixpkgs/nixpkgs-unstable";
    flake-utils.url = "github:numtide/flake-utils";
    flake-compat = {
      url = "github:NixOS/flake-compat";
      flake = false;
    };

    discord-net = {
      url = "path:./discord-net";
      inputs = {
        nixpkgs.follows = "nixpkgs";
        flake-utils.follows = "flake-utils";
      };
    };
    python-uvicorn = {
      url = "path:./python-uvicorn";
      inputs = {
        nixpkgs.follows = "nixpkgs";
        flake-utils.follows = "flake-utils";
      };
    };
  };

  outputs =
    {
      nixpkgs,
      flake-utils,
      discord-net,
      python-uvicorn,
      ...
    }:
    flake-utils.lib.eachDefaultSystem (
      system:
      let
        pkgs = import nixpkgs { inherit system; };
        fragments = [
          discord-net.devShellFragments.${system}.default
          python-uvicorn.devShellFragments.${system}.default
        ];
        mergedPackages = builtins.concatLists (map (f: f.packages or [ ]) fragments);
        mergedEnv = builtins.foldl' (a: b: a // (b.env or { })) { } fragments;
        mergedShellHook = builtins.concatStringsSep "\n" (map (f: f.shellHook or "") fragments);
      in
      {
        devShells.default = pkgs.mkShellNoCC {
          packages =
            (with pkgs; [
              just
              just-lsp

              postgresql
              podman
              podman-compose

              prek
              nixd # LSP for Nix
              nixfmt
            ])
            ++ mergedPackages;
          env = mergedEnv;
          shellHook = mergedShellHook;
        };
      }
    );
}
