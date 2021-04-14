using System;
using Microsoft.Exchange.Data.Storage.Optics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MailboxAssociationLogConfiguration : LogConfigurationBase
	{
		protected override string Component
		{
			get
			{
				return "MailboxAssociationLog";
			}
		}

		protected override string Type
		{
			get
			{
				return "Mailbox Association Log";
			}
		}

		protected override Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ModernGroupsTracer;
			}
		}

		public static MailboxAssociationLogConfiguration Default
		{
			get
			{
				if (MailboxAssociationLogConfiguration.defaultInstance == null)
				{
					MailboxAssociationLogConfiguration.defaultInstance = new MailboxAssociationLogConfiguration();
				}
				return MailboxAssociationLogConfiguration.defaultInstance;
			}
		}

		private static MailboxAssociationLogConfiguration defaultInstance;
	}
}
