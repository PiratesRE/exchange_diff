using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.BDM.Pets.DNSManagement
{
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "Zone", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.BDM.Pets.DNSManagement")]
	public class Zone : IExtensibleDataObject
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
		public bool IsDisabled
		{
			get
			{
				return this.IsDisabledField;
			}
			set
			{
				this.IsDisabledField = value;
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

		private bool IsDisabledField;

		private Guid ZoneGUIDField;
	}
}
