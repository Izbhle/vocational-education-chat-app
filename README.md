# vocational-education-chat-app

Demonstration of a network Chat application build in C#.
Uses Avalonia for a GUI.

## Runtime
The project uses `.NET 7`
To initialize the project run

```sh
dotnet restore
dotnet tool restore
```

## Dev setup
This repository is set up to to work on a Linux system using the flatpak version of VSCode.
Editor config is included in the repository.

The `terminal.integrated.*` options in `.vscode/settings.json` need to be removed on other setups of VSCode.

The flatpak version needs sdks for `mono6` and `dotnet7` to be installed.

## Integration Tests

Tests for this project include automated integration tests to verify network functionality.
Those tests need access to the following ports: `1240-1250`

## Test Coverage
Generate a test coverage report by running the script.
```sh
. skripts/create-coveragereport.sh
```
The report is saved as a website at `coveragereport/index.html`

## Diagrams
The included diagrams are made using `draw.io`, a plugin for editing them in vscode is in the extension recommendations.