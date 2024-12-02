{
  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixpkgs-unstable";
    nuget-packageslock2nix = {
      url = "github:mdarocha/nuget-packageslock2nix/main";
      inputs.nixpkgs.follows = "nixpkgs";
    };
  };

  outputs = { nixpkgs, nuget-packageslock2nix, ... }: {
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
        nugetDeps = nuget-packageslock2nix.lib {
          system = "aarch64-darwin";
          name = "aoc-2024";
          lockfiles = [
            ./packages.lock.json
          ];
        };
      };

      formatter.aarch64-darwin = nixpkgs.legacyPackages.aarch64-darwin.nixpkgs-fmt;

      devShells.aarch64-darwin.default = let
        pkgs = import nixpkgs { system = "aarch64-darwin"; };
      in
      pkgs.mkShell {
        buildInputs = with pkgs; [
          dotnetCorePackages.sdk_9_0
          fantomas
        ];
      };
  };
}
