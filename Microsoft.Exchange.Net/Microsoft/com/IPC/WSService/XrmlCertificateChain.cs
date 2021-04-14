using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.com.IPC.WSService
{
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[DataContract(Name = "XrmlCertificateChain", Namespace = "http://microsoft.com/IPC/WSService")]
	[DebuggerStepThrough]
	public class XrmlCertificateChain : IExtensibleDataObject
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
		public string[] CertificateChain
		{
			get
			{
				return this.CertificateChainField;
			}
			set
			{
				this.CertificateChainField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string[] CertificateChainField;
	}
}
