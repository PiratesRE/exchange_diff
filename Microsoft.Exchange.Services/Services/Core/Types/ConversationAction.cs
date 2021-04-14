using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "ConversationActionType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class ConversationAction
	{
		[XmlElement("Action", typeof(ConversationActionType))]
		[IgnoreDataMember]
		public ConversationActionType Action { get; set; }

		[XmlIgnore]
		[DataMember(Name = "Action", IsRequired = true)]
		public string ActionString
		{
			get
			{
				return this.Action.ToString();
			}
			set
			{
				this.Action = (ConversationActionType)Enum.Parse(typeof(ConversationActionType), value);
			}
		}

		[DataMember]
		[XmlArrayItem("String", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] Categories { get; set; }

		[XmlIgnore]
		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public bool ClearCategories { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		[XmlIgnore]
		public string[] CategoriesToRemove { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		[XmlElement("ConversationLastSyncTime")]
		[DateTimeString]
		public string ConversationLastSyncTime { get; set; }

		[DataMember(IsRequired = true)]
		[XmlElement("ConversationId")]
		public ItemId ConversationId { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[XmlElement("ContextFolderId")]
		public TargetFolderId ContextFolderId { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[XmlElement("DestinationFolderId")]
		public TargetFolderId DestinationFolderId { get; set; }

		[DataMember(IsRequired = false)]
		[XmlElement("EnableAlwaysDelete")]
		public bool EnableAlwaysDelete { get; set; }

		[XmlElement("ProcessRightAway")]
		[DataMember(IsRequired = false)]
		public bool ProcessRightAway { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[XmlElement("IsRead")]
		public bool? IsRead { get; set; }

		[IgnoreDataMember]
		[XmlElement("DeleteType")]
		public DisposalType? DeleteType { get; set; }

		[DataMember(Name = "DeleteType", IsRequired = false, EmitDefaultValue = false)]
		[XmlIgnore]
		public string DeleteTypeString
		{
			get
			{
				if (this.DeleteType != null)
				{
					return this.DeleteType.Value.ToString();
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					this.DeleteType = (DisposalType?)Enum.Parse(typeof(DisposalType), value);
				}
			}
		}

		[DataMember(IsRequired = false)]
		[XmlElement("RetentionPolicyType")]
		public RetentionType? RetentionPolicyType { get; set; }

		[XmlElement("RetentionPolicyTagId")]
		[DataMember(IsRequired = false)]
		public string RetentionPolicyTagId { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[XmlElement("Flag")]
		public FlagType Flag { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[XmlElement("SuppressReadReceipts")]
		public bool? SuppressReadReceipts { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		[XmlIgnore]
		public bool? IsClutter { get; set; }
	}
}
