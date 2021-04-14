using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15.TestTenantManagement
{
	[DataContract(Name = "Tenant", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.SyntheticSvc.Contracts")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class Tenant : IExtensibleDataObject
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
		public string AdminPassword
		{
			get
			{
				return this.AdminPasswordField;
			}
			set
			{
				this.AdminPasswordField = value;
			}
		}

		[DataMember]
		public string AdminUPN
		{
			get
			{
				return this.AdminUPNField;
			}
			set
			{
				this.AdminUPNField = value;
			}
		}

		[DataMember]
		public string ExchangeServiceInstance
		{
			get
			{
				return this.ExchangeServiceInstanceField;
			}
			set
			{
				this.ExchangeServiceInstanceField = value;
			}
		}

		[DataMember]
		public string ForefrontServiceInstance
		{
			get
			{
				return this.ForefrontServiceInstanceField;
			}
			set
			{
				this.ForefrontServiceInstanceField = value;
			}
		}

		[DataMember]
		public string LyncServiceInstance
		{
			get
			{
				return this.LyncServiceInstanceField;
			}
			set
			{
				this.LyncServiceInstanceField = value;
			}
		}

		[DataMember]
		public string SharepointServiceInstance
		{
			get
			{
				return this.SharepointServiceInstanceField;
			}
			set
			{
				this.SharepointServiceInstanceField = value;
			}
		}

		[DataMember]
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

		[DataMember]
		public TenantOffer[] TenantOffers
		{
			get
			{
				return this.TenantOffersField;
			}
			set
			{
				this.TenantOffersField = value;
			}
		}

		[DataMember]
		public string TestPartnerScenario
		{
			get
			{
				return this.TestPartnerScenarioField;
			}
			set
			{
				this.TestPartnerScenarioField = value;
			}
		}

		private ExtensionDataObject extensionDataField;

		private string AdminPasswordField;

		private string AdminUPNField;

		private string ExchangeServiceInstanceField;

		private string ForefrontServiceInstanceField;

		private string LyncServiceInstanceField;

		private string SharepointServiceInstanceField;

		private Guid TenantIdField;

		private TenantOffer[] TenantOffersField;

		private string TestPartnerScenarioField;
	}
}
