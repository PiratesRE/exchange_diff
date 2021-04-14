using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Management.Transport;

namespace Microsoft.Office.CompliancePolicy.Validators
{
	internal class SharepointSource
	{
		public static SharepointSource Parse(string identity)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("identity", identity);
			string[] array = identity.Split(SharepointSource.tokenSeparator, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 0)
			{
				throw new SpIdentityFormatException(Strings.SpParserVersionNotSpecified);
			}
			int num;
			if (!int.TryParse(array[0], out num))
			{
				throw new SpIdentityFormatException(Strings.SpParserInvalidVersionType(array[0]));
			}
			if (!SharepointSource.identityParsers.ContainsKey(num))
			{
				throw new SpIdentityFormatException(Strings.SpParserVersionNotSupported(num));
			}
			return SharepointSource.identityParsers[num](array);
		}

		private static SharepointSource VersionOneIdentityParser(string[] tokens)
		{
			if (6 != tokens.Length)
			{
				throw new SpIdentityFormatException(Strings.SpParserUnexpectedNumberOfTokens(1, 6, tokens.Length));
			}
			if (!string.Equals("Web", tokens[1], StringComparison.OrdinalIgnoreCase))
			{
				throw new SpIdentityFormatException(Strings.SpParserUnexpectedContainerType("Web", tokens[1]));
			}
			if (!Uri.IsWellFormedUriString(tokens[2], UriKind.Absolute))
			{
				throw new SpIdentityFormatException(Strings.SpParserInvalidSiteUrl(tokens[2]));
			}
			string siteUrl = tokens[2];
			string title = tokens[3];
			Guid siteId;
			if (!Guid.TryParse(tokens[4], out siteId))
			{
				throw new SpIdentityFormatException(Strings.SpParserInvalidSiteId(tokens[4]));
			}
			Guid webId;
			if (!Guid.TryParse(tokens[5], out webId))
			{
				throw new SpIdentityFormatException(Strings.SpParserInvalidWebId(tokens[5]));
			}
			return new SharepointSource(siteUrl, title, siteId, webId);
		}

		public SharepointSource(string siteUrl, string title, Guid siteId, Guid webId)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("siteUrl", siteUrl);
			ArgumentValidator.ThrowIfNullOrEmpty("title", title);
			this.SiteUrl = siteUrl;
			this.Title = title;
			this.SiteId = siteId;
			this.WebId = webId;
			this.SetIdentity();
		}

		private void SetIdentity()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(1);
			stringBuilder.Append(SharepointSource.tokenSeparator[0]);
			stringBuilder.Append("Web");
			stringBuilder.Append(SharepointSource.tokenSeparator[0]);
			stringBuilder.Append(this.SiteUrl);
			stringBuilder.Append(SharepointSource.tokenSeparator[0]);
			stringBuilder.Append(this.Title);
			stringBuilder.Append(SharepointSource.tokenSeparator[0]);
			stringBuilder.Append(this.SiteId);
			stringBuilder.Append(SharepointSource.tokenSeparator[0]);
			stringBuilder.Append(this.WebId);
			stringBuilder.Append(SharepointSource.tokenSeparator[0]);
			this.Identity = stringBuilder.ToString();
		}

		public string Identity { get; private set; }

		public Guid WebId { get; private set; }

		public Guid SiteId { get; private set; }

		public string SiteUrl { get; private set; }

		public string Title { get; private set; }

		private const int CurrentVersion = 1;

		private const string ContainerType = "Web";

		private static readonly char[] tokenSeparator = new char[]
		{
			';'
		};

		private static readonly Dictionary<int, SharepointSource.ParserDelegate> identityParsers = new Dictionary<int, SharepointSource.ParserDelegate>
		{
			{
				1,
				new SharepointSource.ParserDelegate(SharepointSource.VersionOneIdentityParser)
			}
		};

		private delegate SharepointSource ParserDelegate(string[] tokens);
	}
}
