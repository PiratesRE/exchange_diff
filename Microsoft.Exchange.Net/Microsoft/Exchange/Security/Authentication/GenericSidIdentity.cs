using System;
using System.Security.Principal;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Security.Authentication
{
	[Serializable]
	public class GenericSidIdentity : ClientSecurityContextIdentity
	{
		public GenericSidIdentity(string name, string type, SecurityIdentifier sid) : this(name, type, sid, null)
		{
		}

		public GenericSidIdentity(string name, string type, SecurityIdentifier sid, string partitionId) : base(name, type)
		{
			if (sid == null)
			{
				throw new ArgumentNullException("sid");
			}
			this.sid = sid;
			this.partitionId = partitionId;
			this.serializedSidValue = sid.Value;
		}

		public override SecurityIdentifier Sid
		{
			get
			{
				if (this.sid == null)
				{
					this.sid = new SecurityIdentifier(this.serializedSidValue);
				}
				return this.sid;
			}
		}

		public string PartitionId
		{
			get
			{
				return this.partitionId;
			}
		}

		internal override ClientSecurityContext CreateClientSecurityContext()
		{
			return new ClientSecurityContext(new SecurityAccessToken
			{
				UserSid = this.Sid.Value
			});
		}

		private string serializedSidValue;

		[NonSerialized]
		private SecurityIdentifier sid;

		[NonSerialized]
		private readonly string partitionId;
	}
}
