using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	[Cmdlet("Export", "JournalRuleCollection", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class ExportJournalRuleCollection : ExportRuleCollectionTaskBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageExportJournalRuleCollection;
			}
		}

		public ExportJournalRuleCollection()
		{
			base.RuleCollectionName = "JournalingVersioned";
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				base.RuleStorageManager = new ADJournalRuleStorageManager(base.RuleCollectionName, base.DataSession);
			}
			catch (RuleCollectionNotInAdException)
			{
				this.WriteWarning(Strings.RuleCollectionNotFoundDuringExport(base.RuleCollectionName));
				return;
			}
			base.InternalProcessRecord();
		}
	}
}
