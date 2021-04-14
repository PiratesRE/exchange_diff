using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Dal
{
	public class DalProbeBinder : Binder
	{
		public override MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] names, out object state)
		{
			int argCount = (args != null) ? args.Length : 0;
			MethodBase[] array = (from method in match
			where method.GetParameters().Length == argCount
			select method).ToArray<MethodBase>();
			if (array.Length == 1)
			{
				state = null;
				return array[0];
			}
			return Type.DefaultBinder.BindToMethod(bindingAttr, match, ref args, modifiers, culture, names, out state);
		}

		public override FieldInfo BindToField(BindingFlags bindingAttr, FieldInfo[] match, object value, CultureInfo culture)
		{
			return null;
		}

		public override MethodBase SelectMethod(BindingFlags bindingAttr, MethodBase[] match, Type[] types, ParameterModifier[] modifiers)
		{
			return Type.DefaultBinder.SelectMethod(bindingAttr, match, types, modifiers);
		}

		public override PropertyInfo SelectProperty(BindingFlags bindingAttr, PropertyInfo[] match, Type returnType, Type[] indexes, ParameterModifier[] modifiers)
		{
			return null;
		}

		public override object ChangeType(object value, Type myChangeType, CultureInfo culture)
		{
			return Convert.ChangeType(value, myChangeType);
		}

		public override void ReorderArgumentArray(ref object[] args, object state)
		{
		}
	}
}
