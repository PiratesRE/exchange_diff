using System;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class AutoDiscoverSmtpDomain : SmtpDomain
	{
		public AutoDiscoverSmtpDomain(string domain) : this(domain, true)
		{
		}

		private AutoDiscoverSmtpDomain(string domain, bool check) : base(domain, check)
		{
		}

		public bool AutoDiscover { get; set; }

		public new static AutoDiscoverSmtpDomain Parse(string text)
		{
			bool autoDiscover;
			string domain = AutoDiscoverSmtpDomain.Parse(text, out autoDiscover);
			return new AutoDiscoverSmtpDomain(domain)
			{
				AutoDiscover = autoDiscover
			};
		}

		public static bool TryParse(string text, out AutoDiscoverSmtpDomain obj)
		{
			if (!string.IsNullOrEmpty(text))
			{
				bool autoDiscover;
				string domain = AutoDiscoverSmtpDomain.Parse(text, out autoDiscover);
				if (SmtpAddress.IsValidDomain(domain))
				{
					obj = new AutoDiscoverSmtpDomain(domain, false)
					{
						AutoDiscover = autoDiscover
					};
					return true;
				}
			}
			obj = null;
			return false;
		}

		public new static AutoDiscoverSmtpDomain GetDomainPart(RoutingAddress address)
		{
			string domainPart = address.DomainPart;
			if (domainPart != null)
			{
				return new AutoDiscoverSmtpDomain(domainPart, false);
			}
			return null;
		}

		public bool Equals(AutoDiscoverSmtpDomain rhs)
		{
			return base.Equals(rhs);
		}

		public override string ToString()
		{
			if (!this.AutoDiscover)
			{
				return base.ToString();
			}
			return string.Format("{0}{1}", "autod:", base.ToString());
		}

		private static string Parse(string text, out bool autoDiscover)
		{
			autoDiscover = false;
			if (!string.IsNullOrEmpty(text) && text.StartsWith("autod:", StringComparison.OrdinalIgnoreCase))
			{
				autoDiscover = true;
				return text.Substring("autod:".Length);
			}
			return text;
		}

		private const string AutoDiscoverPrefix = "autod:";
	}
}
