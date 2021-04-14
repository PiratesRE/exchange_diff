using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.BOX.Shell
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "SuiteServiceInfo", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.Shell")]
	[DebuggerStepThrough]
	public class SuiteServiceInfo : IExtensibleDataObject
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
		public string[] SuiteServiceProxyOriginAllowedList
		{
			get
			{
				return this.SuiteServiceProxyOriginAllowedListField;
			}
			set
			{
				this.SuiteServiceProxyOriginAllowedListField = value;
			}
		}

		[DataMember]
		public string SuiteServiceProxyScriptUrl
		{
			get
			{
				return this.SuiteServiceProxyScriptUrlField;
			}
			set
			{
				this.SuiteServiceProxyScriptUrlField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string[] SuiteServiceProxyOriginAllowedListField;

		private string SuiteServiceProxyScriptUrlField;
	}
}
