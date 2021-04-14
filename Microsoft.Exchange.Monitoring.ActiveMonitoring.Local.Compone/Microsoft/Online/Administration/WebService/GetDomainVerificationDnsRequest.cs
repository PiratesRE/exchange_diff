﻿using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "GetDomainVerificationDnsRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[DebuggerStepThrough]
	public class GetDomainVerificationDnsRequest : Request
	{
		[DataMember]
		public string DomainName
		{
			get
			{
				return this.DomainNameField;
			}
			set
			{
				this.DomainNameField = value;
			}
		}

		[DataMember]
		public DomainVerificationMode? Mode
		{
			get
			{
				return this.ModeField;
			}
			set
			{
				this.ModeField = value;
			}
		}

		private string DomainNameField;

		private DomainVerificationMode? ModeField;
	}
}
