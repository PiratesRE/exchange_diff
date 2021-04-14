using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data
{
	[DataContract(Name = "TenantInfo", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class TenantInfo : IExtensibleDataObject
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
		public string[] NoneExistNamespaces
		{
			get
			{
				return this.NoneExistNamespacesField;
			}
			set
			{
				this.NoneExistNamespacesField = value;
			}
		}

		[DataMember(IsRequired = true)]
		public KeyValuePair<string, string>[] Properties
		{
			get
			{
				return this.PropertiesField;
			}
			set
			{
				this.PropertiesField = value;
			}
		}

		[DataMember(IsRequired = true)]
		public Guid TenantId
		{
			get
			{
				return this.TenantIdField;
			}
			set
			{
				this.TenantIdField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string[] NoneExistNamespacesField;

		private KeyValuePair<string, string>[] PropertiesField;

		private Guid TenantIdField;
	}
}
