using System;
using System.Globalization;
using System.Net;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class GccUtils
	{
		public static DatacenterServerAuthentication DatacenterServerAuthentication
		{
			get
			{
				return GccUtils.DatacenterServerAuth;
			}
		}

		public static string CurrentProxyKey
		{
			get
			{
				if (!GccUtils.IsGlobalCriminalComplianceEnabled)
				{
					return string.Empty;
				}
				return Convert.ToBase64String(GccUtils.DatacenterServerAuth.CurrentSecretKey);
			}
		}

		public static string PreviousProxyKey
		{
			get
			{
				if (!GccUtils.IsGlobalCriminalComplianceEnabled)
				{
					return string.Empty;
				}
				return Convert.ToBase64String(GccUtils.DatacenterServerAuth.PreviousSecretKey);
			}
		}

		internal static bool IsGlobalCriminalComplianceEnabled
		{
			get
			{
				return GccUtils.isGlobalCriminalComplianceEnabled.Member;
			}
		}

		public static bool AreStoredSecretKeysValid()
		{
			if (!GccUtils.IsGlobalCriminalComplianceEnabled)
			{
				return false;
			}
			bool result;
			try
			{
				GccUtils.RefreshProxySecretKeys();
				result = true;
			}
			catch (InvalidDatacenterProxyKeyException)
			{
				result = false;
			}
			return result;
		}

		public static bool SetStoreSessionClientIPEndpointsFromHttpRequest(StoreSession session, HttpRequest httpRequest, bool useServerToServerHeaders)
		{
			bool result = true;
			HttpContext httpContext = null;
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (httpRequest == null)
			{
				httpContext = HttpContext.Current;
				if (httpContext != null)
				{
					httpRequest = httpContext.Request;
				}
			}
			IPAddress ipv6None;
			IPAddress ipv6Loopback;
			if (httpRequest != null)
			{
				result = GccUtils.InternalGetClientIPEndpointsFromHttpRequest((httpContext != null) ? new HttpContextWrapper(httpContext) : null, new HttpRequestWrapper(httpRequest), out ipv6None, out ipv6Loopback, useServerToServerHeaders, false);
			}
			else
			{
				ipv6None = IPAddress.IPv6None;
				ipv6Loopback = IPAddress.IPv6Loopback;
			}
			session.SetClientIPEndpoints(ipv6None, ipv6Loopback);
			return result;
		}

		public static bool SetStoreSessionClientIPEndpointsFromHttpRequest(StoreSession session, HttpRequest httpRequest)
		{
			return GccUtils.SetStoreSessionClientIPEndpointsFromHttpRequest(session, httpRequest, false);
		}

		public static bool SetStoreSessionClientIPEndpointsFromXproxy(StoreSession session, string authString, string clientIp, string serverIp, NetworkConnection connection)
		{
			if (!GccUtils.IsGlobalCriminalComplianceEnabled)
			{
				return false;
			}
			bool result = true;
			IPAddress address;
			IPAddress address2;
			if (GccUtils.IsValidAuthString(authString))
			{
				if (!IPAddress.TryParse(clientIp, out address))
				{
					address = connection.RemoteEndPoint.Address;
					result = false;
				}
				if (!IPAddress.TryParse(serverIp, out address2))
				{
					address2 = connection.LocalEndPoint.Address;
					result = false;
				}
			}
			else
			{
				result = false;
				address = connection.RemoteEndPoint.Address;
				address2 = connection.LocalEndPoint.Address;
			}
			session.SetClientIPEndpoints(address, address2);
			return result;
		}

		public static void CopyClientIPEndpointsForServerToServerProxy(HttpContext originalContext, HttpWebRequest targetRequest)
		{
			if (originalContext == null)
			{
				throw new ArgumentNullException("originalContext");
			}
			if (targetRequest == null)
			{
				throw new ArgumentNullException("targetRequest");
			}
			IPAddress ipaddress;
			IPAddress ipaddress2;
			GccUtils.InternalGetClientIPEndpointsFromHttpRequest(new HttpContextWrapper(originalContext), null, out ipaddress, out ipaddress2, false, false);
			targetRequest.Headers["x-ServerToServer-OriginalClient"] = ipaddress.ToString();
			targetRequest.Headers["x-ServerToServer-OriginalServer"] = ipaddress2.ToString();
		}

		public static string GetAuthStringForThisServer()
		{
			if (!GccUtils.IsGlobalCriminalComplianceEnabled)
			{
				return string.Empty;
			}
			GccUtils.EnsureProxyKeysAreLoaded();
			return GccUtils.DatacenterServerAuth.GetAuthenticationString();
		}

		public static bool IsValidAuthString(string authString)
		{
			if (!GccUtils.IsGlobalCriminalComplianceEnabled)
			{
				return false;
			}
			if (string.IsNullOrEmpty(authString))
			{
				return false;
			}
			GccUtils.EnsureProxyKeysAreLoaded();
			return GccUtils.DatacenterServerAuth.ValidateAuthenticationString(authString);
		}

		public static void RefreshProxySecretKeys()
		{
			if (!GccUtils.IsGlobalCriminalComplianceEnabled)
			{
				return;
			}
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeLabs");
			if (registryKey == null)
			{
				throw new InvalidDatacenterProxyKeyException();
			}
			string text = (string)registryKey.GetValue("CurrentProxyKey", null);
			string previousKeyBase = (string)registryKey.GetValue("PreviousProxyKey", null);
			string currentIVBase = (string)registryKey.GetValue("CurrentIVProxyKey", null);
			string previousIVBase = (string)registryKey.GetValue("PreviousIVProxyKey", null);
			registryKey.Close();
			if (string.IsNullOrEmpty(text) || !GccUtils.DatacenterServerAuth.TrySetCurrentAndPreviousSecretKeys(text, previousKeyBase, currentIVBase, previousIVBase))
			{
				throw new InvalidDatacenterProxyKeyException();
			}
		}

		public static bool GetClientIPEndpointsFromHttpRequest(HttpContext httpContext, out IPAddress clientIPAddress, out IPAddress serverIPAddress, bool useServerToServerHeaders, bool trustEntireForwardedForHeader)
		{
			return GccUtils.InternalGetClientIPEndpointsFromHttpRequest(new HttpContextWrapper(httpContext), null, out clientIPAddress, out serverIPAddress, useServerToServerHeaders, trustEntireForwardedForHeader);
		}

		public static bool GetClientIPEndpointsFromHttpRequest(HttpContextBase httpContext, out IPAddress clientIPAddress, out IPAddress serverIPAddress, bool useServerToServerHeaders, bool trustEntireForwardedForHeader)
		{
			return GccUtils.InternalGetClientIPEndpointsFromHttpRequest(httpContext, null, out clientIPAddress, out serverIPAddress, useServerToServerHeaders, trustEntireForwardedForHeader);
		}

		public static string GetServerAddress(HttpContext httpContext)
		{
			return GccUtils.InternalGetServerAddress(new HttpContextWrapper(httpContext), null);
		}

		public static string GetClientAddress(HttpContext httpContext)
		{
			return GccUtils.InternalGetClientAddress(new HttpContextWrapper(httpContext), null);
		}

		public static string GetClientAddress(HttpContextBase httpContext)
		{
			return GccUtils.InternalGetClientAddress(httpContext, null);
		}

		public static string GetClientPort(HttpContext httpContext)
		{
			return GccUtils.InternalGetClientPort(new HttpContextWrapper(httpContext), null);
		}

		public static bool TryGetGccProxyInfo(HttpContext httpContext, out string gccProxyInfo)
		{
			gccProxyInfo = null;
			bool flag;
			string text;
			string text2;
			if (!GccUtils.InternalTryGetGccProxyInfo(new HttpContextWrapper(httpContext), null, out gccProxyInfo, out flag, out text, out text2) || !flag)
			{
				gccProxyInfo = null;
				return false;
			}
			return true;
		}

		public static bool TryCreateGccProxyInfo(HttpContext httpContext, out string gccProxyInfo)
		{
			return GccUtils.InternalTryCreateGccProxyInfo(new HttpContextWrapper(httpContext), null, out gccProxyInfo);
		}

		private static bool InternalGetClientIPEndpointsFromHttpRequest(HttpContextBase httpContext, HttpRequestBase httpRequest, out IPAddress clientIPAddress, out IPAddress serverIPAddress, bool useServerToServerHeaders, bool trustEntireForwardedForHeader)
		{
			if (httpContext != null)
			{
				if (httpRequest == null)
				{
					httpRequest = httpContext.Request;
				}
			}
			else if (httpRequest == null)
			{
				throw new ArgumentException("HttpContext and HttpRequest cannot both be null");
			}
			bool flag = false;
			bool flag2 = true;
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = null;
			if (useServerToServerHeaders)
			{
				text = httpRequest.Headers["x-ServerToServer-OriginalClient"];
				text2 = httpRequest.Headers["x-ServerToServer-OriginalServer"];
			}
			if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text2))
			{
				string text4;
				if (GccUtils.InternalTryGetGccProxyInfo(httpContext, httpRequest, out text4, out flag, out text, out text2))
				{
					flag2 = false;
				}
				else
				{
					text = httpRequest.Headers["X-Forwarded-For"];
					if (!string.IsNullOrEmpty(text))
					{
						text3 = GccUtils.InternalGetServerAddress(httpContext, httpRequest);
						text2 = text3;
						if (trustEntireForwardedForHeader)
						{
							int num = text.IndexOf(',');
							if (num > -1)
							{
								text = text.Substring(0, num);
							}
						}
						else
						{
							int num2 = text.LastIndexOf(',');
							if (num2 > -1)
							{
								text = text.Substring(num2 + 1);
							}
						}
						flag2 = false;
						flag = true;
					}
				}
				if (flag2 || !flag)
				{
					text = GccUtils.InternalGetClientAddress(httpContext, httpRequest);
					text2 = (text3 ?? GccUtils.InternalGetServerAddress(httpContext, httpRequest));
				}
			}
			if (string.IsNullOrEmpty(text) || !IPAddress.TryParse(text, out clientIPAddress))
			{
				clientIPAddress = IPAddress.IPv6None;
			}
			if (string.IsNullOrEmpty(text2) || !IPAddress.TryParse(text2, out serverIPAddress))
			{
				serverIPAddress = IPAddress.IPv6Loopback;
			}
			return flag2 || flag;
		}

		private static TReturn InternalExecuteWorkerRequest<TReturn>(HttpContextBase httpContext, HttpRequestBase httpRequest, Func<HttpWorkerRequest, TReturn> httpWorkerDelegate, Func<HttpRequestBase, TReturn> httpRequestDelegate)
		{
			if (httpContext != null)
			{
				HttpWorkerRequest httpWorkerRequest = (HttpWorkerRequest)httpContext.GetService(typeof(HttpWorkerRequest));
				if (httpWorkerRequest != null)
				{
					return httpWorkerDelegate(httpWorkerRequest);
				}
				if (httpRequest == null)
				{
					httpRequest = httpContext.Request;
				}
			}
			else if (httpRequest == null)
			{
				throw new ArgumentException("HttpContext and HttpRequest cannot both be null");
			}
			return httpRequestDelegate(httpRequest);
		}

		private static string InternalGetServerAddress(HttpContextBase httpContext, HttpRequestBase httpRequest)
		{
			return GccUtils.InternalExecuteWorkerRequest<string>(httpContext, httpRequest, (HttpWorkerRequest x) => x.GetLocalAddress(), (HttpRequestBase x) => x.ServerVariables["LOCAL_ADDR"]);
		}

		private static string InternalGetClientAddress(HttpContextBase httpContext, HttpRequestBase httpRequest)
		{
			return GccUtils.InternalExecuteWorkerRequest<string>(httpContext, httpRequest, (HttpWorkerRequest x) => x.GetRemoteAddress(), (HttpRequestBase x) => x.ServerVariables["REMOTE_ADDR"]);
		}

		private static string InternalGetClientPort(HttpContextBase httpContext, HttpRequestBase httpRequest)
		{
			return GccUtils.InternalExecuteWorkerRequest<string>(httpContext, httpRequest, (HttpWorkerRequest x) => x.GetRemotePort().ToString(), (HttpRequestBase x) => x.ServerVariables["REMOTE_PORT"]);
		}

		private static bool InternalTryGetGccProxyInfo(HttpContextBase httpContext, HttpRequestBase httpRequest, out string gccProxyInfo, out bool trustedProxy, out string clientIpRaw, out string serverIpRaw)
		{
			trustedProxy = false;
			clientIpRaw = string.Empty;
			serverIpRaw = string.Empty;
			gccProxyInfo = GccUtils.InternalExecuteWorkerRequest<string>(httpContext, httpRequest, delegate(HttpWorkerRequest request)
			{
				if (GccUtils.gccKnownRequestHeaderIndex == null)
				{
					GccUtils.gccKnownRequestHeaderIndex = new int?(HttpWorkerRequest.GetKnownRequestHeaderIndex("X-GCC-PROXYINFO"));
				}
				if (GccUtils.gccKnownRequestHeaderIndex.Value != -1)
				{
					return request.GetKnownRequestHeader(GccUtils.gccKnownRequestHeaderIndex.Value);
				}
				return request.GetUnknownRequestHeader("X-GCC-PROXYINFO");
			}, (HttpRequestBase request) => request.Headers["X-GCC-PROXYINFO"]);
			if (string.IsNullOrEmpty(gccProxyInfo))
			{
				gccProxyInfo = null;
				return false;
			}
			int num = gccProxyInfo.IndexOf(',');
			if (num > 0 && num < gccProxyInfo.Length - 1)
			{
				string authString = gccProxyInfo.Substring(0, num);
				bool flag = false;
				try
				{
					flag = GccUtils.IsValidAuthString(authString);
				}
				catch (InvalidDatacenterProxyKeyException)
				{
				}
				if (flag)
				{
					clientIpRaw = gccProxyInfo.Substring(num + 1);
					num = clientIpRaw.IndexOf(',');
					if (num > 0 && num < clientIpRaw.Length - 1)
					{
						serverIpRaw = clientIpRaw.Substring(num + 1);
						clientIpRaw = clientIpRaw.Substring(0, num);
						trustedProxy = true;
					}
				}
			}
			return true;
		}

		private static bool InternalTryCreateGccProxyInfo(HttpContextBase httpContext, HttpRequestBase httpRequest, out string gccProxyInfo)
		{
			gccProxyInfo = null;
			string text = GccUtils.InternalGetClientAddress(httpContext, httpRequest);
			string text2 = GccUtils.InternalGetServerAddress(httpContext, httpRequest);
			IPAddress ipv6None;
			if (string.IsNullOrEmpty(text) || !IPAddress.TryParse(text, out ipv6None))
			{
				ipv6None = IPAddress.IPv6None;
			}
			IPAddress ipv6Loopback;
			if (string.IsNullOrEmpty(text2) || !IPAddress.TryParse(text2, out ipv6Loopback))
			{
				ipv6Loopback = IPAddress.IPv6Loopback;
			}
			string text3 = null;
			try
			{
				text3 = GccUtils.GetAuthStringForThisServer();
			}
			catch (InvalidDatacenterProxyKeyException)
			{
			}
			if (!string.IsNullOrEmpty(text3))
			{
				gccProxyInfo = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", new object[]
				{
					text3,
					ipv6None,
					ipv6Loopback
				});
				return true;
			}
			return false;
		}

		private static void EnsureProxyKeysAreLoaded()
		{
			lock (GccUtils.RegistryLock)
			{
				if (GccUtils.DatacenterServerAuth.CurrentSecretKey == null)
				{
					GccUtils.RefreshProxySecretKeys();
				}
			}
		}

		public const string GccProxyInfoHeader = "X-GCC-PROXYINFO";

		private const string E14RegistryRoot = "SOFTWARE\\Microsoft\\ExchangeLabs";

		private const string CurrentProxyKeyName = "CurrentProxyKey";

		private const string CurrentIVKeyName = "CurrentIVProxyKey";

		private const string PreviousProxyKeyName = "PreviousProxyKey";

		private const string PreviousIVKeyName = "PreviousIVProxyKey";

		private const string ServerToServerOriginalClientHeader = "x-ServerToServer-OriginalClient";

		private const string ServerToServerOriginalServerHeader = "x-ServerToServer-OriginalServer";

		private const string XForwardedForHeader = "X-Forwarded-For";

		private static readonly DatacenterServerAuthentication DatacenterServerAuth = new DatacenterServerAuthentication();

		private static readonly object RegistryLock = new object();

		private static int? gccKnownRequestHeaderIndex = null;

		private static LazyMember<bool> isGlobalCriminalComplianceEnabled = new LazyMember<bool>(() => VariantConfiguration.InvariantNoFlightingSnapshot.Global.GlobalCriminalCompliance.Enabled);
	}
}
