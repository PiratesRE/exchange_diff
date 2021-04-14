using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "PeopleRefinerDataEntryType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class PeopleRefinerDataEntryType : RefinerDataEntryType
	{
		public PeopleRefinerDataEntryType()
		{
		}

		public PeopleRefinerDataEntryType(string displayName, string smtpAddress, long hitCount, string refinementQuery) : base(hitCount, refinementQuery)
		{
			this.DisplayName = displayName;
			this.SmtpAddress = smtpAddress;
		}

		[DataMember(IsRequired = true)]
		public string DisplayName { get; set; }

		[DataMember(IsRequired = true)]
		public string SmtpAddress { get; set; }
	}
}
