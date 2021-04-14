using System;
using System.Collections;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class ContactLinkingStrings
	{
		public static string GetValueString(object value)
		{
			if (value == null)
			{
				return null;
			}
			string text = value as string;
			if (text != null)
			{
				return ContactLinkingStrings.GetValueString(text);
			}
			byte[] array = value as byte[];
			if (array != null)
			{
				return ContactLinkingStrings.GetValueString(array);
			}
			Guid? nullableGuid = value as Guid?;
			if (nullableGuid != null)
			{
				return ContactLinkingStrings.GetValueString(nullableGuid);
			}
			VersionedId versionedId = value as VersionedId;
			if (versionedId != null)
			{
				return ContactLinkingStrings.GetValueString(versionedId);
			}
			IEnumerable enumerable = value as IEnumerable;
			if (enumerable != null)
			{
				return ContactLinkingStrings.GetValueString(enumerable);
			}
			ExDateTime? nullableExDateTime = value as ExDateTime?;
			if (nullableExDateTime != null)
			{
				return ContactLinkingStrings.GetValueString(nullableExDateTime);
			}
			DateTime? nullableDateTime = value as DateTime?;
			if (nullableDateTime != null)
			{
				return ContactLinkingStrings.GetValueString(nullableDateTime);
			}
			return value.ToString();
		}

		public static string GetValueString(string valueString)
		{
			if (string.IsNullOrEmpty(valueString))
			{
				return null;
			}
			return valueString;
		}

		public static string GetValueString(byte[] byteArray)
		{
			if (byteArray.Length == 0)
			{
				return null;
			}
			return BitConverter.ToString(byteArray).Replace("-", string.Empty);
		}

		public static string GetValueString(Guid? nullableGuid)
		{
			if (nullableGuid != null)
			{
				return nullableGuid.Value.ToString();
			}
			return null;
		}

		public static string GetValueString(ExDateTime? nullableExDateTime)
		{
			if (nullableExDateTime != null)
			{
				return nullableExDateTime.Value.ToString(CultureInfo.InvariantCulture);
			}
			return null;
		}

		public static string GetValueString(DateTime? nullableDateTime)
		{
			if (nullableDateTime != null)
			{
				return nullableDateTime.Value.ToString(CultureInfo.InvariantCulture);
			}
			return null;
		}

		public static string GetValueString(IEnumerable collection)
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("{");
			foreach (object value in collection)
			{
				stringBuilder.Append(ContactLinkingStrings.GetValueString(value));
				stringBuilder.Append(",");
			}
			if (stringBuilder.Length > 1)
			{
				stringBuilder[stringBuilder.Length - 1] = '}';
				return stringBuilder.ToString();
			}
			return string.Empty;
		}

		public static string GetValueString(VersionedId versionedId)
		{
			return versionedId.ObjectId.ToString();
		}
	}
}
