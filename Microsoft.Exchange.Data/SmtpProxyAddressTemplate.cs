using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public sealed class SmtpProxyAddressTemplate : ProxyAddressTemplate
	{
		public SmtpProxyAddressTemplate(string addressTemplate, bool isPrimaryAddress) : base(ProxyAddressPrefix.Smtp, addressTemplate, isPrimaryAddress)
		{
			if (!SmtpProxyAddressTemplate.IsValidSmtpAddressTemplate(addressTemplate))
			{
				throw new ArgumentOutOfRangeException(DataStrings.InvalidSMTPAddressTemplateFormat(addressTemplate), null);
			}
		}

		public static bool IsValidSmtpAddressTemplate(string smtpAddressTemplate)
		{
			if (smtpAddressTemplate == null)
			{
				throw new ArgumentNullException("smtpAddressTemplate");
			}
			int num = smtpAddressTemplate.LastIndexOf('@');
			return num != -1 && SmtpAddress.IsValidDomain(smtpAddressTemplate.Substring(num + 1));
		}
	}
}
