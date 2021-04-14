using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data
{
	[KnownType(typeof(DIFindTenantResponse))]
	[KnownType(typeof(DIFindDomainsResponse))]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "DIResponseBase", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class DIResponseBase : IExtensibleDataObject
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
		public string Diagnostics
		{
			get
			{
				return this.DiagnosticsField;
			}
			set
			{
				this.DiagnosticsField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string DiagnosticsField;
	}
}
