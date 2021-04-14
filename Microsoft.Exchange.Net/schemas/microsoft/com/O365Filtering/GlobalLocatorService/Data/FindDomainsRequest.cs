using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data
{
	[DataContract(Name = "FindDomainsRequest", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class FindDomainsRequest : IExtensibleDataObject
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
		public string[] DomainPropertyNames
		{
			get
			{
				return this.DomainPropertyNamesField;
			}
			set
			{
				this.DomainPropertyNamesField = value;
			}
		}

		[DataMember(IsRequired = true)]
		public string[] DomainsName
		{
			get
			{
				return this.DomainsNameField;
			}
			set
			{
				this.DomainsNameField = value;
			}
		}

		[DataMember]
		public int ReadFlag
		{
			get
			{
				return this.ReadFlagField;
			}
			set
			{
				this.ReadFlagField = value;
			}
		}

		[DataMember]
		public string[] TenantPropertyNames
		{
			get
			{
				return this.TenantPropertyNamesField;
			}
			set
			{
				this.TenantPropertyNamesField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string[] DomainPropertyNamesField;

		private string[] DomainsNameField;

		private int ReadFlagField;

		private string[] TenantPropertyNamesField;
	}
}
