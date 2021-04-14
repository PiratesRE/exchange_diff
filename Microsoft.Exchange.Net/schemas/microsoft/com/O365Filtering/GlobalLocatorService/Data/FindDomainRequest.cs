using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "FindDomainRequest", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class FindDomainRequest : IExtensibleDataObject
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
		public DomainQuery Domain
		{
			get
			{
				return this.DomainField;
			}
			set
			{
				this.DomainField = value;
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
		public TenantQuery Tenant
		{
			get
			{
				return this.TenantField;
			}
			set
			{
				this.TenantField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private DomainQuery DomainField;

		private int ReadFlagField;

		private TenantQuery TenantField;
	}
}
