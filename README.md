# AppOfflineModule

## What is this? / これは何?

This is the ASP.NET Http Module.

If installed this module on your ASP.NET Web application project and configured enabled, then all of the http requests to the web app intercepted by this module and respond HTTP 404 "Not Found".

But only special users who specified by configuration can access the site normaly.

While this module enabled, nobody access login form on the site, of couse. But if the user access special "back door" url then respond user name/password login form which built in this module.

これは ASP.NET の Http モジュールです。

このモジュールを ASP.NET Web アプリケーションプロジェクトに追加して、さらに有効となるよう設定すると、そのサイトへのすべての HTTP 要求はこのモジュールに捕捉され、HTTP 404 "ページが見つかりません" を応答に返します。

ただし、特別に構成ファイルで指定されたユーザーは、通常どおりに Web サイトにアクセスできます。

もちろん、このモジュールが有効である間は誰もそのサイトのログインページを開けません。しかし特別な "バックドア" URL にアクセスすると、このモジュールが内蔵している、ユーザー名/パスワードのログインフォームが表示されます。

## How to install? / インストール方法

You can install this module as NuGet package.

NuGet パッケージからインストールできます。

**example / 例**

    >Install-Package AppOfflineModule

## How to configure? / 設定方法

### Enabling / 有効化

Set the value to "Offline" at "AppOffline.State" key entry in appSettings section, web.config.

web.config の appSettings セクションに、キー = "AppOffline.State" のエントリで値に "Offline" と設定します。

**example / 例:**

    <appSettings>
      <add key="AppOffline.State" value="Offline"/>
    <appSettings>

### Special Users / 特別ユーザー

Set the value to user names at "AppOffline.AllowPassThroughUsers" key entry in appSettings section, web.config.

Or set the vlaue to role names at "AppOffline.AllowPassThroughRoles" key entry.

Thease specified users or roles can access the site normaly when "AppOffline.State" is "Offline".

User names and role names have to  concatinated by comma separator.

web.config の appSettings セクション中、"AppOffline.AllowPassThroughUsers" キーの値にユーザー名を指定します。

あるいは "AppOffline.AllowPassThroughRoles" キーの値にロール名を指定します。

これらの指定されたユーザーまたはロールは、"AppOffline.State" が "Offline" に設定されていても、通常どおりにサイトにアクセスできます。

ユーザー名またはロール名はカンマ区切りで列記してください。

**example / 例**

    <appSettings>
      <add key="AppOffline.AllowPassThroughRoles" value="Administrators"/>
      <add key="AppOffline.AllowPassThroughUsers" value="alice,bob"/>
    </appSettings>

**Notice / 注意**

If there is no settings at both "AppOffline.AllowPassThroughRoles" and "AppOffline.AllowPassThroughUsers" key entry then nobody can normaly access.

もし "AppOffline.AllowPassThroughRoles" と "AppOffline.AllowPassThroughUsers" キーの両方の設定がない場合は、誰も通常のアクセスはできません。

### "Back Door" Access / "バックドア" アクセス

Set the value to the URL of "back door", which open the login forms built in this module, at "AppOffline.BackDoorUrl" key entry in appSettings section, web.config.

the URL can specifiled by application relative url.

その URL を開くと、このモジュール内蔵のログインフォームが開く "バックドア" URL は、web.config の appSettings セクション中、"AppOffline.BackDoorUrl" キーの値に設定します。

指定する URL にはアプリケーション相対 URL で指定できます。

**example / 例**

    <appSettings>
      <add key="AppOffline.BackDoorUrl" value="~/abc123"/>
    </appSettings>

**Notice / 注意**

If there is no settings at "AppOffline.BackDoorUrl" key entry then nobody can "Back Door" access.

もし "AppOffline.BackDoorUrl" キーの設定がない場合は、"バックドア" アクセスはできません。


## How to customize? / カスタマイズ方法

This module's behaviors are published as static properties of functions.

You can replace thease properties to your custom functions.

- AppOfflineModule.Filter.IsEnable: gets or sets the function returns determines whether "Offline" mode is enabled or not.
- AppOfflineModule.Filter.IsAllowPassThrough: gets or sets the function returns determines whether current user can access normaly or not.
- AppOfflineModule.Filter.Action: gets or sets the action respond to http clients in "Offline" mode.

このモジュールの動作はいずれも関数の静的プロパティとして公開されています。

これらプロパティが指す関数をアプリケーションの開始時などで差し替えることが可能です。

- AppOfflineModule.Filter.IsEnable: "Offline" モードを有効とするかどうかを返す関数を取得または設定します。
- AppOfflineModule.Filter.IsAllowPassThrough: 現在のユーザーが "Offline" モードであっても通常どおりのアクセスが可能かどうかを返す関数を取得または設定します。
- AppOfflineModule.Filter.Action: "Offline" モード時に HTTP クライアントへの応答を行うアクションを取得または設定します。

### Example 1 / カスタマイズ例 1

**example for AppOffline enabled before Feb 18 2013(UTC). / (世界標準時)2013年2月18日以前はオフラインとする例**

    // Global.asax.cs
    public class MyApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ...
            AppOfflineModule.Filter.IsEnable = () => DateTime.UtcNow <= DateTime.Parse("Feb 18, 2013");
        }
    }

### Example 2 / カスタマイズ例 2

**example for retrun HTTP status 503 instead of 404. / HTTPステータス 404 の代わりに 503 を返す例**

    // Global.asax.cs
    public class MyApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ...
            AppOfflineModule.Filter.Action = (app) =>
            {
                app.Response.StatusCode = 503;
                app.Response.End();
            };
        }
    }

