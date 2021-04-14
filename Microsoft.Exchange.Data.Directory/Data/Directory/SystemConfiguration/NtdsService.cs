using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class NtdsService : ADNonExchangeObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return NtdsService.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "nTDSService";
			}
		}

		internal static object DoListObjectGetter(IPropertyBag propertyBag)
		{
			char heuristicsChar = NtdsService.GetHeuristicsChar((string)propertyBag[NtdsServiceSchema.Heuristics], 2, '0');
			return heuristicsChar == '1';
		}

		private static char GetHeuristicsChar(string heuristics, int position, char defaultChar)
		{
			if (!string.IsNullOrEmpty(heuristics) && position < heuristics.Length)
			{
				return heuristics[position];
			}
			return defaultChar;
		}

		public bool DoListObject
		{
			get
			{
				return (bool)this[NtdsServiceSchema.DoListObject];
			}
		}

		public int TombstoneLifetime
		{
			get
			{
				return (int)this[NtdsServiceSchema.TombstoneLifetime];
			}
		}

		private const string MostDerivedClass = "nTDSService";

		internal static readonly ADObjectId ContainerId = new ADObjectId("CN=Directory Service,CN=Windows NT,CN=Services");

		private static readonly NtdsServiceSchema schema = ObjectSchema.GetInstance<NtdsServiceSchema>();
	}
}
