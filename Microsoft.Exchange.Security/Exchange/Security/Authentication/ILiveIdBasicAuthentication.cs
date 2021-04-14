using System;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics.Components.Security;

namespace Microsoft.Exchange.Security.Authentication
{
	internal interface ILiveIdBasicAuthentication
	{
		LiveIdAuthResult SyncADPassword(string puid, byte[] userBytes, byte[] passBytes, string remoteOrganizationContext, bool syncHrd);

		SecurityStatus GetWindowsIdentity(byte[] userBytes, byte[] passBytes, out WindowsIdentity identity, out IAccountValidationContext accountValidationContext);

		SecurityStatus GetCommonAccessToken(byte[] userBytes, byte[] passBytes, Guid requestId, out string commonAccessToken, out IAccountValidationContext accountValidationContext);

		string LastRequestErrorMessage { get; }

		LiveIdAuthResult LastAuthResult { get; }

		string ApplicationName { get; set; }

		string UserIpAddress { get; set; }

		bool AllowLiveIDOnlyAuth { get; set; }
	}
}
