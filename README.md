# TaoTray
[![.NET Core Desktop](https://github.com/nerrog/TaoTray/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/nerrog/TaoTray/actions/workflows/dotnet-desktop.yml)

![image](.github/Assets/screenshot.png)

Windows向け原神ステータス確認ソフト

# 特徴
- タスクトレイ常駐ソフト
- 天然樹脂が溢れそうなときや探索派遣の完了時などに通知
- English Support.

# 要件(Requirements)
- Windows10・11
- HoyoLab通行証(ゲーム内アカウントと紐付け済み)

.NETランタイムとWebViewはインストーラーからインストールされます

# ダウンロード(Download)
- [Releases](https://github.com/nerrog/TaoTray/releases)から最新のインストーラーをダウンロード
- インストーラーの指示に従いインストール
- スタートアップに自動的に登録されます

# 使い方(usage)
- TaoTrayを起動する
- ブラウザが出てくるのでログインしたあと「OK」ボタンを押す
- アカウントが複数ある場合は選択画面が出てくるので選択する
- 正常に追加できた通知が飛んでくれば完了
- タスクトレイの胡桃のアイコンをクリックすると表示
- タスクトレイアイコン右クリックでメニューが表示

## For use in English (英語で使う場合)
You cannot change the language in the in-app settings at the moment,
so please open `config.json` and change `Language` to `en-us` manually.

Translated using DeepL. If you find any errors, please send me a pull request.


## アイコンの設定方法
- TaoTray.exeが入っているフォルダに`icon.png`をいう名前を付けてアイコンにしたいファイルを保存すると
自動的に設定されます


# 免責
本ソフトウェアは[MITライセンス](LICENSE.txt)にて提供されています。

本ソフトウェア及び[HoyoLab非公式C#ラッパーライブラリHuTao.NET](https://github.com/nerrog/HuTao.NET)
はmiHoYo及びCOGNOSPHEREとは一切関係ありません。