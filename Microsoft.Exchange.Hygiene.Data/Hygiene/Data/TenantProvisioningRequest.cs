using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class TenantProvisioningRequest : ConfigurablePropertyBag
	{
		public TenantProvisioningRequest()
		{
			this.RequestType = ObjectState.Changed;
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this[TenantProvisioningRequestSchema.OrganizationalUnitRoot].ToString());
			}
		}

		public Guid OrganizationalUnitRoot
		{
			get
			{
				return (Guid)this[TenantProvisioningRequestSchema.OrganizationalUnitRoot];
			}
			set
			{
				this[TenantProvisioningRequestSchema.OrganizationalUnitRoot] = value;
			}
		}

		public ObjectState RequestType
		{
			get
			{
				return (ObjectState)Enum.Parse(typeof(ObjectState), (string)this[TenantProvisioningRequestSchema.RequestType]);
			}
			set
			{
				if (value != ObjectState.Changed && value != ObjectState.Deleted)
				{
					throw new ArgumentException("Only Changed and Deleted are valid RequestType values");
				}
				this[TenantProvisioningRequestSchema.RequestType] = value.ToString();
			}
		}

		public TenantProvisioningRequestFlags RequestFlags
		{
			get
			{
				return (TenantProvisioningRequestFlags)this[TenantProvisioningRequestSchema.RequestFlags];
			}
			set
			{
				this[TenantProvisioningRequestSchema.RequestFlags] = value;
			}
		}

		public string MigrateToRegion
		{
			get
			{
				return this[TenantProvisioningRequestSchema.MigrateToRegion] as string;
			}
			set
			{
				this[TenantProvisioningRequestSchema.MigrateToRegion] = value;
			}
		}

		public string MigrateToInstance
		{
			get
			{
				return this[TenantProvisioningRequestSchema.MigrateToInstance] as string;
			}
			set
			{
				this[TenantProvisioningRequestSchema.MigrateToInstance] = value;
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(TenantProvisioningRequestSchema);
		}
	}
}
