﻿using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "ListPartnerContractsRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	public class ListPartnerContractsRequest : Request
	{
		[DataMember]
		public PartnerContractSearchDefinition PartnerContractSearchDefinition
		{
			get
			{
				return this.PartnerContractSearchDefinitionField;
			}
			set
			{
				this.PartnerContractSearchDefinitionField = value;
			}
		}

		private PartnerContractSearchDefinition PartnerContractSearchDefinitionField;
	}
}
