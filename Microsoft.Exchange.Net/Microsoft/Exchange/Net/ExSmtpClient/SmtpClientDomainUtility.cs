using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.ExSmtpClient
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class SmtpClientDomainUtility
	{
		internal static bool IsValidDomain(string domain)
		{
			if (domain == null || domain.Length > 255)
			{
				return false;
			}
			SmtpClientDomainUtility.ValidationStage validationStage = SmtpClientDomainUtility.ValidationStage.DOMAIN;
			int num = 0;
			int num2 = 0;
			while (num2 < domain.Length && validationStage != SmtpClientDomainUtility.ValidationStage.ERROR)
			{
				char c = domain[num2];
				num2++;
				switch (validationStage)
				{
				case SmtpClientDomainUtility.ValidationStage.DOMAIN:
					if ((c < '\u0080' && SmtpClientDomainUtility.IsLetterOrDigit(c)) || c == '-' || c == '_')
					{
						num = num2;
						validationStage = SmtpClientDomainUtility.ValidationStage.DOMAIN_SUB;
						continue;
					}
					break;
				case SmtpClientDomainUtility.ValidationStage.DOMAIN_SUB:
					if (c == '.')
					{
						if (num2 - num > 63)
						{
							return false;
						}
						validationStage = SmtpClientDomainUtility.ValidationStage.DOMAIN_DOT;
						continue;
					}
					else if ((c < '\u0080' && SmtpClientDomainUtility.IsLetterOrDigit(c)) || c == '-' || c == '_')
					{
						validationStage = SmtpClientDomainUtility.ValidationStage.DOMAIN_SUB;
						continue;
					}
					break;
				case SmtpClientDomainUtility.ValidationStage.DOMAIN_DOT:
					if ((c < '\u0080' && SmtpClientDomainUtility.IsLetterOrDigit(c)) || c == '-' || c == '_')
					{
						num = num2;
						validationStage = SmtpClientDomainUtility.ValidationStage.DOMAIN_SUB;
						continue;
					}
					break;
				default:
					throw new NotSupportedException("Unexpected value of ValidationStage: " + validationStage.ToString());
				}
				validationStage = SmtpClientDomainUtility.ValidationStage.ERROR;
			}
			return validationStage == SmtpClientDomainUtility.ValidationStage.DOMAIN_SUB && num2 - num < 63;
		}

		private static bool IsLetterOrDigit(char character)
		{
			return (character >= 'a' && character <= 'z') || (character >= 'A' && character <= 'Z') || (character >= '0' && character <= '9');
		}

		private const int MaxDomainName = 255;

		private const int MaxSubDomainName = 63;

		private enum ValidationStage
		{
			DOMAIN,
			DOMAIN_SUB,
			DOMAIN_DOT,
			ERROR
		}
	}
}
