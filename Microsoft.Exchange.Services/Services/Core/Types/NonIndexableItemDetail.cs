using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "NonIndexableItemDetail", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "NonIndexableItemDetailType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class NonIndexableItemDetail
	{
		[XmlElement("ItemId")]
		[DataMember(Name = "ItemId", IsRequired = true)]
		public ItemId ItemId { get; set; }

		[XmlElement("ErrorCode")]
		[DataMember(Name = "ErrorCode", IsRequired = true)]
		public ItemIndexError ErrorCode { get; set; }

		[XmlElement("ErrorDescription")]
		[DataMember(Name = "ErrorDescription", IsRequired = true)]
		public string ErrorDescription { get; set; }

		[DataMember(Name = "IsPartiallyIndexed", IsRequired = true)]
		[XmlElement("IsPartiallyIndexed")]
		public bool IsPartiallyIndexed { get; set; }

		[DataMember(Name = "IsPermanentFailure", IsRequired = true)]
		[XmlElement("IsPermanentFailure")]
		public bool IsPermanentFailure { get; set; }

		[DataMember(Name = "SortValue", IsRequired = true)]
		[XmlElement("SortValue")]
		public string SortValue { get; set; }

		[XmlElement("AttemptCount")]
		[DataMember(Name = "AttemptCount", IsRequired = true)]
		public int AttemptCount { get; set; }

		[XmlIgnore]
		public bool LastAttemptTimeSpecified
		{
			get
			{
				return this.LastAttemptTime != null;
			}
		}

		[XmlElement("LastAttemptTime")]
		[DataMember(Name = "LastAttemptTime", IsRequired = false)]
		public DateTime? LastAttemptTime { get; set; }

		[DataMember(Name = "AdditionalInfo", IsRequired = false)]
		[XmlElement("AdditionalInfo")]
		public string AdditionalInfo { get; set; }
	}
}
