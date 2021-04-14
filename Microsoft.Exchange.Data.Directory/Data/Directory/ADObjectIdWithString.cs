using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	public sealed class ADObjectIdWithString
	{
		public ADObjectIdWithString(string stringValue, ADObjectId objectIdValue)
		{
			if (stringValue == null)
			{
				throw new ArgumentNullException("stringValue");
			}
			if (objectIdValue == null)
			{
				throw new ArgumentNullException("objectIdValue");
			}
			this.stringValue = stringValue;
			this.objectIdValue = objectIdValue;
		}

		internal ADObjectIdWithString(byte[] bytes)
		{
			int count = BitConverter.ToInt32(bytes, 0);
			this.stringValue = Encoding.Unicode.GetString(bytes, 4, count);
			this.objectIdValue = new ADObjectId(bytes, Encoding.Unicode, 4 + Encoding.Unicode.GetByteCount(this.stringValue));
		}

		public string StringValue
		{
			get
			{
				return this.stringValue;
			}
		}

		public ADObjectId ObjectIdValue
		{
			get
			{
				return this.objectIdValue;
			}
		}

		public string ToDNStringSyntax(bool extendedDN)
		{
			string arg;
			if (extendedDN)
			{
				arg = this.objectIdValue.ToExtendedDN();
			}
			else
			{
				arg = this.objectIdValue.ToGuidOrDNString();
			}
			return string.Format("S:{0}:{1}:{2}", this.stringValue.Length, this.stringValue, arg);
		}

		public override bool Equals(object obj)
		{
			ADObjectIdWithString adobjectIdWithString = obj as ADObjectIdWithString;
			return adobjectIdWithString != null && this.stringValue.Equals(adobjectIdWithString.StringValue) && this.objectIdValue.Equals(adobjectIdWithString.ObjectIdValue);
		}

		public bool Equals(ADObjectIdWithString other)
		{
			return this.stringValue.Equals(other.StringValue) && this.objectIdValue.Equals(other.ObjectIdValue);
		}

		public override int GetHashCode()
		{
			return this.stringValue.GetHashCode() ^ this.objectIdValue.GetHashCode();
		}

		public override string ToString()
		{
			return this.ToDNStringSyntax(false);
		}

		internal static ADObjectIdWithString ParseDNStringSyntax(string value, OrganizationId orgId)
		{
			return ADObjectIdWithString.ParseDNStringSyntax(value, Guid.Empty, orgId);
		}

		internal static ADObjectIdWithString ParseDNStringSyntax(string value, Guid partitionGuid, OrganizationId orgId)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			string arg;
			string extendedDN;
			ADObjectIdWithString.ParseStringValue(value, out arg, out extendedDN);
			ADObjectId adobjectId = ADObjectId.ParseExtendedDN(extendedDN, partitionGuid, orgId);
			ExTraceGlobals.ADObjectTracer.TraceDebug<string>(0L, "ADObjectIdWithString.ParseDNStringSyntax - Initialized with string part {0}", arg);
			return new ADObjectIdWithString(arg, adobjectId);
		}

		internal byte[] GetBytes()
		{
			int byteCount = this.GetByteCount();
			byte[] array = new byte[byteCount];
			int num = 0;
			int bytes = Encoding.Unicode.GetBytes(this.stringValue, 0, this.stringValue.Length, array, 4);
			num += ExBitConverter.Write(bytes, array, num);
			num += bytes;
			this.objectIdValue.GetBytes(Encoding.Unicode, array, num);
			return array;
		}

		internal int GetByteCount()
		{
			int byteCount = Encoding.Unicode.GetByteCount(this.stringValue);
			int byteCount2 = this.objectIdValue.GetByteCount(Encoding.Unicode);
			return 4 + byteCount + byteCount2;
		}

		private static void ParseStringValue(string dnString, out string stringPart, out string dnPart)
		{
			if (!ADObjectIdWithString.TryParseStringValue(dnString, out stringPart, out dnPart))
			{
				throw new FormatException(DirectoryStrings.InvalidDNStringFormat(dnString));
			}
		}

		private static bool TryParseStringValue(string value, out string stringPart, out string dnPart)
		{
			stringPart = null;
			dnPart = null;
			if (!value.StartsWith("S:", StringComparison.Ordinal))
			{
				return false;
			}
			char c = ':';
			string[] array = value.Split(new char[]
			{
				c
			});
			if (array.Length < 4)
			{
				return false;
			}
			string text = array[1];
			int length;
			if (!int.TryParse(text, NumberStyles.None, null, out length))
			{
				return false;
			}
			int startIndex = 3 + text.Length;
			stringPart = value.Substring(startIndex, length);
			dnPart = array[array.Length - 1];
			return true;
		}

		private readonly string stringValue;

		private readonly ADObjectId objectIdValue;
	}
}
