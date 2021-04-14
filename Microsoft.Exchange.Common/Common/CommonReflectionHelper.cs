using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Microsoft.Exchange.Common
{
	public class CommonReflectionHelper
	{
		internal static XElement[] GetXmlProperties(object obj)
		{
			Type type = obj.GetType();
			return (from property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
			select new XElement(property.Name, property.GetValue(obj, null))).ToArray<XElement>();
		}
	}
}
