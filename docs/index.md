Ks Game Launcher
====

[Japanese (日本語)](index.ja.md)


## [Download here](https://github.com/anon5r/KsGameLauncher/releases)

<p align="center">
  <img src="https://raw.githubusercontent.com/anon5r/KSGameLauncher/main/docs/res/screen1.png" alt="preview" width="500">
</p>


# What is this?

This is an application to start the BEMANI for コナステ with one click.

When playing the BEMANI games for the コナステ version, you can launch the game without your browser. You can be started with one click from this tool.

## How does it work?

Using the account stored in the computer in advance, login is performed in the same way as when accessing with a browser, and even the button click on the launcher startup screen is automated.

0. Register an account via this app.
1. Select the game to start

Followings are running automatically.

2. Check the login status and login progress (if you have just started app)
3. Access the game launch screen. This is the same as clicking the desktop shortcut.
4. Do the same as clicking the "Start Game" button on the game start screen.
5. Launch the game launcher.


## What is account registration?

Register an account to log in to コナステ in advance.

You can enter your account information from within the tool.

The saved account will be stored in the dedicated area managed in the OS of your computer (check the below).



## Use of account information

Only those distributed from the specified distribution source can use the account information registered through this tool.
There is no communication other than the original purpose (starting the game).


For items distributed outside the specified schedule, the above cannot be guaranteed in consideration of the possibility of modification.



## Launch from shortcuts

You can launch the game directly from the shortcut without using this launcher app.



### 1. Enable shortcuts


Right-click the launcher icon on the task bar, and show "Options".

Click "Enable shortcut launch" in the options screen.
Confirm dialog will be displayed. Check the contents and select "Yes" if there are no problems.


![Option screen](assets/images/shortcut_option_enable.png)

Then you can quit the app.


### 2. Edit the shortcut created by game

Right-click on the shortcut file created during game installation and open its properties.

![Before change URL](assets/images/shortcut_prop1.png)

In the URL field of the Web document tab, copy the string after `game_id=`. This is called _Game ID_.

On this image, `sdvx` is that.
However, in the case of beatmania IIDX INFINITAS, the URL format different. This is where `infinitas` is _GameID_.


![After changing URL](assets/images/shortcut_prop2.png)

Remove all strings on the URL field, and type `ksgamelauncher://launch/`.
After this string, paste the _Game ID_ you just copied.

For this example, it will be `ksgamelauncher://launch/sdvx`.

Similarly, in the case of INFINITAS, type to `ksgamelauncher://launch/infinitas`.

You just press "Apply" and then "OK" to close.


### 3. Click the shortcut to launch

Double-clicking on the shortcut will instantly launch the Ks Game Launcher, but it will exit immediately.

After that, the launcher of the game will be launched.



### If you don't need shortcut launch feature

Please disable this feature from option screen.
Because this feature is update a part of registory on your computer.

You can disable it only 1 click.




# Running environment


OS: Microsoft &reg; Windows &trade; 10 Aniversary Update or more

Required runtime: Microsoft &reg; .NET Framework 4.8

If you need to install it, please install it from here.

https://docs.microsoft.com/ja-jp/dotnet/framework/install/on-windows-10

# How can I use this ?

Just run `ksgamelauncher.exe` file.

First time only, it will download JSON file from the server. Next time it will use `appinfo.json` in the same location as the program, so it will not connect to the server. But if it doesn't exist, it will be downloaded again.


# Uninstall

Delete all files.

If you enabled "Launch from shortcut" feature, please disable this before you deleted.

Because this feature is updated a part of registory in your computer.
However, there is no problem if you uninstall it without disabling it, but it will leave unnecessary settings on your computer.



## Delete settings

If you want to delete completely, please remove under following directory:

`%LOCALAPPDATA%\KsGameLauncher`

Here is stored application settings.


# Supported games

- beatmania IIDX INFINITAS
- pop'n music Lively
- NOSTALGIA (Konaste ver.)
- DanceDanceRevolution GRAND PRIX
- GITADORA (Konaste ver.)
- SOUND VOLTEX EXCEEDGEAR (Konaste ver.)
- Bomber Girl (Konaste ver.)

## About unlisted Konaste games

If the game launching method is the same as above, games not listed are also supported.
To register for the launcher, select "Add Game" from the right-click menu.
You can add it to the displayed window by dragging and dropping the icon created on the desktop after installation.


# FAQ



## Does this app send my ID and password to the any internet?

Partialy yes. because your ID and password will be sent to only legitimate service site during the login process.

Other than that, it will not be sent. All of your account information is stored only on your machine.

But if you feel uneasy, please refrain from using this launcher.

This tool is open source on Github. So if you can read the code, you can check the source code to see how it works.

You can also build, run, and check it yourself using Visual Studio.



## Where are my IDs and passwords stored?

This is used a function called "Credential Manager" in the Windows OS. your ID and password is saved there.
You can see the credential manager from the control panel.

```
Control Panel > User Accounts > Credential Manager > Windows Credentials
```

If you feel suspicious after starting to use it, you can also delete your account information directly from the above.


## App seems to be communicating at the first launch

The program doesn't have a list of games that can be launched immediately after installation, so it gets that data from the server.
The acquired data is saved in the same folder as the executable file.

Once the data is acquired, it will be saved in the computer unless it is deleted, and there will be no communication other than starting the game after that.



## What happens if I click "Sync with Server" on the options screen?

Synchronize the list in [Supported games](#supported-games) with the one published latest version on the server.

(Noted) If you have added a game yourself, it will disappear and only the ones listed.
Use it only if you have problems with your game list or if you have a new game.



## Does this tool violate the rules?


The actual judgment is left to the service operator Konami,
As for the terms, the following part of Article 9 of [PC版コナステ 利用規約](https://p.eagate.573.jp/game/eacloud/p/common/tos_pc.html) describes prohibited items. Excerpt,

```
第9条（禁止事項）
...
(4) 不正ツールを利用あるいは配布した場合。
(5) クライアントソフトを複製、改変、リバースエンジニア、逆コンパイル、逆アセンブル、再現等した場合。
(6) 本サービスを公序良俗および通常の倫理概念に反する方法、用途、目的、ならびに刑法もしくはその他の法律において禁止されている目的において使用した場合。
...
```

This tool is not intended to cheat. It is an auxiliary tool to quickly start the game from login, and it does not affect the game itself or play.


Also, we do not copy, modify, reverse engineer, decompile, or disassemble the client software.


It is not considered to be a method, use, or purpose that goes against public order and morals and ordinary ethical concepts, and is purely for the purpose of launching the game comfortably and with just one click.



## Found issues on this tool

Please report the issue from [Github Issues](https://github.com/anon5r/KsGameLauncher/issues).

This tool is UNOFFICIAL for Konami Amusement. Please DO NOT contact to Konami Amusement. 
