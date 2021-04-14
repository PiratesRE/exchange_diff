using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.VariantConfiguration
{
	public class VariantTypeCollection
	{
		private VariantTypeCollection(IEnumerable<VariantType> variants)
		{
			this.variants = new Dictionary<string, VariantType>(StringComparer.OrdinalIgnoreCase);
			foreach (VariantType variantType in variants)
			{
				this.variants.Add(variantType.Name, variantType);
			}
		}

		public static VariantTypeCollection Create(IEnumerable<VariantType> variants)
		{
			return new VariantTypeCollection(variants);
		}

		public IEnumerable<string> GetNames(bool includeInternal)
		{
			if (includeInternal)
			{
				return this.variants.Keys;
			}
			return from key in this.variants.Keys
			where this.variants[key].Flags.HasFlag(VariantTypeFlags.Public)
			select key;
		}

		public bool Contains(string name, bool includeInternal)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (name.Contains('.'))
			{
				name = name.Split(new char[]
				{
					'.'
				})[0];
				return this.variants.ContainsKey(name) && this.variants[name].Flags.HasFlag(VariantTypeFlags.Prefix) && (includeInternal || this.variants[name].Flags.HasFlag(VariantTypeFlags.Public));
			}
			return this.variants.ContainsKey(name) && (includeInternal || this.variants[name].Flags.HasFlag(VariantTypeFlags.Public));
		}

		public VariantType GetVariantByName(string name, bool includeInternal)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			name = name.Split(new char[]
			{
				'.'
			})[0];
			name = this.GetNames(includeInternal).First((string entry) => name.Equals(entry, StringComparison.InvariantCultureIgnoreCase));
			return this.variants[name];
		}

		private readonly IDictionary<string, VariantType> variants;
	}
}
