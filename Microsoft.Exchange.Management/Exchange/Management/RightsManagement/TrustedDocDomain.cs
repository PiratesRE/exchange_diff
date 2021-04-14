using System;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[Serializable]
	public class TrustedDocDomain
	{
		public TrustedDocDomain()
		{
			this.m_ttdki = new KeyInformation();
		}

		public KeyInformation m_ttdki;

		public string[] m_strLicensorCertChain;

		public string[] m_astrRightsTemplates;
	}
}
