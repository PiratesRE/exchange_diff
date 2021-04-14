using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.Sync
{
	public abstract class BaseCookie : ADObject
	{
		public BaseCookie()
		{
		}

		public BaseCookie(string serviceInstance, byte[] data) : this()
		{
			if (serviceInstance == null)
			{
				throw new ArgumentNullException("serviceInstance");
			}
			this.ServiceInstance = serviceInstance;
			this.Data = data;
			this.BatchId = CombGuidGenerator.NewGuid();
			this.Version = ExchangeObjectVersion.Current.ToString();
			this[BaseCookieSchema.IdentityProp] = new ADObjectId(CombGuidGenerator.NewGuid());
		}

		public override ObjectId Identity
		{
			get
			{
				return this[BaseCookieSchema.IdentityProp] as ObjectId;
			}
		}

		public string ServiceInstance
		{
			get
			{
				return this[BaseCookieSchema.ServiceInstanceProp] as string;
			}
			set
			{
				this[BaseCookieSchema.ServiceInstanceProp] = value;
			}
		}

		public byte[] Data
		{
			get
			{
				return this[BaseCookieSchema.DataProp] as byte[];
			}
			set
			{
				this[BaseCookieSchema.DataProp] = value;
			}
		}

		public string Version
		{
			get
			{
				return this[BaseCookieSchema.VersionProp] as string;
			}
			set
			{
				this[BaseCookieSchema.VersionProp] = value;
			}
		}

		public string ActiveMachine
		{
			get
			{
				return this[BaseCookieSchema.ActiveMachineProperty] as string;
			}
			set
			{
				this[BaseCookieSchema.ActiveMachineProperty] = value;
			}
		}

		public ProvisioningFlags ProvisioningFlags
		{
			get
			{
				return (ProvisioningFlags)this[BaseCookieSchema.ProvisioningFlagsProperty];
			}
			set
			{
				this[BaseCookieSchema.ProvisioningFlagsProperty] = value;
			}
		}

		public Guid BatchId
		{
			get
			{
				return (Guid)this[BaseCookieSchema.BatchIdProp];
			}
			set
			{
				this[BaseCookieSchema.BatchIdProp] = value;
			}
		}

		public DateTime LastChanged
		{
			get
			{
				return (DateTime)this[BaseCookieSchema.LastChangedProp];
			}
			set
			{
				this[BaseCookieSchema.LastChangedProp] = value;
			}
		}

		public bool Complete
		{
			get
			{
				return (bool)this[BaseCookieSchema.CompleteProp];
			}
			set
			{
				this[BaseCookieSchema.CompleteProp] = value;
			}
		}
	}
}
