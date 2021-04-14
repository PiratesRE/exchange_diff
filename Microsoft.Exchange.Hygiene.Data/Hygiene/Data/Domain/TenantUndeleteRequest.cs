using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Domain
{
	internal class TenantUndeleteRequest : ConfigurablePropertyBag
	{
		public Guid TenantId
		{
			get
			{
				return (Guid)this[TenantUndeleteRequest.TenantIdPropertyDefinition];
			}
			set
			{
				this[TenantUndeleteRequest.TenantIdPropertyDefinition] = value;
			}
		}

		public DateTime DeletedDatetime
		{
			get
			{
				return (DateTime)this[TenantUndeleteRequest.DeletedDatetimePropertyDefinition];
			}
			set
			{
				this[TenantUndeleteRequest.DeletedDatetimePropertyDefinition] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal static readonly HygienePropertyDefinition TenantIdPropertyDefinition = new HygienePropertyDefinition("id_TenantId", typeof(Guid));

		internal static readonly HygienePropertyDefinition DeletedDatetimePropertyDefinition = new HygienePropertyDefinition("dt_DeletedDatetime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
