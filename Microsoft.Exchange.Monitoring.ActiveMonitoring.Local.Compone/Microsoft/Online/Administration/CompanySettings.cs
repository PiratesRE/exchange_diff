using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DataContract(Name = "CompanySettings", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class CompanySettings : IExtensibleDataObject
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
		public bool? SelfServePasswordResetEnabled
		{
			get
			{
				return this.SelfServePasswordResetEnabledField;
			}
			set
			{
				this.SelfServePasswordResetEnabledField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private bool? SelfServePasswordResetEnabledField;
	}
}
