using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class FlagType
	{
		[IgnoreDataMember]
		[XmlElement]
		public FlagStatus FlagStatus { get; set; }

		[DataMember(Name = "FlagStatus", Order = 1)]
		[XmlIgnore]
		public string FlagStatusString
		{
			get
			{
				return EnumUtilities.ToString<FlagStatus>(this.FlagStatus);
			}
			set
			{
				this.FlagStatus = EnumUtilities.Parse<FlagStatus>(value);
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 2)]
		[DateTimeString]
		public string StartDate { get; set; }

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, Order = 3)]
		public string DueDate { get; set; }

		[DateTimeString]
		[DataMember(EmitDefaultValue = false, Order = 4)]
		public string CompleteDate { get; set; }

		public bool IsValid()
		{
			switch (this.FlagStatus)
			{
			case FlagStatus.NotFlagged:
				return string.IsNullOrEmpty(this.StartDate) && string.IsNullOrEmpty(this.DueDate) && string.IsNullOrEmpty(this.CompleteDate);
			case FlagStatus.Complete:
				return string.IsNullOrEmpty(this.StartDate) && string.IsNullOrEmpty(this.DueDate);
			case FlagStatus.Flagged:
				return string.IsNullOrEmpty(this.CompleteDate);
			default:
				return false;
			}
		}
	}
}
