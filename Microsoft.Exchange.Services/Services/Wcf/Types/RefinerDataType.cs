using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "RefinerDataType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class RefinerDataType
	{
		public RefinerDataType()
		{
		}

		public RefinerDataType(RefinerCategoryType refinerCategory, RefinerDataEntryType[] refinerDataEntryTypes)
		{
			this.RefinerCategory = refinerCategory;
			this.RefinerDataEntryTypes = refinerDataEntryTypes;
		}

		[DataMember(IsRequired = true)]
		public RefinerCategoryType RefinerCategory { get; set; }

		[DataMember(IsRequired = true)]
		public RefinerDataEntryType[] RefinerDataEntryTypes { get; set; }
	}
}
