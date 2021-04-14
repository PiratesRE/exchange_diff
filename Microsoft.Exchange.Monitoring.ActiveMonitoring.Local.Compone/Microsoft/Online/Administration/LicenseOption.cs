using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "LicenseOption", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	public class LicenseOption : IExtensibleDataObject
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
		public AccountSkuIdentifier AccountSkuId
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
		public string[] DisabledServicePlans
		{
			get
			{
				return this.DisabledServicePlansField;
			}
			set
			{
				this.DisabledServicePlansField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private AccountSkuIdentifier AccountSkuIdField;

		private string[] DisabledServicePlansField;
	}
}
