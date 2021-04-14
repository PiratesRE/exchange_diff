using System;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.HttpProxy
{
	internal class BackEndServerCookieEntry : BackEndCookieEntryBase
	{
		public BackEndServerCookieEntry(string fqdn, int version, ExDateTime expiryTime) : base(BackEndCookieEntryType.Server, expiryTime)
		{
			if (string.IsNullOrEmpty(fqdn))
			{
				throw new ArgumentNullException("fqdn");
			}
			this.Fqdn = fqdn;
			this.Version = version;
		}

		public BackEndServerCookieEntry(string fqdn, int version) : this(fqdn, version, ExDateTime.UtcNow + BackEndCookieEntryBase.BackEndServerCookieLifeTime)
		{
		}

		public string Fqdn { get; private set; }

		public int Version { get; private set; }

		public override bool ShouldInvalidate(BackEndServer badTarget)
		{
			if (badTarget == null)
			{
				throw new ArgumentNullException("badTarget");
			}
			return string.Equals(this.Fqdn, badTarget.Fqdn, StringComparison.OrdinalIgnoreCase);
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				BackEndCookieEntryBase.ConvertBackEndCookieEntryTypeToString(base.EntryType),
				"~",
				this.Fqdn,
				"~",
				this.Version.ToString(),
				"~",
				base.ExpiryTime.ToString("s")
			});
		}
	}
}
