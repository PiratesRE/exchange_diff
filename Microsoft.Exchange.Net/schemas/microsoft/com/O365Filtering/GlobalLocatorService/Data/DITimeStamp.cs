using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data
{
	[KnownType(typeof(DIDomainInfo))]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[KnownType(typeof(DITenantInfo))]
	[DebuggerStepThrough]
	[KnownType(typeof(GLSProperty))]
	[DataContract(Name = "DITimeStamp", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class DITimeStamp : IExtensibleDataObject
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
		public DateTime? ChangedDatetime
		{
			get
			{
				return this.ChangedDatetimeField;
			}
			set
			{
				this.ChangedDatetimeField = value;
			}
		}

		[DataMember]
		public DateTime? CreatedDatetime
		{
			get
			{
				return this.CreatedDatetimeField;
			}
			set
			{
				this.CreatedDatetimeField = value;
			}
		}

		[DataMember]
		public DateTime? DeletedDatetime
		{
			get
			{
				return this.DeletedDatetimeField;
			}
			set
			{
				this.DeletedDatetimeField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private DateTime? ChangedDatetimeField;

		private DateTime? CreatedDatetimeField;

		private DateTime? DeletedDatetimeField;
	}
}
