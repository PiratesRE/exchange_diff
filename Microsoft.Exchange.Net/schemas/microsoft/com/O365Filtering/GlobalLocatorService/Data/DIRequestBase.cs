using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data
{
	[DataContract(Name = "DIRequestBase", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	[DebuggerStepThrough]
	[KnownType(typeof(DIFindTenantRequest))]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[KnownType(typeof(DIFindDomainsRequest))]
	public class DIRequestBase : IExtensibleDataObject
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

		[DataMember(IsRequired = true)]
		public string[] Namespaces
		{
			get
			{
				return this.NamespacesField;
			}
			set
			{
				this.NamespacesField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string[] NamespacesField;
	}
}
