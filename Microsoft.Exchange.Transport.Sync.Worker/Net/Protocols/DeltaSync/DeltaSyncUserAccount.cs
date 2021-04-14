using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.LiveIDAuthentication;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DeltaSyncUserAccount
	{
		private DeltaSyncUserAccount(string username, string password, string puid, bool puidSet, bool passportAuthenticationEnabled)
		{
			this.username = username;
			this.password = password;
			this.puid = puid;
			this.puidSet = puidSet;
			this.authPolicy = "MBI";
			this.emailSyncKey = DeltaSyncCommon.DefaultSyncKey;
			this.folderSyncKey = DeltaSyncCommon.DefaultSyncKey;
			this.passportAuthenticationEnabled = passportAuthenticationEnabled;
		}

		public static DeltaSyncUserAccount CreateDeltaSyncUserForPassportAuth(string username, string password)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("username", username);
			SyncUtilities.ThrowIfArgumentNull("password", password);
			return new DeltaSyncUserAccount(username, password, null, false, true);
		}

		public static DeltaSyncUserAccount CreateDeltaSyncUserForTrustedPartnerAuthWithPuid(string username, string puid)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("username", username);
			SyncUtilities.ThrowIfArgumentNull("puid", puid);
			return new DeltaSyncUserAccount(username, null, puid, true, false);
		}

		public static DeltaSyncUserAccount CreateDeltaSyncUserForTrustedPartnerAuthWithPassword(string username, string password)
		{
			SyncUtilities.ThrowIfArgumentNullOrEmpty("username", username);
			SyncUtilities.ThrowIfArgumentNull("password", password);
			return new DeltaSyncUserAccount(username, password, null, false, false);
		}

		internal string Username
		{
			get
			{
				return this.username;
			}
		}

		internal string Password
		{
			get
			{
				return this.password;
			}
		}

		public AuthenticationToken AuthToken
		{
			get
			{
				return this.authToken;
			}
			set
			{
				this.authToken = value;
			}
		}

		public string PartnerClientToken
		{
			get
			{
				return this.partnerClientToken;
			}
			set
			{
				this.partnerClientToken = value;
				this.partnerClientTokenSet = true;
			}
		}

		public bool IsPartnerClientTokenSet
		{
			get
			{
				return this.partnerClientTokenSet;
			}
		}

		internal string AuthPolicy
		{
			get
			{
				return this.authPolicy;
			}
			set
			{
				this.authPolicy = value;
			}
		}

		internal string Puid
		{
			get
			{
				return this.puid;
			}
			set
			{
				this.puid = value;
				this.puidSet = true;
			}
		}

		internal bool PassportAuthenticationEnabled
		{
			get
			{
				return this.passportAuthenticationEnabled;
			}
		}

		internal bool NeedsAuthentication
		{
			get
			{
				return (this.PassportAuthenticationEnabled && (this.AuthToken == null || this.AuthToken.IsExpired)) || (!this.PassportAuthenticationEnabled && !this.puidSet);
			}
		}

		internal string EmailSyncKey
		{
			get
			{
				return this.emailSyncKey;
			}
			set
			{
				this.emailSyncKey = value;
				this.cachedToString = null;
			}
		}

		internal string FolderSyncKey
		{
			get
			{
				return this.folderSyncKey;
			}
			set
			{
				this.folderSyncKey = value;
				this.cachedToString = null;
			}
		}

		internal string DeltaSyncServer
		{
			get
			{
				return this.deltaSyncServer;
			}
			set
			{
				this.deltaSyncServer = value;
				this.cachedToString = null;
			}
		}

		public override string ToString()
		{
			if (this.cachedToString == null)
			{
				this.cachedToString = string.Format(CultureInfo.InvariantCulture, "DeltaSyncUsername: {0}, EmailSyncKey: {1}, FolderSyncKey: {2}, DeltaSyncServer: {3}", new object[]
				{
					this.username,
					this.emailSyncKey,
					this.folderSyncKey,
					this.deltaSyncServer
				});
			}
			return this.cachedToString;
		}

		public string GetRequestQueryString()
		{
			if (this.passportAuthenticationEnabled)
			{
				return this.AuthToken.EncodedQueryStringTicket;
			}
			return this.GetPartnerAuthQueryString();
		}

		private string GetPartnerAuthQueryString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Ln={0}&Pd={1}&dspk={2}", new object[]
			{
				this.username,
				this.puid,
				this.partnerClientToken
			});
		}

		private const string DefaultAuthPolicy = "MBI";

		private const string PartnerAuthQueryStringFormat = "Ln={0}&Pd={1}&dspk={2}";

		private string username;

		private string password;

		private AuthenticationToken authToken;

		private string authPolicy;

		private string emailSyncKey;

		private string folderSyncKey;

		private string deltaSyncServer;

		private string puid;

		private bool passportAuthenticationEnabled;

		private string cachedToString;

		private string partnerClientToken;

		private bool puidSet;

		private bool partnerClientTokenSet;
	}
}
