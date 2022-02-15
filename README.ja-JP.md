Ks Game Launcher
====
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/anon5r/KSGameLauncher)](https://github.com/anon5r/KSGameLauncher/releases/latest) [![GitHub all releases](https://img.shields.io/github/downloads/anon5r/KSGameLauncher/total)](https://github.com/anon5r/KSGameLauncher/releases/latest)
<a href="https://github.com/anon5r/KSGameLauncher/releases"><img src="https://github.com/anon5r/ksgamelauncher-docs/raw/main/assets/images/app-logo.png" alt="KS Game Launcher" width="120" align="right"></a>

[English](README.md)


<p align="center">
  <a href="https://github.com/anon5r/KSGameLauncher/releases">
    <img src="https://raw.githubusercontent.com/anon5r/ksgamelauncher-docs/main/res/screen1.png" alt="drawing" width="500"/>
  </a>
</p>


# これは何？

コナステのゲームをワンクリックで起動するためのアプリケーションです。

動作、機能などの詳細は[こちらのドキュメント](https://launcher-app.sdvx.net/index.ja.html)を確認してください。


## 処理フロー

### 起動

```mermaid
  flowchart TD;
      A[開始] --> B{appinfo.jsonが存在するか};
      B -- ある --> E;
      B -- ない  --> C[サーバーからappinfo.jsonを取得];
      C ----> D{ファイルを取得/保存できたか};
      D -- Yes --> E[コンテキストメニューに\nゲーム一覧を表示];
      D -- No  --> F[コンテキストメニューに何も表示しない];
      E --> G[起動完了];
```

### ゲーム起動フロー

```mermaid
  flowchart TD;
      subgraph 通常フロー;
        A([ゲームを選択]) --> B{アカウントの設定有無};
        B -- 済み --> C[アカウント情報読込];
        B -- まだ  --> D(['アカウントの設定が必要です'の表示]);
        C --> E{ログインセッションの確認\nログイン済み};
        E -- Yes --> F;
        E -- No  --> AA([ログインフローへ])
        F[ゲーム起動ページへリクエスト] --> G[ゲーム起動ページの読み込み];
        G -- ページの解析 --> H["「ゲームを起動」ボタンをさがす"];
        H --> I[ゲーム起動用カスタムURIを取得];
        I --> J[レジストリからゲームのパスを取得];
        J --> K[カスタムURIをパラメータに指定して\nlauncher.exeを実効];
        K --> L([完了]);
      end
      X([ログインフロー後]) --> F;
```


### ログインフロー

```mermaid
  flowchart TD;
      Start([ログインフロー]) --> LoginScreen[ログイン画面を取得];
      LoginScreen --> ReqOTP{OTPが必要であるか};
      ReqOTP -- Yes --> OTP{{OTP入力ダイアログ表示}};
      ReqOTP -- No  --> SendLogin[認証情報を送信];
      OTP -- OTP入力 --> SendLogin;
      OTP -- キャンセル --> Cancel1([キャンセル処理]);
      SendLogin --> IsSuccess{ログイン成功};
      IsSuccess -- Yes --> 2FARes{二要素認証が必要};
      IsSuccess -- No  --> LoginFail;
      IsSuccess -- Yes --> Continue([ゲーム起動処理続行]);
      2FARes -- Yes --> 2FA{{二要素認証入力ダイアログ表示}};
      2FARes -- No  --> Continue([ゲーム起動処理続行]);
      2FA -- コード入力 --> IsSuccess{ログイン成功};
      2FA -- キャンセル --> Cancel2([キャンセル処理]);
      LoginFail['ログイン失敗'ダイアログの表示];
```


# 開発環境

OS: Microsoft&reg; Windows&trade; 10 以上

必要ランタイム: Microsoft&reg; .NET Framework 4.8

インストールが必要な場合は [こちら](https://docs.microsoft.com/ja-jp/dotnet/framework/install/on-windows-10) からインストールしてください。


## オプショナル

- Docker


# ビルド方法

Microsoft&reg; Visual Studio、または [MSBuild](https://docs.microsoft.com/ja-jp/visualstudio/msbuild/msbuild?view=vs-2022) を用いてビルドすることができます。

## NuGet

いくつかのNuGetライブラリを使用しています。

それらをインストールするためにNuGetから復元する必要があります。

```
nuget restore KsGameLauncher.sln
```


# デバッグ

一部、サーバーからデータをダウンロードする機能があります。
これをシミュレートするために簡易WebサーバーとしてDockerイメージを同梱しています。


## イメージのビルド

```
docker compose build
```

## コンテナの作成、起動

```
docker compose up -d
```

サーバーはポート `8080` で起動します。
接続先 `http://localhost:8080`

ゲーム一覧となる `appinfo.json` は `http://localhost:8080/conf/appinfo.json` となります。


## コンテナの停止

```
docker compose stop
```

## コンテナの削除

```
docker compose down
```

## トラブルシュート

既存のポートと重複する場合は `docker-compose.yml` の `services.web.ports` で変更してください。
