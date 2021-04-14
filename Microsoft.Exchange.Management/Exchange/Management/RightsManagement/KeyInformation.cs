using System;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[Serializable]
	public class KeyInformation
	{
		public string strID;

		public string strIDType;

		public int nCSPType;

		public string strCSPName;

		public string strKeyContainerName;

		public int nKeyNumber;

		public string strEncryptedPrivateKey;
	}
}
