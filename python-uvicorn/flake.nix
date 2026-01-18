{
  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixpkgs-unstable";
    flake-utils.url = "github:numtide/flake-utils";

    # pyproject-nix = {
    #   url = "github:pyproject-nix/pyproject.nix";
    #   inputs.nixpkgs.follows = "nixpkgs";
    # };
  };

  outputs = {
    self,
    nixpkgs,
    ...
  } @ inputs:
    inputs.flake-utils.lib.eachDefaultSystem (
      system: let
        project = "python-uvicorn";
        pkgs = import nixpkgs {inherit system;};
      in {
        packages.container = let
          src = pkgs.runCommand "app-src" {} ''
            mkdir -p $out/python-uvicorn
            cp -r ${pkgs.lib.cleanSource ./.}/* $out/python-uvicorn/
          '';
        in
          pkgs.dockerTools.buildLayeredImage {
            name = project;
            tag = "latest";
            fromImage = pkgs.dockerTools.pullImage {
              imageName = "ghcr.io/prefix-dev/pixi";
              imageDigest = "sha256:9f65c4eb0bf199ffdbb68aadb15632481b0e88c526580a651dce495285d36004";
              hash = "sha256-KigSId31Lx9nngvpD5xHUbrTTjGM4FiwFzfTHGF5Llw=";
              finalImageName = "ghcr.io/prefix-dev/pixi";
              finalImageTag = "latest";
            };
            contents = [src];
            config = {
              Cmd = ["pixi" "run" "run"];
              WorkingDir = "/python-uvicorn";
              Env = [
                "UVICORN_PORT=8000"
                "PATH/python-uvicorn/.pixi/envs/default/bin:$PATH"
              ];
              ExposedPorts = {
                "8000/tcp" = {};
              };
            };
          };
        apps = {
          default = {
            type = "app";
            program = "${pkgs.writeShellScriptBin "container" ''
              podman load < $(nix build --print-out-paths)
              podman run -it --rm localhost/${pkgs.lib.toLower project}:latest
            ''}/bin/container";
          };
        };
        devShells.default = pkgs.mkShellNoCC {
          inputsFrom = builtins.attrValues self.packages.${system};
          packages =
            [self.formatter.${system}]
            ++ (with pkgs; [
              pixi
              ruff
              basedpyright
              tombi

              podman
              hadolint

              nixd
              alejandra
            ]);
        };
        formatter = pkgs.alejandra;
      }
    );
}
