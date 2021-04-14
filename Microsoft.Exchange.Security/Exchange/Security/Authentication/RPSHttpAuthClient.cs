using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using Microsoft.Passport.RPS;

namespace Microsoft.Exchange.Security.Authentication
{
	public class RPSHttpAuthClient : IDisposable
	{
		public RPSHttpAuthClient(bool isMSA, RPS orgIdRps, int rpsTicketLifetime = 3600)
		{
			if (orgIdRps == null)
			{
				throw new ArgumentException("RPS object cannot be null");
			}
			this.httpAuth = new RPSHttpAuth(orgIdRps);
			this.isMSA = isMSA;
			this.rpsTicketLifetime = rpsTicketLifetime;
			this.orgIdRps = orgIdRps;
		}

		public void Dispose()
		{
			if (this.httpAuth != null)
			{
				this.httpAuth.Dispose();
				this.httpAuth = null;
			}
		}

		public RPSProfile Authenticate(string siteName, string authPolicyOverrideValue, bool sslOffloaded, HttpRequest request, RPSPropBag propBag, out int? rpsErrorCode, out string errorString, out RPSTicket rpsTicket)
		{
			RPSProfile rpsprofile = null;
			errorString = null;
			rpsTicket = null;
			rpsErrorCode = null;
			if (!this.isMSA)
			{
				try
				{
					rpsTicket = this.httpAuth.Authenticate(siteName, request, propBag);
					if (rpsTicket != null)
					{
						propBag["SlidingWindow"] = 0;
						if (!string.IsNullOrEmpty(authPolicyOverrideValue))
						{
							propBag["AuthPolicy"] = authPolicyOverrideValue;
						}
						if (!rpsTicket.Validate(propBag))
						{
							errorString = "Validate failed.";
						}
						else
						{
							rpsprofile = RPSCommon.ParseRPSTicket(rpsTicket, this.rpsTicketLifetime, this.GetHashCode(), false, out errorString, false);
							if (rpsprofile != null)
							{
								if (propBag["RPSAuthState"] != null)
								{
									rpsprofile.RPSAuthState = (uint)propBag["RPSAuthState"];
								}
								rpsprofile.ResponseHeader = (string)propBag["RPSRespHeaders"];
							}
						}
					}
				}
				catch (COMException ex)
				{
					rpsErrorCode = new int?(ex.ErrorCode);
					errorString = ex.Message;
				}
				return rpsprofile;
			}
			if (request == null)
			{
				throw new ArgumentException("request cannot be null.");
			}
			try
			{
				string headers = request.ServerVariables["ALL_RAW"];
				string path = request.ServerVariables["PATH_INFO"];
				string method = request.ServerVariables["REQUEST_METHOD"];
				string querystring = request.ServerVariables["QUERY_STRING"];
				bool flag = false;
				MSATokenValidationClient.Instance.AuthenticateWithoutBody(siteName, authPolicyOverrideValue, sslOffloaded, headers, path, method, querystring, out flag, out rpsprofile, out rpsErrorCode, out errorString);
				if (rpsprofile == null && flag)
				{
					UTF8Encoding utf8Encoding = new UTF8Encoding();
					request.InputStream.Seek(0L, SeekOrigin.Begin);
					byte[] bytes = request.BinaryRead(request.TotalBytes);
					string @string = utf8Encoding.GetString(bytes);
					MSATokenValidationClient.Instance.Authenticate(siteName, authPolicyOverrideValue, sslOffloaded, headers, path, method, querystring, @string, out rpsprofile, out rpsErrorCode, out errorString);
				}
				return rpsprofile;
			}
			catch (Exception ex2)
			{
				errorString = ex2.Message;
			}
			return null;
		}

		public string GetLogoutHeaders(string siteName, out int? rpsErrorCode, out string errorString)
		{
			rpsErrorCode = null;
			errorString = null;
			string result = null;
			if (!this.isMSA)
			{
				try
				{
					return this.httpAuth.GetLogoutHeaders(siteName);
				}
				catch (COMException ex)
				{
					rpsErrorCode = new int?(ex.ErrorCode);
					errorString = ex.Message;
					return result;
				}
			}
			result = MSATokenValidationClient.Instance.GetLogoutHeaders(siteName, out rpsErrorCode, out errorString);
			return result;
		}

		public void WriteHeaders(HttpResponse response, string headers)
		{
			this.httpAuth.WriteHeaders(response, headers);
		}

		public string GetSiteProperty(string siteName, string siteProperty)
		{
			if (!this.isMSA)
			{
				return RPSCommon.GetSiteProperty(this.orgIdRps, siteName, siteProperty);
			}
			return MSATokenValidationClient.Instance.GetSiteProperty(siteName, siteProperty);
		}

		public string GetRedirectUrl(string constructUrlParam, string siteName, string formattedReturnUrl, string authPolicy, out int? rpsErrorCode, out string errorString)
		{
			if (!this.isMSA)
			{
				return RPSCommon.GetRedirectUrl(this.orgIdRps, constructUrlParam, siteName, formattedReturnUrl, authPolicy, out rpsErrorCode, out errorString);
			}
			return MSATokenValidationClient.Instance.GetRedirectUrl(constructUrlParam, siteName, formattedReturnUrl, authPolicy, out rpsErrorCode, out errorString);
		}

		public string GetCurrentEnvironment()
		{
			if (!this.isMSA)
			{
				return RPSCommon.GetRPSEnvironmentConfig(this.orgIdRps);
			}
			return MSATokenValidationClient.Instance.GetRPSEnvironmentConfig();
		}

		private RPSHttpAuth httpAuth;

		private readonly bool isMSA;

		private readonly int rpsTicketLifetime;

		private RPS orgIdRps;
	}
}
