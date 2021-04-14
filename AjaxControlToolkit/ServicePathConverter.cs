using System;
using System.ComponentModel;
using System.Globalization;
using System.Web;

namespace AjaxControlToolkit
{
	public class ServicePathConverter : StringConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				string value2 = (string)value;
				if (string.IsNullOrEmpty(value2))
				{
					HttpContext httpContext = HttpContext.Current;
					if (httpContext != null)
					{
						return httpContext.Request.FilePath;
					}
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
