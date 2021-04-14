using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class Authority : IEquatable<Authority>
	{
		public string Fqdn
		{
			get
			{
				return this.fqdn;
			}
		}

		public int PortNumber
		{
			get
			{
				return this.portNumber;
			}
		}

		public Authority(string fqdn, int portNumber)
		{
			if (string.IsNullOrEmpty(fqdn))
			{
				throw new ArgumentNullException("fqdn");
			}
			this.fqdn = fqdn;
			this.portNumber = portNumber;
		}

		public override int GetHashCode()
		{
			return this.fqdn.GetHashCode() ^ this.portNumber.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as Authority);
		}

		public bool Equals(Authority authority)
		{
			return authority != null && this.portNumber == authority.portNumber && string.Equals(this.fqdn, authority.fqdn, StringComparison.OrdinalIgnoreCase);
		}

		public override string ToString()
		{
			return this.fqdn + ":" + this.portNumber;
		}

		private string fqdn;

		private int portNumber;
	}
}
