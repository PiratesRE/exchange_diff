using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class OwaPerTenantTransportSettings : TenantConfigurationCacheableItem<TransportConfigContainer>
	{
		public override long ItemSize
		{
			get
			{
				if (this.tlsSendDomainSecureList == null)
				{
					return 18L;
				}
				int num = 18;
				foreach (string text in this.tlsSendDomainSecureList)
				{
					num += text.Length;
				}
				return (long)num;
			}
		}

		public override void ReadData(IConfigurationSession session)
		{
			TransportConfigContainer[] array = session.Find<TransportConfigContainer>(null, QueryScope.SubTree, null, null, 2);
			if (array == null || array.Length != 1 || array[0] == null)
			{
				throw new InvalidDataResultException(string.Format("One and only one TransportConfigContainer was expected. But found these many: {0}", (array == null) ? 0 : array.Length));
			}
			MultiValuedProperty<SmtpDomain> tlssendDomainSecureList = array[0].TLSSendDomainSecureList;
			if (tlssendDomainSecureList != null)
			{
				this.tlsSendDomainSecureList = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				foreach (SmtpDomain smtpDomain in tlssendDomainSecureList)
				{
					if (!string.IsNullOrEmpty(smtpDomain.Domain))
					{
						this.tlsSendDomainSecureList.Add(smtpDomain.Domain);
					}
				}
			}
		}

		internal bool IsTLSSendSecureDomain(string domainName)
		{
			return this.tlsSendDomainSecureList != null && this.tlsSendDomainSecureList.Count > 0 && this.tlsSendDomainSecureList.Contains(domainName);
		}

		private const int FixedClrObjectOverhead = 18;

		private HashSet<string> tlsSendDomainSecureList;
	}
}
