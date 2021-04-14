using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Reporting
{
	internal class Tenant : ConfigurablePropertyBag
	{
		public Guid TenantId
		{
			get
			{
				return (Guid)this[Tenant.TenantIdProperty];
			}
			internal set
			{
				this[Tenant.TenantIdProperty] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		protected static readonly HygienePropertyDefinition TenantIdProperty = new HygienePropertyDefinition("tenantId", typeof(Guid), Guid.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
