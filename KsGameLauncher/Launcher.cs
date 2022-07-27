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
using KsGameLauncher.Properties;

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

        /// <summary>
        /// Get encoding from WebName
        /// Supports Japanese encoding name aliases similar to Shift-JIS
        /// </summary>
        /// <param name="encodeName"></param>
        /// <returns></returns>
        internal static Encoding EncodingMapJapanese(string encodeName)
        {
            Encoding enc = null;
            string encoding = encodeName.ToLower();
            switch (encoding)
            {
                case "sjis":
                case "s-jis":
                case "windows-31j":
                case "cp932":
                case "ms932":
                    enc = Encoding.GetEncoding("shift-jis");
                    break;
                default:
                    foreach (EncodingInfo encinfo in Encoding.GetEncodings())
                    {
                        Encoding e = encinfo.GetEncoding();
                        if (e.WebName.ToLower() == encoding)
                        {
                            enc = Encoding.GetEncoding(encoding);
                            break;
                        }
                    }
                    if (enc == null)
                        enc = Encoding.UTF8;
                    break;
            }
            return enc;
        }

        /// <summary>
        /// Convert response content in their encoding
        /// </summary>
        /// <param name="response"></param>
        /// <returns>Encoded content</returns>
        internal static string GetResponseContentWithEncoding(HttpResponseMessage response)
        {
            Stream stream = response.Content.ReadAsStreamAsync().Result;
            Encoding enc;
            string encodingName = null;
            if (response.Content.Headers.Contains("Content-Type"))
            {
                encodingName = response.Content.Headers.ContentType.CharSet;
            }

            if (encodingName != null)
            {
                enc = EncodingMapJapanese(encodingName);
            }
            else
                enc = Encoding.Default;

            return ConvertEncoding(stream, enc);
        }

        /// <summary>
        /// Convert text encoding
        /// </summary>
        /// <param name="text">Target text</param>
        /// <param name="encoding">Target encoding name</param>
        /// <returns></returns>
        internal static string ConvertEncoding(string text, string encoding)
        {
            Encoding enc = EncodingMapJapanese(encoding);
            return ConvertEncoding(text, enc);
        }

        /// <summary>
        /// Convert text encoding
        /// </summary>
        /// <param name="stream">Target stream</param>
        /// <param name="encoding">Target encoding name</param>
        /// <returns></returns>
        internal static string ConvertEncoding(Stream stream, string encoding)
        {
            Encoding enc = EncodingMapJapanese(encoding);
            return ConvertEncoding(stream, enc);
        }

        /// <summary>
        /// Convert text encoding
        /// </summary>
        /// <param name="text">Target text</param>
        /// <param name="encoding">Source encoding</param>
        /// <returns></returns>
        internal static string ConvertEncoding(string text, Encoding encoding)
        {
            string content = Encoding.Default.GetString(encoding.GetBytes(text));
            return content;
        }

        /// <summary>
        /// Convert stream text with encoding
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        /// <exception cref="LauncherException"></exception>
        internal static string ConvertEncoding(Stream stream, Encoding encoding)
        {
            string content = "";
            using (TextReader reader = (new StreamReader(stream, encoding, true)) as TextReader)
            {
                content = reader.ReadToEndAsync().Result;

                if (content == null)
                    throw new LauncherException("Cannot load login page");
            }
            return content;
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

            CookieContainer savedCookies = Properties.Settings.Default.Cookies;
            if (savedCookies != null)
            {
                bool expired = true;
                Debug.WriteLine(savedCookies.GetType().ToString());
                foreach (Cookie cookie in savedCookies.GetCookies(new Uri(Properties.Settings.Default.BaseURL)))
                {
                    Debug.WriteLine(cookie.GetType().ToString());
                    // cookie : CookieCollection
                    if (cookie != null && cookie.Name == Properties.Resources.LoginCookieSessionKey)
                    {
                        //expired = collection[Properties.Resources.LoginCookieSessionKey].Expired;
                        expired = cookie.Expired;
                    }
                }
                if (!expired)
                {
                    httpHandler.CookieContainer = savedCookies;
                }
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



        /// <summary>
        /// Check login session is available
        /// </summary>
        /// <returns></returns>
        internal bool IsLogin()
        {
            if (httpClient == null || httpHandler == null)
            {
                httpClient = CreateHttp();
                return false;
            }

            bool isLogin = false;
            Uri BaseURL = new Uri(Properties.Settings.Default.BaseURL);
            CookieCollection cookies = httpHandler.CookieContainer.GetCookies(BaseURL);
            CookieContainer savedCookies = Properties.Settings.Default.Cookies;
            foreach (Cookie cookie in cookies)
            {
                Debug.WriteLine(String.Format("DomainName: {0}; Path={1}", cookie.Domain, cookie.Path));
                Debug.WriteLine(String.Format("CookieName: {0}", cookie.Name));
                if (cookie.Name == Properties.Resources.LoginCookieSessionKey)
                {
                    Debug.WriteLine(String.Format("Cookie: {0}", cookie.ToString()));
                    isLogin = true;
                    if (savedCookies == null || cookie.Expired)
                    {
                        // Save latest cookie value if it does not exists, or expired
                        Properties.Settings.Default.Cookies = httpHandler.CookieContainer;
                        Properties.Settings.Default.Save();
                    }
                    break;
                }
            }
            return isLogin;
        }



        /// <summary>
        /// Get appinfo JSON file
        /// Load from local if exists, otherwise download from the internet
        /// </summary>
        /// <returns></returns>
        public async static Task<string> GetJson()
        {
            try
            {
                string json;
                string localPath = Directory.GetParent(Application.ExecutablePath) + "\\" + Properties.Settings.Default.appInfoLocal;
                if (!File.Exists(localPath))
                {
                    // Load appinfo.json from the internet
                    // Download from `Properties.Settings.Default.appInfoURL`

                    await Utils.AppUtil.DownloadJson();
                }

                try
                {
                    // Load appinfo.json from local
                    using (StreamReader jsonStream =
                           File.OpenText(Path.GetFullPath(localPath)))
                    {
                        json = jsonStream.ReadToEnd();
                        jsonStream.Close();
                    }

#if DEBUG
                    Debug.Write(json);
#endif

                    return json;
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show(String.Format(Properties.Strings.FailedToLoadFile, Properties.Settings.Default.appInfoLocal),
                        Properties.Strings.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
            }
            catch (HttpRequestException)
            {
                MessageBox.Show(Properties.Strings.ErrorGetAppInfoFailed, Properties.Strings.SyncWithServerDialogTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }

            return null;
        }

        /// <summary>
        /// Get login page URL
        /// </summary>
        /// <returns></returns>
        /// <exception cref="LoginUriException"></exception>
        private async Task<Uri> GetLoginUri()
        {
            Debug.WriteLine(String.Format("Get login page: {0}", Properties.Settings.Default.LoginURL));

            using (var response = await httpClient.GetAsync(Properties.Settings.Default.LoginURL))
            {
                response.EnsureSuccessStatusCode();
                
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new LoginUriException("Failed to get login URL");
                return response.RequestMessage.RequestUri;
            }
        }

        /// <summary>
        /// Login progress to KONASTE
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="loginURL"></param>
        /// <returns></returns>
        public async Task<bool> Login(NetworkCredential credential, Uri loginURL)
        {
            if (Properties.Settings.Default.EnableNotification && Program.mainContext != null)
            {
                Program.mainContext.DisplayToolTip(Properties.Strings.IconBalloonMessage_WhileLogin, Properties.Settings.Default.NotificationTimeout);
            }

            string otpCode = "";
            if (Properties.Settings.Default.UseOTP)
            {
                await Task.Factory.StartNew(() =>
                {
                    // Use OTP by token generator device/app
                    var otp = new Forms.OTPDialog();
                    DialogResult otpResult = otp.ShowDialog(string.Format(Properties.Strings.OTPDialogMessage_OTP, 8), 8);
                    if (DialogResult.OK == otpResult)
                    {
                        otpCode = otp.Code;
                    }
                    else if (DialogResult.Cancel == otpResult)
                    {
                        // Canceled process continueing
                        throw new LoginCancelException();
                    }
                });
            }

            httpClient.CancelPendingRequests();
            Debug.WriteLine(String.Format("Start login to: {0}", loginURL.ToString()));
            LoginDataSet loginResponse = await LoginRequest(credential, loginURL, otpCode);
            try
            {
                if (loginResponse.RequestUri.Host.Contains(Properties.Resources.AuthorizeDomain))
                {
                    string responseURL = (string)loginResponse.RequestUri.AbsoluteUri;
                    //string pageContent = ConvertEncoding(loginResponse.Content, Encoding.UTF8);
                    string pageContent = loginResponse.Content;
#if DEBUG
                    //Debug.WriteLine(pageContent);
                    Debug.WriteLine("Login succeed");
#endif
                    loginResponse.Dispose();
                    var twoStepResponse = await LoginTwoStep(pageContent, responseURL);
                    if (twoStepResponse == null)
                        throw new LoginException(Properties.Strings.IncorrectUsernameOrPassword);
                    twoStepResponse.Dispose();
                }
            }
            catch (LoginCancelException ex)
            {
                throw ex;
            }
            catch (LoginException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return IsLogin();
        }

        /// <summary>
        /// Send request as login process
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="loginURL"></param>
        /// <param name="otpCode"></param>
        /// <returns></returns>
        private async Task<LoginDataSet> LoginRequest(NetworkCredential credential, Uri loginURL, string otpCode)
        {
            // Request login page
            using (HttpResponseMessage message = await httpClient.GetAsync(loginURL))
            {
                HtmlParser parser = new HtmlParser();
                string loginPageContent = await message.Content.ReadAsStringAsync();
                IHtmlDocument document = await parser.ParseDocumentAsync(loginPageContent);
#if DEBUG
                Debug.WriteLine(String.Format("Response: {0}", message.Headers.ToString()));
                //Debug.WriteLine(String.Format("{0}", loginPageContent));
#endif
                var form = document.QuerySelector(Properties.Settings.Default.selector_login_form);
                var csrfToken = document.QuerySelector(Properties.Settings.Default.selector_csrf);
                var loginUsername = document.QuerySelector(Properties.Settings.Default.selector_login_user);
                var loginPassword = document.QuerySelector(Properties.Settings.Default.selector_login_pass);


                string formAction = form.GetAttribute("action");
                string postURLString = message.RequestMessage.RequestUri.AbsoluteUri.Remove(message.RequestMessage.RequestUri.AbsoluteUri.LastIndexOf('/')) + "/" + formAction;


                Dictionary<string, string> requstParams = new Dictionary<string, string>()
                {
                    { csrfToken.GetAttribute("name"), csrfToken.GetAttribute("value") },
                    { loginUsername.GetAttribute("name"), credential.UserName },
                    { loginPassword.GetAttribute("name"), credential.Password },
                    { "otpass", otpCode }
                };


                return await SendRequest(new Uri(postURLString), requstParams);
            }
        }

        /// <summary>
        /// Send request with parameters to URL by POST
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestParams"></param>
        /// <returns></returns>
        /// <exception cref="LoginException"></exception>
        private async Task<LoginDataSet> SendRequest(Uri url, Dictionary<string, string> requestParams)
        {
            if (httpClient == null || httpHandler == null)
            {
                httpClient = CreateHttp();
            }

            FormUrlEncodedContent postQuery = new FormUrlEncodedContent(requestParams);
#if DEBUG
            Debug.WriteLine(string.Format("postURLString: {0}", url.ToString()));
            Debug.WriteLine(string.Format("postQuery: {0}", await postQuery.ReadAsStringAsync()));
#endif

            using (HttpResponseMessage response = await httpClient.PostAsync(url, postQuery))
            {
                response.EnsureSuccessStatusCode();

                if (response.RequestMessage.RequestUri.AbsolutePath.Contains("/login_error.html"))
                {
#if DEBUG
                    Debug.WriteLine(string.Format("respond redirected URL: {0}", response.RequestMessage.RequestUri.ToString()));
#endif
                    throw new LoginException(Properties.Strings.IncorrectUsernameOrPassword);
                }
                if (response.RequestMessage.RequestUri.AbsolutePath.Contains("/timeout.html"))
                {
                    throw new LoginException(Properties.Strings.AuthorizeFailed);
                }
#if DEBUG
                //string content = GetResponseContentWithEncoding(response);
                Debug.WriteLine(string.Format("Located URL: {0}", response.RequestMessage.RequestUri.ToString()));
                //Debug.WriteLine(content);
#endif
                return new LoginDataSet(response);
            }
        }

        /// <summary>
        /// Send request for 2FA code
        /// </summary>
        /// <param name="PageContent">Page content to parse for 2FA</param>
        /// <param name="urlString">2FA page URL</param>
        /// <returns></returns>
        /// <exception cref="LoginCancelException"></exception>
        private async Task<LoginDataSet> LoginTwoStep(string PageContent, string urlString)
        {
            return await LoginTwoStep(PageContent, new Uri(urlString));
        }

        /// <summary>
        /// Send request for 2FA code
        /// </summary>
        /// <param name="PageContent">Page content to parse for 2FA</param>
        /// <param name="uri">2FA page URL</param>
        /// <returns></returns>
        /// <exception cref="LoginCancelException">If the user chooses to cancel, it will be thrown to the calling parent</exception>
        private async Task<LoginDataSet> LoginTwoStep(string PageContent, Uri uri)
        {
#if DEBUG
            Debug.WriteLine(PageContent);
#endif
            HtmlParser parser = new HtmlParser();
            IHtmlDocument document = await parser.ParseDocumentAsync(PageContent);
            var form = document.QuerySelector(Properties.Settings.Default.selector_2fa_form);
            if (form == null)
                return null;

            var csrfToken = document.QuerySelector(Properties.Settings.Default.selector_csrf);
            var twoStepPincode = document.QuerySelector(Properties.Settings.Default.selector_2fa_pincode);
            var twoStepPersistent = document.QuerySelector(Properties.Settings.Default.selector_2fa_pincod_persistnt);


            string formAction = form.GetAttribute("action");
            string postURLString = uri.AbsoluteUri.Remove(uri.AbsoluteUri.LastIndexOf('/')) + "/" + formAction;


            string twoStepCode = "";
            await Task.Factory.StartNew(() =>
            {
                // Use OTP by token generator device/app
                var otp = new Forms.OTPDialog();
                DialogResult otpResult = otp.ShowDialog(string.Format(Properties.Strings.OTPDialogMessage_2FA, 6), 6);
                if (DialogResult.OK == otpResult)
                {
                    twoStepCode = otp.Code;
                }
                else if (DialogResult.Cancel == otpResult)
                {
                    // Canceled process continueing
                    throw new LoginCancelException();
                }
            });

            Dictionary<string, string> requstParams = new Dictionary<string, string>()
            {
                {csrfToken.GetAttribute("name"), csrfToken.GetAttribute("value")},
                { twoStepPincode.GetAttribute("name"), twoStepCode },
                { twoStepPersistent.GetAttribute("name"), "on" }
            };
            try
            {
                return await SendRequest(new Uri(postURLString), requstParams);
            }
            catch (LoginException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Start game launcher
        /// </summary>
        /// <param name="app"></param>
        public async Task StartApp(AppInfo app)
        {
            NetworkCredential credential = GetCredential();
            if (credential == null)
            {
                MessageBox.Show(Properties.Strings.ShouldBeSetAccountBeforeLaunch, Properties.Strings.ErrorText,
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
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
                    catch (LoginUriException ex)
                    {
                        MessageBox.Show(Properties.Strings.FailedToGetAuthURL, Properties.Strings.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        throw ex;
                    }
                    catch (LoginCancelException ex)
                    {
                        // Canceled process
                        instance.httpClient.Dispose();
                        instance.httpClient = null;
                        throw ex;
                    }
                    catch (LoginException ex)
                    {
                        throw ex;
                    }
                }


                // Request again to launcher URL
#if DEBUG
                Debug.WriteLine(String.Format("Open launcher URL:  {0}", app.Launch.URL));
#endif
                if (Properties.Settings.Default.EnableNotification && Program.mainContext != null)
                {
                    Program.mainContext.DisplayToolTip(String.Format(Properties.Strings.IconBalloonMessage_Launching, app.Name), Properties.Settings.Default.NotificationTimeout);
                }
                using (HttpResponseMessage response = await httpClient.GetAsync(app.Launch.GetUri()))
                {
                    response.EnsureSuccessStatusCode();

                    Debug.WriteLine(String.Format("After get launcher URL:  {0}", response.RequestMessage.RequestUri));
                    if (response.RequestMessage.RequestUri.Host.Contains(Properties.Resources.AuthorizeDomain))
                    {
                        //if (!await Login(GetCredential(), response.RequestMessage.RequestUri))
                        //{
                            MessageBox.Show(Properties.Strings.IncorrectUsernameOrPassword, Properties.Strings.AppName,
                                MessageBoxButtons.OK, MessageBoxIcon.Error,
                                MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                            await Logout();
                            return;
                        //}
                    }

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        switch (response.StatusCode)
                        {
                            case HttpStatusCode.Redirect:
                                Uri redirectUri = response.Headers.Location;
                                MessageBox.Show(Properties.Strings.CheckFollowingPage,
                                    Properties.Strings.AppName, MessageBoxButtons.OK, MessageBoxIcon.Warning,
                                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                Utils.Common.OpenUrlByDefaultBrowser(redirectUri);
                                break;

                            case HttpStatusCode.NotFound:
                            case HttpStatusCode.Forbidden:
                                throw new LauncherException(Properties.Strings.LauncherURLCannotBeUsed);

                            case HttpStatusCode.InternalServerError:
                            case HttpStatusCode.ServiceUnavailable:
                                throw new LauncherException(Properties.Strings.ServiceIsTemporaryUnavailable);

                            default:
                                throw new LauncherException(String.Format("{0} => {1} {2}", Properties.Strings.UnknownStatusReceived, (int)response.StatusCode, response.ReasonPhrase));
                        }
                    }

                    // Page URL check
                    string loadedURL = response.RequestMessage.RequestUri.ToString();
                    if (response.RequestMessage.RequestUri.Host.Contains(Properties.Resources.AuthorizeDomain))
                    {
                        await Logout();
                        throw new LoginException(Properties.Strings.LoginSessionHasBeenExpired);
                    }
                    else if (loadedURL.Contains(Properties.Resources.TosCheckPath))
                    {
                        throw new GameTermsOfServiceException(Properties.Strings.ShouldCheckTermOfService, loadedURL);
                    }
                    else if (loadedURL.Contains(Properties.Settings.Default.PrivacyConfirmPath))
                    {
                        throw new PrivacyConfirmationException(Properties.Strings.ConfirmPrivacyPolicy, loadedURL);
                    }

                    string content = GetResponseContentWithEncoding(response);

                    // Status 200 returns while maintenance
                    if (content.Contains(Properties.Resources.MaintenanceCheckString))
                    {
                        // Display their maintenance message
                        //throw new LauncherException(content);
                        throw new LauncherException(Properties.Strings.UnderMaintenanceMessage, loadedURL);
                    }
#if DEBUG
                    Debug.WriteLine(String.Format("Response page URI: {0}", response.RequestMessage.RequestUri.ToString()));
                    //Debug.WriteLine(content);
#endif

                    // parse launcher page
                    await LauncherLoginPage(content, app.Launch.Selector);
                }

                return;
            }
            catch (HttpRequestException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (TaskCanceledException)
            {
                // Task cancelled / Connection Timeout
                Console.WriteLine(Properties.Strings.ConnectionTimeout);
            }
            catch (LoginException e)
            {
                instance.httpClient = null;
#if !DEBUG
                MessageBox.Show(e.Message, Properties.Strings.LoginExceptionDialogName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
#else
                throw e;
#endif
            }
            catch (GameTermsOfServiceException ex)
            {
                MessageBox.Show(ex.Message, Properties.Strings.GameTermsOfServiceExceptionDialogName,
                    MessageBoxButtons.OK, MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                Utils.Common.OpenUrlByDefaultBrowser(ex.GetTosURL());
            }
            catch (PrivacyConfirmationException ex)
            {
                MessageBox.Show(ex.Message, Properties.Strings.PrivacyConfirmationExceptionDialogName,
                    MessageBoxButtons.OK, MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                Utils.Common.OpenUrlByDefaultBrowser(ex.GetPrivacyURL());

            }
            catch (Exception e)
            {
                throw e;
            }

            return;
        }

        async Task<bool> LauncherLoginPage(string content, string querySelector)
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
                MessageBox.Show("Failed to parse page!", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return false;
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
            return true;
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
            if (instance.httpClient == null)
                instance.httpClient = instance.CreateHttp();

            HttpResponseMessage response = await instance.httpClient.GetAsync(Properties.Settings.Default.LogoutURL);
            response.EnsureSuccessStatusCode();

            instance.httpClient = null;
            Properties.Settings.Default.Cookies = null;
            Properties.Settings.Default.Save();

            return response;
        }

        /// <summary>
        /// Launch game by ID
        /// </summary>
        /// <param name="gameID"></param>
        internal async static Task LaunchGames(string gameID)
        {
            string json = await Launcher.GetJson();
            if (json == null)
            {
                MessageBox.Show("There are no games", Properties.Strings.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AppInfo.LoadFromJson(json);

            if (!AppInfo.ContainID(gameID))
            {
                MessageBox.Show("Unsupported game specified", Properties.Strings.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AppInfo appInfo = AppInfo.Find(gameID);

            if (appInfo == null)
            {
                MessageBox.Show("Cannot find that game you specified", Properties.Strings.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Launcher launcher = Launcher.Create();
                await launcher.StartApp(appInfo);
            }
            catch (LoginCancelException)
            {
                // Canceled process while login
                instance.httpClient = null;
                return;
            }
            catch (LoginException ex)
            {
                instance.httpClient = null;
                //MessageBox.Show(ex.Message, Properties.Strings.LoginExceptionDialogName,
                //    MessageBoxButtons.OK, MessageBoxIcon.Error,
                //    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                //return;
#if DEBUG
                MessageBox.Show(String.Format(
                    "Launcher: {0}, Exception: {1}\nMessage: {2}\n\nSource: {3}\n\n{4}",
                    appInfo.Name, ex.GetType().Name, ex.Message, ex.Source, ex.StackTrace)
                , Properties.Strings.ErrorWhileLogin, MessageBoxButtons.OK, MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
#else
                MessageBox.Show(ex.Message, Properties.Strings.ErrorWhileLogin, MessageBoxButtons.OK, MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
#endif

            }
            catch (LauncherException ex)
            {
                if (ex.OpenURL != null)
                {
                    DialogResult result = MessageBox.Show(ex.Message, Properties.Strings.AppName, MessageBoxButtons.YesNo,
                        MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    if (result == DialogResult.Yes)
                    {
                        Utils.Common.OpenUrlByDefaultBrowser(ex.OpenURL);
                    }
                }
                else
                {
#if DEBUG
                    MessageBox.Show(String.Format(
                        "Launcher: {0}, Exception: {1}\nMessage: {2}\n\nSource: {3}\n\n{4}",
                        appInfo.Name, ex.GetType().Name, ex.Message, ex.Source, ex.StackTrace)
                    , ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
#else
                    MessageBox.Show(ex.Message, Properties.Strings.ErrorWhileLogin, MessageBoxButtons.OK, MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
#endif
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(
                    "Launcher: {0}, Exception: {1}\nMessage: {2}\n\nSource: {3}\n\n{4}",
                    appInfo.Name, ex.GetType().Name, ex.Message, ex.Source, ex.StackTrace)
                , ex.GetType().Name, MessageBoxButtons.OK, MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        internal class LoginDataSet : IDisposable
        {
            private Uri _requestUri;
            private HttpStatusCode _statusCode;
            private string _content;
            private System.Net.Http.Headers.HttpContentHeaders _headers;

            public Uri RequestUri
            {
                get { return _requestUri; }
            }
            public System.Net.Http.Headers.HttpContentHeaders Headers
            {
                get { return _headers; }
            }
            public HttpStatusCode StatusCode
            {
                get { return _statusCode; }
            }

            public string Content
            {
                get { return _content; }
            }

            public LoginDataSet(HttpResponseMessage response, string content)
            {
                _requestUri = response.RequestMessage.RequestUri;
                _statusCode = response.StatusCode;
                _content = content;
                _headers = response.Content.Headers;
            }

            public LoginDataSet(HttpResponseMessage response)
            {
                _requestUri = response.RequestMessage.RequestUri;
                _statusCode = response.StatusCode;
                _headers = response.Content.Headers;
                //_content = response.Content.ReadAsStringAsync().Result;
                Stream stream = response.Content.ReadAsStreamAsync().Result;
                Encoding enc = null;
                if (response.Content.Headers.Contains("Content-Type"))
                {
                    enc = EncodingMapJapanese(response.Content.Headers.ContentType.CharSet);
                }
                else
                {
                    //enc = Encoding.GetEncoding("shift-jis");
                    enc = Encoding.UTF8;
                }
                _content = ConvertEncoding(stream, enc);

            }

            ~LoginDataSet()
            {
                Dispose();
            }

            public void Dispose()
            {
                _content = null;
                _requestUri = null;
            }

        }
    }
}
