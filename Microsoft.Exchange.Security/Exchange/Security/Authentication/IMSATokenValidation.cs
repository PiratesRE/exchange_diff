using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Security.Authentication
{
	[ServiceContract(ConfigurationName = "Microsoft.Exchange.Security.Authentication.IMSATokenValidation")]
	public interface IMSATokenValidation
	{
		[OperationContract]
		bool ParseCompactToken(int tokenType, string token, string siteName, int rpsTicketLifetime, out RPSProfile rpsProfile, out string errorString);

		[OperationContract]
		bool ValidateCompactToken(int tokenType, string token, string siteName, out string errorString);

		[OperationContract]
		bool Authenticate(string siteName, string authPolicyOverrideValue, bool sslOffloaded, string headers, string path, string method, string querystring, string body, out RPSProfile rpsProfile, out int? rpsError, out string errorString);

		[OperationContract]
		bool AuthenticateWithoutBody(string siteName, string authPolicyOverrideValue, bool sslOffloaded, string headers, string path, string method, string querystring, out bool needBody, out RPSProfile rpsProfile, out int? rpsError, out string errorString);

		[OperationContract]
		string GetRedirectUrl(string constructUrlParam, string siteName, string returnUrl, string authPolicy, out int? error, out string errorString);

		[OperationContract]
		string GetSiteProperty(string siteName, string siteProperty);

		[OperationContract]
		string GetLogoutHeaders(string siteName, out int? error, out string errorString);

		[OperationContract]
		string GetRPSEnvironmentConfig();
	}
}
