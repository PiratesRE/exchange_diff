using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services
{
	[KnownType(typeof(CompanyResponseInfoSet))]
	[DataContract(Name = "ResponseInfoSet", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[KnownType(typeof(DomainResponseInfoSet))]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[DebuggerStepThrough]
	[Serializable]
	internal class ResponseInfoSet : IExtensibleDataObject
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
		internal bool OperationSuccessful
		{
			get
			{
				return this.OperationSuccessfulField;
			}
			set
			{
				this.OperationSuccessfulField = value;
			}
		}

		[NonSerialized]
		private ExtensionDataObject extensionDataField;

		[OptionalField]
		private bool OperationSuccessfulField;
	}
}
