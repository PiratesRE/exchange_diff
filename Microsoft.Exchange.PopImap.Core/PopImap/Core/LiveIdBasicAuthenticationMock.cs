using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.PopImap.Core
{
	internal class LiveIdBasicAuthenticationMock : ILiveIdBasicAuthentication
	{
		public LiveIdBasicAuthenticationMock(ILiveIdBasicAuthentication innerLiveIdBasicAuth)
		{
			this.innerLiveIdBasicAuth = innerLiveIdBasicAuth;
		}

		public string ApplicationName
		{
			get
			{
				return this.innerLiveIdBasicAuth.ApplicationName;
			}
			set
			{
				this.innerLiveIdBasicAuth.ApplicationName = value;
			}
		}

		public string UserIpAddress
		{
			get
			{
				return this.innerLiveIdBasicAuth.UserIpAddress;
			}
			set
			{
				this.innerLiveIdBasicAuth.UserIpAddress = value;
			}
		}

		public bool AllowLiveIDOnlyAuth
		{
			get
			{
				return this.innerLiveIdBasicAuth.AllowLiveIDOnlyAuth;
			}
			set
			{
				this.innerLiveIdBasicAuth.AllowLiveIDOnlyAuth = value;
			}
		}

		public LiveIdAuthResult LastAuthResult { get; private set; }

		public string LastRequestErrorMessage { get; private set; }

		public SecurityStatus GetWindowsIdentity(byte[] userBytes, byte[] passBytes, out WindowsIdentity identity, out IAccountValidationContext accountValidationContext)
		{
			string text = null;
			ProtocolBaseServices.FaultInjectionTracer.TraceTest<string>(3112578365U, ref text);
			accountValidationContext = null;
			if (!string.IsNullOrEmpty(text))
			{
				userBytes = Encoding.ASCII.GetBytes(text);
				return this.innerLiveIdBasicAuth.GetWindowsIdentity(userBytes, passBytes, out identity, out accountValidationContext);
			}
			this.LastRequestErrorMessage = "UserIP in Auth: " + this.UserIpAddress;
			identity = null;
			this.LastAuthResult = LiveIdAuthResult.Success;
			return SecurityStatus.LogonDenied;
		}

		public SecurityStatus GetCommonAccessToken(byte[] userBytes, byte[] passBytes, Guid requestId, out string commonAccessToken, out IAccountValidationContext accountValidationContext)
		{
			string text = null;
			ProtocolBaseServices.FaultInjectionTracer.TraceTest<string>(3112578365U, ref text);
			accountValidationContext = null;
			if (!string.IsNullOrEmpty(text))
			{
				userBytes = Encoding.ASCII.GetBytes(text);
				SecurityStatus commonAccessToken2 = this.innerLiveIdBasicAuth.GetCommonAccessToken(userBytes, passBytes, requestId, out commonAccessToken, out accountValidationContext);
				if (commonAccessToken2 == SecurityStatus.OK)
				{
					CommonAccessToken commonAccessToken3 = CommonAccessToken.Deserialize(commonAccessToken);
					commonAccessToken3.ExtensionData.Remove("UserSid");
					commonAccessToken = commonAccessToken3.Serialize();
				}
				return commonAccessToken2;
			}
			this.LastRequestErrorMessage = "UserIP in Auth: " + this.UserIpAddress;
			commonAccessToken = null;
			this.LastAuthResult = LiveIdAuthResult.Success;
			return SecurityStatus.LogonDenied;
		}

		public LiveIdAuthResult SyncADPassword(string puid, byte[] userBytes, byte[] passBytes, string remoteOrganizationContext, bool syncHrd)
		{
			throw new NotImplementedException();
		}

		private ILiveIdBasicAuthentication innerLiveIdBasicAuth;
	}
}
