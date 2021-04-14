using System;
using System.ServiceModel;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.Authentication
{
	public class MSATokenValidationClient : IMSATokenValidation
	{
		private MSATokenValidationClient()
		{
			NetNamedPipeBinding netNamedPipeBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
			netNamedPipeBinding.OpenTimeout = new TimeSpan(0, 0, 10);
			netNamedPipeBinding.ReceiveTimeout = new TimeSpan(0, 0, 10);
			netNamedPipeBinding.SendTimeout = new TimeSpan(0, 0, 10);
			netNamedPipeBinding.CloseTimeout = new TimeSpan(0, 0, 10);
			this.proxyPool = DirectoryServiceProxyPool<IMSATokenValidation>.CreateDirectoryServiceProxyPool(string.Format("MSATokenValidation", new object[0]), new EndpointAddress("net.pipe://localhost/MSATokenValidation/service.svc"), ExTraceGlobals.AuthenticationTracer, 100, netNamedPipeBinding, delegate(Exception x, string y)
			{
				if (x is CommunicationException || x is TimeoutException)
				{
					return new TransientException(new LocalizedString(x.ToString()));
				}
				return x;
			}, (Exception x, string y) => x, SecurityEventLogConstants.Tuple_GeneralException, false);
		}

		private static void InitializeProxyPoolIfRequired()
		{
			if (MSATokenValidationClient.globalInstance == null)
			{
				lock (MSATokenValidationClient.lockObject)
				{
					if (MSATokenValidationClient.globalInstance == null)
					{
						MSATokenValidationClient.globalInstance = new MSATokenValidationClient();
					}
				}
			}
		}

		public static MSATokenValidationClient Instance
		{
			get
			{
				MSATokenValidationClient.InitializeProxyPoolIfRequired();
				return MSATokenValidationClient.globalInstance;
			}
		}

		public bool ParseCompactToken(int tokenType, string token, string siteName, int rpsTicketLifetime, out RPSProfile rpsProfile, out string errorString)
		{
			rpsProfile = null;
			errorString = null;
			RPSProfile tempRPSProfile = null;
			string tempErrorString = null;
			bool authResult = false;
			try
			{
				this.proxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<IMSATokenValidation> proxy)
				{
					authResult = proxy.Client.ParseCompactToken(tokenType, token, siteName, rpsTicketLifetime, out tempRPSProfile, out tempErrorString);
				}, string.Format("Calling ParseCompactToken", new object[0]), 3);
			}
			catch (Exception ex)
			{
				tempErrorString = ex.Message;
			}
			rpsProfile = tempRPSProfile;
			errorString = tempErrorString;
			return authResult;
		}

		public bool ValidateCompactToken(int tokenType, string token, string siteName, out string errorString)
		{
			errorString = null;
			bool authResult = false;
			string tempErrorString = null;
			try
			{
				this.proxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<IMSATokenValidation> proxy)
				{
					authResult = proxy.Client.ValidateCompactToken(tokenType, token, siteName, out tempErrorString);
				}, string.Format("Calling ParseCompactToken", new object[0]), 3);
			}
			catch (Exception ex)
			{
				tempErrorString = ex.Message;
			}
			errorString = tempErrorString;
			return authResult;
		}

		public bool Authenticate(string siteName, string authPolicyOverrideValue, bool sslOffloaded, string headers, string path, string method, string querystring, string body, out RPSProfile rpsProfile, out int? rpsError, out string errorString)
		{
			errorString = null;
			rpsError = null;
			bool authResult = false;
			string tempErrorString = null;
			RPSProfile tempRPSProfile = null;
			int? tempRpsError = null;
			this.proxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<IMSATokenValidation> proxy)
			{
				authResult = proxy.Client.Authenticate(siteName, authPolicyOverrideValue, sslOffloaded, headers, path, method, querystring, body, out tempRPSProfile, out tempRpsError, out tempErrorString);
			}, string.Format("Calling ParseCompactToken", new object[0]), 3);
			errorString = tempErrorString;
			rpsProfile = tempRPSProfile;
			rpsError = tempRpsError;
			return authResult;
		}

		public bool AuthenticateWithoutBody(string siteName, string authPolicyOverrideValue, bool sslOffloaded, string headers, string path, string method, string querystring, out bool needBody, out RPSProfile rpsProfile, out int? rpsError, out string errorString)
		{
			errorString = null;
			needBody = false;
			rpsError = null;
			bool authResult = false;
			string tempErrorString = null;
			int? tempRpsError = null;
			RPSProfile tempRPSProfile = null;
			bool tempNeedBody = false;
			this.proxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<IMSATokenValidation> proxy)
			{
				authResult = proxy.Client.AuthenticateWithoutBody(siteName, authPolicyOverrideValue, sslOffloaded, headers, path, method, querystring, out tempNeedBody, out tempRPSProfile, out tempRpsError, out tempErrorString);
			}, string.Format("Calling ParseCompactToken", new object[0]), 3);
			errorString = tempErrorString;
			rpsProfile = tempRPSProfile;
			needBody = tempNeedBody;
			rpsError = tempRpsError;
			return authResult;
		}

		public string GetRedirectUrl(string constructUrlParam, string siteName, string formattedReturnUrl, string authPolicy, out int? error, out string errorString)
		{
			error = null;
			errorString = null;
			string redirectUrl = null;
			string tempErrorString = null;
			int? tempError = null;
			this.proxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<IMSATokenValidation> proxy)
			{
				redirectUrl = proxy.Client.GetRedirectUrl(constructUrlParam, siteName, formattedReturnUrl, authPolicy, out tempError, out tempErrorString);
			}, "Calling GetRedirectUrl", 3);
			errorString = tempErrorString;
			error = tempError;
			return redirectUrl;
		}

		public string GetSiteProperty(string siteName, string siteProperty)
		{
			if (siteName == null)
			{
				throw new ArgumentNullException("siteName");
			}
			if (siteProperty == null)
			{
				throw new ArgumentNullException("siteProperty");
			}
			string property = null;
			this.proxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<IMSATokenValidation> proxy)
			{
				property = proxy.Client.GetSiteProperty(siteName, siteProperty);
			}, "Calling GetSiteProperty", 3);
			return property;
		}

		public string GetLogoutHeaders(string siteName, out int? error, out string errorString)
		{
			string logoutHeaders = null;
			int? tempError = null;
			string tempErrorString = null;
			this.proxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<IMSATokenValidation> proxy)
			{
				logoutHeaders = proxy.Client.GetLogoutHeaders(siteName, out tempError, out tempErrorString);
			}, "GetLogoutHeaders", 3);
			error = tempError;
			errorString = tempErrorString;
			return logoutHeaders;
		}

		public string GetRPSEnvironmentConfig()
		{
			string environementConfig = null;
			this.proxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<IMSATokenValidation> proxy)
			{
				environementConfig = proxy.Client.GetRPSEnvironmentConfig();
			}, "GetRPSEnvironmentConfig", 3);
			return environementConfig;
		}

		internal const int WCFTimeoutInSeconds = 10;

		internal const string TokenValidationNamedPipeURI = "net.pipe://localhost/MSATokenValidation/service.svc";

		private static MSATokenValidationClient globalInstance = null;

		private static object lockObject = new object();

		private DirectoryServiceProxyPool<IMSATokenValidation> proxyPool;
	}
}
