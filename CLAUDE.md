# CLAUDE.md

Guidance for Claude Code when working in this repository.

## Commit and PR conventions

Do **not** add any attribution lines. No `Co-Authored-By:` trailer on commits, and no
"Generated with Claude Code" line in pull request descriptions. This overrides the
default Claude Code attribution behaviour.

Work through branches and pull requests. Do not push directly to `main`.

## Which project is live

| Path | Status |
| --- | --- |
| `src/KsGameLauncher2` | **The application.** .NET 10 (`net10.0-windows`), WinForms |
| `src/KsGameLauncher2.Tests` | xUnit test project |
| `KsGameLauncher` | **Deprecated.** The original .NET Framework 4.8 version, kept for reference only |

The deprecated project no longer builds under the current SDK and is excluded from the
solution's default build configuration. Make changes in `src/KsGameLauncher2`; leave
`KsGameLauncher` alone.

## Commands

```
dotnet build KsGameLauncher.sln
dotnet test src/KsGameLauncher2.Tests/KsGameLauncher2.Tests.csproj
dotnet publish src/KsGameLauncher2/KsGameLauncher2.csproj -c Release -r win-x64 --self-contained false
```

Packages restore as part of the build. There is no `nuget restore` step.

Publishing produces a single-file executable; the options live in `KsGameLauncher2.csproj`,
but `-r win-x64` has to be passed on the command line. Releases are automated — pushing a
`v*` tag builds the artifact and attaches it to a GitHub release. See the README for details.

## Testing constraints

Tests stop short of the real Konaste service. Launching a game requires an active Konaste
subscription and goes through a private OAuth flow, so the login and launch paths cannot be
exercised automatically.

- Do not call into `Launcher.StartApp` / `Login` / `LaunchGames` from tests. They show
  `MessageBox` dialogs, which hang the test runner.
- WinForms tests need an STA thread — use `[StaFact]` from `Xunit.StaFact`, not `[Fact]`.
- Assume no Konaste game is installed. `GameRegistry.IsInstalled` returning `false` is the
  normal case on a development machine, and tests should be written against that.

## Things that are easy to get wrong

- `appinfo.json` must be resolved through `AppUtil.GetAppInfoLocalPath()`. The
  `appInfoLocal` setting is a bare file name, so passing it straight to a `File` API
  resolves it against the current working directory. The app is also started by the OS as a
  custom URI handler (`konaste.*://launch/...`) with an arbitrary working directory.
- `Microsoft.Web.WebView2` looks unused but must stay — it is planned for the WebView2-based
  login rewrite (issue #29).
- `Properties/Settings.Designer.cs` is regenerated on build. Do not hand-edit it to resolve
  a clash with the hand-written `Properties/Settings.cs` partial class; remove the entry from
  `Settings.settings` instead, the way `Cookies` is handled.
