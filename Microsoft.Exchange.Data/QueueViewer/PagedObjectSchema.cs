using System;
using System.Net;
using System.Net.Sockets;

namespace Microsoft.Exchange.Data.QueueViewer
{
	internal abstract class PagedObjectSchema : ObjectSchema
	{
		internal abstract int GetFieldIndex(string fieldName);

		internal abstract bool TryGetFieldIndex(string fieldName, out int index);

		internal abstract string GetFieldName(int field);

		internal abstract ProviderPropertyDefinition GetFieldByName(string fieldName);

		internal abstract bool IsBasicField(int field);

		internal abstract Type GetFieldType(int field);

		internal abstract bool MatchField(int field, PagedDataObject pagedDataObject, object matchPattern, MatchOptions matchOptions);

		internal abstract int CompareField(int field, PagedDataObject pagedDataObject, object value);

		internal abstract int CompareField(int field, PagedDataObject object1, PagedDataObject object2);

		protected static int CompareString(string v1, string v2)
		{
			return string.Compare(v1, v2, StringComparison.OrdinalIgnoreCase);
		}

		internal static bool MatchString(string sourceText, string matchText, MatchOptions matchOptions)
		{
			if (sourceText == null)
			{
				sourceText = string.Empty;
			}
			switch (matchOptions)
			{
			case MatchOptions.FullString:
				return sourceText.Equals(matchText, StringComparison.OrdinalIgnoreCase);
			case MatchOptions.SubString:
				return sourceText.IndexOf(matchText, StringComparison.OrdinalIgnoreCase) != -1;
			case MatchOptions.Prefix:
				return sourceText.StartsWith(matchText, StringComparison.OrdinalIgnoreCase);
			case MatchOptions.Suffix:
				return sourceText.EndsWith(matchText, StringComparison.OrdinalIgnoreCase);
			default:
				throw new InvalidOperationException();
			}
		}

		protected static int CompareDateTimeNullable(DateTime? v1, DateTime? v2)
		{
			if (v1 == v2)
			{
				return 0;
			}
			if (v1 != null && v2 == null)
			{
				return 1;
			}
			if (v1 == null && v2 != null)
			{
				return -1;
			}
			return v1.Value.CompareTo(v2.Value);
		}

		protected static int CompareIPAddress(IPAddress v1, IPAddress v2)
		{
			if (v1 == null)
			{
				throw new ArgumentNullException("Null IPAddress", "v1");
			}
			if (v2 == null)
			{
				throw new ArgumentNullException("Null IPAddress", "v2");
			}
			if (v1.AddressFamily == AddressFamily.InterNetwork && v2.AddressFamily == AddressFamily.InterNetworkV6)
			{
				return -1;
			}
			if (v1.AddressFamily == AddressFamily.InterNetworkV6 && v2.AddressFamily == AddressFamily.InterNetwork)
			{
				return 1;
			}
			byte[] addressBytes = v1.GetAddressBytes();
			byte[] addressBytes2 = v2.GetAddressBytes();
			for (int i = 0; i < addressBytes.Length; i++)
			{
				if (addressBytes[i] < addressBytes2[i])
				{
					return -1;
				}
				if (addressBytes[i] > addressBytes2[i])
				{
					return 1;
				}
			}
			return 0;
		}
	}
}
