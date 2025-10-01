{
  description = "Development Shell with Nix if you don't know what to install";

  inputs = {
    self.submodules = true;
    nixpkgs.url = "github:NixOS/nixpkgs/nixpkgs-unstable";
    flake-utils.url = "github:numtide/flake-utils";
  };

  outputs = { self, nixpkgs, flake-utils }:
    flake-utils.lib.eachDefaultSystem (system:
      let
        pkgs = import nixpkgs { inherit system; };

        #=================
        # Podman-Docker fix
        #=================
        # There seems to have some isses with permission and daemon
        # ..with nix, if so try uncommenting code below

        #=================
        # podmanSetupScript = let
        #   registriesConf = pkgs.writeText "registries.conf" ''
        #     [registries.search]
        #     registries = [ 'docker.io' ]
        #
        #     [registries.block]
        #     registries = []
        #   '';
        # in pkgs.writeScript "podman-setup" ''
        #   #!${pkgs.runtimeShell}
        #   policy="$XDG_CONFIG_HOME/containers/policy.json"
        #   registries="$XDG_CONFIG_HOME/containers/registries.conf"
        #
        #   if ! test -f "$policy"; then
        #     install -Dm555 ${pkgs.skopeo.src}/default-policy.json "$policy"
        #   fi
        #
        #   if ! test -f "$registries"; then
        #     install -Dm555 ${registriesConf} "$registries"
        #   fi
        # '';
        #
        # dockerCompat = pkgs.runCommandNoCC "docker-podman-compat" { } ''
        #   mkdir -p $out/bin
        #   ln -s ${pkgs.podman}/bin/podman $out/bin/docker
        # '';
        #=================
        # Then add this in shellHook
        #=================
        # ${podmanSetupScript}
        # export BUILDAH_ISOLATION=chroot
        # export CGROUP_MANAGER=cgroupfs
        #=================
        # And this inside packages:
        # dockerCompat

      in {
        devShells.default = pkgs.mkShell {
          packages = [
            # dockerCompat

            pkgs.dotnet-sdk
            pkgs.dotnet-ef

            pkgs.podman
            pkgs.podman-compose
          ];

          # ${podmanSetupScript}
          shellHook = ''
          # export BUILDAH_ISOLATION=chroot
          # export CGROUP_MANAGER=cgroupfs
          echo "You are using pre-configured Nix Shell for HoloSimpCart-NET"
          '';
        };
      }
    );
}
