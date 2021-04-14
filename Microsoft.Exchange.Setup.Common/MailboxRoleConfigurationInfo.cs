using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxRoleConfigurationInfo : MailboxBaseConfigurationInfo
	{
		public override string Name
		{
			get
			{
				return "MailboxRole";
			}
		}

		public override LocalizedString DisplayName
		{
			get
			{
				return Strings.MailboxRoleDisplayName;
			}
		}

		public override decimal Size
		{
			get
			{
				return RequiredDiskSpaceStatistics.MailboxRole;
			}
		}
	}
}
