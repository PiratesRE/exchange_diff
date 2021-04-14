using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.FfoReporting.Common
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	internal sealed class ODataInput : Attribute
	{
		internal ODataInput(string propertyName)
		{
			this.propertyName = propertyName;
		}

		internal void SetCmdletProperty(object ffoTask, object value)
		{
			PropertyInfo property = ffoTask.GetType().GetProperty(this.propertyName);
			if (property == null)
			{
				throw new NullReferenceException("Property does not exist.");
			}
			if (typeof(IList).IsAssignableFrom(property.PropertyType))
			{
				IList list = ODataInput.ConvertToList(value);
				if (list == null || list.Count <= 0)
				{
					return;
				}
				IList list2 = (IList)property.GetValue(ffoTask, null);
				list2.Clear();
				using (IEnumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object value2 = enumerator.Current;
						list2.Add(value2);
					}
					return;
				}
			}
			object value3 = ValueConvertor.ConvertValue(value, property.PropertyType, null);
			property.SetValue(ffoTask, value3);
		}

		private static IList ConvertToList(object value)
		{
			string text = value as string;
			IList list;
			if (!string.IsNullOrEmpty(text))
			{
				list = text.Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
			}
			else
			{
				list = (value as IList);
				if (list == null)
				{
					list = new List<object>
					{
						value
					};
				}
			}
			return list;
		}

		private readonly string propertyName;
	}
}
