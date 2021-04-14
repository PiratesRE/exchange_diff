using System;
using System.Reflection;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement
{
	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	internal sealed class DisplayNameAttribute : Attribute
	{
		public DisplayNameAttribute(string name)
		{
			this.Name = name;
		}

		public DisplayNameAttribute(string attNamespace, string name)
		{
			this.Name = attNamespace + "." + name;
		}

		public string Name { get; private set; }

		internal static string GetEnumName(Enum value)
		{
			Type type = value.GetType();
			FieldInfo declaredField = value.GetType().GetTypeInfo().GetDeclaredField(value.ToString());
			DisplayNameAttribute[] array = (DisplayNameAttribute[])declaredField.GetCustomAttributes(typeof(DisplayNameAttribute), true);
			string result;
			if (array.Length > 0)
			{
				result = array[0].Name;
			}
			else
			{
				result = type.Name + "." + value.ToString();
			}
			return result;
		}
	}
}
