using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class FfoTraceTenant : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return this.TenantId;
			}
		}

		public ADObjectId TenantId
		{
			get
			{
				return this[FfoTraceTenant.TenantIdProp] as ADObjectId;
			}
			set
			{
				this[FfoTraceTenant.TenantIdProp] = value;
			}
		}

		public string DatabaseName
		{
			get
			{
				return this[FfoTraceTenant.DatabaseNameProp] as string;
			}
			set
			{
				this[FfoTraceTenant.DatabaseNameProp] = value;
			}
		}

		public DateTime ChangedDatetime
		{
			get
			{
				return (DateTime)this[FfoTraceTenant.ChangedDatetimeProp];
			}
			set
			{
				this[FfoTraceTenant.ChangedDatetimeProp] = value;
			}
		}

		public DateTime CurrentDatetime
		{
			get
			{
				return (DateTime)this[FfoTraceTenant.CurrentDatetimeProp];
			}
			set
			{
				this[FfoTraceTenant.CurrentDatetimeProp] = value;
			}
		}

		public static readonly ADPropertyDefinition TenantIdProp = ADObjectSchema.OrganizationalUnitRoot;

		public static readonly HygienePropertyDefinition DatabaseNameProp = new HygienePropertyDefinition("DatabaseName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition ChangedDatetimeProp = new HygienePropertyDefinition("ChangedDatetime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition CurrentDatetimeProp = new HygienePropertyDefinition("CurrentDatetime", typeof(DateTime), DateTime.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
