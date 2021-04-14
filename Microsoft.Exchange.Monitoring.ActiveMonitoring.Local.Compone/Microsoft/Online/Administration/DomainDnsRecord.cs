using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DataContract(Name = "DomainDnsRecord", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[KnownType(typeof(DomainDnsTxtRecord))]
	[KnownType(typeof(DomainDnsMXRecord))]
	[KnownType(typeof(DomainDnsNullRecord))]
	[KnownType(typeof(DomainDnsSrvRecord))]
	[KnownType(typeof(DomainDnsCnameRecord))]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class DomainDnsRecord : IExtensibleDataObject
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
		public DomainCapabilities? Capability
		{
			get
			{
				return this.CapabilityField;
			}
			set
			{
				this.CapabilityField = value;
			}
		}

		[DataMember]
		public bool? IsOptional
		{
			get
			{
				return this.IsOptionalField;
			}
			set
			{
				this.IsOptionalField = value;
			}
		}

		[DataMember]
		public string Label
		{
			get
			{
				return this.LabelField;
			}
			set
			{
				this.LabelField = value;
			}
		}

		[DataMember]
		public Guid? ObjectId
		{
			get
			{
				return this.ObjectIdField;
			}
			set
			{
				this.ObjectIdField = value;
			}
		}

		[DataMember]
		public int? Ttl
		{
			get
			{
				return this.TtlField;
			}
			set
			{
				this.TtlField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private DomainCapabilities? CapabilityField;

		private bool? IsOptionalField;

		private string LabelField;

		private Guid? ObjectIdField;

		private int? TtlField;
	}
}
