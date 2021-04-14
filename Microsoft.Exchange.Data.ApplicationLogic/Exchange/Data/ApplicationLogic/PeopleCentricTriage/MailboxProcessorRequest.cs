using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.PeopleCentricTriage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MailboxProcessorRequest
	{
		public DateTime? LastLogonTime { get; set; }

		public IStoreSession MailboxSession { get; set; }

		public bool IsFlightEnabled { get; set; }

		public bool IsPublicFolderMailbox { get; set; }

		public bool IsGroupMailbox { get; set; }

		public bool IsTeamSiteMailbox { get; set; }

		public bool IsSharedMailbox { get; set; }

		public string DiagnosticsText { get; set; }

		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.DiagnosticsText))
			{
				return base.ToString();
			}
			return this.DiagnosticsText;
		}
	}
}
