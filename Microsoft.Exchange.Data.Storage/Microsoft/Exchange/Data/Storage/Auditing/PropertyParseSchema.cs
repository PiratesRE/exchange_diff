using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PropertyParseSchema
	{
		public PropertyParseSchema(string label, SimpleProviderPropertyDefinition propertyDefinition, Func<string, object> propertyParser)
		{
			this.Label = label;
			this.Property = propertyDefinition;
			if (propertyParser == null)
			{
				this.PropertyParser = delegate(string line)
				{
					if (!string.IsNullOrEmpty(line))
					{
						return line;
					}
					return null;
				};
				return;
			}
			this.PropertyParser = propertyParser;
		}

		public string Label { get; private set; }

		public SimpleProviderPropertyDefinition Property { get; private set; }

		public Func<string, object> PropertyParser { get; private set; }
	}
}
