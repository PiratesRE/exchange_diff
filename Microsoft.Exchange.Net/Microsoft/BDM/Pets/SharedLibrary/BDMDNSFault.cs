using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Microsoft.BDM.Pets.SharedLibrary
{
	[KnownType(typeof(DomainNameInUseFault))]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[KnownType(typeof(KeyNotFoundFault))]
	[KnownType(typeof(InvalidArgumentFault))]
	[DataContract(Name = "BDMDNSFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.BDM.Pets.SharedLibrary")]
	[DebuggerStepThrough]
	public class BDMDNSFault : IExtensibleDataObject
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
		public string Description
		{
			get
			{
				return this.DescriptionField;
			}
			set
			{
				this.DescriptionField = value;
			}
		}

		[DataMember]
		public ExceptionDetail ErrorDetails
		{
			get
			{
				return this.ErrorDetailsField;
			}
			set
			{
				this.ErrorDetailsField = value;
			}
		}

		[DataMember]
		public string ReasonCode
		{
			get
			{
				return this.ReasonCodeField;
			}
			set
			{
				this.ReasonCodeField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string DescriptionField;

		private ExceptionDetail ErrorDetailsField;

		private string ReasonCodeField;
	}
}
