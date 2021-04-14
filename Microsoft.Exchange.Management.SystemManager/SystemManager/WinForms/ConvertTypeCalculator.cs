using System;
using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class ConvertTypeCalculator
	{
		public static void Convert(DataRow dataRow)
		{
			foreach (object obj in dataRow.Table.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				Type type = dataColumn.ExtendedProperties["ExpectedType"] as Type;
				string text = dataRow[dataColumn] as string;
				if (type != null && dataRow[dataColumn].GetType() != type && text != null)
				{
					if (type == typeof(Unlimited<int>))
					{
						dataRow[dataColumn] = Unlimited<int>.Parse(text);
					}
					else if (type == typeof(Unlimited<EnhancedTimeSpan>))
					{
						dataRow[dataColumn] = Unlimited<EnhancedTimeSpan>.Parse(text);
					}
					else if (type == typeof(EmailAddressPolicyPriority))
					{
						dataRow[dataColumn] = EmailAddressPolicyPriority.Parse(text);
					}
					else if (type == typeof(SmtpDomainWithSubdomains))
					{
						dataRow[dataColumn] = SmtpDomainWithSubdomains.Parse(text);
					}
					else if (type == typeof(SmtpAddress))
					{
						dataRow[dataColumn] = SmtpAddress.Parse(text);
					}
					else if (type == typeof(ProxyAddress))
					{
						dataRow[dataColumn] = ProxyAddress.Parse(text);
					}
					else if (type == typeof(MailboxId))
					{
						dataRow[dataColumn] = MailboxId.Parse(text);
					}
					else if (type == typeof(UMLanguage))
					{
						dataRow[dataColumn] = UMLanguage.Parse(text);
					}
					else
					{
						if (!(type == typeof(ExchangeObjectVersion)))
						{
							throw new ArgumentException(string.Format("Type {0} is not supported convert from string yet", type));
						}
						Regex regex = new Regex("^(?<Major>\\d+)\\.(?<Minor>\\d+) \\((?<buildMajor>\\d+)\\.(?<buildMinor>\\d+)\\.(?<buildVersion>\\d+)\\.(?<buildRevison>\\d+)\\)$");
						Match match = regex.Match(text);
						if (!match.Success)
						{
							throw new ArgumentException(string.Format("{0} is not a valid ExchangeObjectVersion", text));
						}
						dataRow[dataColumn] = new ExchangeObjectVersion(byte.Parse(match.Result("${Major}")), byte.Parse(match.Result("${Minor}")), byte.Parse(match.Result("${buildMajor}")), byte.Parse(match.Result("${buildMinor}")), ushort.Parse(match.Result("${buildVersion}")), ushort.Parse(match.Result("${buildRevison}")));
					}
				}
			}
		}
	}
}
