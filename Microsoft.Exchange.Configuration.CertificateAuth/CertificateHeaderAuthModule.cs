using System;
using System.Configuration;
using System.Security.Principal;
using System.Text;
using System.Web;
using Microsoft.Exchange.Configuration.CertificateAuthentication.EventLog;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.CertificateAuthentication;

namespace Microsoft.Exchange.Configuration.CertificateAuthentication
{
	public class CertificateHeaderAuthModule : IHttpModule
	{
		static CertificateHeaderAuthModule()
		{
			if (!int.TryParse(ConfigurationManager.AppSettings["CertificateHeaderAuthentication.MaxRetryForADTransient"], out CertificateHeaderAuthModule.maxRetryForADTransient))
			{
				CertificateHeaderAuthModule.maxRetryForADTransient = 2;
			}
			int num;
			if (!int.TryParse(ConfigurationManager.AppSettings["CertificateHeaderAuthentication.UserCacheMappingMaximumSize"], out num) || num < 0)
			{
				num = 300;
			}
			if (num != 0)
			{
				int num2;
				if (!int.TryParse(ConfigurationManager.AppSettings["CertificateHeaderAuthentication.UserCacheMappingExpirationInHours"], out num2) || num2 < 0)
				{
					num2 = 4;
				}
				CertificateHeaderAuthModule.certCache = new CertificateADUserCache(num2, num);
			}
		}

		void IHttpModule.Init(HttpApplication application)
		{
			application.AuthenticateRequest += CertificateHeaderAuthModule.OnAuthenticateRequestHandler;
		}

		void IHttpModule.Dispose()
		{
		}

		private static void OnAuthenticateRequest(object source, EventArgs args)
		{
			HttpApplication httpApplication = (HttpApplication)source;
			HttpContext context = httpApplication.Context;
			if (context.Request.IsAuthenticated)
			{
				return;
			}
			HttpRequest request = context.Request;
			if (!CertificateHeaderAuthModule.IsValidCertificateHeaderRequest(request))
			{
				return;
			}
			Logger.LogVerbose("Request of Authentication for certificate {0}.", new object[]
			{
				request.Headers["X-Exchange-PowerShell-Client-Cert-Subject"]
			});
			int i = 0;
			while (i < CertificateHeaderAuthModule.maxRetryForADTransient)
			{
				try
				{
					X509Identifier x509Identifier = CertificateHeaderAuthModule.CreateCertificateIdentity(request);
					ADUser aduser = CertificateHeaderAuthModule.GetUserFromCache(x509Identifier);
					if (aduser == null)
					{
						aduser = CertificateAuthenticationModule.ResolveCertificate(x509Identifier, null);
						if (aduser != null)
						{
							CertificateHeaderAuthModule.AddUserToCache(x509Identifier, aduser);
						}
					}
					if (aduser == null)
					{
						Logger.LogEvent(CertificateHeaderAuthModule.eventLogger, TaskEventLogConstants.Tuple_CertAuth_UserNotFound, request.Headers["X-Exchange-PowerShell-Client-Cert-Subject"], new object[]
						{
							request.Headers["X-Exchange-PowerShell-Client-Cert-Subject"],
							"CertificateHeader"
						});
						Logger.LogVerbose("Certificate authentication succeeded but certificate {0} cannot be mapped to an AD account.", new object[]
						{
							request.Headers["X-Exchange-PowerShell-Client-Cert-Subject"]
						});
						break;
					}
					IIdentity identity;
					if (aduser.RecipientTypeDetails == RecipientTypeDetails.LinkedUser)
					{
						identity = new GenericIdentity(aduser.Sid.ToString(), "CertificateLinkedUser");
					}
					else
					{
						identity = new WindowsIdentity(aduser.UserPrincipalName);
					}
					if (!OrganizationId.ForestWideOrgId.Equals(aduser.OrganizationId))
					{
						HttpContext.Current.Items[CertificateAuthenticationModule.TenantCertificateOrganizaitonItemName] = aduser.OrganizationId.OrganizationalUnit.Name;
					}
					context.User = new GenericPrincipal(identity, new string[0]);
					Logger.LogVerbose("User correctly authenticated and linked to Certificate {0}.", new object[]
					{
						request.Headers["X-Exchange-PowerShell-Client-Cert-Subject"]
					});
					if (i > 0)
					{
						Logger.LogEvent(CertificateHeaderAuthModule.eventLogger, TaskEventLogConstants.Tuple_CertAuth_TransientRecovery, null, new object[]
						{
							request.Headers["X-Exchange-PowerShell-Client-Cert-Subject"],
							i,
							"CertificateHeader"
						});
					}
					break;
				}
				catch (ADTransientException ex)
				{
					i++;
					if (i == 1)
					{
						Logger.LogEvent(CertificateHeaderAuthModule.eventLogger, TaskEventLogConstants.Tuple_CertAuth_TransientError, null, new object[]
						{
							ex,
							request.Headers["X-Exchange-PowerShell-Client-Cert-Subject"],
							"CertificateHeader"
						});
					}
					Logger.LogError(string.Format("AD Transient Error when processing certificate authentication {0}.", request.Headers["X-Exchange-PowerShell-Client-Cert-Subject"]), ex);
					if (i > CertificateHeaderAuthModule.maxRetryForADTransient)
					{
						throw;
					}
				}
				catch (Exception ex2)
				{
					Logger.LogEvent(CertificateHeaderAuthModule.eventLogger, TaskEventLogConstants.Tuple_CertAuth_ServerError, null, new object[]
					{
						ex2,
						request.Headers["X-Exchange-PowerShell-Client-Cert-Subject"],
						"CertificateHeader"
					});
					Logger.LogError(string.Format("AD Transient Error when processing certificate authentication {0}.", request.Headers["X-Exchange-PowerShell-Client-Cert-Subject"]), ex2);
					throw;
				}
			}
		}

