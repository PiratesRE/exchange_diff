using System;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class LiveIdBasicAuthInfo
	{
		public byte[] username;

		public byte[] password;

		public byte[] passwordHash;

		public bool isValidCookie;

		public string key;

		public UserType userType;

		public string puid;

		public string ticket;

		public bool isExpired = true;

		public string passwordExpirationHint;

		public bool isAppPassword;

		public bool authenticatedByOfflineOrgId;

		public LiveIdAuthResult? OfflineOrgIdFailureResult;
	}
}
