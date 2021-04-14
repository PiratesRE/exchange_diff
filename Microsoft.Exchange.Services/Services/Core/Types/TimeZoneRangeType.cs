﻿using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class TimeZoneRangeType
	{
		[DataMember(EmitDefaultValue = false, Order = 0)]
		[XmlAttribute]
		public string UtcTime { get; set; }

		[DataMember(EmitDefaultValue = true, Order = 0)]
		[XmlAttribute]
		public int Offset { get; set; }
	}
}
