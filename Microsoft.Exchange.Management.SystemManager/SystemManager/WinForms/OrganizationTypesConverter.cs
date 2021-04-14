using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Management.SnapIn;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class OrganizationTypesConverter : TypeConverter
	{
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string text = value.ToString();
			return (from c in text.Split(new char[]
			{
				','
			})
			where !string.IsNullOrEmpty(c)
			select (OrganizationType)Enum.Parse(typeof(OrganizationType), c)).ToArray<OrganizationType>();
		}
	}
}
