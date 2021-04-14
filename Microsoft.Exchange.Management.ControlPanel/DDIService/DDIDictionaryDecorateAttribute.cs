using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Exchange.Management.DDIService
{
	public class DDIDictionaryDecorateAttribute : DDICollectionDecoratorAttribute
	{
		[DefaultValue(true)]
		public bool UseKeys { get; set; }

		public DDIDictionaryDecorateAttribute()
		{
			this.UseKeys = true;
		}

		public override List<string> Validate(object target, Service profile)
		{
			IEnumerable target2 = null;
			if (target != null)
			{
				IDictionary dictionary = target as IDictionary;
				if (dictionary == null)
				{
					throw new ArgumentException("DDIDictionaryDecorateAttribute can only be applied to type which implemented the IDictionary interface");
				}
				target2 = (this.UseKeys ? dictionary.Keys : dictionary.Values);
			}
			return base.Validate(target2, profile);
		}
	}
}
