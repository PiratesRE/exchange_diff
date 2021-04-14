using System;
using System.Security.Principal;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class SuggesterSidCompositeKey
	{
		public SuggesterSidCompositeKey(SecurityIdentifier sid, string fqdn)
		{
			if (sid == null)
			{
				throw new ArgumentNullException("sid");
			}
			this.key = SuggesterSidCompositeKey.BuildKey(sid, fqdn);
		}

		internal static string BuildKey(SecurityIdentifier sid, string fqdn)
		{
			return string.Format("{0}{1}", sid, fqdn);
		}

		public string Key
		{
			get
			{
				return this.key;
			}
		}

		public override string ToString()
		{
			return this.Key;
		}

		private string key;
	}
}
