﻿using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class AddressEntityType : BaseEntityType
	{
		[XmlElement]
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public string Address { get; set; }
	}
}
