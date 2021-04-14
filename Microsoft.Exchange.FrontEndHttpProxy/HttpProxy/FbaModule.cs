using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.HttpProxy.EventLogs;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Web.Administration;
using Microsoft.Win32;

namespace Microsoft.Exchange.HttpProxy
{
	public class FbaModule : ProxyModule
	{
		internal FbaModule()
		{
		}

		private static ExactTimeoutCache<string, byte[]> KeyCache { get; set; } = new ExactTimeoutCache<string, byte[]>(delegate(string k, byte[] v, RemoveReason r)
		{
			FbaModule.UpdateCacheSizeCounter();
		}, null, null, FbaModule.FbaKeyCacheSizeLimit.Value, false);

		internal static bool IsCadataCookie(string cookieName)
		{
			return string.Compare(cookieName, "cadata", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(cookieName, "cadataKey", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(cookieName, "cadataIV", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(cookieName, "cadataSig", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(cookieName, "cadataTTL", StringComparison.OrdinalIgnoreCase) == 0;
		}

		internal static void InvalidateKeyCache(HttpRequest httpRequest)
		{
			if (httpRequest == null)
			{
				throw new ArgumentNullException("httpRequest");
			}
			foreach (string text in FbaModule.KeyCacheCookieKeys)
			{
				string text2 = (httpRequest.Cookies[text] != null) ? httpRequest.Cookies[text].Value : null;
				if (!string.IsNullOrEmpty(text2))
				{
					ExTraceGlobals.VerboseTracer.TraceDebug<string, string>(0L, "[FbaModule::InvalidateKeyCache] Removing key cache entry {0}: {1}", text, text2);
					FbaModule.KeyCache.Remove(text2);
				}
			}
			FbaModule.UpdateCacheSizeCounter();
		}

		internal static void SetCadataCookies(HttpApplication httpApplication)
		{
			HttpContext context = httpApplication.Context;
			HttpRequest request = context.Request;
			HttpResponse response = context.Response;
			byte[] rgb = null;
			byte[] rgb2 = null;
			string s = context.Items["Authorization"] as string;
			int num = (int)context.Items["flags"];
			HttpCookieCollection cookies = request.Cookies;
			using (AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider())
			{
				aesCryptoServiceProvider.GenerateKey();
				aesCryptoServiceProvider.GenerateIV();
				rgb = aesCryptoServiceProvider.Key;
				rgb2 = aesCryptoServiceProvider.IV;
				using (ICryptoTransform cryptoTransform = aesCryptoServiceProvider.CreateEncryptor())
				{
					byte[] bytes = Encoding.Unicode.GetBytes(s);
					byte[] inArray = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
					FbaModule.CreateAndAddCookieToResponse(request, response, "cadata", Convert.ToBase64String(inArray));
				}
				FbaModule.SetCadataTtlCookie(aesCryptoServiceProvider, num, request, response);
			}
			X509Certificate2 sslCertificate = FbaModule.GetSslCertificate(request);
			RSACryptoServiceProvider rsacryptoServiceProvider = sslCertificate.PublicKey.Key as RSACryptoServiceProvider;
			byte[] inArray2 = rsacryptoServiceProvider.Encrypt(rgb, true);
			byte[] inArray3 = rsacryptoServiceProvider.Encrypt(rgb2, true);
			FbaModule.CreateAndAddCookieToResponse(request, response, "cadataKey", Convert.ToBase64String(inArray2));
			FbaModule.CreateAndAddCookieToResponse(request, response, "cadataIV", Convert.ToBase64String(inArray3));
			byte[] bytes2 = Encoding.Unicode.GetBytes("Fba Rocks!");
			byte[] inArray4 = rsacryptoServiceProvider.Encrypt(bytes2, true);
			FbaModule.CreateAndAddCookieToResponse(request, response, "cadataSig", Convert.ToBase64String(inArray4));
		}

		protected override void OnBeginRequestInternal(HttpApplication httpApplication)
		{
			this.basicAuthString = null;
			this.destinationUrl = null;
			this.userName = null;
			this.cadataKeyString = null;
			this.cadataIVString = null;
			this.symKey = null;
			this.symIV = null;
			this.flags = 0;
			this.password = null;
			httpApplication.Context.Items["AuthType"] = "FBA";
			if (!this.HandleFbaAuthFormPost(httpApplication))
			{
				try
				{
					this.ParseCadataCookies(httpApplication);
				}
				catch (MissingSslCertificateException)
				{
					AspNetHelper.TransferToErrorPage(httpApplication.Context, ErrorFE.FEErrorCodes.SSLCertificateProblem);
				}
			}
			base.OnBeginRequestInternal(httpApplication);
		}

		protected override void OnPostAuthorizeInternal(HttpApplication httpApplication)
		{
			if (this.basicAuthString != null)
			{
				HttpContext context = httpApplication.Context;
				HttpRequest request = context.Request;
				context.Items.Add("destination", this.destinationUrl);
				context.Items.Add("flags", this.flags);
				context.Items.Add("Authorization", this.basicAuthString);
				context.Items.Add("username", this.userName);
				context.Items.Add("password", this.password);
				ProxyRequestHandler proxyRequestHandler = new FbaFormPostProxyRequestHandler();
				PerfCounters.HttpProxyCountersInstance.TotalRequests.Increment();
				proxyRequestHandler.Run(context);
				return;
			}
			if (this.cadataKeyString != null && this.cadataIVString != null && this.symKey != null && this.symIV != null)
			{
				FbaModule.KeyCache.TryInsertSliding(this.cadataKeyString, this.symKey, TimeSpan.FromMinutes((double)FbaModule.DefaultPrivateKeyTimeToLiveInMinutes));
				FbaModule.KeyCache.TryInsertSliding(this.cadataIVString, this.symIV, TimeSpan.FromMinutes((double)FbaModule.DefaultPrivateKeyTimeToLiveInMinutes));
				FbaModule.UpdateCacheSizeCounter();
			}
			base.OnPostAuthorizeInternal(httpApplication);
		}

		protected override void OnEndRequestInternal(HttpApplication httpApplication)
		{
			HttpRequest request = httpApplication.Context.Request;
			HttpResponse response = httpApplication.Context.Response;
			RequestDetailsLogger current = RequestDetailsLoggerBase<RequestDetailsLogger>.GetCurrent(httpApplication.Context);
			if (httpApplication.Context.Items[Constants.RequestCompletedHttpContextKeyName] == null && !UrlUtilities.IsIntegratedAuthUrl(request.Url) && !UrlUtilities.IsOwaMiniUrl(request.Url) && (response.StatusCode == 401 || (HttpProxyGlobals.ProtocolType == ProtocolType.Ecp && (response.StatusCode == 403 || response.StatusCode == 404))))
			{
				FbaModule.LogonReason reason = FbaModule.LogonReason.None;
				if (request.Headers["Authorization"] != null)
				{
					reason = FbaModule.LogonReason.InvalidCredentials;
				}
				bool flag = request.Url.AbsolutePath.Equals("/owa/auth.owa", StringComparison.OrdinalIgnoreCase);
				if (request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase) || flag)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(current, "NoCookies", "302 - GET/E14AuthPost");
					this.RedirectToFbaLogon(httpApplication, reason);
				}
				else if (request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(current, "NoCookies", "440 - POST");
					this.Send440Response(httpApplication, true);
				}
				else
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(current, "NoCookies", "440 - " + request.HttpMethod);
					this.Send440Response(httpApplication, false);
				}
			}
			base.OnEndRequestInternal(httpApplication);
		}

		private static void DetermineKeyIntervalsIfNecessary()
		{
			if (FbaModule.haveDeterminedKeyIntervals)
			{
				return;
			}
			lock (FbaModule.LockObject)
			{
				if (!FbaModule.haveDeterminedKeyIntervals)
				{
					try
					{
						using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA", false))
						{
							if (registryKey == null)
							{
								ExTraceGlobals.VerboseTracer.TraceError(0L, "[FbaModule::DetermineKeyIntervalsIfNecessary] Error opening reg key to retrieve registry value.");
								return;
							}
							object value = registryKey.GetValue("PrivateTimeout");
							if (value != null && value is int)
							{
								int num = (int)value;
								if (num >= 1 && num <= 43200)
								{
									FbaModule.fbaPrivateKeyTTL = new TimeSpan(0, 0, (FbaModule.TtlReissueDivisor + 1) * (num * 60 / FbaModule.TtlReissueDivisor));
									FbaModule.fbaPrivateKeyReissueInterval = new TimeSpan(0, 0, num * 60 / FbaModule.TtlReissueDivisor);
								}
							}
							object value2 = registryKey.GetValue("PublicTimeout");
							if (value2 != null && value2 is int)
							{
								int num2 = (int)value2;
								if (num2 >= 1 && num2 <= 43200)
								{
									FbaModule.fbaPublicKeyTTL = new TimeSpan(0, 0, (FbaModule.TtlReissueDivisor + 1) * (num2 * 60 / FbaModule.TtlReissueDivisor));
									FbaModule.fbaPublicKeyReissueInterval = new TimeSpan(0, 0, num2 * 60 / FbaModule.TtlReissueDivisor);
								}
							}
							object value3 = registryKey.GetValue("MowaTimeout");
							if (value3 != null && value3 is int)
							{
								int num3 = (int)value3;
								if (num3 >= 1 && num3 <= 43200)
								{
									FbaModule.fbaMowaKeyTTL = new TimeSpan(0, 0, (FbaModule.TtlReissueDivisor + 1) * (num3 * 60 / FbaModule.TtlReissueDivisor));
									FbaModule.fbaMowaKeyReissueInterval = new TimeSpan(0, 0, num3 * 60 / FbaModule.TtlReissueDivisor);
								}
							}
							if (FbaModule.fbaPublicKeyTTL > FbaModule.fbaPrivateKeyTTL)
							{
								FbaModule.fbaPrivateKeyTTL = FbaModule.fbaPublicKeyTTL;
								FbaModule.fbaPrivateKeyReissueInterval = FbaModule.fbaPublicKeyReissueInterval;
							}
							if (FbaModule.fbaPrivateKeyTTL > FbaModule.fbaMowaKeyTTL)
							{
								FbaModule.fbaMowaKeyTTL = FbaModule.fbaPrivateKeyTTL;
								FbaModule.fbaMowaKeyReissueInterval = FbaModule.fbaPrivateKeyReissueInterval;
							}
						}
						FbaModule.haveDeterminedKeyIntervals = true;
					}
					catch (SecurityException)
					{
						ExTraceGlobals.VerboseTracer.TraceError(0L, "[FbaModule::DetermineKeyIntervalsIfNecessary] Security exception encountered while retrieving registry value.");
					}
					catch (UnauthorizedAccessException)
					{
						ExTraceGlobals.VerboseTracer.TraceError(0L, "[FbaModule::DetermineKeyIntervalsIfNecessary] Unauthorized exception encountered while retrieving registry value.");
					}
				}
			}
		}

		private static void SetCadataTtlCookie(AesCryptoServiceProvider aes, int flags, HttpRequest httpRequest, HttpResponse httpResponse)
		{
			using (ICryptoTransform cryptoTransform = aes.CreateEncryptor())
			{
				FbaModule.DetermineKeyIntervalsIfNecessary();
				bool flag = (flags & 4) == 4;
				bool flag2 = FbaModule.IsMowa(httpRequest, flag);
				ExDateTime exDateTime = ExDateTime.UtcNow.AddTicks(flag2 ? FbaModule.fbaMowaKeyTTL.Ticks : (flag ? FbaModule.fbaPrivateKeyTTL.Ticks : FbaModule.fbaPublicKeyTTL.Ticks));
				byte[] array = new byte[9];
				ExBitConverter.Write(exDateTime.UtcTicks, array, 0);
				array[8] = (byte)flags;
				byte[] inArray = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
				FbaModule.CreateAndAddCookieToResponse(httpRequest, httpResponse, "cadataTTL", Convert.ToBase64String(inArray));
			}
		}

		private static bool IsMowa(HttpRequest request, bool isTrusted)
		{
			return isTrusted && request.Headers["X-OWA-Protocol"] == "MOWA";
		}

		private static void CreateAndAddCookieToResponse(HttpRequest request, HttpResponse response, string name, string value)
		{
			HttpCookie httpCookie = new HttpCookie(name, value);
			httpCookie.HttpOnly = true;
			httpCookie.Secure = request.IsSecureConnection;
			response.Cookies.Add(httpCookie);
		}

		private static void UpdateCacheSizeCounter()
		{
			PerfCounters.HttpProxyCacheCountersInstance.FbaModuleKeyCacheSize.RawValue = (long)FbaModule.KeyCache.Count;
		}

		private static string GetExternalUrlScheme(ref int port)
		{
			if (port == 80)
			{
				port = 443;
			}
			return Uri.UriSchemeHttps;
		}

		private static X509Certificate2 GetSslCertificate(HttpRequest httpRequest)
		{
			if (!FbaModule.loadedSslCert)
			{
				lock (FbaModule.LockObject)
				{
					if (!FbaModule.loadedSslCert)
					{
						X509Certificate2 x509Certificate = FbaModule.LoadSslCertificate(httpRequest);
						if (x509Certificate == null)
						{
							ExTraceGlobals.VerboseTracer.TraceError(0L, "[FbaModule::GetSslCertificate] LoadSslCertificate returns null.");
							Diagnostics.Logger.LogEvent(FrontEndHttpProxyEventLogConstants.Tuple_ErrorLoadingSslCert, null, new object[]
							{
								HttpProxyGlobals.ProtocolType.ToString()
							});
						}
						FbaModule.sslCert = x509Certificate;
						FbaModule.loadedSslCert = true;
					}
				}
			}
			if (FbaModule.sslCert == null)
			{
				throw new MissingSslCertificateException();
			}
			return FbaModule.sslCert;
		}

		private static X509Certificate2 LoadSslCertificate(HttpRequest httpRequest)
		{
			ExTraceGlobals.VerboseTracer.TraceDebug(0L, "[FbaModule::LoadSslCertificate] Loading SSL certificate.");
			NameValueCollection serverVariables = httpRequest.ServerVariables;
			string text = serverVariables["INSTANCE_ID"];
			if (string.IsNullOrEmpty(text))
			{
				ExTraceGlobals.VerboseTracer.TraceError(0L, "[FbaModule::LoadSslCertificate] INSTANCE_ID server variable was returned as null or empty!");
				return null;
			}
			int num;
			if (!int.TryParse(text, out num))
			{
				ExTraceGlobals.VerboseTracer.TraceError<string>(0L, "[FbaModule::LoadSslCertificate] Could not parse instance id {0}", text);
				return null;
			}
			byte[] array = null;
			string text2 = null;
			using (ServerManager serverManager = new ServerManager())
			{
				Site site = null;
				foreach (Site site2 in serverManager.Sites)
				{
					if (site2.Id == (long)num)
					{
						site = site2;
						break;
					}
				}
				if (site == null)
				{
					ExTraceGlobals.VerboseTracer.TraceError<int>(0L, "[FbaModule::LoadSslCertificate] Could not find site with id {0}", num);
					return null;
				}
				foreach (Binding binding in site.Bindings)
				{
					if (binding.Protocol == "https")
					{
						array = binding.CertificateHash;
						text2 = binding.CertificateStoreName;
						if (array != null && text2 != null)
						{
							break;
						}
					}
				}
				if (array == null || text2 == null)
				{
					ExTraceGlobals.VerboseTracer.TraceError(0L, "[FbaModule::LoadSslCertificate] Could not find certificate information in bindings");
					return null;
				}
			}
			X509Certificate2 x509Certificate = null;
			X509Store x509Store = new X509Store(text2, StoreLocation.LocalMachine);
			x509Store.Open(OpenFlags.OpenExistingOnly);
			try
			{
				foreach (X509Certificate2 x509Certificate2 in x509Store.Certificates)
				{
					byte[] certHash = x509Certificate2.GetCertHash();
					if (certHash.Length == array.Length)
					{
						bool flag = true;
						for (int i = 0; i < certHash.Length; i++)
						{
							if (certHash[i] != array[i])
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							x509Certificate = x509Certificate2;
							break;
						}
					}
				}
			}
			finally
			{
				x509Store.Close();
			}
			if (x509Certificate == null)
			{
				ExTraceGlobals.VerboseTracer.TraceError(0L, "[FbaModule::LoadSslCertificate] Could not find SSL certificate in store.");
				return null;
			}
			return x509Certificate;
		}

		private bool RedirectToFbaLogon(HttpApplication httpApplication, FbaModule.LogonReason reason)
		{
			HttpContext context = httpApplication.Context;
			HttpRequest request = context.Request;
			HttpResponse response = context.Response;
			Utility.DeleteFbaAuthCookies(request, response);
			UriBuilder uriBuilder = new UriBuilder();
			uriBuilder.Host = request.Url.Host;
			int port = uriBuilder.Port;
			uriBuilder.Scheme = FbaModule.GetExternalUrlScheme(ref port);
			uriBuilder.Port = port;
			uriBuilder.Path = "/owa/auth/logon.aspx";
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("url=");
			if (this.destinationUrl != null)
			{
				stringBuilder.Append(HttpUtility.UrlEncode(new UriBuilder(this.destinationUrl)
				{
					Scheme = uriBuilder.Scheme,
					Port = uriBuilder.Port
				}.Uri.AbsoluteUri.ToString()));
			}
			else
			{
				string text = new UriBuilder(request.GetFullRawUrl())
				{
					Scheme = uriBuilder.Scheme,
					Port = uriBuilder.Port
				}.Uri.AbsoluteUri;
				string strB = request.Url.Segments[request.Url.Segments.Length - 1];
				if (string.Compare("auth.owa", strB, StringComparison.OrdinalIgnoreCase) == 0)
				{
					int startIndex = text.LastIndexOf("auth.owa") - 1;
					text = text.Remove(startIndex);
				}
				string text2 = HttpUtility.UrlDecode(request.Headers["X-OWA-ExplicitLogonUser"]);
				if (!string.IsNullOrEmpty(text2) && !text.Contains(text2))
				{
					string value = HttpUtility.UrlEncode("/");
					string applicationPath = request.ApplicationPath;
					int num = text.IndexOf(applicationPath, StringComparison.OrdinalIgnoreCase);
					if (num == -1)
					{
						stringBuilder.Append(HttpUtility.UrlEncode(text));
						if (text[text.Length - 1] != '/')
						{
							stringBuilder.Append(value);
						}
						stringBuilder.Append(HttpUtility.UrlEncode(text2));
						stringBuilder.Append(value);
					}
					else
					{
						num += applicationPath.Length;
						if (num < text.Length && text[num] == '/')
						{
							num++;
						}
						stringBuilder.Append(HttpUtility.UrlEncode(text.Substring(0, num)));
						if (text[num - 1] != '/')
						{
							stringBuilder.Append(value);
						}
						stringBuilder.Append(HttpUtility.UrlEncode(text2));
						stringBuilder.Append(value);
						stringBuilder.Append(HttpUtility.UrlEncode(text.Substring(num)));
					}
				}
				else
				{
					int num2 = text.IndexOf('?');
					string text3 = null;
					if (text.ToLowerInvariant().Contains("logoff.owa"))
					{
						if (!LogOnSettings.IsLegacyLogOff)
						{
							uriBuilder.Path = "/owa/" + LogOnSettings.SignOutPageUrl;
						}
						if (num2 >= 0)
						{
							string[] source = text.Substring(num2 + 1).Split(new char[]
							{
								'&'
							});
							string text4 = source.FirstOrDefault((string x) => x.StartsWith("url=", StringComparison.OrdinalIgnoreCase));
							if (text4 != null)
							{
								text3 = text4.Substring("url=".Length);
							}
						}
					}
					if (text3 == null)
					{
						string str;
						text3 = ((!UrlUtilities.IsCmdWebPart(request) && UrlUtilities.ShouldRedirectQueryParamsAsHashes(new Uri(text), out str)) ? HttpUtility.UrlEncode(str) : HttpUtility.UrlEncode(text));
					}
					stringBuilder.Append(text3);
				}
			}
			stringBuilder.AppendFormat("&reason={0}", (int)reason);
			uriBuilder.Query = stringBuilder.ToString();
			ExTraceGlobals.VerboseTracer.TraceDebug<FbaModule.LogonReason, string>((long)this.GetHashCode(), "RedirectToFbaLogon - Reason: {0}, URL: {1}", reason, uriBuilder.ToString());
			base.PfdTracer.TraceRedirect("FbaAuth", uriBuilder.ToString());
			response.Redirect(uriBuilder.ToString(), false);
			httpApplication.CompleteRequest();
			return true;
		}

		private void Send440Response(HttpApplication httpApplication, bool isPost)
		{
			HttpRequest request = httpApplication.Context.Request;
			HttpResponse response = httpApplication.Context.Response;
			response.StatusCode = 440;
			response.StatusDescription = "Login Timeout";
			Utility.DeleteFbaAuthCookies(request, response);
			response.ContentType = "text/html";
			response.Headers["Connection"] = "close";
			if (isPost)
			{
				response.Output.Write("<HTML><SCRIPT>if (parent.navbar != null) parent.location = self.location;else self.location = self.location;</SCRIPT><BODY>440 Login Timeout</BODY></HTML>");
			}
			else
			{
				response.Output.Write("<HTML><BODY>440 Login Timeout</BODY></HTML>");
			}
			base.PfdTracer.TraceResponse("FbaAuth", response);
			httpApplication.CompleteRequest();
		}

		private bool HandleFbaAuthFormPost(HttpApplication httpApplication)
		{
			HttpContext context = httpApplication.Context;
			HttpRequest request = context.Request;
			HttpResponse response = context.Response;
			if (request.GetHttpMethod() != HttpMethod.Post)
			{
				return false;
			}
			string strB = request.Url.Segments[request.Url.Segments.Length - 1];
			if (string.Compare("auth.owa", strB, StringComparison.OrdinalIgnoreCase) != 0 && string.Compare("owaauth.dll", strB, StringComparison.OrdinalIgnoreCase) != 0)
			{
				return false;
			}
			if (string.IsNullOrEmpty(request.ContentType))
			{
				request.ContentType = "application/x-www-form-urlencoded";
			}
			SecureHtmlFormReader secureHtmlFormReader = new SecureHtmlFormReader(request);
			secureHtmlFormReader.AddSensitiveInputName("password");
			SecureNameValueCollection secureNameValueCollection = null;
			try
			{
				if (!secureHtmlFormReader.TryReadSecureFormData(out secureNameValueCollection))
				{
					AspNetHelper.EndResponse(context, HttpStatusCode.BadRequest);
				}
				string text = null;
				string text2 = null;
				SecureString secureString = null;
				string text3 = null;
				secureNameValueCollection.TryGetUnsecureValue("username", out text2);
				secureNameValueCollection.TryGetSecureValue("password", out secureString);
				secureNameValueCollection.TryGetUnsecureValue("destination", out text);
				secureNameValueCollection.TryGetUnsecureValue("flags", out text3);
				if (text == null || text2 == null || secureString == null || text3 == null || !this.CheckPostDestination(text, context.Request))
				{
					AspNetHelper.EndResponse(context, HttpStatusCode.BadRequest);
				}
				this.password = secureString.Copy();
				this.userName = text2;
				this.destinationUrl = text;
				int num;
				if (int.TryParse(text3, NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
				{
					this.flags = num;
				}
				else
				{
					this.flags = 0;
				}
				text2 += ":";
				Encoding @default = Encoding.Default;
				int maxByteCount = @default.GetMaxByteCount(text2.Length + secureString.Length);
				using (SecureArray<byte> secureArray = new SecureArray<byte>(maxByteCount))
				{
					int num2 = @default.GetBytes(text2, 0, text2.Length, secureArray.ArrayValue, 0);
					using (SecureArray<char> secureArray2 = secureString.ConvertToSecureCharArray())
					{
						num2 += @default.GetBytes(secureArray2.ArrayValue, 0, secureArray2.Length(), secureArray.ArrayValue, num2);
						this.basicAuthString = "Basic " + Convert.ToBase64String(secureArray.ArrayValue, 0, num2);
						request.Headers["Authorization"] = this.basicAuthString;
					}
				}
			}
			finally
			{
				if (secureNameValueCollection != null)
				{
					secureNameValueCollection.Dispose();
				}
			}
			ExTraceGlobals.VerboseTracer.TraceDebug<Uri>(0L, "HandleFbaAuthFormPost - {0}", request.Url);
			return true;
		}

		private void ParseCadataCookies(HttpApplication httpApplication)
		{
			HttpContext context = httpApplication.Context;
			HttpRequest request = context.Request;
			HttpResponse response = context.Response;
			RequestDetailsLogger current = RequestDetailsLoggerBase<RequestDetailsLogger>.GetCurrent(context);
			string text = null;
			if (request.Cookies["cadata"] != null && request.Cookies["cadata"].Value != null)
			{
				text = request.Cookies["cadata"].Value;
			}
			string text2 = null;
			if (request.Cookies["cadataKey"] != null && request.Cookies["cadataKey"].Value != null)
			{
				text2 = request.Cookies["cadataKey"].Value;
			}
			string text3 = null;
			if (request.Cookies["cadataIV"] != null && request.Cookies["cadataIV"].Value != null)
			{
				text3 = request.Cookies["cadataIV"].Value;
			}
			string text4 = null;
			if (request.Cookies["cadataSig"] != null && request.Cookies["cadataSig"].Value != null)
			{
				text4 = request.Cookies["cadataSig"].Value;
			}
			string text5 = null;
			if (request.Cookies["cadataTTL"] != null && request.Cookies["cadataTTL"].Value != null)
			{
				text5 = request.Cookies["cadataTTL"].Value;
			}
			if (text == null || text2 == null || text3 == null || text4 == null || text5 == null)
			{
				return;
			}
			byte[] array = null;
			byte[] array2 = null;
			PerfCounters.HttpProxyCacheCountersInstance.FbaModuleKeyCacheHitsRateBase.Increment();
			FbaModule.KeyCache.TryGetValue(text2, out array);
			FbaModule.KeyCache.TryGetValue(text3, out array2);
			if (array != null && array2 != null)
			{
				PerfCounters.HttpProxyCacheCountersInstance.FbaModuleKeyCacheHitsRate.Increment();
				goto IL_362;
			}
			string text6 = null;
			RSACryptoServiceProvider rsacryptoServiceProvider;
			try
			{
				X509Certificate2 sslCertificate = FbaModule.GetSslCertificate(request);
				rsacryptoServiceProvider = (sslCertificate.PrivateKey as RSACryptoServiceProvider);
				if (rsacryptoServiceProvider != null)
				{
					byte[] rgb = Convert.FromBase64String(text4);
					byte[] bytes = rsacryptoServiceProvider.Decrypt(rgb, true);
					string @string = Encoding.Unicode.GetString(bytes);
					if (string.Compare(@string, "Fba Rocks!", StringComparison.Ordinal) != 0)
					{
						text6 = "does not match the SSL certificate on the Cafe web-site on another server in this Cafe array";
					}
				}
				else
				{
					text6 = "does not contain RSACryptoServiceProvider";
					if (ExTraceGlobals.VerboseTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						ExTraceGlobals.VerboseTracer.TraceDebug((long)this.GetHashCode(), "[FbaModule::ParseCadataCookies] Certificate:{0},Name:{1},Thumbprint:{2},PrivateKeyKey.(Exchange/Signature)Algorighm:{3} has no RSACryptoServiceProvider", new object[]
						{
							sslCertificate.Subject,
							sslCertificate.FriendlyName,
							sslCertificate.Thumbprint,
							(sslCertificate.PrivateKey == null) ? "NULL" : (sslCertificate.PrivateKey.KeyExchangeAlgorithm + "/" + sslCertificate.PrivateKey.SignatureAlgorithm)
						});
					}
				}
			}
			catch (CryptographicException arg)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<CryptographicException>((long)this.GetHashCode(), "[FbaModule::ParseCadataCookies] Received CryptographicException {0} decrypting cadataSig", arg);
				return;
			}
			if (text6 == null)
			{
				byte[] rgb2 = Convert.FromBase64String(text2);
				byte[] rgb3 = Convert.FromBase64String(text3);
				try
				{
					array = rsacryptoServiceProvider.Decrypt(rgb2, true);
					array2 = rsacryptoServiceProvider.Decrypt(rgb3, true);
				}
				catch (CryptographicException arg2)
				{
					ExTraceGlobals.VerboseTracer.TraceDebug<CryptographicException>((long)this.GetHashCode(), "[FbaModule::ParseCadataCookies] Received CryptographicException {0} decrypting symKey/symIV", arg2);
					return;
				}
				this.cadataKeyString = text2;
				this.cadataIVString = text3;
				this.symKey = array;
				this.symIV = array2;
				goto IL_362;
			}
			ExTraceGlobals.VerboseTracer.TraceError<string, string>((long)this.GetHashCode(), "[FbaModule::ParseCadataCookies] {0} {1}", "Error in validating Cadata signature. This most likely indicates that the SSL certifcate on the Cafe web-site on this server ", text6);
			return;
			IL_362:
			using (AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider())
			{
				aesCryptoServiceProvider.Key = array;
				aesCryptoServiceProvider.IV = array2;
				using (ICryptoTransform cryptoTransform = aesCryptoServiceProvider.CreateDecryptor())
				{
					byte[] array3 = Convert.FromBase64String(text5);
					byte[] array4 = null;
					try
					{
						array4 = cryptoTransform.TransformFinalBlock(array3, 0, array3.Length);
					}
					catch (CryptographicException arg3)
					{
						ExTraceGlobals.VerboseTracer.TraceDebug<CryptographicException>((long)this.GetHashCode(), "[FbaModule::ParseCadataCookies] Received CryptographicException {0} transforming TTL", arg3);
						return;
					}
					if (array4.Length < 1)
					{
						ExTraceGlobals.VerboseTracer.TraceDebug((long)this.GetHashCode(), "[FbaModule::ParseCadataCookies] TTL length was less than 1.");
						return;
					}
					long ticks = BitConverter.ToInt64(array4, 0);
					int num = (int)array4[8];
					bool flag = (num & 4) == 4;
					context.Items["Flags"] = num;
					ExDateTime t = new ExDateTime(ExTimeZone.UtcTimeZone, ticks);
					ExDateTime utcNow = ExDateTime.UtcNow;
					if (t < utcNow)
					{
						if (request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase))
						{
							if (request.QueryString.ToString().StartsWith("oeh=1&", StringComparison.OrdinalIgnoreCase))
							{
								RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(current, "LoginTimeout", "440 - GET/OEH");
								this.Send440Response(httpApplication, false);
							}
							else
							{
								RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(current, "LoginTimeout", "302 - GET/Timeout");
								this.RedirectToFbaLogon(httpApplication, FbaModule.LogonReason.Timeout);
							}
						}
						else if (request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
						{
							RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(current, "LoginTimeout", "440 - POST");
							this.Send440Response(httpApplication, true);
						}
						else
						{
							RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(current, "LoginTimeout", "440 - " + request.HttpMethod);
							this.Send440Response(httpApplication, false);
						}
						return;
					}
					FbaModule.DetermineKeyIntervalsIfNecessary();
					ExDateTime t2 = t.AddTicks(-2L * (flag ? FbaModule.fbaPrivateKeyReissueInterval.Ticks : FbaModule.fbaPublicKeyReissueInterval.Ticks));
					if (t2 < utcNow && OwaAuthenticationHelper.IsOwaUserActivityRequest(request))
					{
						FbaModule.SetCadataTtlCookie(aesCryptoServiceProvider, num, request, response);
					}
				}
				using (ICryptoTransform cryptoTransform2 = aesCryptoServiceProvider.CreateDecryptor())
				{
					byte[] array5 = Convert.FromBase64String(text);
					byte[] bytes2 = null;
					try
					{
						bytes2 = cryptoTransform2.TransformFinalBlock(array5, 0, array5.Length);
					}
					catch (CryptographicException arg4)
					{
						ExTraceGlobals.VerboseTracer.TraceDebug<CryptographicException>((long)this.GetHashCode(), "[FbaModule::ParseCadataCookies] Received CryptographicException {0} transforming auth", arg4);
						return;
					}
					string string2 = Encoding.Unicode.GetString(bytes2);
					request.Headers["Authorization"] = string2;
				}
			}
		}

		private bool CheckPostDestination(string destination, HttpRequest request)
		{
			if (string.IsNullOrEmpty(destination))
			{
				return false;
			}
			if (destination.StartsWith("/", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			try
			{
				Uri uri = new Uri(destination);
				if (!string.Equals(uri.Host, request.Url.Host, StringComparison.OrdinalIgnoreCase))
				{
					ExTraceGlobals.VerboseTracer.TraceError<string>((long)this.GetHashCode(), "[FbaModule::CheckPostRequestDestination] Destination URL {0} does not match the request host, generating 400 response.", destination);
					return false;
				}
			}
			catch (UriFormatException)
			{
				ExTraceGlobals.VerboseTracer.TraceError<string>((long)this.GetHashCode(), "[FbaModule::CheckPostRequestDestination] Destination URL {0} is not a valid URL, generating 400 response.", destination);
				return false;
			}
			return true;
		}

		internal const string LogoffPage = "/logoff.owa";

		internal const string FlagsHttpContextKeyName = "Flags";

		internal const byte IsDownlevelFlagValue = 1;

		internal const byte IsTrustedFlagValue = 4;

		internal const string PasswordFormName = "password";

		internal const string DestinationFormName = "destination";

		internal const string UsernameFormName = "username";

		internal const string FlagsFormName = "flags";

		internal const string BasicAuthHeader = "Authorization";

		private const string AuthPost = "auth.owa";

		private const string LegacyAuthPost = "owaauth.dll";

		private const string LogonPage = "/auth/logon.aspx";

		private const string LogonPath = "/owa";

		private const string E14OwaAuthPost = "/owa/auth.owa";

		private const string HttpGetMethod = "GET";

		private const string HttpPostMethod = "POST";

		private const string OehParameter = "oeh=1&";

		private const string CadataSig = "Fba Rocks!";

		private const string BasicHeaderValue = "Basic ";

		private const string CertServerIssuer = "CERT_SERVER_ISSUER";

		private const string CertServerSubject = "CERT_SERVER_SUBJECT";

		private const string ResponseBody440 = "<HTML><BODY>440 Login Timeout</BODY></HTML>";

		private const string ResponseBody440Post = "<HTML><SCRIPT>if (parent.navbar != null) parent.location = self.location;else self.location = self.location;</SCRIPT><BODY>440 Login Timeout</BODY></HTML>";

		private const string PasswordResetPage = "/auth/expiredpassword.aspx";

		private const int ErrorPasswordMustChange = 460659;

		private const int ErrorPasswordExpired = 460082;

		private const string CommaSpace = ", ";

		private const string FbaPrivateCookieTTLValueName = "PrivateTimeout";

		private const string FbaPublicCookieTTLValueName = "PublicTimeout";

		private const string FbaMowaCookieTTLValueName = "MowaTimeout";

		private const int FbaMinimumTimeout = 1;

		private const int FbaMaximumTimeout = 43200;

		private static readonly int DefaultPrivateKeyTimeToLiveInMinutes = 480;

		private static readonly int DefaultPublicKeyTimeToLiveInMinutes = 15;

		private static readonly int MowaKeyTimeToLiveInMinutes = 960;

		private static readonly int TtlReissueDivisor = 2;

		private static readonly string[] CommaSpaceArray = new string[]
		{
			", "
		};

		private static readonly IntAppSettingsEntry FbaKeyCacheSizeLimit = new IntAppSettingsEntry(HttpProxySettings.Prefix("FbaKeyCacheSizeLimit"), 25000, ExTraceGlobals.VerboseTracer);

		private static readonly string[] KeyCacheCookieKeys = new string[]
		{
			"cadataKey",
			"cadataIV"
		};

		private static readonly object LockObject = new object();

		private static TimeSpan fbaPrivateKeyTTL = new TimeSpan(0, (FbaModule.TtlReissueDivisor + 1) * (FbaModule.DefaultPrivateKeyTimeToLiveInMinutes / FbaModule.TtlReissueDivisor), 0);

		private static TimeSpan fbaPrivateKeyReissueInterval = new TimeSpan(0, 0, FbaModule.DefaultPrivateKeyTimeToLiveInMinutes * 60 / FbaModule.TtlReissueDivisor);

		private static TimeSpan fbaPublicKeyTTL = new TimeSpan(0, 0, (FbaModule.TtlReissueDivisor + 1) * (FbaModule.DefaultPublicKeyTimeToLiveInMinutes * 60 / FbaModule.TtlReissueDivisor));

		private static TimeSpan fbaPublicKeyReissueInterval = new TimeSpan(0, 0, FbaModule.DefaultPublicKeyTimeToLiveInMinutes * 60 / FbaModule.TtlReissueDivisor);

		private static TimeSpan fbaMowaKeyTTL = new TimeSpan(0, (FbaModule.TtlReissueDivisor + 1) * (FbaModule.MowaKeyTimeToLiveInMinutes / FbaModule.TtlReissueDivisor), 0);

		private static TimeSpan fbaMowaKeyReissueInterval = new TimeSpan(0, 0, FbaModule.MowaKeyTimeToLiveInMinutes * 60 / FbaModule.TtlReissueDivisor);

		private static bool haveDeterminedKeyIntervals = false;

		private static bool loadedSslCert = false;

		private static X509Certificate2 sslCert;

		private string basicAuthString;

		private string destinationUrl;

		private string userName;

		private SecureString password;

		private string cadataKeyString;

		private string cadataIVString;

		private byte[] symKey;

		private byte[] symIV;

		private int flags;

		protected enum LogonReason
		{
			None,
			Logoff,
			InvalidCredentials,
			Timeout,
			ChangePasswordLogoff,
			BlockedByClientAccessRules,
			EACBlockedByClientAccessRules
		}
	}
}
