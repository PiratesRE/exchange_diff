using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class CompleteNameType
	{
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public string Title { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public string FirstName { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 3)]
		public string MiddleName { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 4)]
		public string LastName { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 5)]
		public string Suffix { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 6)]
		public string Initials { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 7)]
		public string FullName { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 8)]
		public string Nickname { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 9)]
		public string YomiFirstName { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 10)]
		public string YomiLastName { get; set; }
	}
}
