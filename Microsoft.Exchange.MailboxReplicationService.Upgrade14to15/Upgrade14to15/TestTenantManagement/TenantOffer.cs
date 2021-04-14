using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15.TestTenantManagement
{
	[DataContract(Name = "TenantOffer", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.SyntheticSvc.Contracts")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	public class TenantOffer : IExtensibleDataObject
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
		public string Description
		{
			get
			{
				return this.DescriptionField;
			}
			set
			{
				this.DescriptionField = value;
			}
		}

		[DataMember]
		public int LicenseCount
		{
			get
			{
				return this.LicenseCountField;
			}
			set
			{
				this.LicenseCountField = value;
			}
		}

		[DataMember]
		public string Name
		{
			get
			{
				return this.NameField;
			}
			set
			{
				this.NameField = value;
			}
		}

		[DataMember]
		public string OcpOffer
		{
			get
			{
				return this.OcpOfferField;
			}
			set
			{
				this.OcpOfferField = value;
			}
		}

		[DataMember]
		public OfferType OfferType
		{
			get
			{
				return this.OfferTypeField;
			}
			set
			{
				this.OfferTypeField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string DescriptionField;

		private int LicenseCountField;

		private string NameField;

		private string OcpOfferField;

		private OfferType OfferTypeField;
	}
}
