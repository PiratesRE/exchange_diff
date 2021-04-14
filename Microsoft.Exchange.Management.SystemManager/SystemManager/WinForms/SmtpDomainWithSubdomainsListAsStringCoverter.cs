using System;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class SmtpDomainWithSubdomainsListAsStringCoverter : ICustomTextConverter, ICustomFormatter
	{
		object ICustomTextConverter.Parse(Type deservedType, string s, IFormatProvider provider)
		{
			if (deservedType != typeof(MultiValuedProperty<SmtpDomainWithSubdomains>))
			{
				throw new NotSupportedException();
			}
			if (string.IsNullOrEmpty(s))
			{
				throw new FormatException(Strings.ValueCanNotBeEmpty);
			}
			MultiValuedProperty<SmtpDomainWithSubdomains> multiValuedProperty = new MultiValuedProperty<SmtpDomainWithSubdomains>();
			foreach (string text in s.Split(new char[]
			{
				','
			}))
			{
				if (!string.IsNullOrEmpty(text))
				{
					SmtpDomainWithSubdomains smtpDomainWithSubdomains = SmtpDomainWithSubdomains.Parse(text);
					if (SmtpDomainWithSubdomains.StarDomain.Equals(smtpDomainWithSubdomains))
					{
						throw new FormatException(Strings.DisallowStarDomainConstraintText);
					}
					if (!multiValuedProperty.Contains(smtpDomainWithSubdomains))
					{
						multiValuedProperty.Add(smtpDomainWithSubdomains);
					}
				}
			}
			return multiValuedProperty;
		}

		string ICustomFormatter.Format(string format, object arg, IFormatProvider formatProvider)
		{
			StringBuilder stringBuilder = new StringBuilder();
			MultiValuedProperty<SmtpDomainWithSubdomains> multiValuedProperty = arg as MultiValuedProperty<SmtpDomainWithSubdomains>;
			if (multiValuedProperty != null)
			{
				foreach (SmtpDomainWithSubdomains smtpDomainWithSubdomains in multiValuedProperty)
				{
					stringBuilder.Append(','.ToString());
					stringBuilder.Append(smtpDomainWithSubdomains.ToString());
				}
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Remove(0, 1);
			}
			return stringBuilder.ToString();
		}

		public const char SeperatorSign = ',';
	}
}
