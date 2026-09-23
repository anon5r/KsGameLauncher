using KsGameLauncher2.Properties;
using Xunit;

namespace KsGameLauncher2.Tests
{
    /// <summary>
    /// Strings.Designer.cs のプロパティと Strings.resx のエントリの対応を検証する。
    /// Designer は Visual Studio のデザイン時ツールが生成するもので dotnet build では再生成されないため、
    /// resx にだけ追加すると「プロパティが無くてコンパイルエラー」、Designer にだけ追加すると
    /// 「実行時に null が返ってダイアログが空になる」という食い違いが起きる。後者はビルドでは気付けない。
    /// </summary>
    public class StringsResourceTests
    {
        public static TheoryData<string> WebLauncherMessages() =>
            new()
            {
                nameof(Strings.WebView2RuntimeNotInstalled),
                nameof(Strings.UnexpectedLaunchUri),
                nameof(Strings.GameLauncherNotInstalled),
            };

        [Theory]
        [MemberData(nameof(WebLauncherMessages))]
        public void Message_ResolvesToNonEmptyString(string name)
        {
            string? value = (string?)typeof(Strings)
                .GetProperty(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                .GetValue(null);

            Assert.False(string.IsNullOrWhiteSpace(value),
                $"Strings.{name} resolved to null or empty. The entry is probably missing from Strings.resx.");
        }

        /// <summary>
        /// WebLauncher がスキーム名を差し込んで使うため、プレースホルダーが失われていないことを確認する。
        /// </summary>
        [Fact]
        public void GameLauncherNotInstalled_KeepsItsPlaceholder()
        {
            Assert.Contains("{0}", Strings.GameLauncherNotInstalled);

            string formatted = string.Format(Strings.GameLauncherNotInstalled, "konaste.sdvx");

            Assert.Contains("konaste.sdvx", formatted);
            Assert.DoesNotContain("{0}", formatted);
        }
    }
}
