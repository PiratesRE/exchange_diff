using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Autodiscover.ConfigurationCache
{
	internal class ServerId
	{
		public ServerId(ADObjectId id)
		{
			this.id = id;
			this.legacyDN = null;
		}

		public ServerId(string legacyDN)
		{
			this.id = null;
			this.legacyDN = legacyDN;
		}

		public string Key
		{
			get
			{
				if (this.id == null)
				{
					return this.legacyDN;
				}
				return this.id.ToString();
			}
		}

		private ADObjectId id;

		private string legacyDN;
	}
}
