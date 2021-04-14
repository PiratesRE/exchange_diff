using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.BOX.Shell
{
	[DebuggerStepThrough]
	[DataContract(Name = "ShellWebServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.Shell")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class ShellWebServiceFault : IExtensibleDataObject
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
		public FaultCode FaultCode
		{
			get
			{
				return this.FaultCodeField;
			}
			set
			{
				this.FaultCodeField = value;
			}
		}

		[DataMember]
		public string Message
		{
			get
			{
				return this.MessageField;
			}
			set
			{
				this.MessageField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private FaultCode FaultCodeField;

		private string MessageField;
	}
}
