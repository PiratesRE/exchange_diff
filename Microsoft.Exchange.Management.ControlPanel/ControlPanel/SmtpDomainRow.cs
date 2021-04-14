using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[KnownType(typeof(SmtpDomainRow))]
	[DataContract]
	public class SmtpDomainRow : BaseRow
	{
		public SmtpDomainRow(SmtpDomain smtpDomain) : base(new Identity(smtpDomain.Domain, smtpDomain.Domain), null)
		{
			AutoDiscoverSmtpDomain autoDiscoverSmtpDomain = smtpDomain as AutoDiscoverSmtpDomain;
			if (autoDiscoverSmtpDomain != null)
			{
				this.autodiscover = autoDiscoverSmtpDomain.AutoDiscover;
			}
		}

		[DataMember]
		public string Domain
		{
			get
			{
				return base.Identity.DisplayName;
			}
			set
			{
			}
		}

		[DataMember]
		public string Value
		{
			get
			{
				if (!this.autodiscover)
				{
					return this.Domain;
				}
				return "autod:" + this.Domain;
			}
			set
			{
			}
		}

		private const string AutodiscoverDomainPrefx = "autod:";

		private readonly bool autodiscover;
	}
}
