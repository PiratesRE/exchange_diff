using System;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[Serializable]
	public class EncryptedTrustedDocDomain
	{
		public EncryptedTrustedDocDomain()
		{
			this.m_strTrustedDocDomainInfo = null;
			this.m_strKeyData = null;
		}

		public string m_strTrustedDocDomainInfo;

		public string m_strKeyData;
	}
}
