using System;
using System.Globalization;
using System.Security.Principal;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Transport.RecipientAPI;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ObscureUrl : PublishingUrl
	{
		private ObscureUrl(Uri uri, SharingDataType dataType, Guid mailboxGuid, string domain, string guidIdentity, ObscureKind obscureKind, SecurityIdentifier reachUserSid) : base(uri, dataType, ObscureUrl.CalculateIdentity(dataType, guidIdentity))
		{
			Util.ThrowOnNullOrEmptyArgument(domain, "domain");
			Util.ThrowOnNullOrEmptyArgument(guidIdentity, "guidIdentity");
			this.ReachUserSid = reachUserSid;
			this.mailboxGuid = mailboxGuid;
			this.domain = domain;
			this.guidIdentity = guidIdentity;
			this.obscureKind = obscureKind;
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public override string Domain
		{
			get
			{
				return this.domain;
			}
		}

		public string GuidIdentity
		{
			get
			{
				return this.guidIdentity;
			}
		}

		public ObscureKind ObscureKind
		{
			get
			{
				return this.obscureKind;
			}
		}

		public SecurityIdentifier ReachUserSid { get; private set; }

		internal override string TraceInfo
		{
			get
			{
				string text = string.Concat(new object[]
				{
					"DataType=",
					base.DataType.ToString(),
					"; MailboxGuid=",
					this.MailboxGuid.ToString(),
					"; Domain=",
					this.Domain,
					"; GuidIdentity=",
					this.GuidIdentity,
					"; Identity=",
					base.Identity,
					"; ObscureKind=",
					this.ObscureKind
				});
				if (this.ReachUserSid != null)
				{
					text = text + "ReachUserSid=" + this.ReachUserSid;
				}
				return text;
			}
		}

		public static ObscureUrl CreatePublishReachCalendarUrl(string externalUrl, Guid mailboxGuid, string domain, string existingIdentity, SecurityIdentifier reachUserSid)
		{
			Util.ThrowOnNullOrEmptyArgument(externalUrl, "externalUrl");
			Util.ThrowOnNullOrEmptyArgument(domain, "domain");
			Util.ThrowOnNullArgument(reachUserSid, "reachUserSid");
			if (!ExternalUser.IsValidReachSid(reachUserSid))
			{
				throw new ArgumentException("Not an valid reach user sid.", "reachUserSid");
			}
			SharingDataType reachCalendar = SharingDataType.ReachCalendar;
			string text = string.IsNullOrEmpty(existingIdentity) ? Guid.NewGuid().ToString("N") : ObscureUrl.ParseGuidIdentity(reachCalendar, existingIdentity).ToString("N");
			string text2 = ObscureUrl.CalculateHashString(ObscureKind.Normal, text);
			string uriString = string.Format("{0}/{1}/{2}@{3}/{4}{5}/{6}/{7}", new object[]
			{
				externalUrl.TrimEnd(new char[]
				{
					'/'
				}),
				reachCalendar.ExternalName,
				mailboxGuid.ToString("N"),
				domain,
				text,
				text2,
				reachUserSid.ToString(),
				reachCalendar.PublishResourceName
			});
			ObscureUrl obscureUrl = new ObscureUrl(new Uri(uriString, UriKind.Absolute), reachCalendar, mailboxGuid, domain, text, ObscureKind.Normal, reachUserSid);
			ExTraceGlobals.SharingTracer.TraceDebug<ObscureUrl, string>(0L, "ObscureUrl.CreatePublishReachCalendarUrl(): Created an instance of ObscureUrl: {0} - {1}.", obscureUrl, obscureUrl.TraceInfo);
			return obscureUrl;
		}

		public static ObscureUrl CreatePublishCalendarUrl(string externalUrl, Guid mailboxGuid, string domain)
		{
			Util.ThrowOnNullOrEmptyArgument(externalUrl, "externalUrl");
			Util.ThrowOnNullOrEmptyArgument(domain, "domain");
			SharingDataType calendar = SharingDataType.Calendar;
			int num = 1;
			string text;
			string text2;
			for (;;)
			{
				text = Guid.NewGuid().ToString("N");
				text2 = ObscureUrl.CalculateHashString(ObscureKind.Restricted, text);
				if (!StringComparer.Ordinal.Equals(text2, ObscureUrl.CalculateHashString(ObscureKind.Normal, text)))
				{
					goto IL_7D;
				}
				if (++num > 5)
				{
					break;
				}
				ExTraceGlobals.SharingTracer.TraceDebug<string, int>(0L, "ObscureUrl.CreatePublishCalendarUrl(): Hash strings are identical for GUID {0} - Generating the GUID. Tried times = {1}.", text, num);
			}
			throw new TransientException(ServerStrings.ExTooManyObjects("ObscureUrl", num, 5));
			IL_7D:
			string uriString = string.Format("{0}/{1}/{2}@{3}/{4}{5}/{6}", new object[]
			{
				externalUrl.TrimEnd(new char[]
				{
					'/'
				}),
				calendar.ExternalName,
				mailboxGuid.ToString("N"),
				domain,
				text,
				text2,
				calendar.PublishResourceName
			});
			ObscureUrl obscureUrl = new ObscureUrl(new Uri(uriString, UriKind.Absolute), calendar, mailboxGuid, domain, text, ObscureKind.Restricted, null);
			ExTraceGlobals.SharingTracer.TraceDebug<ObscureUrl, string>(0L, "ObscureUrl.CreatePublishCalendarUrl(): Created an instance of ObscureUrl: {0} - {1}.", obscureUrl, obscureUrl.TraceInfo);
			return obscureUrl;
		}

		internal static bool TryParse(string url, out ObscureUrl obscureUrl)
		{
			obscureUrl = null;
			if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
			{
				ExTraceGlobals.SharingTracer.TraceError<string>(0L, "ObscureUrl.TryParse(): The string '{0}' is not an Absolute Uri.", url);
				return false;
			}
			Uri uri = new Uri(url, UriKind.Absolute);
			string localPath = uri.LocalPath;
			ExTraceGlobals.SharingTracer.TraceDebug<string>(0L, "ObscureUrl.TryParse(): Get path of url: {0}", localPath);
			Match match = ObscureUrl.ObscureUrlRegex.Match(localPath);
			if (!match.Success)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<string>(0L, "ObscureUrl.TryParse(): The string '{0}' is not desired ObscureUrl. ", url);
				return false;
			}
			SharingDataType sharingDataType = SharingDataType.FromPublishResourceName(match.Result("${datatype}"));
			Guid guid = Guid.Empty;
			Exception ex = null;
			try
			{
				guid = new Guid(match.Result("${mailboxguid}"));
			}
			catch (FormatException ex2)
			{
				ex = ex2;
			}
			catch (OverflowException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<string, Exception>(0L, "ObscureUrl.TryParse(): The string '{0}' is not desired ObscureUrl. Exception = {1}.", url, ex);
				return false;
			}
			string text = match.Result("${guid}");
			string x = match.Result("${hash}");
			SecurityIdentifier securityIdentifier = null;
			string text2 = match.Result("${sid}");
			if (sharingDataType == SharingDataType.ReachCalendar)
			{
				if (!StringComparer.Ordinal.Equals(x, ObscureUrl.CalculateHashString(ObscureKind.Normal, text)))
				{
					ExTraceGlobals.SharingTracer.TraceDebug<string>(0L, "ObscureUrl.TryParse(): Incorrect hash value for reachUrl:'{0}'", url);
					return false;
				}
				if (string.IsNullOrEmpty(text2))
				{
					ExTraceGlobals.SharingTracer.TraceDebug<string>(0L, "ObscureUrl.TryParse(): sid value missing for reachUrl:'{0}'", url);
					return false;
				}
				securityIdentifier = new SecurityIdentifier(text2);
				if (!ExternalUser.IsValidReachSid(securityIdentifier))
				{
					ExTraceGlobals.SharingTracer.TraceDebug<string>(0L, "ObscureUrl.TryParse(): Not a valid reach sid:'{0}'", url);
					return false;
				}
				obscureUrl = new ObscureUrl(uri, sharingDataType, guid, match.Result("${domain}"), text, ObscureKind.Normal, securityIdentifier);
				ExTraceGlobals.SharingTracer.TraceDebug<string, string>(0L, "ObscureUrl.TryParse(): The url {0} is parsed as ReachCalendar ObscureUrl {1}.", url, obscureUrl.TraceInfo);
				return true;
			}
			else
			{
				if (sharingDataType != SharingDataType.Calendar)
				{
					ExTraceGlobals.SharingTracer.TraceDebug<SharingDataType>(0L, "ObscureUrl.TryParse(): Sharing Datatype:{0} is not supported ", sharingDataType);
					return false;
				}
				if (!string.IsNullOrEmpty(text2))
				{
					ExTraceGlobals.SharingTracer.TraceDebug<string>(0L, "ObscureUrl.TryParse(): SID is not applicable for published calendar url:'{0}'", url);
					return false;
				}
				foreach (object obj in Enum.GetValues(typeof(ObscureKind)))
				{
					ObscureKind obscureKind = (ObscureKind)obj;
					if (StringComparer.Ordinal.Equals(x, ObscureUrl.CalculateHashString(obscureKind, text)))
					{
						obscureUrl = new ObscureUrl(uri, sharingDataType, guid, match.Result("${domain}"), text, obscureKind, securityIdentifier);
						ExTraceGlobals.SharingTracer.TraceDebug<string, string>(0L, "ObscureUrl.TryParse(): The url {0} is parsed as Calendar ObscureUrl {1}.", url, obscureUrl.TraceInfo);
						return true;
					}
				}
				ExTraceGlobals.SharingTracer.TraceDebug<string>(0L, "ObscureUrl.TryParse(): Incorrect hash value for Calendar url:{0}", url);
				return false;
			}
		}

		private static string CalculateIdentity(SharingDataType dataType, string guidIdentity)
		{
			Util.ThrowOnNullArgument(dataType, "dataType");
			Util.ThrowOnNullOrEmptyArgument(guidIdentity, "guidIdentity");
			return dataType.PublishResourceName + "\\" + guidIdentity;
		}

		private static Guid ParseGuidIdentity(SharingDataType dataType, string obscureIdentity)
		{
			int num = dataType.PublishResourceName.Length + 1 + 32;
			if (obscureIdentity.Length != num)
			{
				throw new ArgumentException(obscureIdentity, "obscureIdentity");
			}
			int num2 = obscureIdentity.IndexOf(dataType.PublishResourceName, StringComparison.OrdinalIgnoreCase);
			if (num2 < 0)
			{
				throw new ArgumentException(obscureIdentity, "obscureIdentity");
			}
			return new Guid(obscureIdentity.Substring(dataType.PublishResourceName.Length + 1));
		}

		private static string CalculateHashString(ObscureKind obscureKind, string guidIdentity)
		{
			StringHasher stringHasher = new StringHasher();
			return stringHasher.GetHash(guidIdentity + obscureKind).ToString(NumberFormatInfo.InvariantInfo);
		}

		internal override SharingAnonymousIdentityCacheKey CreateKey()
		{
			return new ObscureUrlKey(this);
		}

		internal ObscureUrl ChangeToKind(ObscureKind obscureKind)
		{
			if (base.DataType == SharingDataType.ReachCalendar || this.ObscureKind == obscureKind)
			{
				return this;
			}
			string str = ObscureUrl.CalculateHashString(this.ObscureKind, this.GuidIdentity);
			string str2 = ObscureUrl.CalculateHashString(obscureKind, this.GuidIdentity);
			string originalString = base.Uri.OriginalString;
			string uriString = originalString.Replace(this.GuidIdentity + str, this.GuidIdentity + str2);
			return new ObscureUrl(new Uri(uriString, UriKind.Absolute), base.DataType, this.MailboxGuid, this.Domain, this.GuidIdentity, obscureKind, this.ReachUserSid);
		}

		private const string ObscureCalendarUrlFormat = "{0}/{1}/{2}@{3}/{4}{5}/{6}";

		private const string ObscureReachCalendarUrlFormat = "{0}/{1}/{2}@{3}/{4}{5}/{6}/{7}";

		private const int MaxTryTimes = 5;

		private static readonly Regex ObscureUrlRegex = new Regex("^/owa/calendar/(?<mailboxguid>\\w{32})@(?<domain>[\\S.]+)/(?<guid>\\w{32})(?<hash>[0-9]+)(/(?<sid>S-1-8-([0-9]{1,10})-([0-9]{1,10})-([0-9]{1,10})-([0-9]{1,10})))?/(?<datatype>calendar|reachcalendar)(.html|.ics)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private readonly Guid mailboxGuid;

		private readonly string domain;

		private readonly string guidIdentity;

		private readonly ObscureKind obscureKind;
	}
}
