using System;

namespace Microsoft.Exchange.Security.OAuth
{
	internal sealed class OfficeExtensionInfo
	{
		public OfficeExtensionInfo(string extensionId, string scope)
		{
			OAuthCommon.VerifyNonNullArgument("extensionId", extensionId);
			this.extensionId = extensionId;
			this.scope = scope;
		}

		public bool IsScopedToken
		{
			get
			{
				return !string.IsNullOrEmpty(this.scope);
			}
		}

		public string Scope
		{
			get
			{
				return this.scope;
			}
		}

		public string ExtensionId
		{
			get
			{
				return this.extensionId;
			}
		}

		private readonly string scope;

		private readonly string extensionId;
	}
}
