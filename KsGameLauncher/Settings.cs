namespace KsGameLauncher.Properties
{


    // このクラスでは設定クラスでの特定のイベントを処理することができます:
    //  SettingChanging イベントは、設定値が変更される前に発生します。
    //  PropertyChanged イベントは、設定値が変更された後に発生します。
    //  SettingsLoaded イベントは、設定値が読み込まれた後に発生します。
    //  SettingsSaving イベントは、設定値が保存される前に発生します。
    internal sealed partial class Settings
    {

        public Settings()
        {
            // // 設定の保存と変更のイベント ハンドラーを追加するには、以下の行のコメントを解除します:
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //
        }

        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            // SettingChangingEvent イベントを処理するコードをここに追加してください。
        }

        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // SettingsSaving イベントを処理するコードをここに追加してください。
        }


        [System.Configuration.UserScopedSettingAttribute()]
        [System.Configuration.SettingsDescription("Keeps track of whether you have upgraded the configuration file from the previous version.")]
        [System.Configuration.DefaultSettingValue("false")]
        public System.Boolean IsUpgrated
        {
            get { return (System.Boolean)this["IsUpgrated"]; }
            set { this["IsUpgrated"] = value; }
        }


        [System.Configuration.UserScopedSettingAttribute()]
        [System.Configuration.SettingsDescription("Cookie as login session")]
        [System.Configuration.DefaultSettingValue(null)]
        [System.Configuration.SettingsSerializeAs(System.Configuration.SettingsSerializeAs.Binary)]
        public System.Net.Cookie Cookie
        {
            get
            {
                if (this["Cookie"] == null)
                    return null;
                return (System.Net.Cookie)this["Cookie"];
            }
            set { this["Cookie"] = value; }
        }
    }
}
