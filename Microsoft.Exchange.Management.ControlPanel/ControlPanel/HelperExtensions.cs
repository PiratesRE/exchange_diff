using System;
using System.Linq;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public static class HelperExtensions
	{
		public static SmtpAddress[] ToSmtpAddressArray(this string smtpAddresses)
		{
			if (!string.IsNullOrEmpty(smtpAddresses))
			{
				string[] source = smtpAddresses.ToArrayOfStrings();
				return (from address in source
				select new SmtpAddress(address)).ToArray<SmtpAddress>();
			}
			return null;
		}

		public static string ToSmtpAddressesString(this SmtpAddress[] addresses)
		{
			if (addresses != null)
			{
				return (from address in addresses
				where true
				select address.ToString()).ToArray<string>().StringArrayJoin(",");
			}
			return null;
		}
	}
}
