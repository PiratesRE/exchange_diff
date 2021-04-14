using System;
using System.Globalization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class ByteQuantifiedSizeExtensions
	{
		public static string ToAppropriateUnitFormatString(this ByteQuantifiedSize size)
		{
			return size.ToAppropriateUnitFormatString("{0:0}");
		}

		public static string ToAppropriateUnitFormatString(this Unlimited<ByteQuantifiedSize> size)
		{
			if (size.IsUnlimited)
			{
				return OwaOptionStrings.Unlimited;
			}
			return size.Value.ToAppropriateUnitFormatString("{0:0.##}");
		}

		public static string ToAppropriateUnitFormatString(this ByteQuantifiedSize size, string valueFormat)
		{
			if (size.ToTB() > 0UL)
			{
				return string.Format(OwaOptionStrings.MailboxUsageUnitTB, string.Format(CultureInfo.InvariantCulture, valueFormat, new object[]
				{
					size.ToBytes() / 1099511627776.0
				}));
			}
			if (size.ToGB() > 0UL)
			{
				return string.Format(OwaOptionStrings.MailboxUsageUnitGB, string.Format(CultureInfo.InvariantCulture, valueFormat, new object[]
				{
					size.ToBytes() / 1073741824.0
				}));
			}
			if (size.ToMB() > 0UL)
			{
				return string.Format(OwaOptionStrings.MailboxUsageUnitMB, string.Format(CultureInfo.InvariantCulture, valueFormat, new object[]
				{
					size.ToBytes() / 1048576.0
				}));
			}
			if (size.ToKB() > 0UL)
			{
				return string.Format(OwaOptionStrings.MailboxUsageUnitKB, string.Format(CultureInfo.InvariantCulture, valueFormat, new object[]
				{
					size.ToBytes() / 1024.0
				}));
			}
			return string.Format(CultureInfo.InvariantCulture, OwaOptionStrings.MailboxUsageUnitB, new object[]
			{
				size.ToBytes()
			});
		}
	}
}
