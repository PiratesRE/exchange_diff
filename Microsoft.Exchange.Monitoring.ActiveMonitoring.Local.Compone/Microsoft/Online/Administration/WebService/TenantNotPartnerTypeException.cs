using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DataContract(Name = "TenantNotPartnerTypeException", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class TenantNotPartnerTypeException : InvalidParameterException
	{
		[DataMember]
		public string PartnerId
		{
			get
			{
				return this.PartnerIdField;
			}
			set
			{
				this.PartnerIdField = value;
			}
		}

		private string PartnerIdField;
	}
}
