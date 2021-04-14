using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DataContract(Name = "SetPartnerInformationRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class SetPartnerInformationRequest : Request
	{
		[DataMember]
		public PartnerInformation PartnerInformation
		{
			get
			{
				return this.PartnerInformationField;
			}
			set
			{
				this.PartnerInformationField = value;
			}
		}

		private PartnerInformation PartnerInformationField;
	}
}
