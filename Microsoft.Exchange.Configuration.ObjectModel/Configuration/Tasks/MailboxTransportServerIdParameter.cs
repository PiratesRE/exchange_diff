using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class MailboxTransportServerIdParameter : ExchangeTransportServerIdParameter
	{
		public MailboxTransportServerIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public MailboxTransportServerIdParameter(MailboxServer server) : base(server.Id)
		{
		}

		public MailboxTransportServerIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public MailboxTransportServerIdParameter()
		{
		}

		protected MailboxTransportServerIdParameter(string identity) : base(identity)
		{
		}

		public static MailboxTransportServerIdParameter Parse(string identity)
		{
			return new MailboxTransportServerIdParameter(identity);
		}

		public static MailboxTransportServerIdParameter CreateIdentity(MailboxTransportServerIdParameter identityPassedIn)
		{
			return new MailboxTransportServerIdParameter("Mailbox")
			{
				identityPassedIn = identityPassedIn
			};
		}
	}
}
