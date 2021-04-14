using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.ContentTypes.Tnef;

namespace Microsoft.Exchange.Data.MsgStorage.Internal
{
	internal static class Util
	{
		internal static void ThrowOnNullArgument(object argument, string argumentName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(argumentName);
			}
		}

		internal static void ThrowOnOutOfRange(bool condition, string argumentName)
		{
			if (!condition)
			{
				throw new ArgumentOutOfRangeException(argumentName);
			}
		}

		internal static void ThrowOnFailure(MsgStorageErrorCode errorCode, int hResult, string errorMessage)
		{
			if (hResult != 0)
			{
				throw new MsgStorageException(errorCode, errorMessage, hResult);
			}
		}

		internal static string AttachmentStorageName(int attachmentIndex)
		{
			return string.Format(CultureInfo.InvariantCulture, "__attach_version1.0_#{0:X8}", new object[]
			{
				attachmentIndex
			});
		}

		internal static string RecipientStorageName(int recipientIndex)
		{
			return string.Format(CultureInfo.InvariantCulture, "__recip_version1.0_#{0:X8}", new object[]
			{
				recipientIndex
			});
		}

		internal static string PropertyStreamName(TnefPropertyTag propertyTag)
		{
			return string.Format(CultureInfo.InvariantCulture, "__substg1.0_{0:X8}", new object[]
			{
				(uint)propertyTag
			});
		}

		internal static string PropertyStreamName(TnefPropertyTag propertyTag, int index)
		{
			return string.Format(CultureInfo.InvariantCulture, "__substg1.0_{0:X8}-{1:X8}", new object[]
			{
				(uint)propertyTag,
				index
			});
		}

		internal static int GetUnicodeByteCount(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return 2;
			}
			if (value[value.Length - 1] == '\0')
			{
				return Util.UnicodeEncoding.GetByteCount(value);
			}
			return Util.UnicodeEncoding.GetByteCount(value) + 2;
		}

		internal static int StringToUnicodeBytes(string value, byte[] buffer)
		{
			if (string.IsNullOrEmpty(value))
			{
				buffer[1] = (buffer[0] = 0);
				return 2;
			}
			if (value[value.Length - 1] == '\0')
			{
				return Util.UnicodeEncoding.GetBytes(value, 0, value.Length, buffer, 0);
			}
			int bytes = Util.UnicodeEncoding.GetBytes(value, 0, value.Length, buffer, 0);
			buffer[bytes++] = 0;
			buffer[bytes++] = 0;
			return bytes;
		}

		internal static string UnicodeBytesToString(byte[] bytes, int length)
		{
			if (length >= 2 && bytes[length - 1] == 0 && bytes[length - 2] == 0)
			{
				length -= 2;
			}
			return Util.UnicodeEncoding.GetString(bytes, 0, length);
		}

		internal static string AnsiBytesToString(byte[] bytes, int length, Encoding encoding)
		{
			if (length >= 1 && bytes[length - 1] == 0)
			{
				length--;
			}
			return encoding.GetString(bytes, 0, length);
		}

		internal static void InvokeComCall(MsgStorageErrorCode errorCode, Util.ComCall comCall)
		{
			try
			{
				comCall();
			}
			catch (IOException exc)
			{
				throw new MsgStorageException(errorCode, MsgStorageStrings.ComExceptionThrown, exc);
			}
			catch (COMException ex)
			{
				if (ex.ErrorCode == -2147287038)
				{
					throw new MsgStorageNotFoundException(errorCode, MsgStorageStrings.ComExceptionThrown, ex);
				}
				throw new MsgStorageException(errorCode, MsgStorageStrings.ComExceptionThrown, ex);
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static Util()
		{
			byte[] emptyStringBytes = new byte[2];
			Util.EmptyStringBytes = emptyStringBytes;
			Util.EmptyByteArray = new byte[0];
			Util.ClassIdFileAttachment = new Guid("00020D05-0000-0000-C000-000000000046");
			Util.ClassIdMessageAttachment = new Guid("00020D09-0000-0000-C000-000000000046");
			Util.ClassIdMessage = new Guid("00020D0B-0000-0000-C000-000000000046");
			Util.UnicodeEncoding = new UnicodeEncoding(false, false);
		}

		internal const string PropertiesStreamName = "__properties_version1.0";

		internal const string NamedStorageName = "__nameid_version1.0";

		internal const string NamedEntriesStreamName = "__substg1.0_00030102";

		internal const string NamedGuidStreamName = "__substg1.0_00020102";

		internal const string NamedStringsStreamName = "__substg1.0_00040102";

		private const string RecipientStorageNameFormat = "__recip_version1.0_#{0:X8}";

		private const string AttachmentStorageNameFormat = "__attach_version1.0_#{0:X8}";

		private const string PropertyStreamNameFormat = "__substg1.0_{0:X8}";

		private const string PropertyIndexStreamNameFormat = "__substg1.0_{0:X8}-{1:X8}";

		private const int ErrorCodeObjectNotFound = -2147287038;

		internal const int MaxShortValueLength = 32768;

		internal const int MaxMultivaluedArraySize = 2048;

		internal const int AttachMethodMessage = 5;

		internal const int AttachMethodOle = 6;

		internal static readonly byte[] EmptyStringBytes;

		internal static readonly byte[] EmptyByteArray;

		internal static readonly Guid ClassIdFileAttachment;

		internal static readonly Guid ClassIdMessageAttachment;

		internal static readonly Guid ClassIdMessage;

		internal static readonly Encoding UnicodeEncoding;

		internal delegate void ComCall();
	}
}
