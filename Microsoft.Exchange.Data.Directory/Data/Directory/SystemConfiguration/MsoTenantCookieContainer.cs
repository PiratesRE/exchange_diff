using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class MsoTenantCookieContainer : Organization
	{
		internal MultiValuedProperty<byte[]> MsoForwardSyncRecipientCookie
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[MsoTenantCookieContainerSchema.MsoForwardSyncRecipientCookie];
			}
		}

		internal MultiValuedProperty<byte[]> MsoForwardSyncNonRecipientCookie
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[MsoTenantCookieContainerSchema.MsoForwardSyncNonRecipientCookie];
			}
		}

		internal string ExternalDirectoryOrganizationId
		{
			get
			{
				return (string)this[MsoTenantCookieContainerSchema.ExternalDirectoryOrganizationId];
			}
		}

		internal MultiValuedProperty<string> DirSyncStatus
		{
			get
			{
				return (MultiValuedProperty<string>)this[OrganizationSchema.DirSyncStatus];
			}
		}

		internal string DirSyncServiceInstance
		{
			get
			{
				return (string)this[MsoTenantCookieContainerSchema.DirSyncServiceInstance];
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ExchangeConfigurationUnit.MostDerivedClass;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return MsoTenantCookieContainer.schema;
			}
		}

		private static readonly MsoTenantCookieContainerSchema schema = ObjectSchema.GetInstance<MsoTenantCookieContainerSchema>();
	}
}
