using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class FederatedDomain
	{
		public FederatedDomain(SmtpDomain domain)
		{
			this.Domain = domain;
		}

		public FederatedDomain(SmtpDomain domain, DomainState state)
		{
			this.Domain = domain;
			this.State = state;
			this.containsExtendedInfo = true;
		}

		public SmtpDomain Domain { get; private set; }

		public DomainState State { get; private set; }

		public override string ToString()
		{
			if (this.Domain == null || string.IsNullOrEmpty(this.Domain.Domain))
			{
				return string.Empty;
			}
			if (this.containsExtendedInfo)
			{
				return this.Domain + "=" + this.State.ToString();
			}
			return this.Domain.ToString();
		}

		private bool containsExtendedInfo;
	}
}
