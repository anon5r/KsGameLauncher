Ks Game Launcher
====

[Japanese (日本語)](index.ja.md)



<p align="center">
  <img src="https://raw.githubusercontent.com/anon5r/KSGameLauncher/main/docs/res/screen1.png" alt="preview" width="500">
</p>


# What is this?

This is an application to start the BEMANI for コナステ with one click.


## How does it work?

When playing the BEMANI games for the コナステ version, you can launch the game without your browser. You can be started with one click from this tool.


## What is account registration?

Register an account to log in to コナステ in advance.

You can enter your account information from within the tool.

The saved account will be stored in the dedicated area managed in the OS of your computer (check the below).

```
Control Panel > User Accounts > Credential Manager > Windows Credentials
```


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

In the URL field of the Web document tab, copy the string after `game_id=`.
On this image, `sdvx` is that.


![After changing URL](assets/images/shortcut_prop2.png)

Remove all strings on the URL field, and type `ksgamelauncher://launch/`.
After this string, paste the string you just copied.

For this example, it will be `ksgamelauncher://launch/sdvx`.


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

## Delete settings

If you want to delete completely, please remove under following directory:

`%LOCALAPPDATA%\KsGameLauncher`

Here is stored application settings.



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

If you feel suspicious after starting to use it, you can also delete your account information directly from the above.




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
