using System;
using System.Collections;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class TextConverter : ICustomTextConverter, ICustomFormatter
	{
		protected virtual string NullValueText
		{
			get
			{
				return string.Empty;
			}
		}

		string ICustomFormatter.Format(string format, object arg, IFormatProvider formatProvider)
		{
			string result = this.NullValueText;
			arg = this.PreFormatObject(arg, format, formatProvider);
			if (arg is string)
			{
				result = (string)arg;
			}
			else if (arg != null && !DBNull.Value.Equals(arg))
			{
				result = this.FormatObject(format, arg, formatProvider);
			}
			return result;
		}

		object ICustomTextConverter.Parse(Type resultType, string s, IFormatProvider provider)
		{
			if (resultType == null)
			{
				throw new ArgumentNullException("resultType");
			}
			s = (s ?? string.Empty);
			object obj = this.PreParseObject(resultType, s, provider);
			if (obj != null && obj.GetType() != resultType)
			{
				if (obj.GetType() == typeof(string))
				{
					obj = this.ParseObject((string)obj, provider);
				}
				if (obj != null && obj.GetType() != resultType)
				{
					try
					{
						obj = ValueConvertor.ConvertValue(obj, resultType, provider);
					}
					catch (NotImplementedException)
					{
					}
				}
			}
			return obj;
		}

		protected virtual string FormatObject(string format, object arg, IFormatProvider formatProvider)
		{
			string result = this.NullValueText;
			Type type = arg.GetType();
			if (type == typeof(DateTime) && format == null)
			{
				format = "F";
			}
			if (type.IsEnum)
			{
				result = LocalizedDescriptionAttribute.FromEnum(arg.GetType(), arg);
			}
			else if (typeof(IFormattable).IsAssignableFrom(type))
			{
				result = ((IFormattable)arg).ToString(format, formatProvider);
			}
			else if (type == typeof(bool))
			{
				result = (((bool)arg) ? Strings.TrueString : Strings.FalseString);
			}
			else if (type == typeof(double) || type == typeof(float))
			{
				result = ((double)arg).ToString(TextConverter.DoubleFormatString);
			}
			else
			{
				result = arg.ToString();
			}
			return result;
		}

		protected virtual object ParseObject(string s, IFormatProvider provider)
		{
			return s;
		}

		private object PreFormatObject(object obj, string format, IFormatProvider formatProvider)
		{
			if (obj == null)
			{
				return null;
			}
			Type type = obj.GetType();
			if (typeof(IList).IsAssignableFrom(type))
			{
				obj = this.ConvertListToString(obj as IList, format, formatProvider);
			}
			else if (type.IsGenericType)
			{
				if (type.GetGenericTypeDefinition() == typeof(Unlimited<>))
				{
					if (!(bool)TextConverter.GetPropertyValue(obj.GetType(), obj, "IsUnlimited"))
					{
						obj = TextConverter.GetPropertyValue(obj.GetType(), obj, "Value");
					}
					else
					{
						obj = Strings.UnlimitedString.ToString();
					}
				}
				else if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					if (!(bool)TextConverter.GetPropertyValue(obj.GetType(), obj, "HasValue"))
					{
						obj = TextConverter.GetPropertyValue(obj.GetType(), obj, "Value");
					}
					else
					{
						obj = this.NullValueText;
					}
				}
			}
			return obj;
		}

		private object PreParseObject(Type resultType, string s, IFormatProvider formatProvider)
		{
			object result = s;
			if (typeof(IList).IsAssignableFrom(resultType))
			{
				throw new NotSupportedException();
			}
			if (resultType.IsGenericType)
			{
				Type type = resultType.GetGenericArguments()[0];
				if (resultType.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Unlimited<>))
					{
						resultType = type;
						type = resultType.GetGenericArguments()[0];
					}
					else if (string.IsNullOrEmpty(s))
					{
						result = null;
					}
					else
					{
						result = ValueConvertor.ConvertValue(this.Parse(type, s, formatProvider), resultType, formatProvider);
					}
				}
				if (resultType.GetGenericTypeDefinition() == typeof(Unlimited<>))
				{
					if (string.Compare(Strings.UnlimitedString.ToString(), s) == 0)
					{
						result = TextConverter.GetPropertyValue(resultType, null, "UnlimitedValue");
					}
					else
					{
						result = ValueConvertor.ConvertValue(this.Parse(type, s, formatProvider), resultType, formatProvider);
					}
				}
			}
			else if (resultType == typeof(bool) && s != null)
			{
				bool flag;
				if (bool.TryParse(s, out flag))
				{
					result = flag;
				}
				else if (string.Compare(s, Strings.TrueString) == 0)
				{
					result = true;
				}
				else if (string.Compare(s, Strings.FalseString) == 0)
				{
					result = false;
				}
			}
			return result;
		}

		private string ConvertListToString(IList list, string format, IFormatProvider formatProvider)
		{
			string result = string.Empty;
			if (list.Count > 0)
			{
				int num = Math.Min(list.Count, 10);
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < num; i++)
				{
					if (i != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(((ICustomFormatter)this).Format(format, list[i], formatProvider));
				}
				if (num < list.Count)
				{
					stringBuilder.Append(" [...]");
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		private static object GetPropertyValue(Type t, object obj, string propertyName)
		{
			return t.GetProperty(propertyName).GetValue(obj, null);
		}

		private const int maxEntriesInToString = 10;

		public static string DoubleFormatString = "0.######";

		public static readonly ICustomTextConverter DefaultConverter = new TextConverter();
	}
}
