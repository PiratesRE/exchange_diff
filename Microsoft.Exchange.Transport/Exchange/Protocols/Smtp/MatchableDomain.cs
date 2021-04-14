using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class MatchableDomain
	{
		public MatchableDomain(SmtpDomainWithSubdomains domain)
		{
			ArgumentValidator.ThrowIfNull("domain", domain);
			if (domain.IsStar)
			{
				throw new ArgumentException("Domain cannot be just \"*\"");
			}
			this.domain = domain;
			this.dotCount = 0;
			this.firstDotIndex = -1;
			string domainName = this.DomainName;
			for (int i = 0; i < domainName.Length; i++)
			{
				if (domainName[i] == '.')
				{
					this.dotCount++;
					if (this.dotCount == 1)
					{
						this.firstDotIndex = i;
					}
				}
			}
		}

		public SmtpDomainWithSubdomains Domain
		{
			get
			{
				return this.domain;
			}
		}

		public string DomainName
		{
			get
			{
				return this.domain.SmtpDomain.Domain;
			}
		}

		public bool IncludeSubdomains
		{
			get
			{
				return this.domain.IncludeSubDomains;
			}
		}

		public int DotCount
		{
			get
			{
				return this.dotCount;
			}
		}

		public int MatchCertName(string certName, MatchOptions options, int matchDotCountThreshold)
		{
			if (string.IsNullOrEmpty(certName))
			{
				return -1;
			}
			int num = 0;
			bool flag = certName[0] == '*';
			if (flag)
			{
				if (certName.Length < 3 || certName[1] != '.')
				{
					return -1;
				}
				num = 2;
			}
			if (this.EqualsToSubstring(certName, num))
			{
				return int.MaxValue;
			}
			bool flag2 = (options & MatchOptions.MultiLevelCertWildcards) != MatchOptions.None;
			if (matchDotCountThreshold < this.dotCount && (flag != this.IncludeSubdomains || (flag && flag2)))
			{
				int num2 = this.dotCount;
				int num3 = 0;
				bool flag3 = false;
				if (flag)
				{
					if (this.firstDotIndex == -1 || this.DomainName.Length <= certName.Length - num)
					{
						return -1;
					}
					if (flag2)
					{
						num--;
						num3 = this.DomainName.Length - certName.Length + 1;
						flag3 = true;
					}
					else
					{
						num3 = this.firstDotIndex + 1;
						num2--;
						if (matchDotCountThreshold >= num2)
						{
							return -1;
						}
					}
				}
				else
				{
					num = certName.IndexOf('.');
					if (num == -1)
					{
						return -1;
					}
					num++;
				}
				if (this.EndsWithSubstring(num3, certName, num))
				{
					if (flag3)
					{
						num2 = this.CountDotsFrom(num3 + 1);
						if (matchDotCountThreshold >= num2)
						{
							return -1;
						}
					}
					return num2;
				}
			}
			return -1;
		}

		private bool EqualsToSubstring(string s, int substrStartIndex)
		{
			return this.EndsWithSubstring(0, s, substrStartIndex);
		}

		private bool EndsWithSubstring(int startIndex, string s, int substrStartIndex)
		{
			string domainName = this.DomainName;
			return domainName.Length - startIndex == s.Length - substrStartIndex && string.Compare(domainName, startIndex, s, substrStartIndex, domainName.Length - startIndex, StringComparison.OrdinalIgnoreCase) == 0;
		}

		private int CountDotsFrom(int startIndex)
		{
			string domainName = this.DomainName;
			int num = 0;
			for (int i = startIndex; i < domainName.Length; i++)
			{
				if (domainName[i] == '.')
				{
					num++;
				}
			}
			return num;
		}

		private const char Dot = '.';

		private const char Star = '*';

		private readonly SmtpDomainWithSubdomains domain;

		private readonly int dotCount;

		private readonly int firstDotIndex;
	}
}
