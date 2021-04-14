﻿using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services
{
	[DataContract(Name = "TargetAction", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	internal enum TargetAction
	{
		[EnumMember]
		None,
		[EnumMember]
		Add,
		[EnumMember]
		Set,
		[EnumMember]
		Remove
	}
}
