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
	public class ConversationActionType
	{
		public ConversationActionTypeType Action;

		public ItemIdType ConversationId;

		public TargetFolderIdType ContextFolderId;

		public DateTime ConversationLastSyncTime;

		[XmlIgnore]
		public bool ConversationLastSyncTimeSpecified;

		public bool ProcessRightAway;

		[XmlIgnore]
		public bool ProcessRightAwaySpecified;

		public TargetFolderIdType DestinationFolderId;

		[XmlArrayItem("String", IsNullable = false)]
		public string[] Categories;

		public bool EnableAlwaysDelete;

		[XmlIgnore]
		public bool EnableAlwaysDeleteSpecified;

		public bool IsRead;

		[XmlIgnore]
		public bool IsReadSpecified;

		public DisposalType DeleteType;

		[XmlIgnore]
		public bool DeleteTypeSpecified;

		public RetentionType RetentionPolicyType;

		[XmlIgnore]
		public bool RetentionPolicyTypeSpecified;

		public string RetentionPolicyTagId;

		public FlagType Flag;

		public bool SuppressReadReceipts;

		[XmlIgnore]
		public bool SuppressReadReceiptsSpecified;
	}
}
