using System;

namespace Microsoft.Exchange.Services.Core.CssConverter
{
	public class CssProperty
	{
		public CssProperty()
		{
		}

		public CssProperty(string propertyName, string propertyValue)
		{
			this.Name = propertyName;
			this.Value = propertyValue;
		}

		public string Name { get; set; }

		public string Value { get; set; }

		public override string ToString()
		{
			return string.Format("{0}: {1}; ", this.Name, this.Value);
		}
	}
}
