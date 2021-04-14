using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class DDIUtil
	{
		public static string NullableDateTimeToUserString(object value)
		{
			if (DDIHelper.IsEmptyValue(value))
			{
				return null;
			}
			DateTime? dateTime = (DateTime?)value;
			if (dateTime == null)
			{
				return null;
			}
			return dateTime.Value.LocalToUserDateTimeGeneralFormatString();
		}

		public static string EnumToLocalizedString(object value)
		{
			if (value.GetType().Equals(typeof(DBNull)))
			{
				return string.Empty;
			}
			return LocalizedDescriptionAttribute.FromEnum(value.GetType(), value);
		}

		public static string EnumToLocalizedStringForOwaOption(object value)
		{
			return LocalizedDescriptionAttribute.FromEnumForOwaOption(value.GetType(), value);
		}

		public static int Length(object value)
		{
			if (!DDIHelper.IsEmptyValue(value))
			{
				Array array = value as Array;
				if (array != null)
				{
					return array.Length;
				}
			}
			return 0;
		}

		public static string[] ToStringArray(this MultiValuedProperty<string> stringProperty)
		{
			if (MultiValuedPropertyBase.IsNullOrEmpty(stringProperty))
			{
				return new string[0];
			}
			return stringProperty.ToArray();
		}

		public static int GetLength(this object stringProperty)
		{
			MultiValuedProperty<string> multiValuedProperty = stringProperty as MultiValuedProperty<string>;
			if (multiValuedProperty != null)
			{
				string[] array = multiValuedProperty.ToStringArray();
				if (array != null)
				{
					return array.Length;
				}
			}
			return 0;
		}

		public static string Join(object separator, object value)
		{
			if (DDIHelper.IsEmptyValue(value))
			{
				return string.Empty;
			}
			List<object> list = value as List<object>;
			if (DDIHelper.IsEmptyValue(list))
			{
				return string.Empty;
			}
			if (separator == null || separator.ToString() == null)
			{
				separator = string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object obj in list)
			{
				if (!DDIHelper.IsEmptyValue(obj) && !DDIHelper.IsEmptyValue(obj.ToString()))
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(separator);
					}
					stringBuilder.Append(obj.ToString());
				}
			}
			return stringBuilder.ToString();
		}

		public static List<object> ADObjectIdsToNames(object adObjectIds)
		{
			IEnumerable<ADObjectId> enumerable = adObjectIds as IEnumerable<ADObjectId>;
			if (enumerable != null)
			{
				return (from x in enumerable
				select x.Name).ToList<object>();
			}
			return null;
		}

		internal static string ConvertUnlimitedToString<T>(object value, Converter<T, string> converter) where T : struct, IComparable
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (converter == null)
			{
				throw new ArgumentNullException("converter");
			}
			if (DBNull.Value.Equals(value))
			{
				return string.Empty;
			}
			Unlimited<T> unlimited = (Unlimited<T>)value;
			if (unlimited.IsUnlimited)
			{
				return "unlimited";
			}
			return converter(unlimited.Value);
		}

		internal static string ConvertStringToUnlimited(string value, Converter<string, string> converter)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException("value");
			}
			if (converter == null)
			{
				throw new ArgumentNullException("converter");
			}
			if (!value.Equals("unlimited", StringComparison.OrdinalIgnoreCase))
			{
				return converter(value);
			}
			return value;
		}

		public static string ZeroToUnlimited(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException("value");
			}
			if (value == "0")
			{
				return "unlimited";
			}
			return value;
		}

		public static string ZeroToUnlimited(object value)
		{
			if (value == null)
			{
				throw new ArgumentException("value");
			}
			return DDIUtil.ZeroToUnlimited(value.ToString());
		}

		public static string ConcatenateStringAndSubstring(object firstString, object secondString)
		{
			if (firstString == null)
			{
				return string.Empty;
			}
			if (secondString == null || secondString.ToString().Equals(string.Empty) || secondString.ToString().Equals("Success"))
			{
				return firstString.ToString();
			}
			return string.Format("{0} ({1})", firstString.ToString(), secondString.ToString());
		}

		public static string ByteQuantifiedSizeToMBString(object value)
		{
			ulong num = 0UL;
			try
			{
				Unlimited<ByteQuantifiedSize> unlimited = (Unlimited<ByteQuantifiedSize>)value;
				if (unlimited.IsUnlimited)
				{
					return "unlimited";
				}
				num = unlimited.Value.ToBytes();
			}
			catch (InvalidCastException)
			{
				try
				{
					num = ((ByteQuantifiedSize)value).ToBytes();
				}
				catch (InvalidCastException)
				{
					return null;
				}
			}
			return Math.Round(num / 1048576.0).ToString();
		}

		public static ByteQuantifiedSize MBStringToByteQuantifiedSize(object value)
		{
			string text = value as string;
			if (text != null)
			{
				return ByteQuantifiedSize.Parse((ulong.Parse(text) * 1024UL * 1024UL).ToString());
			}
			throw new ArgumentException("The argument should be string.");
		}

		public static Unlimited<ByteQuantifiedSize> MBStringToUnlimitedByteQuantifiedSize(object value)
		{
			string text = value as string;
			if (text == null)
			{
				throw new ArgumentException("The argument should be string.");
			}
			if (string.Equals(text.Trim(), Unlimited<ByteQuantifiedSize>.UnlimitedString, StringComparison.OrdinalIgnoreCase))
			{
				return Unlimited<ByteQuantifiedSize>.UnlimitedValue;
			}
			return Unlimited<ByteQuantifiedSize>.Parse((ulong.Parse(text) * 1024UL * 1024UL).ToString());
		}

		public static string RetractUsernameFromPSCredential(object value)
		{
			PSCredential pscredential = value as PSCredential;
			if (pscredential != null)
			{
				string empty = string.Empty;
				string empty2 = string.Empty;
				PSCredentialHelper.GetUserPassFromCredential(pscredential, out empty, out empty2, false);
				return empty;
			}
			return string.Empty;
		}

		public static string RetractPlainPasswordFromPSCredential(object value)
		{
			PSCredential pscredential = value as PSCredential;
			if (pscredential != null)
			{
				string empty = string.Empty;
				string empty2 = string.Empty;
				PSCredentialHelper.GetUserPassFromCredential(pscredential, out empty, out empty2, false);
				return empty2;
			}
			return string.Empty;
		}

		public static bool IsInRole(string role)
		{
			return RbacCheckerWrapper.RbacChecker.IsInRole(role);
		}

		public static string UrlDecode(string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				return HttpUtility.UrlDecode(value);
			}
			return value;
		}

		internal static PowerShellResults InsertWarning(PowerShellResults result, string warning)
		{
			string[] warnings = new string[]
			{
				warning
			};
			if (result.Warnings == null)
			{
				result.Warnings = warnings;
			}
			else
			{
				result.Warnings = result.Warnings.Concat(new string[]
				{
					warning
				}).ToArray<string>();
			}
			return result;
		}

		internal static PowerShellResults InsertError(PowerShellResults result, string error)
		{
			Microsoft.Exchange.Management.ControlPanel.ErrorRecord[] array = new Microsoft.Exchange.Management.ControlPanel.ErrorRecord[]
			{
				new Microsoft.Exchange.Management.ControlPanel.ErrorRecord(new Exception(error))
			};
			if (result.ErrorRecords == null)
			{
				result.ErrorRecords = array;
			}
			else
			{
				result.ErrorRecords = result.ErrorRecords.Concat(array).ToArray<Microsoft.Exchange.Management.ControlPanel.ErrorRecord>();
			}
			return result;
		}

		internal static PowerShellResults[] InsertWarningIfSucceded(PowerShellResults[] results, string warning)
		{
			if (results != null && results.Length == 1 && results[0].Succeeded)
			{
				DDIUtil.InsertWarning(results[0], warning);
			}
			return results;
		}
	}
}
