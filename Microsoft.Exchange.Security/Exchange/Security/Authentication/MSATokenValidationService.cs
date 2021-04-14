using System;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceModel;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Security.Authentication.FederatedAuthService;
using Microsoft.Passport.RPS;

namespace Microsoft.Exchange.Security.Authentication
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class MSATokenValidationService : IMSATokenValidation
	{
		protected static RPS MSARpsSession
		{
			get
			{
				if (MSATokenValidationService.msaRPSSession == null)
				{
					lock (MSATokenValidationService.lockRoot)
					{
						if (MSATokenValidationService.msaRPSSession == null)
						{
							string text = null;
							string a = MSATokenValidationService.ReadAppConfig("MSA", "true");
							string s = MSATokenValidationService.ReadAppConfig("slidingWindow", MSATokenValidationService.slidingWindow.ToString());
							int num = 0;
							bool flag2 = int.TryParse(s, out num);
							if (flag2)
							{
								MSATokenValidationService.slidingWindow = num;
							}
							try
							{
								RPS rps = new RPS();
								if (string.Equals(a, "true", StringComparison.OrdinalIgnoreCase))
								{
									MSATokenValidationService.IsMSA = true;
									text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft Passport RPS\\LiveIdConfig");
									rps.Initialize(text);
									ExTraceGlobals.AuthenticationTracer.TraceDebug(0L, "MSA RPS Session initialized successfully");
								}
								else
								{
									MSATokenValidationService.IsMSA = false;
									rps.Initialize(text);
									ExTraceGlobals.AuthenticationTracer.TraceDebug(0L, "OrgId RPS Session initialized successfully");
								}
								MSATokenValidationService.msaRPSSession = rps;
							}
							catch (COMException ex)
							{
								ExTraceGlobals.AuthenticationTracer.TraceError<int>(0L, "RPS initialization failed with error {0}", ex.ErrorCode);
								AuthServiceHelper.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_MSARPSLoadException, "RPSLoad", new object[]
								{
									text,
									ex.ToString()
								});
								throw;
							}
						}
					}
				}
				return MSATokenValidationService.msaRPSSession;
			}
		}

		public bool ValidateCompactToken(int tokenType, string token, string siteName, out string errorString)
		{
			if (string.IsNullOrEmpty(token))
			{
				throw new ArgumentException("token");
			}
			errorString = null;
			bool result;
			try
			{
				using (RPSPropBag rpspropBag = new RPSPropBag(MSATokenValidationService.MSARpsSession))
				{
					using (RPSAuth rpsauth = new RPSAuth(MSATokenValidationService.MSARpsSession))
					{
						using (RPSTicket rpsticket = rpsauth.Authenticate(siteName, token, (uint)tokenType, rpspropBag))
						{
							result = (rpsticket != null);
						}
					}
				}
			}
			catch (ArgumentException ex)
			{
				errorString = string.Format("Token:{0}. SiteName:{1}. Error:{2}", token, siteName, ex.Message);
				result = false;
			}
			catch (COMException ex2)
			{
				errorString = string.Format("Error parsing compact token {0} {1}", ex2.ErrorCode, ex2.ToString());
				result = false;
			}
			catch (Exception ex3)
			{
				errorString = string.Format("Unexpected exception: {0}", ex3.ToString());
				AuthServiceHelper.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_MSARPSTicketParsingException, "RPSLoad", new object[]
				{
					siteName,
					ex3.ToString()
				});
				result = false;
			}
			return result;
		}

		public bool ParseCompactToken(int tokenType, string token, string siteName, int rpsTicketLifetime, out RPSProfile rpsProfile, out string errorString)
		{
			if (string.IsNullOrEmpty(token))
			{
				throw new ArgumentException("token");
			}
			rpsProfile = null;
			errorString = null;
			try
			{
				using (RPSPropBag rpspropBag = new RPSPropBag(MSATokenValidationService.MSARpsSession))
				{
					using (RPSAuth rpsauth = new RPSAuth(MSATokenValidationService.MSARpsSession))
					{
						using (RPSTicket rpsticket = rpsauth.Authenticate(siteName, token, (uint)tokenType, rpspropBag))
						{
							if (rpsticket != null && !rpsticket.Validate(rpspropBag))
							{
								int num = (int)rpspropBag["reasonhr"];
								RPSErrorCategory rpserrorCategory = RPSErrorHandler.CategorizeRPSError(num);
								errorString = string.Format("Live token failed Validate() with {0}: {1} error=0x{2:x}.", rpserrorCategory, Enum.GetName(typeof(RPSErrorCode), num) ?? string.Empty, num);
								return false;
							}
							if (rpsticket == null)
							{
								return false;
							}
							rpsProfile = RPSCommon.ParseRPSTicket(rpsticket, rpsTicketLifetime, this.GetHashCode(), true, out errorString, true);
						}
					}
				}
			}
			catch (ArgumentException ex)
			{
				errorString = string.Format("Token:{0}. SiteName:{1}. Error:{2}", token, siteName, ex.Message);
				return false;
			}
			catch (COMException ex2)
			{
				errorString = string.Format("Error parsing compact token {0} {1}", ex2.ErrorCode, ex2.ToString());
				return false;
			}
			catch (Exception ex3)
			{
				errorString = string.Format("Unexpected exception: {0}", ex3.ToString());
				AuthServiceHelper.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_MSARPSTicketParsingException, "RPSLoad", new object[]
				{
					siteName,
					ex3.ToString()
				});
				return false;
			}
			return true;
		}

		public bool Authenticate(string siteName, string authPolicyOverrideValue, bool sslOffloaded, string headers, string path, string method, string querystring, string body, out RPSProfile rpsProfile, out int? rpsError, out string errorString)
		{
			rpsProfile = null;
			errorString = null;
			bool flag;
			return this.AuthenticationInternal(siteName, authPolicyOverrideValue, sslOffloaded, headers, path, method, querystring, body, out rpsProfile, out flag, out rpsError, out errorString);
		}

		public bool AuthenticateWithoutBody(string siteName, string authPolicyOverrideValue, bool sslOffloaded, string headers, string path, string method, string querystring, out bool needBody, out RPSProfile rpsProfile, out int? rpsError, out string errorString)
		{
			rpsProfile = null;
			errorString = null;
			needBody = false;
			return this.AuthenticationInternal(siteName, authPolicyOverrideValue, sslOffloaded, headers, path, method, querystring, null, out rpsProfile, out needBody, out rpsError, out errorString);
		}

		public string GetRedirectUrl(string constructUrlParam, string siteName, string formattedReturnUrl, string authPolicy, out int? error, out string errorString)
		{
			string result = null;
			error = null;
			errorString = null;
			try
			{
				result = RPSCommon.GetRedirectUrl(MSATokenValidationService.MSARpsSession, constructUrlParam, siteName, formattedReturnUrl, authPolicy, out error, out errorString);
			}
			catch (Exception ex)
			{
				errorString = ex.Message;
			}
			return result;
		}

		public string GetSiteProperty(string siteName, string siteProperty)
		{
			return RPSCommon.GetSiteProperty(MSATokenValidationService.MSARpsSession, siteName, siteProperty);
		}

		public string GetLogoutHeaders(string siteName, out int? error, out string errorString)
		{
			string result = null;
			error = null;
			errorString = null;
			try
			{
				using (RPSHttpAuth rpshttpAuth = new RPSHttpAuth(MSATokenValidationService.MSARpsSession))
				{
					result = rpshttpAuth.GetLogoutHeaders(siteName);
				}
			}
			catch (COMException ex)
			{
				error = new int?(ex.ErrorCode);
				errorString = ex.Message;
			}
			return result;
		}

		public string GetRPSEnvironmentConfig()
		{
			return RPSCommon.GetRPSEnvironmentConfig(MSATokenValidationService.MSARpsSession);
		}

		private bool AuthenticationInternal(string siteName, string authPolicyOverrideValue, bool sslOffloaded, string headers, string path, string method, string querystring, string body, out RPSProfile rpsProfile, out bool needBody, out int? rpsError, out string errorString)
		{
			rpsProfile = null;
			errorString = null;
			needBody = false;
			rpsError = null;
			try
			{
				using (RPSPropBag rpspropBag = new RPSPropBag(MSATokenValidationService.MSARpsSession))
				{
					if (!MSATokenValidationService.IsMSA)
					{
						rpspropBag["SlidingWindow"] = 0;
					}
					else
					{
						rpspropBag["SlidingWindow"] = MSATokenValidationService.slidingWindow;
					}
					if (sslOffloaded)
					{
						rpspropBag["HTTPS"] = 1;
					}
					using (RPSHttpAuthEx rpshttpAuthEx = new RPSHttpAuthEx(MSATokenValidationService.MSARpsSession))
					{
						if (string.IsNullOrEmpty(body))
						{
							using (RPSTicket rpsticket = rpshttpAuthEx.Authenticate(siteName, headers, path, method, querystring, rpspropBag, out needBody))
							{
								if (rpsticket == null)
								{
									return false;
								}
								if (!string.IsNullOrEmpty(authPolicyOverrideValue))
								{
									rpspropBag["AuthPolicy"] = authPolicyOverrideValue;
								}
								if (!rpsticket.Validate(rpspropBag))
								{
									errorString = "Validate failed.";
									return false;
								}
								rpsProfile = RPSCommon.ParseRPSTicket(rpsticket, -1, this.GetHashCode(), false, out errorString, true);
								if (rpsProfile != null)
								{
									if (rpspropBag["RPSAuthState"] != null)
									{
										rpsProfile.RPSAuthState = (uint)rpspropBag["RPSAuthState"];
									}
									rpsProfile.ResponseHeader = (string)rpspropBag["RPSRespHeaders"];
								}
								goto IL_1DA;
							}
						}
						using (RPSTicket rpsticket2 = rpshttpAuthEx.Authenticate(siteName, headers, path, method, querystring, body, rpspropBag))
						{
							if (rpsticket2 == null)
							{
								return false;
							}
							if (!string.IsNullOrEmpty(authPolicyOverrideValue))
							{
								rpspropBag["AuthPolicy"] = authPolicyOverrideValue;
							}
							if (!rpsticket2.Validate(rpspropBag))
							{
								return false;
							}
							rpsProfile = RPSCommon.ParseRPSTicket(rpsticket2, -1, this.GetHashCode(), false, out errorString, true);
							if (rpsProfile != null)
							{
								if (rpspropBag["RPSAuthState"] != null)
								{
									rpsProfile.RPSAuthState = (uint)rpspropBag["RPSAuthState"];
								}
								rpsProfile.ResponseHeader = (string)rpspropBag["RPSRespHeaders"];
							}
						}
						IL_1DA:;
					}
				}
			}
			catch (COMException ex)
			{
				rpsError = new int?(ex.ErrorCode);
				errorString = ex.Message;
			}
			catch (Exception ex2)
			{
				errorString = string.Format("Unexpected exception: {0}", ex2.ToString());
				AuthServiceHelper.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_MSARPSTicketParsingException, "RPSLoad", new object[]
				{
					siteName,
					ex2.ToString()
				});
			}
			return rpsProfile != null;
		}

		private static string ReadAppConfig(string setting, string defaultValue)
		{
			string text = defaultValue;
			try
			{
				string text2 = ConfigurationManager.AppSettings[setting];
				if (text2 != null)
				{
					text = text2;
					ExTraceGlobals.AuthenticationTracer.TraceDebug<string, string>(0L, "Read setting '{0}' value '{1}' from app.Config file", setting, text);
				}
			}
			catch (ConfigurationException ex)
			{
				AuthServiceHelper.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_ConfigReadError, setting, new object[]
				{
					setting,
					defaultValue,
					ex
				});
				ExTraceGlobals.AuthenticationTracer.TraceWarning<string, string, string>(0L, "Exception caught reading {0} value from app.config file, using default value {1}. {2}", setting, text, ex.ToString());
			}
			ExTraceGlobals.AuthenticationTracer.Information<string, string>(0L, "{0} = {1}", setting, text);
			return text;
		}

		private static RPS msaRPSSession;

		private static object lockRoot = new object();

		private static bool IsMSA = true;

		private static int slidingWindow = 900;
	}
}
