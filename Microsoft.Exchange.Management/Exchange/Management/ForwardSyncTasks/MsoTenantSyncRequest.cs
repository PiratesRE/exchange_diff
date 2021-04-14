using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Sync.CookieManager;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[Serializable]
	public class MsoTenantSyncRequest : ConfigurableObject
	{
		public MsoTenantSyncRequest() : base(new SimpleProviderPropertyBag())
		{
		}

		internal MsoTenantSyncRequest(MsoTenantCookieContainer organization, MsoFullSyncCookie recipientCookie, MsoFullSyncCookie companyCookie) : base(new SimpleProviderPropertyBag())
		{
			this.propertyBag.SetField(this.propertyBag.ObjectVersionPropertyDefinition, ExchangeObjectVersion.Exchange2010);
			this.propertyBag.SetField(this.propertyBag.ObjectIdentityPropertyDefinition, organization.Identity);
			this.ExternalDirectoryOrganizationId = organization.ExternalDirectoryOrganizationId;
			this.ServiceInstanceId = organization.DirSyncServiceInstance;
			MsoFullSyncCookie msoFullSyncCookie = recipientCookie ?? companyCookie;
			if (msoFullSyncCookie != null)
			{
				this.SyncType = msoFullSyncCookie.SyncType;
				this.Requestor = msoFullSyncCookie.SyncRequestor;
				this.WhenCreated = ((msoFullSyncCookie.WhenSyncRequested != DateTime.MinValue) ? new DateTime?(msoFullSyncCookie.WhenSyncRequested) : null);
				this.WhenSyncStarted = ((msoFullSyncCookie.WhenSyncStarted != DateTime.MinValue) ? new DateTime?(msoFullSyncCookie.WhenSyncStarted) : null);
				if (recipientCookie != null)
				{
					this.WhenLastRecipientCookieCommitted = ((recipientCookie.Timestamp != DateTime.MinValue) ? new DateTime?(recipientCookie.Timestamp) : null);
				}
				if (companyCookie != null)
				{
					this.WhenLastCompanyCookieCommitted = ((companyCookie.Timestamp != DateTime.MinValue) ? new DateTime?(companyCookie.Timestamp) : null);
				}
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MsoTenantSyncRequest.Schema;
			}
		}

		public TenantSyncType SyncType
		{
			get
			{
				return (TenantSyncType)base[MsoTenantSyncRequestSchema.TenantSyncType];
			}
			private set
			{
				base[MsoTenantSyncRequestSchema.TenantSyncType] = value;
			}
		}

		public string Requestor
		{
			get
			{
				return (string)base[MsoTenantSyncRequestSchema.Requestor];
			}
			private set
			{
				base[MsoTenantSyncRequestSchema.Requestor] = value;
			}
		}

		public string ExternalDirectoryOrganizationId
		{
			get
			{
				return (string)base[MsoTenantSyncRequestSchema.ExternalDirectoryOrganizationId];
			}
			private set
			{
				base[MsoTenantSyncRequestSchema.ExternalDirectoryOrganizationId] = value;
			}
		}

		public string ServiceInstanceId
		{
			get
			{
				return (string)base[MsoTenantSyncRequestSchema.ServiceInstanceId];
			}
			private set
			{
				base[MsoTenantSyncRequestSchema.ServiceInstanceId] = value;
			}
		}

		public DateTime? WhenSyncStarted
		{
			get
			{
				return (DateTime?)base[MsoTenantSyncRequestSchema.WhenSyncStarted];
			}
			private set
			{
				base[MsoTenantSyncRequestSchema.WhenSyncStarted] = value;
			}
		}

		public DateTime? WhenLastRecipientCookieCommitted
		{
			get
			{
				return (DateTime?)base[MsoTenantSyncRequestSchema.WhenLastRecipientCookieCommitted];
			}
			private set
			{
				base[MsoTenantSyncRequestSchema.WhenLastRecipientCookieCommitted] = value;
			}
		}

		public DateTime? WhenLastCompanyCookieCommitted
		{
			get
			{
				return (DateTime?)base[MsoTenantSyncRequestSchema.WhenLastCompanyCookieCommitted];
			}
			private set
			{
				base[MsoTenantSyncRequestSchema.WhenLastCompanyCookieCommitted] = value;
			}
		}

		public DateTime? WhenCreated
		{
			get
			{
				return new DateTime?((DateTime)base[MsoTenantSyncRequestSchema.WhenCreated]);
			}
			private set
			{
				base[MsoTenantSyncRequestSchema.WhenCreated] = value;
			}
		}

		private static readonly MsoTenantSyncRequestSchema Schema = ObjectSchema.GetInstance<MsoTenantSyncRequestSchema>();
	}
}
