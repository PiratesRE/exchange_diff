using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "AccountSkuIdentifier", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[DebuggerStepThrough]
	public class AccountSkuIdentifier : IExtensibleDataObject
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
		public string AccountName
		{
			get
			{
				return this.AccountNameField;
			}
			set
			{
				this.AccountNameField = value;
			}
		}

		[DataMember]
		public string SkuPartNumber
		{
			get
			{
				return this.SkuPartNumberField;
			}
			set
			{
				this.SkuPartNumberField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string AccountNameField;

		private string SkuPartNumberField;
	}
}
