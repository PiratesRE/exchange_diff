using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DebuggerStepThrough]
	[DataContract(Name = "ContractVersionHeader", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class ContractVersionHeader : IExtensibleDataObject
	{
		public ExtensionDataObject ExtensionData
		{
			get
			{
				return this.extensionDataField;
			}
			set
			{
				this.extensionDataField = value;
			}
		}

		[DataMember]
		public Version BecVersion
		{
			get
			{
				return this.BecVersionField;
			}
			set
			{
				this.BecVersionField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private Version BecVersionField;
	}
}
