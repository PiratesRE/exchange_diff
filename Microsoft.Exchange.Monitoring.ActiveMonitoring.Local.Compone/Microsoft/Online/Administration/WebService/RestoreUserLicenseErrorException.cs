﻿using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DataContract(Name = "RestoreUserLicenseErrorException", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class RestoreUserLicenseErrorException : DataOperationException
	{
		[DataMember]
		public AccountSkuIdentifier[] Licenses
		{
			get
			{
				return this.LicensesField;
			}
			set
			{
				this.LicensesField = value;
			}
		}

		private AccountSkuIdentifier[] LicensesField;
	}
}
