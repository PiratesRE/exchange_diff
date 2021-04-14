using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "DomainInfo", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class DomainInfo : IExtensibleDataObject
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
		public string DomainKey
		{
			get
			{
				return this.DomainKeyField;
			}
			set
			{
				this.DomainKeyField = value;
			}
		}

		[DataMember(IsRequired = true)]
		public string DomainName
		{
			get
			{
				return this.DomainNameField;
			}
			set
			{
				this.DomainNameField = value;
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

		private ExtensionDataObject extensionDataField;

		private string DomainKeyField;

		private string DomainNameField;

		private string[] NoneExistNamespacesField;

		private KeyValuePair<string, string>[] PropertiesField;
	}
}
