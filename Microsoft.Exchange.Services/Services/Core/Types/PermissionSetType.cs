using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "Permissions")]
	[Serializable]
	public class PermissionSetType : BasePermissionSetType
	{
		[DataMember(EmitDefaultValue = false)]
		[XmlArrayItem("Permission", IsNullable = false)]
		public PermissionType[] Permissions { get; set; }

		[XmlArrayItem("UnknownEntry", IsNullable = false)]
		[DataMember(EmitDefaultValue = false)]
		public string[] UnknownEntries { get; set; }
	}
}
