{
  inputs = { nixpkgs.url = "github:NixOS/nixpkgs/nixpkgs-unstable"; };

  outputs = { self, nixpkgs }:
    let
      projectName = "HoloSimpCart-NET";
      system = "x86_64-linux";
      pkgs = import nixpkgs { inherit system; };

      podmanSetupScript = let
        registriesConf = pkgs.writeText "registries.conf" ''
          [registries.search]
          registries = [ 'docker.io' ]

          [registries.block]
          registries = []
        '';
      in pkgs.writeScript "podman-setup" ''
        #!${pkgs.runtimeShell}
        policy="$XDG_CONFIG_HOME/containers/policy.json"
        registries="$XDG_CONFIG_HOME/containers/registries.conf"

        if ! test -f "$policy"; then
          install -Dm555 ${pkgs.skopeo.src}/default-policy.json "$policy"
        fi

        if ! test -f "$registries"; then
          install -Dm555 ${registriesConf} "$registries"
        fi
      '';

      dockerCompat = pkgs.runCommandNoCC "docker-podman-compat" { } ''
        mkdir -p $out/bin
        ln -s ${pkgs.podman}/bin/podman $out/bin/docker
      '';
    in {
      devShells.${system}.default = pkgs.mkShell {
        buildInputs = [
          dockerCompat

          pkgs.dotnet-sdk
          pkgs.dotnet-ef

          pkgs.podman
          pkgs.podman-compose

          pkgs.nushell
          # pkgs.fuse-overlayfs
        ];

        shellHook = ''
          ${podmanSetupScript}
          export BUILDAH_ISOLATION=chroot
          # export CGROUP_MANAGER=cgroupfs
          echo "You are using pre-configured Nix Shell for HoloSimpCart-NET"
        '';
      };
    };
}
