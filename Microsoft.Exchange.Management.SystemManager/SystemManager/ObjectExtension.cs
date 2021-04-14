using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager
{
	public static class ObjectExtension
	{
		private static bool IsQuotationRequired(object value)
		{
			return value is string;
		}

		public static string ToUserFriendText(this object item, string separator, ObjectExtension.IsQuotationRequiredDelegate IsQuotationRequired, int lengthConstraint)
		{
			string text = string.Empty;
			if (item != null)
			{
				if (string.IsNullOrEmpty(separator))
				{
					throw new ArgumentNullException("separator");
				}
				if (IsQuotationRequired == null)
				{
					throw new ArgumentNullException("IsQuotationRequired");
				}
				Type type = typeof(ICollection);
				if (item.GetType().IsGenericType)
				{
					type = typeof(ICollection<>).MakeGenericType(item.GetType().GetGenericArguments());
				}
				if (type.IsAssignableFrom(item.GetType()))
				{
					text = ObjectExtension.CollectionToString(item as IEnumerable, separator, lengthConstraint, IsQuotationRequired);
				}
				else
				{
					text = ObjectExtension.AtomToString(item);
				}
				if (IsQuotationRequired(item) && !string.IsNullOrEmpty(text))
				{
					text = text.Replace("'", "''");
					text = Strings.QuotedString(text);
				}
			}
			return text;
		}

		private static string AtomToString(object item)
		{
			string text = string.Empty;
			if (item != null)
			{
				if (item is Enum)
				{
					text = LocalizedDescriptionAttribute.FromEnum(item.GetType(), item);
				}
				else if (item is ADObjectId)
				{
					ADObjectId adobjectId = (ADObjectId)item;
					text = ((!string.IsNullOrEmpty(adobjectId.Name)) ? adobjectId.Name : adobjectId.ToString());
				}
				else if (item is CultureInfo)
				{
					text = ((CultureInfo)item).DisplayName;
				}
				else if (item is DateTime)
				{
					text = ((DateTime)item).ToLongDateString();
				}
				else
				{
					text = item.ToString();
				}
				if (string.IsNullOrEmpty(text))
				{
					text = string.Empty;
				}
			}
			return text;
		}

		private static string CollectionToString(IEnumerable collection, string separator, int lengthConstraint, ObjectExtension.IsQuotationRequiredDelegate IsQuotationRequired)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (collection != null)
			{
				bool flag = true;
				foreach (object obj in collection)
				{
					stringBuilder.Append(flag ? string.Empty : separator);
					flag = false;
					if (obj != null)
					{
						stringBuilder.Append(obj.ToUserFriendText(separator, IsQuotationRequired, lengthConstraint - stringBuilder.Length));
					}
					if (lengthConstraint > 0 && stringBuilder.Length >= lengthConstraint)
					{
						stringBuilder.Remove(lengthConstraint, stringBuilder.Length - lengthConstraint);
						stringBuilder.Append(" ...");
						break;
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static string ToUserFriendText(this object item, string separator, ObjectExtension.IsQuotationRequiredDelegate IsQuotationRequired)
		{
			return item.ToUserFriendText(separator, IsQuotationRequired, 5120);
		}

		public static string ToUserFriendText(this object item, string separator)
		{
			return item.ToUserFriendText(separator, new ObjectExtension.IsQuotationRequiredDelegate(ObjectExtension.IsQuotationRequired));
		}

		public static string ToUserFriendText(this object item)
		{
			return item.ToUserFriendText(CultureInfo.CurrentUICulture.TextInfo.ListSeparator);
		}

		public static string ToQuotationEscapedString(this object item)
		{
			string result = string.Empty;
			if (item != null)
			{
				result = item.ToString().Replace("'", "''");
			}
			return result;
		}

		public static string ToSustainedString(this object item)
		{
			string result = string.Empty;
			if (item is ADObjectId && !Guid.Empty.Equals(((ADObjectId)item).ObjectGuid))
			{
				result = ((ADObjectId)item).ObjectGuid.ToString();
			}
			else
			{
				result = item.ToQuotationEscapedString();
			}
			return result;
		}

		public static bool IsNullValue(this object item)
		{
			return item == null || DBNull.Value.Equals(item);
		}

		public static bool IsTrue(this object item)
		{
			if (item.IsNullValue())
			{
				return false;
			}
			if (item is bool)
			{
				return (bool)item;
			}
			throw new ArgumentException("item is not bool or Nullable<bool>");
		}

		public static bool IsFalse(object item)
		{
			if (item.IsNullValue())
			{
				return false;
			}
			if (item is bool)
			{
				return !(bool)item;
			}
			throw new ArgumentException("value is not bool or Nullable<bool>");
		}

		public delegate bool IsQuotationRequiredDelegate(object value);
	}
}
