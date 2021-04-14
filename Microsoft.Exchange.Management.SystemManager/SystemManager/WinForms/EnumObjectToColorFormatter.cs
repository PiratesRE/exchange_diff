using System;
using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class EnumObjectToColorFormatter : IToColorFormatter
	{
		public EnumObjectToColorFormatter(Dictionary<Enum, Color> colorMappings)
		{
			this.colorMappings = colorMappings;
		}

		public Color Format(object colorKey)
		{
			if (colorKey != null)
			{
				Enum key = (Enum)colorKey;
				if (this.colorMappings != null && this.colorMappings.ContainsKey(key))
				{
					return this.colorMappings[key];
				}
			}
			return Color.Empty;
		}

		private Dictionary<Enum, Color> colorMappings;
	}
}
