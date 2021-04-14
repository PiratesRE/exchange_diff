using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	[Cmdlet("Import", "JournalRuleCollection", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class ImportJournalRuleCollection : GetMultitenancySystemConfigurationObjectTask<RuleIdParameter, TransportRule>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageImportJournalRuleCollection;
			}
		}

		[Parameter(Mandatory = true, Position = 0)]
		public byte[] FileData
		{
			get
			{
				return (byte[])base.Fields["FileData"];
			}
			set
			{
				base.Fields["FileData"] = value;
			}
		}

		protected override void InternalValidate()
		{
			if (this.FileData == null)
			{
				base.WriteError(new ArgumentException(Strings.ImportFileDataIsNull), ErrorCategory.InvalidArgument, "FileData");
				return;
			}
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			if (!base.ShouldContinue(Strings.PromptToOverwriteRulesOnImport))
			{
				return;
			}
			ADJournalRuleStorageManager adjournalRuleStorageManager;
			try
			{
				adjournalRuleStorageManager = new ADJournalRuleStorageManager("JournalingVersioned", base.DataSession);
			}
			catch (RuleCollectionNotInAdException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, null);
				return;
			}
			TransportRuleCollection transportRuleCollection = null;
			using (Stream stream = new MemoryStream(this.FileData))
			{
				try
				{
					transportRuleCollection = (TransportRuleCollection)JournalingRuleParser.Instance.LoadStream(stream);
				}
				catch (ParserException exception2)
				{
					base.WriteError(exception2, ErrorCategory.InvalidData, "FileData");
					return;
				}
			}
			JournalRuleObject journalRuleObject = new JournalRuleObject();
			foreach (Microsoft.Exchange.MessagingPolicies.Rules.Rule rule in transportRuleCollection)
			{
				JournalingRule journalingRule = (JournalingRule)rule;
				try
				{
					journalRuleObject.Deserialize(journalingRule);
				}
				catch (RecipientInvalidException exception3)
				{
					base.WriteError(exception3, ErrorCategory.InvalidArgument, journalRuleObject.JournalEmailAddress);
					return;
				}
				catch (JournalRuleCorruptException exception4)
				{
					base.WriteError(exception4, ErrorCategory.InvalidArgument, journalingRule.Name);
				}
				if (journalingRule.IsTooAdvancedToParse)
				{
					base.WriteError(new InvalidOperationException(Strings.CannotCreateRuleDueToVersion(journalingRule.Name)), ErrorCategory.InvalidOperation, null);
					return;
				}
			}
			try
			{
				adjournalRuleStorageManager.ReplaceRules(transportRuleCollection, this.ResolveCurrentOrganization());
			}
			catch (DataValidationException exception5)
			{
				base.WriteError(exception5, ErrorCategory.InvalidArgument, null);
			}
		}
	}
}
