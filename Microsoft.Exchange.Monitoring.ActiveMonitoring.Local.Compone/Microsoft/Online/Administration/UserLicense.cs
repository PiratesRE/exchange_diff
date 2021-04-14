using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DataContract(Name = "UserLicense", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class UserLicense : IExtensibleDataObject
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
		public AccountSkuIdentifier AccountSku
		{
			get
			{
				return this.AccountSkuField;
			}
			set
			{
				this.AccountSkuField = value;
			}
		}

		[DataMember]
		public string AccountSkuId
		{
			get
			{
				return this.AccountSkuIdField;
			}
			set
			{
				this.AccountSkuIdField = value;
			}
		}

		[DataMember]
		public ServiceStatus[] ServiceStatus
		{
			get
			{
				return this.ServiceStatusField;
			}
			set
			{
				this.ServiceStatusField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private AccountSkuIdentifier AccountSkuField;

		private string AccountSkuIdField;

		private ServiceStatus[] ServiceStatusField;
	}
}
