using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Clients.Common;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public class BootSlabDefinition : SlabDefinition
	{
		public override Slab GetSlab(string[] featureNames, LayoutType layout, out IEnumerable<string> slabDependencies)
		{
			slabDependencies = new string[0];
			return this.GetSlab(featureNames, layout);
		}

		public Slab GetSlab(ICollection<string> featureNames, LayoutType layout)
		{
			if (featureNames == null)
			{
				throw new ArgumentNullException("featureNames");
			}
			SlabBinding binding = this.FindClosestBinding(featureNames);
			IEnumerable<string> enumerable;
			return this.GetSlabForBinding(binding, layout, out enumerable);
		}

		private SlabBinding FindClosestBinding(ICollection<string> featureNames)
		{
			if (featureNames.Count == 0)
			{
				return base.FindDefaultBinding();
			}
			SlabBinding slabBinding = base.Bindings.FirstOrDefault((SlabBinding b) => b.IsDefault);
			int num = (slabBinding != null) ? 0 : -1;
			foreach (SlabBinding slabBinding2 in base.Bindings)
			{
				bool flag = slabBinding2.Features.Any((string f) => !featureNames.Contains(f));
				if (slabBinding != slabBinding2 && !flag)
				{
					int num2 = (from feature in slabBinding2.Features
					where featureNames.Contains(feature, StringComparer.OrdinalIgnoreCase)
					select feature).Count<string>();
					if ((slabBinding != null || num2 != 0) && num2 > 0 && num < num2)
					{
						slabBinding = slabBinding2;
						num = num2;
					}
				}
			}
			if (slabBinding == null)
			{
				throw new SlabBindingNotFoundException("Closest binding cannot be found");
			}
			return slabBinding;
		}
	}
}
