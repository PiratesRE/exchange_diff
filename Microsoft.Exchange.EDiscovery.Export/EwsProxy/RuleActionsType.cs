using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[Serializable]
	public class RuleActionsType
	{
		[XmlArrayItem("String", IsNullable = false)]
		public string[] AssignCategories
		{
			get
			{
				return this.assignCategoriesField;
			}
			set
			{
				this.assignCategoriesField = value;
			}
		}

		public TargetFolderIdType CopyToFolder
		{
			get
			{
				return this.copyToFolderField;
			}
			set
			{
				this.copyToFolderField = value;
			}
		}

		public bool Delete
		{
			get
			{
				return this.deleteField;
			}
			set
			{
				this.deleteField = value;
			}
		}

		[XmlIgnore]
		public bool DeleteSpecified
		{
			get
			{
				return this.deleteFieldSpecified;
			}
			set
			{
				this.deleteFieldSpecified = value;
			}
		}

		[XmlArrayItem("Address", IsNullable = false)]
		public EmailAddressType[] ForwardAsAttachmentToRecipients
		{
			get
			{
				return this.forwardAsAttachmentToRecipientsField;
			}
			set
			{
				this.forwardAsAttachmentToRecipientsField = value;
			}
		}

		[XmlArrayItem("Address", IsNullable = false)]
		public EmailAddressType[] ForwardToRecipients
		{
			get
			{
				return this.forwardToRecipientsField;
			}
			set
			{
				this.forwardToRecipientsField = value;
			}
		}

		public ImportanceChoicesType MarkImportance
		{
			get
			{
				return this.markImportanceField;
			}
			set
			{
				this.markImportanceField = value;
			}
		}

		[XmlIgnore]
		public bool MarkImportanceSpecified
		{
			get
			{
				return this.markImportanceFieldSpecified;
			}
			set
			{
				this.markImportanceFieldSpecified = value;
			}
		}

		public bool MarkAsRead
		{
			get
			{
				return this.markAsReadField;
			}
			set
			{
				this.markAsReadField = value;
			}
		}

		[XmlIgnore]
		public bool MarkAsReadSpecified
		{
			get
			{
				return this.markAsReadFieldSpecified;
			}
			set
			{
				this.markAsReadFieldSpecified = value;
			}
		}

		public TargetFolderIdType MoveToFolder
		{
			get
			{
				return this.moveToFolderField;
			}
			set
			{
				this.moveToFolderField = value;
			}
		}

		public bool PermanentDelete
		{
			get
			{
				return this.permanentDeleteField;
			}
			set
			{
				this.permanentDeleteField = value;
			}
		}

		[XmlIgnore]
		public bool PermanentDeleteSpecified
		{
			get
			{
				return this.permanentDeleteFieldSpecified;
			}
			set
			{
				this.permanentDeleteFieldSpecified = value;
			}
		}

		[XmlArrayItem("Address", IsNullable = false)]
		public EmailAddressType[] RedirectToRecipients
		{
			get
			{
				return this.redirectToRecipientsField;
			}
			set
			{
				this.redirectToRecipientsField = value;
			}
		}

		[XmlArrayItem("Address", IsNullable = false)]
		public EmailAddressType[] SendSMSAlertToRecipients
		{
			get
			{
				return this.sendSMSAlertToRecipientsField;
			}
			set
			{
				this.sendSMSAlertToRecipientsField = value;
			}
		}

		public ItemIdType ServerReplyWithMessage
		{
			get
			{
				return this.serverReplyWithMessageField;
			}
			set
			{
				this.serverReplyWithMessageField = value;
			}
		}

		public bool StopProcessingRules
		{
			get
			{
				return this.stopProcessingRulesField;
			}
			set
			{
				this.stopProcessingRulesField = value;
			}
		}

		[XmlIgnore]
		public bool StopProcessingRulesSpecified
		{
			get
			{
				return this.stopProcessingRulesFieldSpecified;
			}
			set
			{
				this.stopProcessingRulesFieldSpecified = value;
			}
		}

		private string[] assignCategoriesField;

		private TargetFolderIdType copyToFolderField;

		private bool deleteField;

		private bool deleteFieldSpecified;

		private EmailAddressType[] forwardAsAttachmentToRecipientsField;

		private EmailAddressType[] forwardToRecipientsField;

		private ImportanceChoicesType markImportanceField;

		private bool markImportanceFieldSpecified;

		private bool markAsReadField;

		private bool markAsReadFieldSpecified;

		private TargetFolderIdType moveToFolderField;

		private bool permanentDeleteField;

		private bool permanentDeleteFieldSpecified;

		private EmailAddressType[] redirectToRecipientsField;

		private EmailAddressType[] sendSMSAlertToRecipientsField;

		private ItemIdType serverReplyWithMessageField;

		private bool stopProcessingRulesField;

		private bool stopProcessingRulesFieldSpecified;
	}
}
