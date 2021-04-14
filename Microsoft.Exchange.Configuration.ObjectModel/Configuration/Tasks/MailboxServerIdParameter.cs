using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class MailboxServerIdParameter : RoleServerIdParameter
	{
		public MailboxServerIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public MailboxServerIdParameter(MailboxServer server) : base(server.Id)
		{
		}

		public MailboxServerIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public MailboxServerIdParameter()
		{
		}

		protected MailboxServerIdParameter(string identity) : base(identity)
		{
		}

		protected override ServerRole RoleRestriction
		{
			get
			{
				return ServerRole.Mailbox;
			}
		}

		public new static MailboxServerIdParameter Parse(string identity)
		{
			return new MailboxServerIdParameter(identity);
		}
	}
}
