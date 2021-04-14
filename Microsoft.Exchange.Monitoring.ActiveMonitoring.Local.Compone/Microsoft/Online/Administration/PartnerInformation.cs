using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DataContract(Name = "PartnerInformation", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class PartnerInformation : IExtensibleDataObject
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
		public CompanyType? CompanyType
		{
			get
			{
				return this.CompanyTypeField;
			}
			set
			{
				this.CompanyTypeField = value;
			}
		}

		[DataMember]
		public Guid[] Contracts
		{
			get
			{
				return this.ContractsField;
			}
			set
			{
				this.ContractsField = value;
			}
		}

		[DataMember]
		public bool? DapEnabled
		{
			get
			{
				return this.DapEnabledField;
			}
			set
			{
				this.DapEnabledField = value;
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
		public string PartnerCommerceUrl
		{
			get
			{
				return this.PartnerCommerceUrlField;
			}
			set
			{
				this.PartnerCommerceUrlField = value;
			}
		}

		[DataMember]
		public string PartnerCompanyName
		{
			get
			{
				return this.PartnerCompanyNameField;
			}
			set
			{
				this.PartnerCompanyNameField = value;
			}
		}

		[DataMember]
		public string PartnerHelpUrl
		{
			get
			{
				return this.PartnerHelpUrlField;
			}
			set
			{
				this.PartnerHelpUrlField = value;
			}
		}

		[DataMember]
		public string[] PartnerSupportEmails
		{
			get
			{
				return this.PartnerSupportEmailsField;
			}
			set
			{
				this.PartnerSupportEmailsField = value;
			}
		}

		[DataMember]
		public string[] PartnerSupportTelephones
		{
			get
			{
				return this.PartnerSupportTelephonesField;
			}
			set
			{
				this.PartnerSupportTelephonesField = value;
			}
		}

		[DataMember]
		public string PartnerSupportUrl
		{
			get
			{
				return this.PartnerSupportUrlField;
			}
			set
			{
				this.PartnerSupportUrlField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private CompanyType? CompanyTypeField;

		private Guid[] ContractsField;

		private bool? DapEnabledField;

		private Guid? ObjectIdField;

		private string PartnerCommerceUrlField;

		private string PartnerCompanyNameField;

		private string PartnerHelpUrlField;

		private string[] PartnerSupportEmailsField;

		private string[] PartnerSupportTelephonesField;

		private string PartnerSupportUrlField;
	}
}
