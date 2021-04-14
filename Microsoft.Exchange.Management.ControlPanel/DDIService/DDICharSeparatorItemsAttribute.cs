using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.Exchange.Management.DDIService
{
	public class DDICharSeparatorItemsAttribute : DDICollectionDecoratorAttribute
	{
		[DefaultValue(" ")]
		public string Separators { get; set; }

		public DDICharSeparatorItemsAttribute()
		{
			this.Separators = ", ";
		}

		public override List<string> Validate(object target, Service profile)
		{
			string text = target as string;
			List<string> target2 = new List<string>();
			if (!string.IsNullOrEmpty(text))
			{
				target2 = text.Split(this.Separators.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList<string>();
			}
			return base.Validate(target2, profile);
		}
	}
}
