Ks Game Launcher
====
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/anon5r/KSGameLauncher)](https://github.com/anon5r/KSGameLauncher/releases/latest) [![GitHub all releases](https://img.shields.io/github/downloads/anon5r/KSGameLauncher/total)](https://github.com/anon5r/KSGameLauncher/releases/latest)
<a href="https://github.com/anon5r/KSGameLauncher/releases"><img src="https://github.com/anon5r/ksgamelauncher-docs/raw/main/assets/images/app-logo.png" alt="KS Game Launcher" width="120" align="right"></a>

[Japanese (日本語)](README.ja-JP.md)


<p align="center">
  <a href="https://github.com/anon5r/KSGameLauncher/releases">
    <img src="https://raw.githubusercontent.com/anon5r/ksgamelauncher-docs/main/res/screen1.png" alt="drawing" width="500"/>
  </a>
</p>


# What is this?

This is an application to start the BEMANI for Konaste (コナステ) with one click.

Please check [this document](https://launcher-app.sdvx.net) for details such as operation and functions.


# Development Environment

OS: Microsoft&reg; Windows&trade; 10 or later

Required runtime: Microsoft&reg; .NET Framework 4.8


## Optional

- Docker


# How can I build this

You can build on your machine with Microsoft&reg; Visual Studio or [MSBuild](https://docs.microsoft.com/visualstudio/msbuild/msbuild?view=vs-2022) 

## NuGet

We are using some NuGet libraries.

You need to run the following command to install from NuGet.

```
nuget restore KsGameLauncher.sln
```




# For debugging

A part of features will be download the data from the server.
To simulate this, we have included a Docker image configuration file for the simple web.


## Build image

```
docker compose build
```

## Create a container, and start

```
docker compose up -d
```

Server will start on port `8080`
Connect to `http://localhost:8080`

`appinfo.json` will be put on `http://localhost:8080/conf/appinfo.json`.



## Stop the container

```
docker compose stop
```

## Remove the container

```
docker compose down
```

## Troubleshoot

If it overlaps with an existing port, change it with `services.web.ports` in` docker-compose.yml`.


# Process flow

### Start up

```mermaid
  flowchart TD;
      A([Start]) --> B{Is appinfo.json already exists?};
      B -- Yes --> E;
      B -- No  --> C[Get appinfo.json file from the server];
      C ----> D{Can I get and save it to disk?};
      D -- Yes --> E[list games in context menu];
      D -- No  --> F[No content in context menu];
      E --> G([Finish startup]);
      F --> G([Finish startup]);
```

### Launch the game

```mermaid
  flowchart TD;
      subgraph Normal flow;
        A([Choose the game]) --> B{Is the user account already set up?};
        B -- Yes --> C[Load account information];
        B -- No  --> D([Display dialog notifying 'Account settings required']);
        C --> E{Check login session\n Already login ?};
        E -- Yes --> F;
        E -- No  --> AA([Go to login flow])
        F[Send request to launche the game] --> G[Load the game launch page];
        G -- Parse page --> H[Find 'Launch the game' button];
        H --> I[Get custom URI for launch the game from the button];
        I --> J[Find installed game path from registry];
        J --> K[Run launcher.exe with custom URI parameters];
        K --> L([Finish]);
      end
      X([After login]) --> F;
```


### Login flow

```mermaid
  flowchart TD;
      Start([Login flow]) --> LoginScreen[Send request to login screen];
      LoginScreen --> ReqOTP{Required OTP ?};
      ReqOTP -- Yes --> OTP{{Display OTP dialog}};
      ReqOTP -- No  --> SendLogin[Send request with credentials];
      OTP -- Input OTP --> SendLogin[Send request with credentials];
      OTP -- Cancel --> Cancel1([Cancel process]);
      SendLogin --> IsSuccess1{Succeed login ?};
      IsSuccess1 -- Yes --> 2FARes{Require 2FA ?};
      IsSuccess1 -- No  --> LoginFail;
      2FARes -- Yes --> 2FA{{Display 2FA input dialog}};
      2FARes -- No  --> Continue([Go to launch process]);
      2FA -- Input 2FA --> IsSuccess2{Succeed login ?};
      2FA -- Cancel --> Cancel2([Cancel process]);
      IsSuccess2 -- Yes --> Continue([Continue launching process]);
      IsSuccess2 -- No  --> LoginFail;
      LoginFail([Display dialog 'Failed to login']);
```