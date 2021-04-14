using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class DDIObjectInstanceConverter : TypeConverter
	{
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			Type type = (Type)new DDIObjectTypeConverter().ConvertFrom(value);
			return type.GetConstructor(new Type[0]).Invoke(new object[0]);
		}
	}
}
