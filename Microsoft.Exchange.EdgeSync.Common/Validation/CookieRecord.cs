using System;

namespace Microsoft.Exchange.EdgeSync.Validation
{
	[Serializable]
	public class CookieRecord
	{
		public string BaseDN;

		public string DomainController;

		public DateTime LastUpdated;

		public int CookieLength;
	}
}
