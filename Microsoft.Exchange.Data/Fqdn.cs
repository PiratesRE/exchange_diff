using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class Fqdn : SmtpDomain
	{
		public Fqdn(string fqdn) : base(fqdn)
		{
		}

		public new static Fqdn Parse(string fqdn)
		{
			return new Fqdn(fqdn);
		}

		public static bool TryParse(string fqdn, out Fqdn obj)
		{
			if (Fqdn.IsValidFqdn(fqdn))
			{
				obj = new Fqdn(fqdn);
				return true;
			}
			obj = null;
			return false;
		}

		public static bool IsValidFqdn(string fqdn)
		{
			return SmtpAddress.IsValidDomain(fqdn);
		}

		public static implicit operator string(Fqdn fqdn)
		{
			if (fqdn != null)
			{
				return fqdn.Domain;
			}
			return null;
		}
	}
}
