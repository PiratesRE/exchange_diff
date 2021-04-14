using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class StampGroup : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				if (this.schema == null)
				{
					this.schema = ObjectSchema.GetInstance<StampGroupSchema>();
				}
				return this.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return StampGroup.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public new string Name
		{
			get
			{
				return (string)this[StampGroupSchema.Name];
			}
			internal set
			{
				this[StampGroupSchema.Name] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> Servers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[StampGroupSchema.Servers];
			}
		}

		internal bool IsStampGroupEmpty()
		{
			using (MultiValuedProperty<ADObjectId>.Enumerator enumerator = this.Servers.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					ADObjectId adobjectId = enumerator.Current;
					return false;
				}
			}
			return true;
		}

		private static string mostDerivedClass = "msExchMDBAvailabilityGroup";

		[NonSerialized]
		private StampGroupSchema schema;
	}
}
