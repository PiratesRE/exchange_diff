using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal class ProbeDefinitionIndex : WorkDefinitionIndex<ProbeDefinition>
	{
		internal static IIndexDescriptor<ProbeDefinition, string> TypeName(string typeName)
		{
			return new ProbeDefinitionIndex.ProbeDefinitionIndexDescriptorForTypeName(typeName);
		}

		private class ProbeDefinitionIndexDescriptorForTypeName : WorkDefinitionIndex<ProbeDefinition>.WorkDefinitionIndexBase<ProbeDefinition, string>
		{
			internal ProbeDefinitionIndexDescriptorForTypeName(string key) : base(key)
			{
			}

			public override IEnumerable<string> GetKeyValues(ProbeDefinition item)
			{
				yield return item.TypeName;
				yield break;
			}

			public override IDataAccessQuery<ProbeDefinition> ApplyIndexRestriction(IDataAccessQuery<ProbeDefinition> query)
			{
				IEnumerable<ProbeDefinition> enumerable = from d in query
				select d;
				if (IndexCapabilities.SupportsCaseInsensitiveStringComparison)
				{
					enumerable = from d in enumerable
					where d.TypeName.Equals(base.Key, StringComparison.OrdinalIgnoreCase)
					select d;
				}
				else
				{
					enumerable = from d in enumerable
					where d.TypeName.Equals(base.Key)
					select d;
				}
				return query.AsDataAccessQuery<ProbeDefinition>(enumerable);
			}
		}
	}
}
