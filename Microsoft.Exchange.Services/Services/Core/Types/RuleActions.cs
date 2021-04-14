using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "RuleActionsType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class RuleActions
	{
		[XmlArrayItem("String", Type = typeof(string))]
		[XmlArray(Order = 0)]
		public string[] AssignCategories { get; set; }

		[XmlElement(Order = 1)]
		public TargetFolderId CopyToFolder { get; set; }

		[XmlElement(Order = 2)]
		public bool Delete { get; set; }

		[XmlIgnore]
		public bool DeleteSpecified { get; set; }

		[XmlArrayItem("Address", Type = typeof(EmailAddressWrapper))]
		[XmlArray(Order = 3)]
		public EmailAddressWrapper[] ForwardAsAttachmentToRecipients { get; set; }

		[XmlArrayItem("Address", Type = typeof(EmailAddressWrapper))]
		[XmlArray(Order = 4)]
		public EmailAddressWrapper[] ForwardToRecipients { get; set; }

		[XmlElement(Order = 5)]
		public ImportanceType MarkImportance { get; set; }

		[XmlIgnore]
		public bool MarkImportanceSpecified { get; set; }

		[XmlElement(Order = 6)]
		public bool MarkAsRead { get; set; }

		[XmlIgnore]
		public bool MarkAsReadSpecified { get; set; }

		[XmlElement(Order = 7)]
		public TargetFolderId MoveToFolder { get; set; }

		[XmlElement(Order = 8)]
		public bool PermanentDelete { get; set; }

		[XmlIgnore]
		public bool PermanentDeleteSpecified { get; set; }

		[XmlArrayItem("Address", Type = typeof(EmailAddressWrapper))]
		[XmlArray(Order = 9)]
		public EmailAddressWrapper[] RedirectToRecipients { get; set; }

		[XmlArrayItem("Address", Type = typeof(EmailAddressWrapper))]
		[XmlArray(Order = 10)]
		public EmailAddressWrapper[] SendSMSAlertToRecipients { get; set; }

		[XmlElement(Order = 11)]
		public ItemId ServerReplyWithMessage { get; set; }

		[XmlElement(Order = 12)]
		public bool StopProcessingRules { get; set; }

		[XmlIgnore]
		public bool StopProcessingRulesSpecified { get; set; }

		[XmlIgnore]
		internal EwsRule Rule { get; set; }

		public RuleActions()
		{
		}

		private RuleActions(EwsRule rule)
		{
			this.Rule = rule;
		}

		internal static RuleActions CreateFromServerRuleActions(IList<ActionBase> serverActions, EwsRule rule, int hashCode, MailboxSession session)
		{
			RuleActions ruleActions = new RuleActions(rule);
			foreach (ActionBase actionBase in serverActions)
			{
				switch (actionBase.ActionType)
				{
				case ActionType.MoveToFolderAction:
					ruleActions.MoveToFolder = new TargetFolderId(IdConverter.ConvertStoreFolderIdToFolderId(((MoveToFolderAction)actionBase).Id, session));
					continue;
				case ActionType.DeleteAction:
					ruleActions.Delete = true;
					ruleActions.DeleteSpecified = true;
					continue;
				case ActionType.CopyToFolderAction:
					ruleActions.CopyToFolder = new TargetFolderId(IdConverter.ConvertStoreFolderIdToFolderId(((CopyToFolderAction)actionBase).Id, session));
					continue;
				case ActionType.ForwardToRecipientsAction:
					ruleActions.ForwardToRecipients = ParticipantsAddressesConverter.ToAddresses(((ForwardToRecipientsAction)actionBase).Participants);
					continue;
				case ActionType.ForwardAsAttachmentToRecipientsAction:
					ruleActions.ForwardAsAttachmentToRecipients = ParticipantsAddressesConverter.ToAddresses(((ForwardAsAttachmentToRecipientsAction)actionBase).Participants);
					continue;
				case ActionType.RedirectToRecipientsAction:
					ruleActions.RedirectToRecipients = ParticipantsAddressesConverter.ToAddresses(((RedirectToRecipientsAction)actionBase).Participants);
					continue;
				case ActionType.ServerReplyMessageAction:
					ruleActions.ServerReplyWithMessage = IdConverter.ConvertStoreItemIdToItemId(((ServerReplyMessageAction)actionBase).Id, session);
					continue;
				case ActionType.MarkImportanceAction:
					ruleActions.MarkImportance = (ImportanceType)((MarkImportanceAction)actionBase).Importance;
					ruleActions.MarkImportanceSpecified = true;
					continue;
				case ActionType.StopProcessingAction:
					ruleActions.StopProcessingRules = true;
					ruleActions.StopProcessingRulesSpecified = true;
					continue;
				case ActionType.SendSmsAlertToRecipientsAction:
					ruleActions.SendSMSAlertToRecipients = ParticipantsAddressesConverter.ToAddresses(((SendSmsAlertToRecipientsAction)actionBase).Participants);
					continue;
				case ActionType.AssignCategoriesAction:
					ruleActions.AssignCategories = ((AssignCategoriesAction)actionBase).Categories;
					continue;
				case ActionType.PermanentDeleteAction:
					ruleActions.PermanentDelete = true;
					ruleActions.PermanentDeleteSpecified = true;
					continue;
				case ActionType.MarkAsReadAction:
					ruleActions.MarkAsRead = true;
					ruleActions.MarkAsReadSpecified = true;
					continue;
				}
				ExTraceGlobals.GetInboxRulesCallTracer.TraceError<ActionType>((long)hashCode, "UnsupportedPredicateType={0};", actionBase.ActionType);
				rule.IsNotSupported = true;
				return null;
			}
			return ruleActions;
		}

		internal bool HasActions()
		{
			return this.HasNonBooleanActions() || this.Delete || this.MarkAsRead || this.PermanentDelete || this.StopProcessingRules;
		}

		internal bool SpecifiedActions()
		{
			return this.HasNonBooleanActions() || this.DeleteSpecified || this.MarkAsReadSpecified || this.PermanentDeleteSpecified || this.StopProcessingRulesSpecified;
		}

		private bool HasNonBooleanActions()
		{
			return (this.AssignCategories != null && 0 < this.AssignCategories.Length) || this.CopyToFolder != null || (this.ForwardAsAttachmentToRecipients != null && 0 < this.ForwardAsAttachmentToRecipients.Length) || (this.ForwardToRecipients != null && 0 < this.ForwardToRecipients.Length) || this.MarkImportanceSpecified || this.MoveToFolder != null || (this.RedirectToRecipients != null && 0 < this.RedirectToRecipients.Length) || (this.SendSMSAlertToRecipients != null && 0 < this.SendSMSAlertToRecipients.Length) || this.ServerReplyWithMessage != null;
		}
	}
}
