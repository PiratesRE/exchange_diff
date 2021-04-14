using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.com.IPC.WSService;

namespace Microsoft.com.IPC.WSServerLicensingService
{
	[DebuggerStepThrough]
	[DataContract(Name = "BatchLicenseResponses", Namespace = "http://microsoft.com/IPC/WSServerLicensingService")]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	public class BatchLicenseResponses : IExtensibleDataObject
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
		public LicenseResponse[] LicenseResponses
		{
			get
			{
				return this.LicenseResponsesField;
			}
			set
			{
				this.LicenseResponsesField = value;
			}
		}

		[DataMember]
		public XrmlCertificateChain ServerLicenseCertificateChain
		{
			get
			{
				return this.ServerLicenseCertificateChainField;
			}
			set
			{
				this.ServerLicenseCertificateChainField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private LicenseResponse[] LicenseResponsesField;

		private XrmlCertificateChain ServerLicenseCertificateChainField;
	}
}
