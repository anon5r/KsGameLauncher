using AdysTech.CredentialManager;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KsGameLauncher
{

    internal class Launcher
    {
        internal HttpClient httpClient;
        internal HttpClientHandler httpHandler;

        private static Launcher instance;



        internal static Launcher Create()
        {
            if (instance == null)
            {
                instance = new Launcher();
                instance.httpClient = instance.CreateHttp();
            }

            return instance;
        }

        private Launcher()
        {

        }


        private HttpClient CreateHttp()
        {
            httpHandler = new HttpClientHandler()
            {
                UseCookies = true,
                AllowAutoRedirect = true,
            };
            if (Properties.Settings.Default.UseProxy)
            {
                httpHandler.Proxy = WebRequest.GetSystemWebProxy();
            }
            HttpClient httpClient = new HttpClient(httpHandler)
            {
                Timeout = TimeSpan.FromMilliseconds(10000)
            };
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // User-Agent
            httpClient.DefaultRequestHeaders.Add("User-Agent",
                String.Format("Mozilla/5.0 {0} {1}/{2}",
                System.Environment.OSVersion.VersionString,
                Application.ProductName,
                Application.ProductVersion)
            );
            httpClient.DefaultRequestHeaders.Add("Accept-Language", CultureInfo.CurrentUICulture.Name);

            // Timeout
            httpClient.Timeout = TimeSpan.FromSeconds(10.0);

            return httpClient;
        }




        internal bool IsLogin()
        {
            if (httpClient == null || httpHandler == null)
            {
                httpClient = CreateHttp();
                return false;
            }

            bool isLogin = false;
            CookieCollection cookies = httpHandler.CookieContainer.GetCookies(new Uri(Properties.Settings.Default.BaseURL));
            foreach (Cookie cookie in cookies)
            {
                Debug.WriteLine(String.Format("Cookie: {0}", cookie.Name));
                if (cookie.Name == Properties.Resources.LoginCookieSessionKey)
                {
                    isLogin = true;
                    break;
                }
            }
            return isLogin;
        }



        private async Task<Uri> GetLoginUri()
        {
            Debug.WriteLine(String.Format("Get login page: {0}", Properties.Settings.Default.LoginURL));

            using (var response = await httpClient.GetAsync(Properties.Settings.Default.LoginURL))
            {
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();

                if (content == null || !content.StartsWith("https:"))
                    throw new LoginUriException("Failed to get login URL");

                return new Uri(content);
            }

        }

        public async Task<bool> Login(NetworkCredential credential, Uri loginURL)
        {
            if (Properties.Settings.Default.EnableNotification)
            {
                MainForm.DisplayToolTip(Resources.IconBalloonMessage_WhileLogin, Properties.Settings.Default.NotificationTimeout);
            }

            httpClient.CancelPendingRequests();
            Debug.WriteLine(String.Format("Start login to: {0}", loginURL.ToString()));

            // Request login page
            using (HttpResponseMessage response = await httpClient.GetAsync(loginURL))
            {
                HtmlParser parser = new HtmlParser();
                string loginPageContent = await response.Content.ReadAsStringAsync();
                IHtmlDocument document = await parser.ParseDocumentAsync(loginPageContent);
#if DEBUG
                Debug.WriteLine(String.Format("Response: {0}", response.Headers.ToString()));
                //Debug.WriteLine(String.Format("{0}", loginPageContent));
#endif
                var form = document.QuerySelector(Properties.Settings.Default.selector_login_form);
                var csrfToken = document.QuerySelector(Properties.Settings.Default.selector_login_csrf);
                var loginUsername = document.QuerySelector(Properties.Settings.Default.selector_login_user);
                var loginPassword = document.QuerySelector(Properties.Settings.Default.selector_login_pass);

                string formAction = form.GetAttribute("action");
                string postURLString = response.RequestMessage.RequestUri.AbsoluteUri.Remove(response.RequestMessage.RequestUri.AbsoluteUri.LastIndexOf('/')) + "/" + formAction;


                Dictionary<string, string> requstParams = new Dictionary<string, string>()
                {
                    {csrfToken.GetAttribute("name"), csrfToken.GetAttribute("value")},
                    { loginUsername.GetAttribute("name"), credential.UserName },
                    { loginPassword.GetAttribute("name"), credential.Password },
                    { "otpass", "" }
                };
                try
                {
                    await SendLoginRequest(new Uri(postURLString), requstParams);
                }
                catch (LoginException ex)
                {
                    throw ex;
                }

                return IsLogin();
            }
        }

        private async Task<HttpResponseMessage> SendLoginRequest(Uri url, Dictionary<string, string> requestParams)
        {

            FormUrlEncodedContent postQuery = new FormUrlEncodedContent(requestParams);
#if DEBUG
            Debug.WriteLine(String.Format("postURLString: {0}", url.ToString()));
            Debug.WriteLine(String.Format("postQuery: {0}", await postQuery.ReadAsStringAsync()));
#endif

            using (HttpResponseMessage response = await httpClient.PostAsync(url, postQuery))
            {
                response.EnsureSuccessStatusCode();

                if (response.RequestMessage.RequestUri.AbsolutePath.Contains("/login_error.html"))
                {
                    throw new LoginException(Resources.IncorrectUsernameOrPassword);
                }
                if (response.RequestMessage.RequestUri.AbsolutePath.Contains("/timeout.html"))
                {
                    throw new LoginException(Resources.AuthorizeFailed);
                }
#if DEBUG
                Debug.WriteLine(response.RequestMessage.RequestUri.ToString());
                //string rescontent = await response.Content.ReadAsStringAsync();
                //Debug.WriteLine(rescontent);
#endif
                if (response.RequestMessage.RequestUri.Host.Contains(Properties.Resources.AuthorizeDomain))
                {
                    throw new LoginException(Resources.IncorrectUsernameOrPassword);
                }
                return response;
            }
        }

        async public void StartApp(AppInfo app)
        {
            NetworkCredential credential = GetCredential();
            if (credential == null)
            {
                MessageBox.Show(Resources.ShouldBeSetAccountBeforeLaunch, Resources.ErrorText,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (!IsLogin())
                {
                    try
                    {
                        // Try to login
                        Uri loginUri = await GetLoginUri();
#if DEBUG
                        Debug.WriteLine(string.Format("Login URL: {0}", loginUri.ToString()));
#endif
                        if (!await Login(GetCredential(), loginUri))
                        {
                            MessageBox.Show(string.Format("Failed to login, cannot launch {0}", app.Name));
                            return;
                        }
                    }
                    catch (LoginException ex)
                    {
                        MessageBox.Show(ex.Message, Resources.LoginExceptionDialogName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }


                // Request again to launcher URL
#if DEBUG
                Debug.WriteLine(String.Format("Open launcher URL:  {0}", app.Launch.URL));
#endif
                if (Properties.Settings.Default.EnableNotification)
                {
                    MainForm.DisplayToolTip(String.Format(Resources.IconBalloonMessage_Launching, app.Name), Properties.Settings.Default.NotificationTimeout);
                }
                using (HttpResponseMessage response = await httpClient.GetAsync(app.Launch.GetUri()))
                {
                    response.EnsureSuccessStatusCode();


                    if (response.RequestMessage.RequestUri.Host.Contains(Properties.Resources.AuthorizeDomain))
                    {
                        MessageBox.Show(Resources.IncorrectUsernameOrPassword);
                        return;
                    }

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        switch (response.StatusCode)
                        {
                            case HttpStatusCode.Redirect:
                                Uri redirectUri = response.Headers.Location;
                                MessageBox.Show(Resources.CheckFollowingPage);
                                Utils.Common.OpenUrlByDefaultBrowser(redirectUri);
                                break;

                            case HttpStatusCode.NotFound:
                            case HttpStatusCode.Forbidden:
                                throw new LauncherException(Resources.LauncherURLCannotBeUsed);

                            case HttpStatusCode.InternalServerError:
                            case HttpStatusCode.ServiceUnavailable:
                                throw new LauncherException(Resources.ServiceIsTemporaryUnavailable);

                            default:
                                throw new LauncherException(String.Format("{0} => {1} {2}", Resources.UnknownStatusReceived, (int)response.StatusCode, response.ReasonPhrase));
                        }
                    }

                    // Page URL check
                    string loadedURL = response.RequestMessage.RequestUri.ToString();
                    if (response.RequestMessage.RequestUri.Host.Contains(Properties.Resources.AuthorizeDomain))
                    {
                        throw new LoginException(Resources.LoginSessionHasBeenExpired);
                    }
                    else if (loadedURL.Contains(Properties.Resources.TosCheckPath))
                    {
                        throw new GameTermsOfServiceException(Resources.ShouldCheckTermOfService, loadedURL);
                    }


                    Stream stream = await response.Content.ReadAsStreamAsync();

                    Encoding enc;
                    if (!response.Content.Headers.Contains("Content-Type"))
                    {
                        switch (response.Content.Headers.ContentType.CharSet.ToLower())
                        {
                            case "sjis":
                            case "s-jis":
                            case "windows-31j":
                            case "cp932":
                            case "ms932":
                                enc = Encoding.GetEncoding("shift-jis");
                                break;

                            default:
                                enc = Encoding.GetEncoding(response.Content.Headers.ContentType.CharSet);
                                break;
                        }
                        enc = Encoding.GetEncoding(response.Content.Headers.ContentType.CharSet);
                    }
                    else
                        enc = Encoding.Default;


                    string content;
                    using (TextReader reader = (new StreamReader(stream, enc, true)) as TextReader)
                    {
                        content = await reader.ReadToEndAsync();

                        if (content == null)
                        {
                            throw new LauncherException("Cannot load login page");
                        }

                        // Status 200 returns while maintenance
                        if (content.Contains(Properties.Resources.MaintenanceCheckString))
                        {
                            // Display their maintenance message
                            throw new LauncherException(content);
                        }
#if DEBUG
                        Debug.WriteLine(String.Format("Response page URI: {0}", response.RequestMessage.RequestUri.ToString()));
                        //Debug.WriteLine(content);
#endif

                        // parse launcher page
                        LauncherLoginPage(content, app.Launch.Selector);
                    }

                }

            }
            catch (HttpRequestException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (TaskCanceledException)
            {
                // Task cancelled / Connection Timeout
                Console.WriteLine(Resources.ConnectionTimeout);
            }
            catch (LoginException e)
            {
                MessageBox.Show(e.Message, Resources.LoginExceptionDialogName);
            }
            catch (GameTermsOfServiceException ex)
            {
                MessageBox.Show(ex.Message, Resources.GameTermsOfServiceExceptionDialogName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Utils.Common.OpenUrlByDefaultBrowser(ex.GetTosURL());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        async void LauncherLoginPage(string content, string querySelector)
        {
            if (content == null || content.Length < 0)
            {
                throw new LauncherException("Failed to load content.");
            }


            HtmlParser parser = new HtmlParser();
            IHtmlDocument document = await parser.ParseDocumentAsync(content);

            AngleSharp.Dom.IElement launchButton = document.QuerySelector(querySelector);
            if (launchButton == null)
            {
                MessageBox.Show("Failed to parse page!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string launcherCustomProtocol = launchButton.GetAttribute("href");
#if DEBUG
            Debug.WriteLine(String.Format("Launcher custom URI: {0}", launcherCustomProtocol));
#endif

            if (launcherCustomProtocol != null
                && (Regex.IsMatch(launcherCustomProtocol, @"^konaste\.[a-z0-9\-]+://login")
                   || Regex.IsMatch(launcherCustomProtocol, @"^bm2dxinf://login")))
            {
                Uri customUri = new Uri(launcherCustomProtocol);
#if DEBUG
                Debug.WriteLine(String.Format("launcherUri: {0}", launcherCustomProtocol));
                Debug.WriteLine(String.Format("custom scheme: {0}", customUri.Scheme));
#endif
                string launcherPath = Utils.GameRegistry.GetLauncherPath(customUri.Scheme);
#if DEBUG
                Debug.WriteLine(String.Format("Launcher exec path: {0}", launcherPath));
#endif

                Process.Start(launcherPath, launcherCustomProtocol);
            }

        }

        private NetworkCredential GetCredential()
        {
            return CredentialManager.GetCredentials(target: Properties.Resources.CredentialTarget);
        }

        private string GetGamePathFromLauncher(string launcherPath)
        {
            string gamePath = launcherPath.Replace(@"\launcher\modules\launcher.exe", @"game\modules\");
            string[] files = Directory.GetFiles(gamePath, "*.exe");
            if (files.Length < 0)
                return null;
            foreach (string path in Directory.GetFiles(gamePath, "*.exe"))
            {
                return path;
            }
            return null;
        }

        /// <summary>
        /// Logout current session
        /// </summary>
        async public static Task<HttpResponseMessage> Logout()
        {
            Launcher instance = Create();
            HttpResponseMessage response = await instance.httpClient.GetAsync(Properties.Settings.Default.LogoutURL);
            response.EnsureSuccessStatusCode();

            instance.httpClient = null;

            return response;
        }
    }
}
