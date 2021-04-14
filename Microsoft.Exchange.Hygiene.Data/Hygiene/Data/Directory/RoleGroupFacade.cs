using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class RoleGroupFacade : ADObject
	{
		public override ObjectId Identity
		{
			get
			{
				return null;
			}
		}

		public MultiValuedProperty<string> Members
		{
			get
			{
				return (MultiValuedProperty<string>)this[RoleGroupFacadeSchema.Members];
			}
			set
			{
				this[RoleGroupFacadeSchema.Members] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return RoleGroupFacade.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return RoleGroupFacade.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly RoleGroupFacadeSchema schema = ObjectSchema.GetInstance<RoleGroupFacadeSchema>();

		private static string mostDerivedClass = "RoleGroupFacade";
	}
}
