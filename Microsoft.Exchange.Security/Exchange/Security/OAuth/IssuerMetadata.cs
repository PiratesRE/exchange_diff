using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Security.OAuth
{
	internal class IssuerMetadata
	{
		public IssuerMetadata(IssuerKind kind, string id, string realm)
		{
			this.kind = kind;
			this.id = id;
			this.realm = realm;
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		public string Realm
		{
			get
			{
				return this.realm;
			}
		}

		public IssuerKind Kind
		{
			get
			{
				return this.kind;
			}
		}

		public bool HasEmptyRealm
		{
			get
			{
				return OAuthCommon.IsRealmEmpty(this.realm);
			}
		}

		public static bool TryParse(string issuerString, out IssuerMetadata meta)
		{
			meta = null;
			issuerString = issuerString.Trim();
			int num = issuerString.LastIndexOf('@');
			if (num == -1)
			{
				return false;
			}
			string text = issuerString.Substring(num + 1);
			int num2 = issuerString.LastIndexOf('/');
			string text2;
			if (num2 == -1)
			{
				text2 = issuerString.Substring(0, num);
			}
			else
			{
				text2 = issuerString.Substring(0, num2);
			}
			meta = new IssuerMetadata(IssuerKind.External, text2, text);
			return true;
		}

		public static IssuerMetadata[] Parse(string trustedIssuers)
		{
			if (string.IsNullOrEmpty(trustedIssuers))
			{
				return null;
			}
			List<IssuerMetadata> list = null;
			foreach (string issuerString in trustedIssuers.Split(new char[]
			{
				','
			}))
			{
				IssuerMetadata item;
				if (IssuerMetadata.TryParse(issuerString, out item))
				{
					if (list == null)
					{
						list = new List<IssuerMetadata>();
					}
					list.Add(item);
				}
			}
			if (list != null)
			{
				return list.ToArray();
			}
			return null;
		}

		public static IssuerMetadata Create(AuthServer authServer)
		{
			return new IssuerMetadata(IssuerKind.ACS, authServer.IssuerIdentifier, OAuthCommon.IsRealmEmpty(authServer.Realm) ? "*" : authServer.Realm);
		}

		public bool MatchIdAndRealm(IssuerMetadata other)
		{
			return this.MatchId(other) && this.MatchRealm(other);
		}

		public bool MatchId(IssuerMetadata other)
		{
			return OAuthCommon.IsIdMatch(this.Id, other.Id);
		}

		public bool MatchRealm(IssuerMetadata other)
		{
			return OAuthCommon.IsRealmMatch(this.realm, other.realm);
		}

		public string ToTrustedIssuerString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}@{1}", new object[]
			{
				this.Id,
				string.IsNullOrEmpty(this.Realm) ? "*" : this.Realm
			});
		}

		public override bool Equals(object obj)
		{
			IssuerMetadata issuerMetadata = obj as IssuerMetadata;
			return issuerMetadata != null && this.MatchIdAndRealm(issuerMetadata);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}@{1}", new object[]
			{
				this.id,
				this.realm
			});
		}

		private readonly string id;

		private readonly string realm;

		private readonly IssuerKind kind;
	}
}
