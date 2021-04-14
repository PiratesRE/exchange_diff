using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public sealed class FederatedDomainProof
	{
		public SmtpDomain DomainName { get; internal set; }

		public string Name { get; internal set; }

		public string Thumbprint { get; internal set; }

		public string Proof { get; internal set; }

		public string DnsRecord { get; internal set; }

		public FederatedDomainProof()
		{
		}

		public FederatedDomainProof(SmtpDomain domainName, string name, string thumbprint, string proof)
		{
			this.DomainName = domainName;
			this.Name = name;
			this.Thumbprint = thumbprint;
			this.Proof = proof;
			this.DnsRecord = domainName + " TXT IN " + proof;
		}

		public override string ToString()
		{
			return this.DnsRecord;
		}
	}
}
