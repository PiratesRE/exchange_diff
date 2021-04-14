using System;
using System.Drawing;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class DateToColorFormatter : IToColorFormatter
	{
		public Color Format(object colorKey)
		{
			Color result = Color.Empty;
			if (colorKey != null && !DBNull.Value.Equals(colorKey))
			{
				DateTime d = (DateTime)colorKey;
				if ((d - DateTime.UtcNow).TotalDays <= 0.0)
				{
					result = Color.Red;
				}
			}
			return result;
		}
	}
}
