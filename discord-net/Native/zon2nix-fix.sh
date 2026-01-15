#!/usr/bin/env bash
set -euo pipefail

if ! command -v nix-prefetch-git >/dev/null 2>&1; then
  echo "Please install nix-prefetch-git and jq" >&2
  exit 1
fi
INPUT="${1:-/dev/stdin}"

while IFS= read -r line; do
  line="${line%%#*}"         # remove anything after '#', we'll capture commit separately below
  # restore the commit fragment separately (to prefer commit if present)
  if [[ $line == *#* ]]; then
    # this branch won't run because we removed it above, but kept for clarity
    :
  fi
done < /dev/null

# Real parsing and processing
while IFS= read -r raw; do
  [[ -z "$raw" ]] && continue
  # get commit fragment if present
  commit=""
  if [[ "$raw" == *#* ]]; then
    commit="${raw##*#}"
    spec="${raw%%#*}"
  else
    spec="$raw"
  fi

  # remove leading git+
  spec="${spec#git+}"

  # split ?ref=
  tag=""
  if [[ "$spec" == *\?ref=* ]]; then
    tag="${spec#*\?ref=}"
    repourl="${spec%%\?ref=*}"
  else
    repourl="$spec"
  fi

  # normalize to .git
  repourl="${repourl%.git}.git"

  # choose rev: prefer commit then tag
  rev="${commit:-$tag}"
  if [[ -z "$rev" ]]; then
    echo "Skipping (no rev found): $raw" >&2
    continue
  fi

  echo "Prefetching $repourl rev $rev ..." >&2
  json=$(nix-prefetch-git --quiet --url "$repourl" --rev "$rev")
  sha=$(echo "$json" | jq -r .sha256)
  cat <<EOF
# fetchgit for $repourl (rev: $rev)
fetchgit {
  url = "$repourl";
  rev = "$rev";
  sha256 = "$sha";
  fetchSubmodules = false;
  deepClone = true;
}
EOF

done < "$INPUT"
