using System;

namespace Microsoft.Exchange.SecureMail
{
	internal class OrganizationScopeResult
	{
		public OrganizationScopeResult(bool fromOrganizationScope, bool highConfidence, string reason)
		{
			this.fromOrganizationScope = fromOrganizationScope;
			this.highConfidence = highConfidence;
			this.reason = reason;
		}

		public bool FromOrganizationScope
		{
			get
			{
				return this.fromOrganizationScope;
			}
		}

		public bool HighConfidence
		{
			get
			{
				return this.highConfidence;
			}
		}

		public string Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly bool fromOrganizationScope;

		private readonly bool highConfidence;

		private readonly string reason;
	}
}
