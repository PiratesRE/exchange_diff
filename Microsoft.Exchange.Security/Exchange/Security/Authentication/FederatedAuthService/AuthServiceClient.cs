using System;
using System.CodeDom.Compiler;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Security;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	[GeneratedCode("System.ServiceModel", "3.0.0.0")]
	internal class AuthServiceClient : ClientBase<IAuthService>, IAuthService
	{
		internal void AddRef()
		{
			Interlocked.Increment(ref this.refCount);
		}

		internal void Release()
		{
			if (Interlocked.Decrement(ref this.refCount) == 0L)
			{
				bool flag = false;
				try
				{
					if (base.State != CommunicationState.Faulted)
					{
						base.Close();
						flag = true;
					}
				}
				catch (TimeoutException ex)
				{
					ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)this.GetHashCode(), "AuthServiceClient.Release() times out: {0}", ex.Message);
				}
				catch (CommunicationException ex2)
				{
					ExTraceGlobals.AuthenticationTracer.TraceDebug<string>((long)this.GetHashCode(), "AuthServiceClient.Release() has CommunicationException: {0}", ex2.Message);
				}
				finally
				{
					if (!flag)
					{
						base.Abort();
					}
					((IDisposable)this).Dispose();
				}
			}
		}

		internal AuthServiceClient()
		{
		}

		internal AuthServiceClient(string endpointConfigurationName) : base(endpointConfigurationName)
		{
		}

		internal AuthServiceClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		internal AuthServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		internal AuthServiceClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		public IntPtr LogonUserFederationCreds(uint clientProcessId, byte[] remoteUserName, byte[] remotePassword, string remoteOrganizationContext, bool syncLocalAD, string application, string userAgent, string userAddress, out string iisLogMsg)
		{
			return this.LogonUserFederationCreds(clientProcessId, remoteUserName, remotePassword, remoteOrganizationContext, syncLocalAD, application, userAgent, userAddress, Guid.Empty, out iisLogMsg);
		}

		public IntPtr LogonUserFederationCreds(uint clientProcessId, byte[] remoteUserName, byte[] remotePassword, string remoteOrganizationContext, bool syncLocalAD, string application, string userAgent, string userAddress, Guid requestId, out string iisLogMsg)
		{
			IAsyncResult ar = base.Channel.BeginLogonUserFederationCredsAsync(clientProcessId, remoteUserName, remotePassword, remoteOrganizationContext, syncLocalAD, application, userAgent, userAddress, requestId, null, null);
			return base.Channel.EndLogonUserFederationCredsAsync(out iisLogMsg, ar);
		}

		public AuthStatus LogonCommonAccessTokenFederationCredsTest(uint clientProcessId, byte[] remoteUserName, byte[] remotePassword, AuthOptions options, string remoteOrganizationContext, string userEndpoint, string userAgent, string userAddress, Guid requestId, bool? preferOfflineOrgId, TestFailoverFlags testFailOver, out string commonAccessToken, out string iisLogMsg)
		{
			return base.Channel.LogonCommonAccessTokenFederationCredsTest(clientProcessId, remoteUserName, remotePassword, options, remoteOrganizationContext, userEndpoint, userAgent, userAddress, requestId, preferOfflineOrgId, testFailOver, out commonAccessToken, out iisLogMsg);
		}

		public IAsyncResult BeginLogonUserFederationCredsAsync(uint clientProcessId, byte[] remoteUserName, byte[] remotePassword, string remoteOrganizationContext, bool syncLocalAD, string application, string userAgent, string userAddress, AsyncCallback callback, object asyncState)
		{
			return this.BeginLogonUserFederationCredsAsync(clientProcessId, remoteUserName, remotePassword, remoteOrganizationContext, syncLocalAD, application, userAgent, userAddress, Guid.Empty, callback, asyncState);
		}

		public IAsyncResult BeginLogonUserFederationCredsAsync(uint clientProcessId, byte[] remoteUserName, byte[] remotePassword, string remoteOrganizationContext, bool syncLocalAD, string application, string userAgent, string userAddress, Guid requestId, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginLogonUserFederationCredsAsync(clientProcessId, remoteUserName, remotePassword, remoteOrganizationContext, syncLocalAD, application, userAgent, userAddress, requestId, callback, asyncState);
		}

		public IntPtr EndLogonUserFederationCredsAsync(out string iisLogMsg, IAsyncResult result)
		{
			return base.Channel.EndLogonUserFederationCredsAsync(out iisLogMsg, result);
		}

		public AuthStatus LogonCommonAccessTokenFederationCreds(uint clientProcessId, byte[] remoteUserName, byte[] remotePassword, AuthOptions options, string remoteOrganizationContext, string application, string userAgent, string userAddress, out string commonAccessToken, out string iisLogMsg)
		{
			return this.LogonCommonAccessTokenFederationCreds(clientProcessId, remoteUserName, remotePassword, options, remoteOrganizationContext, application, userAgent, userAddress, Guid.Empty, out commonAccessToken, out iisLogMsg);
		}

		public AuthStatus LogonCommonAccessTokenFederationCreds(uint clientProcessId, byte[] remoteUserName, byte[] remotePassword, AuthOptions options, string remoteOrganizationContext, string application, string userAgent, string userAddress, Guid requestId, out string commonAccessToken, out string iisLogMsg)
		{
			IAsyncResult ar = base.Channel.BeginLogonCommonAccessTokenFederationCredsAsync(clientProcessId, remoteUserName, remotePassword, options, remoteOrganizationContext, application, userAgent, userAddress, requestId, null, null);
			return base.Channel.EndLogonCommonAccessTokenFederationCredsAsync(out commonAccessToken, out iisLogMsg, ar);
		}

		public IAsyncResult BeginLogonCommonAccessTokenFederationCredsAsync(uint clientProcessId, byte[] remoteUserName, byte[] remotePassword, AuthOptions options, string remoteOrganizationContext, string application, string userAgent, string userAddress, AsyncCallback callback, object asyncState)
		{
			return this.BeginLogonCommonAccessTokenFederationCredsAsync(clientProcessId, remoteUserName, remotePassword, options, remoteOrganizationContext, application, userAgent, userAddress, Guid.Empty, callback, asyncState);
		}

		public IAsyncResult BeginLogonCommonAccessTokenFederationCredsAsync(uint clientProcessId, byte[] remoteUserName, byte[] remotePassword, AuthOptions options, string remoteOrganizationContext, string application, string userAgent, string userAddress, Guid requestId, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginLogonCommonAccessTokenFederationCredsAsync(clientProcessId, remoteUserName, remotePassword, options, remoteOrganizationContext, application, userAgent, userAddress, requestId, callback, asyncState);
		}

		public AuthStatus EndLogonCommonAccessTokenFederationCredsAsync(out string commonAccessToken, out string iisLogMsg, IAsyncResult result)
		{
			return base.Channel.EndLogonCommonAccessTokenFederationCredsAsync(out commonAccessToken, out iisLogMsg, result);
		}

		public bool IsNego2AuthEnabledForDomain(string domain)
		{
			return base.Channel.IsNego2AuthEnabledForDomain(domain);
		}

		public IAsyncResult BeginIsNego2AuthEnabledForDomainAsync(string domain, AsyncCallback callback, object asyncState)
		{
			return base.Channel.BeginIsNego2AuthEnabledForDomainAsync(domain, callback, asyncState);
		}

		public bool EndIsNego2AuthEnabledForDomainAsync(IAsyncResult result)
		{
			return base.Channel.EndIsNego2AuthEnabledForDomainAsync(result);
		}

		private long refCount;
	}
}
