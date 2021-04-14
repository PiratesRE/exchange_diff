using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class EsnTenant : ConfigurablePropertyBag
	{
		public Guid OrganizationalUnitRoot
		{
			get
			{
				return (Guid)this[EsnTenantSchema.OrganizationalUnitRootProperty];
			}
		}

		public int RecipientCount
		{
			get
			{
				return (int)this[EsnTenantSchema.RecipientCountProperty];
			}
		}

		public int MessageCount
		{
			get
			{
				return (int)this[EsnTenantSchema.MessageCountProperty];
			}
		}

		public override ObjectId Identity
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(EsnTenantSchema);
		}
	}
}
