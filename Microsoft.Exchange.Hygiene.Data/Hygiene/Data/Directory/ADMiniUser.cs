using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[Serializable]
	internal class ADMiniUser : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return this.UserId;
			}
		}

		internal ADObjectId UserId
		{
			get
			{
				return this[ADMiniUserSchema.UserIdProp] as ADObjectId;
			}
			set
			{
				this[ADMiniUserSchema.UserIdProp] = value;
			}
		}

		internal ADObjectId TenantId
		{
			get
			{
				return this[ADMiniUserSchema.TenantIdProp] as ADObjectId;
			}
			set
			{
				this[ADMiniUserSchema.TenantIdProp] = value;
			}
		}

		internal ADObjectId ConfigurationId
		{
			get
			{
				return this[ADMiniUserSchema.ConfigurationIdProp] as ADObjectId;
			}
			set
			{
				this[ADMiniUserSchema.ConfigurationIdProp] = value;
			}
		}

		internal NetID NetId
		{
			get
			{
				return (NetID)this[ADMiniUserSchema.NetIdProp];
			}
			set
			{
				this[ADMiniUserSchema.NetIdProp] = value;
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(ADMiniUserSchema);
		}
	}
}
