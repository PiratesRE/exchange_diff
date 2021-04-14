using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.TextMessaging.MobileDriver.Resources;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal static class EmailMessageHelper
	{
		public static EmailRecipient GetSender(EmailMessage email)
		{
			if (email.Sender != null)
			{
				return email.Sender;
			}
			return email.From;
		}

		public static EmailRecipient GetFrom(EmailMessage email)
		{
			if (email.From != null)
			{
				return email.From;
			}
			return email.Sender;
		}

		public static string GetBodyText(EmailMessage email)
		{
			Body body = email.Body;
			Encoding encoding = Encoding.GetEncoding(body.CharsetName);
			string original = string.Empty;
			using (Stream contentReadStream = body.GetContentReadStream())
			{
				using (StringWriter stringWriter = new StringWriter())
				{
					if (BodyFormat.Html == body.BodyFormat)
					{
						new HtmlToText
						{
							InputEncoding = encoding
						}.Convert(contentReadStream, stringWriter);
						original = stringWriter.ToString();
					}
					else if (BodyFormat.Rtf == body.BodyFormat)
					{
						new RtfToText().Convert(contentReadStream, stringWriter);
						original = stringWriter.ToString();
					}
					else if (BodyFormat.Text == body.BodyFormat)
					{
						using (StreamReader streamReader = new StreamReader(contentReadStream, encoding))
						{
							original = streamReader.ReadToEnd();
						}
					}
				}
			}
			return StringNormalizer.TrimTrailingAndNormalizeEol(original);
		}

		public static MobileRecipient GetMobileRecipientFromImceaAddress(string address)
		{
			MobileRecipient result = null;
			ProxyAddress mobileProxyAddressFromImceaAddress = EmailMessageHelper.GetMobileProxyAddressFromImceaAddress(address);
			if (null == mobileProxyAddressFromImceaAddress)
			{
				return null;
			}
			MobileRecipient.TryParse(mobileProxyAddressFromImceaAddress.AddressString, out result);
			return result;
		}

		public static ProxyAddress GetMobileProxyAddressFromImceaAddress(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				return null;
			}
			ProxyAddress proxyAddress = null;
			if (SmtpProxyAddress.IsEncapsulatedAddress(address))
			{
				if (!SmtpProxyAddress.TryDeencapsulate(address, out proxyAddress))
				{
					return null;
				}
			}
			else
			{
				proxyAddress = ProxyAddress.Parse(ProxyAddressPrefix.Smtp.PrimaryPrefix, address);
			}
			if (!string.Equals(proxyAddress.PrefixString, "MOBILE", StringComparison.OrdinalIgnoreCase))
			{
				return null;
			}
			return proxyAddress;
		}

		public static void ThrowErrorNoProviderForNotificationNDR(string textMessage, string notification)
		{
			throw new MobileDriverStateException(Strings.ErrorNoProviderForNotification(textMessage, notification));
		}
	}
}
