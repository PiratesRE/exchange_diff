using System;
using System.Globalization;

namespace Microsoft.Exchange.Security.OAuth
{
	internal sealed class LocalTokenIssuerMetadata : IssuerMetadata
	{
		public LocalTokenIssuerMetadata(string id, string realm) : base(IssuerKind.PartnerApp, id, realm)
		{
			this.defaultNameId = string.Format(CultureInfo.InvariantCulture, "{0}@{1}", new object[]
			{
				base.Id,
				base.Realm
			});
			this.defaultIssuer = this.GetIssuer(base.Realm);
		}

		public string GetIssuer()
		{
			return this.defaultIssuer;
		}

		public string GetIssuer(string tenantId)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}@{1}", new object[]
			{
				base.Id,
				tenantId
			});
		}

		public string GetNameId()
		{
			return this.defaultNameId;
		}

		private readonly string defaultIssuer;

		private readonly string defaultNameId;
	}
}
