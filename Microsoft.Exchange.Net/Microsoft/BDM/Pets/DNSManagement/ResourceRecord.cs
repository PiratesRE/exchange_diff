using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.BDM.Pets.SharedLibrary.Enums;

namespace Microsoft.BDM.Pets.DNSManagement
{
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "ResourceRecord", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.BDM.Pets.DNSManagement")]
	public class ResourceRecord : IExtensibleDataObject
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
		public bool IsDeleted
		{
			get
			{
				return this.IsDeletedField;
			}
			set
			{
				this.IsDeletedField = value;
			}
		}

		[DataMember]
		public ResourceRecordType RecordType
		{
			get
			{
				return this.RecordTypeField;
			}
			set
			{
				this.RecordTypeField = value;
			}
		}

		[DataMember]
		public Guid ResourceRecordId
		{
			get
			{
				return this.ResourceRecordIdField;
			}
			set
			{
				this.ResourceRecordIdField = value;
			}
		}

		[DataMember]
		public int TTL
		{
			get
			{
				return this.TTLField;
			}
			set
			{
				this.TTLField = value;
			}
		}

		[DataMember]
		public string Value
		{
			get
			{
				return this.ValueField;
			}
			set
			{
				this.ValueField = value;
			}
		}

		[DataMember]
		public Guid ZoneGUID
		{
			get
			{
				return this.ZoneGUIDField;
			}
			set
			{
				this.ZoneGUIDField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string DomainNameField;

		private bool IsDeletedField;

		private ResourceRecordType RecordTypeField;

		private Guid ResourceRecordIdField;

		private int TTLField;

		private string ValueField;

		private Guid ZoneGUIDField;
	}
}
