using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class GlobalLocatorServiceMsaUser : ConfigurableObject
	{
		internal GlobalLocatorServiceMsaUser() : base(new SimpleProviderPropertyBag())
		{
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		public Guid ExternalDirectoryOrganizationId
		{
			get
			{
				return (Guid)this[GlobalLocatorServiceMsaUserSchema.ExternalDirectoryOrganizationId];
			}
			set
			{
				this[GlobalLocatorServiceMsaUserSchema.ExternalDirectoryOrganizationId] = value;
			}
		}

		public SmtpAddress MsaUserMemberName
		{
			get
			{
				return (SmtpAddress)this[GlobalLocatorServiceMsaUserSchema.MsaUserMemberName];
			}
			set
			{
				this[GlobalLocatorServiceMsaUserSchema.MsaUserMemberName] = value;
			}
		}

		public NetID MsaUserNetId
		{
			get
			{
				return (NetID)this[GlobalLocatorServiceMsaUserSchema.MsaUserNetId];
			}
			set
			{
				this[GlobalLocatorServiceMsaUserSchema.MsaUserNetId] = value;
			}
		}

		public string ResourceForest
		{
			get
			{
				return (string)this[GlobalLocatorServiceMsaUserSchema.ResourceForest];
			}
			set
			{
				this[GlobalLocatorServiceMsaUserSchema.ResourceForest] = value;
			}
		}

		public string AccountForest
		{
			get
			{
				return (string)this[GlobalLocatorServiceMsaUserSchema.AccountForest];
			}
			set
			{
				this[GlobalLocatorServiceMsaUserSchema.AccountForest] = value;
			}
		}

		public string TenantContainerCN
		{
			get
			{
				return (string)this[GlobalLocatorServiceMsaUserSchema.TenantContainerCN];
			}
			set
			{
				this[GlobalLocatorServiceMsaUserSchema.TenantContainerCN] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<GlobalLocatorServiceMsaUserSchema>();
			}
		}
	}
}
