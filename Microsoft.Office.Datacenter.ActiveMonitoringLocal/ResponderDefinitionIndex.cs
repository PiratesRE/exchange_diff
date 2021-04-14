using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal class ResponderDefinitionIndex : WorkDefinitionIndex<ResponderDefinition>
	{
		internal static IIndexDescriptor<ResponderDefinition, string> AlertMask(string alertMask)
		{
			return new ResponderDefinitionIndex.ResponderDefinitionIndexDescriptorForAlertMask(alertMask);
		}

		private class ResponderDefinitionIndexDescriptorForAlertMask : WorkDefinitionIndex<ResponderDefinition>.WorkDefinitionIndexBase<ResponderDefinition, string>
		{
			internal ResponderDefinitionIndexDescriptorForAlertMask(string key) : base(key)
			{
			}

			public override IEnumerable<string> GetKeyValues(ResponderDefinition item)
			{
				yield return item.AlertMask;
				yield break;
			}

			public override IDataAccessQuery<ResponderDefinition> ApplyIndexRestriction(IDataAccessQuery<ResponderDefinition> query)
			{
				IEnumerable<ResponderDefinition> enumerable = from d in query
				select d;
				if (IndexCapabilities.SupportsCaseInsensitiveStringComparison)
				{
					enumerable = from d in enumerable
					where d.AlertMask.Equals(base.Key, StringComparison.OrdinalIgnoreCase)
					select d;
				}
				else
				{
					enumerable = from d in enumerable
					where d.AlertMask.Equals(base.Key)
					select d;
				}
				return query.AsDataAccessQuery<ResponderDefinition>(enumerable);
			}
		}
	}
}
