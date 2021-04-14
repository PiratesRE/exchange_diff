using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "RetentionPolicyTagType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Name = "RetentionPolicyTag", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class RetentionPolicyTag
	{
		public RetentionPolicyTag()
		{
		}

		internal RetentionPolicyTag(PolicyTag policyTag) : this(policyTag.Name, policyTag.PolicyGuid, policyTag.TimeSpanForRetention.Days, (ElcFolderType)policyTag.Type, (RetentionActionType)policyTag.RetentionAction, policyTag.Description, policyTag.IsVisible, policyTag.OptedInto, policyTag.IsArchive)
		{
		}

		internal RetentionPolicyTag(string displayName, Guid retentionId, int retentionPeriod, ElcFolderType type, RetentionActionType retentionAction, string description, bool isVisible, bool optedInto, bool isArchive)
		{
			this.DisplayName = displayName;
			this.RetentionId = retentionId;
			this.RetentionPeriod = retentionPeriod;
			this.Type = type;
			this.RetentionAction = retentionAction;
			this.IsVisible = isVisible;
			this.OptedInto = optedInto;
			this.IsArchive = isArchive;
			if (!string.IsNullOrEmpty(description))
			{
				this.Description = description;
			}
		}

		[XmlElement("DisplayName")]
		[DataMember(Name = "DisplayName", IsRequired = true)]
		public string DisplayName { get; set; }

		[XmlElement("RetentionId")]
		[DataMember(Name = "RetentionId", IsRequired = true)]
		public Guid RetentionId { get; set; }

		[DataMember(Name = "RetentionPeriod", IsRequired = true)]
		[XmlElement("RetentionPeriod")]
		public int RetentionPeriod { get; set; }

		[XmlElement("Type")]
		[DataMember(Name = "Type", IsRequired = true)]
		public ElcFolderType Type { get; set; }

		[XmlElement("RetentionAction")]
		[DataMember(Name = "RetentionAction", IsRequired = true)]
		public RetentionActionType RetentionAction { get; set; }

		[DataMember(Name = "Description", IsRequired = false)]
		[XmlElement("Description")]
		public string Description { get; set; }

		[DataMember(Name = "IsVisible", IsRequired = true)]
		[XmlElement("IsVisible")]
		public bool IsVisible { get; set; }

		[DataMember(Name = "OptedInto", IsRequired = true)]
		[XmlElement("OptedInto")]
		public bool OptedInto { get; set; }

		[DataMember(Name = "IsArchive", IsRequired = true)]
		[XmlElement("IsArchive")]
		public bool IsArchive { get; set; }
	}
}
