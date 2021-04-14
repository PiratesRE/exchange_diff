using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class RuleActionsType
	{
		[XmlArrayItem("String", IsNullable = false)]
		public string[] AssignCategories;

		public TargetFolderIdType CopyToFolder;

		public bool Delete;

		[XmlIgnore]
		public bool DeleteSpecified;

		[XmlArrayItem("Address", IsNullable = false)]
		public EmailAddressType[] ForwardAsAttachmentToRecipients;

		[XmlArrayItem("Address", IsNullable = false)]
		public EmailAddressType[] ForwardToRecipients;

		public ImportanceChoicesType MarkImportance;

		[XmlIgnore]
		public bool MarkImportanceSpecified;

		public bool MarkAsRead;

		[XmlIgnore]
		public bool MarkAsReadSpecified;

		public TargetFolderIdType MoveToFolder;

		public bool PermanentDelete;

		[XmlIgnore]
		public bool PermanentDeleteSpecified;

		[XmlArrayItem("Address", IsNullable = false)]
		public EmailAddressType[] RedirectToRecipients;

		[XmlArrayItem("Address", IsNullable = false)]
		public EmailAddressType[] SendSMSAlertToRecipients;

		public ItemIdType ServerReplyWithMessage;

		public bool StopProcessingRules;

		[XmlIgnore]
		public bool StopProcessingRulesSpecified;
	}
}
