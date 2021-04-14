using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "SaveDomainRequest", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class SaveDomainRequest : IExtensibleDataObject
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
		public string CustomTag
		{
			get
			{
				return this.CustomTagField;
			}
			set
			{
				this.CustomTagField = value;
			}
		}

		[DataMember(IsRequired = true)]
		public DomainInfo DomainInfo
		{
			get
			{
				return this.DomainInfoField;
			}
			set
			{
				this.DomainInfoField = value;
			}
		}

		[DataMember]
		public DomainKeyType DomainKeyType
		{
			get
			{
				return this.DomainKeyTypeField;
			}
			set
			{
				this.DomainKeyTypeField = value;
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

		private string CustomTagField;

		private DomainInfo DomainInfoField;

		private DomainKeyType DomainKeyTypeField;

		private Guid TenantIdField;
	}
}