		private static ADUser GetUserFromCache(X509Identifier certificateId)
		{
			if (!CertificateHeaderAuthModule.IsUserCacheEnabled())
			{
				return null;
			}
			return CertificateHeaderAuthModule.certCache.GetUser(certificateId);
		}

		private static void AddUserToCache(X509Identifier certificateId, ADUser user)
		{
			if (!CertificateHeaderAuthModule.IsUserCacheEnabled())
			{
				return;
			}
			CertificateHeaderAuthModule.certCache.AddUser(certificateId, user);
		}

		private static bool IsUserCacheEnabled()
		{
			return CertificateHeaderAuthModule.certCache != null;
		}

		private static X509Identifier CreateCertificateIdentity(HttpRequest request)
		{
			return new X509Identifier(CertificateHeaderAuthModule.FixCertificateDN(request.Headers["X-Exchange-PowerShell-Client-Cert-Issuer"]), CertificateHeaderAuthModule.FixCertificateDN(request.Headers["X-Exchange-PowerShell-Client-Cert-Subject"]));
		}

		private static string FixCertificateDN(string originalDn)
		{
			if (!string.IsNullOrEmpty(originalDn))
			{
				StringBuilder stringBuilder = new StringBuilder(originalDn);
				if (originalDn.Contains(",ST="))
				{
					stringBuilder.Replace(",ST=", ",S=");
				}
				if (originalDn.Contains("emailAddress="))
				{
					stringBuilder.Replace("emailAddress=", "E=");
				}
				if (!originalDn.Contains(", "))
				{
					stringBuilder.Replace(",", ", ");
				}
				return stringBuilder.ToString();
			}
			return originalDn;
		}

		private static bool IsValidCertificateHeaderRequest(HttpRequest request)
		{
			return !string.IsNullOrEmpty(request.Headers["Authorization"]) && request.Headers["Authorization"].Equals("http://schemas.dmtf.org/wbem/wsman/1/wsman/secprofile/https/mutual") && !string.IsNullOrEmpty(request.Headers["X-Exchange-PowerShell-Client-Cert-Issuer"]) && !string.IsNullOrEmpty(request.Headers["X-Exchange-PowerShell-Client-Cert-Subject"]);
		}

		private const string WSManCertAuthorizationHeader = "http://schemas.dmtf.org/wbem/wsman/1/wsman/secprofile/https/mutual";

		private const string CertificateIssuerHeaderName = "X-Exchange-PowerShell-Client-Cert-Issuer";

		private const string CertificateSubjectHeaderName = "X-Exchange-PowerShell-Client-Cert-Subject";

		private static readonly ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.CertAuthTracer.Category, "MSExchange Certificate Authentication Module");

		private static readonly EventHandler OnAuthenticateRequestHandler = new EventHandler(CertificateHeaderAuthModule.OnAuthenticateRequest);

		private static CertificateADUserCache certCache = null;

		private static int maxRetryForADTransient;
	}
}
