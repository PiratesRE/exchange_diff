using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	internal class SyncPropertyRefresh : ADObject
	{
		public SyncPropertyRefresh(string serviceInstance)
		{
			if (serviceInstance == null)
			{
				throw new ArgumentNullException("serviceInstance");
			}
			this.ServiceInstance = serviceInstance;
			base.SetId(new ADObjectId(DalHelper.GetTenantDistinguishedName(this.ServiceInstance), CombGuidGenerator.NewGuid()));
		}

		public SyncPropertyRefresh()
		{
		}

		public string ServiceInstance
		{
			get
			{
				return this[ADObjectSchema.RawName] as string;
			}
			set
			{
				this[ADObjectSchema.RawName] = value;
			}
		}

		public SyncPropertyRefreshStatus Status
		{
			get
			{
				return (SyncPropertyRefreshStatus)this[SyncPropertyRefreshSchema.Status];
			}
			set
			{
				this[SyncPropertyRefreshSchema.Status] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return SyncPropertyRefresh.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return SyncPropertyRefresh.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly string mostDerivedClass = "SyncPropertyRefresh";

		private static readonly SyncPropertyRefreshSchema schema = ObjectSchema.GetInstance<SyncPropertyRefreshSchema>();
	}
}
