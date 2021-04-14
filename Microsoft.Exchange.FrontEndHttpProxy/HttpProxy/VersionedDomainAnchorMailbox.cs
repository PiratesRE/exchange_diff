using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.HttpProxy
{
	internal class VersionedDomainAnchorMailbox : DomainAnchorMailbox
	{
		public VersionedDomainAnchorMailbox(string domain, int version, IRequestContext requestContext) : base(AnchorSource.DomainAndVersion, domain + "~" + version.ToString(), requestContext)
		{
			this.domain = domain;
			this.Version = version;
		}

		public override string Domain
		{
			get
			{
				return this.domain;
			}
		}

		public int Version { get; private set; }

		public static AnchorMailbox GetAnchorMailbox(string domain, string versionString, IRequestContext requestContext)
		{
			ServerVersion serverVersion = VersionedDomainAnchorMailbox.ParseServerVersion(versionString);
			if (serverVersion == null)
			{
				return new DomainAnchorMailbox(domain, requestContext);
			}
			return new VersionedDomainAnchorMailbox(domain, serverVersion.Major, requestContext);
		}

		protected override ADRawEntry LoadADRawEntry()
		{
			if (this.Version >= 15)
			{
				return base.LoadADRawEntry();
			}
			IRecipientSession session = base.GetDomainRecipientSession();
			ADRawEntry ret = DirectoryHelper.InvokeAccountForest(base.RequestContext.LatencyTracker, () => HttpProxyBackEndHelper.GetE14EDiscoveryMailbox(session));
			return base.CheckForNullAndThrowIfApplicable<ADRawEntry>(ret);
		}

		private static ServerVersion ParseServerVersion(string versionString)
		{
			ServerVersion result = null;
			if (!string.IsNullOrEmpty(versionString))
			{
				Match match = Constants.ExchClientVerRegex.Match(versionString);
				ServerVersion serverVersion;
				if (match.Success && RegexUtilities.TryGetServerVersionFromRegexMatch(match, out serverVersion) && serverVersion.Major >= 14)
				{
					result = serverVersion;
				}
			}
			return result;
		}

		private readonly string domain;
	}
}
