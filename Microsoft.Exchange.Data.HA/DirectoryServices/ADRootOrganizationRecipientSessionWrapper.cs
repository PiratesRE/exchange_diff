using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ADRootOrganizationRecipientSessionWrapper : IADRootOrganizationRecipientSession
	{
		public static ADRootOrganizationRecipientSessionWrapper CreateWrapper(IRootOrganizationRecipientSession session)
		{
			ExAssert.RetailAssert(session != null, "'session' instance to wrap must not be null!");
			return new ADRootOrganizationRecipientSessionWrapper(session);
		}

		private ADRootOrganizationRecipientSessionWrapper(IRootOrganizationRecipientSession session)
		{
			ExAssert.RetailAssert(session != null, "'session' instance to wrap must not be null!");
			this.m_session = session;
		}

		public SecurityIdentifier GetExchangeServersUsgSid()
		{
			return this.m_session.GetExchangeServersUsgSid();
		}

		private IRootOrganizationRecipientSession m_session;
	}
}
