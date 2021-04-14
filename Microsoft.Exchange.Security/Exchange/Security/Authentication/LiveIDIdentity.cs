using System;

namespace Microsoft.Exchange.Security.Authentication
{
	[Serializable]
	public class LiveIDIdentity : SidBasedIdentity
	{
		public LiveIDIdentity(string userPrincipal, string userSid, string memberName, string partitionId = null, LiveIdLoginAttributes loginAttributes = null, string userNetId = null) : base(userPrincipal, userSid, memberName, "OrgId", partitionId)
		{
			this.LoginAttributes = loginAttributes;
			this.UserNetId = userNetId;
		}

		public bool HasAcceptedAccruals
		{
			get
			{
				return this.hasAcceptedAccruals;
			}
			protected internal set
			{
				this.hasAcceptedAccruals = value;
			}
		}

		public LiveIdLoginAttributes LoginAttributes { get; private set; }

		internal string UserNetId { get; private set; }

		internal const string LiveIdAuthType = "OrgId";

		private bool hasAcceptedAccruals;
	}
}
