using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[AttributeUsage(AttributeTargets.Field)]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class SeverityLevelAttribute : Attribute
	{
		internal SeverityLevelAttribute() : this(SeverityLevel.Information)
		{
		}

		internal SeverityLevelAttribute(SeverityLevel level)
		{
			this.severityLevel = level;
		}

		internal SeverityLevel SeverityLevel
		{
			get
			{
				return this.severityLevel;
			}
		}

		public static SeverityLevel FromEnum(Type enumType, object value)
		{
			if (enumType == null)
			{
				throw new ArgumentNullException("enumType");
			}
			if (!enumType.GetTypeInfo().IsEnum)
			{
				throw new ArgumentException("enumType must be an enum.", "enumType");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			object obj = Enum.ToObject(enumType, value);
			string[] array = obj.ToString().Split(new char[]
			{
				','
			});
			if (array.Length != 1)
			{
				throw new ArgumentException("value must be an single enum.", "value");
			}
			if (SeverityLevelAttribute.enum2SeverityLevelMap == null)
			{
				Dictionary<object, SeverityLevel> value2 = new Dictionary<object, SeverityLevel>();
				Interlocked.CompareExchange<Dictionary<object, SeverityLevel>>(ref SeverityLevelAttribute.enum2SeverityLevelMap, value2, null);
			}
			SeverityLevel severityLevel = SeverityLevel.Information;
			lock (SeverityLevelAttribute.enum2SeverityLevelMap)
			{
				if (SeverityLevelAttribute.enum2SeverityLevelMap.TryGetValue(obj, out severityLevel))
				{
					return severityLevel;
				}
				string name = array[0].Trim();
				FieldInfo field = enumType.GetField(name);
				if (null != field)
				{
					object[] customAttributes = field.GetCustomAttributes(typeof(SeverityLevelAttribute), false);
					if (customAttributes != null && customAttributes.Length > 0)
					{
						severityLevel = ((SeverityLevelAttribute)customAttributes[0]).SeverityLevel;
					}
				}
				SeverityLevelAttribute.enum2SeverityLevelMap.Add(obj, severityLevel);
			}
			return severityLevel;
		}

		private static Dictionary<object, SeverityLevel> enum2SeverityLevelMap;

		private SeverityLevel severityLevel;
	}
}
