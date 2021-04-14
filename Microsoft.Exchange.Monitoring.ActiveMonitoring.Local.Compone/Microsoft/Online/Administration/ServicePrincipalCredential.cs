using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DataContract(Name = "ServicePrincipalCredential", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class ServicePrincipalCredential : IExtensibleDataObject
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
		public DateTime? EndDate
		{
			get
			{
				return this.EndDateField;
			}
			set
			{
				this.EndDateField = value;
			}
		}

		[DataMember]
		public Guid? KeyGroupId
		{
			get
			{
				return this.KeyGroupIdField;
			}
			set
			{
				this.KeyGroupIdField = value;
			}
		}

		[DataMember]
		public Guid? KeyId
		{
			get
			{
				return this.KeyIdField;
			}
			set
			{
				this.KeyIdField = value;
			}
		}

		[DataMember]
		public DateTime? StartDate
		{
			get
			{
				return this.StartDateField;
			}
			set
			{
				this.StartDateField = value;
			}
		}

		[DataMember]
		public ServicePrincipalCredentialType Type
		{
			get
			{
				return this.TypeField;
			}
			set
			{
				this.TypeField = value;
			}
		}

		[DataMember]
		public ServicePrincipalCredentialUsage? Usage
		{
			get
			{
				return this.UsageField;
			}
			set
			{
				this.UsageField = value;
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

		private ExtensionDataObject extensionDataField;

		private DateTime? EndDateField;

		private Guid? KeyGroupIdField;

		private Guid? KeyIdField;

		private DateTime? StartDateField;

		private ServicePrincipalCredentialType TypeField;

		private ServicePrincipalCredentialUsage? UsageField;

		private string ValueField;
	}
}
