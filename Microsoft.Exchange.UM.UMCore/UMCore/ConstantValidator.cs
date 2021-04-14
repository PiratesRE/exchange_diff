using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class ConstantValidator
	{
		private ConstantValidator()
		{
			ConstantValidator.cache = new Hashtable();
			this.AddConstantsToCache(typeof(Constants));
		}

		internal static ConstantValidator Instance
		{
			get
			{
				if (ConstantValidator.instance == null)
				{
					ConstantValidator.instance = new ConstantValidator();
				}
				return ConstantValidator.instance;
			}
		}

		internal static void Release()
		{
			ConstantValidator.instance = null;
			ConstantValidator.cache = null;
		}

		internal bool ValidateVariableName(string varname)
		{
			return ConstantValidator.ValidateConstant(typeof(Constants.VariableName), varname);
		}

		internal bool ValidateCondition(string varname)
		{
			return ConstantValidator.ValidateConstant(typeof(Constants.Condition), varname) || ConstantValidator.ValidateConstant(typeof(Constants.VariableName), varname);
		}

		internal bool ValidateAction(string varname)
		{
			return ConstantValidator.ValidateConstant(typeof(Constants.Action), varname);
		}

		internal bool ValidateEvent(string varname)
		{
			return Regex.IsMatch(varname, "[0-9#*A-D]+") || ConstantValidator.ValidateConstant(typeof(Constants.TransitionEvent), varname) || ConstantValidator.ValidateConstant(typeof(Constants.RecoEvent), varname);
		}

		internal bool ValidateRecoEvent(string varname)
		{
			return ConstantValidator.ValidateConstant(typeof(Constants.RecoEvent), varname);
		}

		internal bool ValidatePromptLimit(string varname)
		{
			return ConstantValidator.ValidateConstant(typeof(Constants.PromptLimits), varname);
		}

		private static bool ValidateConstant(Type outerType, string varname)
		{
			ArrayList arrayList = ConstantValidator.cache[varname] as ArrayList;
			string value = outerType.ToString();
			if (arrayList == null)
			{
				return false;
			}
			foreach (object obj in arrayList)
			{
				string text = (string)obj;
				if (text.StartsWith(value, false, CultureInfo.InvariantCulture))
				{
					return true;
				}
			}
			return false;
		}

		private void AddConstantsToCache(Type outerType)
		{
			BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
			FieldInfo[] fields = outerType.GetFields(bindingAttr);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (typeof(string) == fieldInfo.FieldType && fieldInfo.IsLiteral)
				{
					string key = (string)fieldInfo.GetValue(null);
					if (!ConstantValidator.cache.ContainsKey(key))
					{
						ConstantValidator.cache[key] = new ArrayList();
					}
					((ArrayList)ConstantValidator.cache[key]).Add(outerType.ToString());
				}
			}
			Type[] nestedTypes = outerType.GetNestedTypes(bindingAttr);
			foreach (Type outerType2 in nestedTypes)
			{
				this.AddConstantsToCache(outerType2);
			}
		}

		private static ConstantValidator instance;

		private static Hashtable cache;
	}
}
