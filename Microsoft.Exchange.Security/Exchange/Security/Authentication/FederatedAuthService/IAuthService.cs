using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	[ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
	[ServiceContract(ConfigurationName = "Microsoft.Exchange.Security.Authentication.FederatedAuthService.IAuthService")]
	internal interface IAuthService
	{
		[FaultContract(typeof(InvalidOperationException))]
		[OperationContract]
		[FaultContract(typeof(ArgumentException))]
		IntPtr LogonUserFederationCreds(uint clientProcessId, byte[] remoteUserName, byte[] remotePassword, string remoteOrganizationContext, bool syncLocalAD, string userEndpoint, string userAgent, string userAddress, Guid requestId, out string iisLogMsg);

		[FaultContract(typeof(ArgumentException))]
		[FaultContract(typeof(InvalidOperationException))]
		[OperationContract]
		AuthStatus LogonCommonAccessTokenFederationCredsTest(uint clientProcessId, byte[] remoteUserName, byte[] remotePassword, AuthOptions options, string remoteOrganizationContext, string userEndpoint, string userAgent, string userAddress, Guid requestId, bool? preferOfflineOrgId, TestFailoverFlags testFailOver, out string commonAccessToken, out string iisLogMsg);

		[FaultContract(typeof(InvalidOperationException))]
		[OperationContract(AsyncPattern = true)]
		[FaultContract(typeof(ArgumentException))]
		IAsyncResult BeginLogonUserFederationCredsAsync(uint clientProcessId, byte[] remoteUserName, byte[] remotePassword, string remoteOrganizationContext, bool syncLocalAD, string userEndpoint, string userAgent, string userAddress, Guid requestId, AsyncCallback callback, object state);

		IntPtr EndLogonUserFederationCredsAsync(out string iisLogMsg, IAsyncResult ar);

		[OperationContract(AsyncPattern = true)]
		[FaultContract(typeof(InvalidOperationException))]
		[FaultContract(typeof(ArgumentException))]
		IAsyncResult BeginLogonCommonAccessTokenFederationCredsAsync(uint clientProcessId, byte[] remoteUserName, byte[] remotePassword, AuthOptions options, string remoteOrganizationContext, string userEndpoint, string userAgent, string userAddress, Guid requestId, AsyncCallback callback, object state);

		AuthStatus EndLogonCommonAccessTokenFederationCredsAsync(out string commonAccessToken, out string iisLogMsg, IAsyncResult ar);

		[FaultContract(typeof(ArgumentException))]
		[OperationContract]
		bool IsNego2AuthEnabledForDomain(string domain);

		[FaultContract(typeof(ArgumentException))]
		[FaultContract(typeof(InvalidOperationException))]
		[OperationContract(AsyncPattern = true)]
		IAsyncResult BeginIsNego2AuthEnabledForDomainAsync(string domain, AsyncCallback callback, object state);

		bool EndIsNego2AuthEnabledForDomainAsync(IAsyncResult ar);
	}
}
