﻿using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SaveExtensionCustomPropertiesParameters
	{
		[DataMember(IsRequired = true, Order = 1)]
		public string ExtensionId { get; set; }

		[DataMember(IsRequired = true, Order = 2)]
		public string ItemId { get; set; }

		[DataMember(IsRequired = true, Order = 3)]
		public string CustomProperties { get; set; }
	}
}
