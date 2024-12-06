{
  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixpkgs-unstable";
  };

  outputs = { nixpkgs, ... }: {
    packages.aarch64-darwin.default =
      let
        pkgs = import nixpkgs { system = "aarch64-darwin"; };
      in
      pkgs.buildDotnetModule {
        pname = "aoc-2024";
        version = "0.0.1";
        dotnet-sdk = pkgs.dotnetCorePackages.sdk_9_0;
        dotnet-runtime = pkgs.dotnetCorePackages.runtime_9_0;
        src = ./.;
        nugetDeps = ./deps.nix;
      };

      formatter.aarch64-darwin = nixpkgs.legacyPackages.aarch64-darwin.nixpkgs-fmt;

      devShells.aarch64-darwin.default = let
        pkgs = import nixpkgs { system = "aarch64-darwin"; };
      in
      pkgs.mkShell {
        buildInputs = with pkgs; [
          dotnetCorePackages.sdk_9_0
          fantomas
          nuget-to-nix
        ];
      };
  };
}
