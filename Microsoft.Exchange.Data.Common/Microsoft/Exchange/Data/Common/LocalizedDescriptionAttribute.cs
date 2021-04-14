using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace Microsoft.Exchange.Data.Common
{
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
	[Serializable]
	public class LocalizedDescriptionAttribute : DescriptionAttribute, ILocalizedString
	{
		public LocalizedDescriptionAttribute()
		{
		}

		public LocalizedDescriptionAttribute(LocalizedString description)
		{
			this.description = description;
		}

		public LocalizedString LocalizedString
		{
			get
			{
				return this.description;
			}
		}

		public static string FromEnum(Type enumType, object value)
		{
			return LocalizedDescriptionAttribute.FromEnum(enumType, value, null);
		}

		public static string FromEnum(Type enumType, object value, CultureInfo culture)
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
			if (LocalizedDescriptionAttribute.locEnumStringTable == null)
			{
				Dictionary<string, Dictionary<object, string>> value2 = new Dictionary<string, Dictionary<object, string>>();
				Interlocked.CompareExchange<Dictionary<string, Dictionary<object, string>>>(ref LocalizedDescriptionAttribute.locEnumStringTable, value2, null);
			}
			string text;
			lock (LocalizedDescriptionAttribute.locEnumStringTable)
			{
				if (culture == null)
				{
					culture = CultureInfo.CurrentCulture;
				}
				Dictionary<object, string> dictionary;
				if (!LocalizedDescriptionAttribute.locEnumStringTable.TryGetValue(culture.Name, out dictionary))
				{
					dictionary = new Dictionary<object, string>();
					LocalizedDescriptionAttribute.locEnumStringTable.Add(culture.Name, dictionary);
				}
				if (dictionary.TryGetValue(obj, out text))
				{
					return text;
				}
				string[] array = obj.ToString().Split(new char[]
				{
					','
				});
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < array.Length; i++)
				{
					string fieldName = array[i].Trim();
					string value3 = fieldName;
					foreach (FieldInfo fieldInfo in from x in enumType.GetTypeInfo().DeclaredFields
					where x.Name == fieldName
					select x)
					{
						using (IEnumerator<object> enumerator2 = fieldInfo.GetCustomAttributes(false).Where((object x) => x is DescriptionAttribute).GetEnumerator())
						{
							if (enumerator2.MoveNext())
							{
								object obj3 = enumerator2.Current;
								if (obj3 is LocalizedDescriptionAttribute)
								{
									value3 = ((LocalizedDescriptionAttribute)obj3).LocalizedString.ToString(culture);
								}
								else
								{
									value3 = ((DescriptionAttribute)obj3).Description;
								}
							}
						}
					}
					if (i != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(value3);
				}
				text = stringBuilder.ToString();
				dictionary.Add(obj, text);
			}
			return text;
		}

		public sealed override string Description
		{
			[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
			get
			{
				return this.description;
			}
		}

		private static Dictionary<string, Dictionary<object, string>> locEnumStringTable;

		private LocalizedString description;
	}
}
