﻿using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ContainingNonWhitespaceValidatorInfo : ValidatorInfo
	{
		public ContainingNonWhitespaceValidatorInfo() : base("ContainingNonWhitespaceValidator")
		{
		}
	}
}
