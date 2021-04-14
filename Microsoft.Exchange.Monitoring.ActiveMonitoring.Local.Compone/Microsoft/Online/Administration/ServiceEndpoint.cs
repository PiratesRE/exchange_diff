using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "ServiceEndpoint", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	public class ServiceEndpoint : IExtensibleDataObject
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
		public string Address
		{
			get
			{
				return this.AddressField;
			}
			set
			{
				this.AddressField = value;
			}
		}

		[DataMember]
		public string Name
		{
			get
			{
				return this.NameField;
			}
			set
			{
				this.NameField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string AddressField;

		private string NameField;
	}
}
