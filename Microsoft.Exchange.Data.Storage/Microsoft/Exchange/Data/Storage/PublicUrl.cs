using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PublicUrl : PublishingUrl
	{
		private PublicUrl(Uri uri, SharingDataType dataType, SmtpAddress smtpAddress, string folderName) : base(uri, dataType, PublicUrl.CalculateIdentity(dataType, folderName))
		{
			this.smtpAddress = smtpAddress;
			this.folderName = folderName;
		}

		public SmtpAddress SmtpAddress
		{
			get
			{
				return this.smtpAddress;
			}
		}

		public override string Domain
		{
			get
			{
				return this.smtpAddress.Domain;
			}
		}

		public string FolderName
		{
			get
			{
				return this.folderName;
			}
		}

		internal override string TraceInfo
		{
			get
			{
				return string.Concat(new string[]
				{
					"DataType=",
					base.DataType.ToString(),
					"; SmtpAddress=",
					this.SmtpAddress.ToString(),
					"; FolderName=",
					this.FolderName,
					"; Identity=",
					base.Identity
				});
			}
		}

		public static PublicUrl Create(string externalUrl, SharingDataType dataType, SmtpAddress smtpAddress, string folderName, SharingAnonymousIdentityCollection sharingAnonymousIdentities)
		{
			Util.ThrowOnNullOrEmptyArgument(externalUrl, "externalUrl");
			Util.ThrowOnNullArgument(dataType, "dataType");
			Util.ThrowOnNullOrEmptyArgument(dataType.ExternalName, "dataType.ExternalName");
			Util.ThrowOnNullOrEmptyArgument(folderName, "folderName");
			string text = string.Join(string.Empty, folderName.Split(".*$&+,/:;=?@\"\\<>#%{}|\\^~[]`".ToCharArray()));
			if (string.IsNullOrEmpty(text))
			{
				text = "MyCalendar";
			}
			text = text.Replace(" ", "_");
			if (sharingAnonymousIdentities != null)
			{
				string text2 = text;
				int num = 0;
				for (;;)
				{
					string urlId = PublicUrl.CalculateIdentity(dataType, text2);
					if (!sharingAnonymousIdentities.Contains(urlId))
					{
						goto IL_CD;
					}
					if (++num > 50)
					{
						break;
					}
					ExTraceGlobals.SharingTracer.TraceDebug<string, int>(0L, "PublicUrl.Create(): {0} has been used in Sharing Anonymous Identities - Appending post fix: {1}.", text, num);
					text2 = string.Format("{0}({1})", text, num);
				}
				throw new CannotShareFolderException(ServerStrings.ExTooManyObjects("PublicUrl", num, 50));
				IL_CD:
				text = text2;
			}
			string uriString = string.Format("{0}/{1}/{2}/{3}/{1}", new object[]
			{
				externalUrl.TrimEnd(new char[]
				{
					'/'
				}),
				dataType.ExternalName,
				smtpAddress.ToString(),
				text
			});
			PublicUrl publicUrl = new PublicUrl(new Uri(uriString, UriKind.Absolute), dataType, smtpAddress, text);
			ExTraceGlobals.SharingTracer.TraceDebug<PublicUrl, string>(0L, "PublicUrl.Create(): Created an instance of PublicUrl: {0} - {1}.", publicUrl, publicUrl.TraceInfo);
			return publicUrl;
		}

		internal static bool TryParse(string url, out PublicUrl publicUrl)
		{
			publicUrl = null;
			Uri uri = null;
			if (!PublishingUrl.IsAbsoluteUriString(url, out uri))
			{
				ExTraceGlobals.SharingTracer.TraceError<string>(0L, "PublicUrl.TryParse(): The string '{0}' is not an valid Uri.", url);
				return false;
			}
			string localPath = uri.LocalPath;
			ExTraceGlobals.SharingTracer.TraceDebug<string>(0L, "PublicUrl.TryParse(): Get path of url: {0}", localPath);
			Match match = PublicUrl.PublicUrlRegex.Match(localPath);
			if (!match.Success)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<string>(0L, "PublicUrl.TryParse(): The string '{0}' is not PublicUrl.", url);
				return false;
			}
			SharingDataType dataType = SharingDataType.FromExternalName(match.Result("${datatype}"));
			string text = match.Result("${address}");
			if (!SmtpAddress.IsValidSmtpAddress(text))
			{
				ExTraceGlobals.SharingTracer.TraceDebug<string>(0L, "PublicUrl.TryParse(): {0} is not valid SMTP address.", text);
				return false;
			}
			publicUrl = new PublicUrl(uri, dataType, new SmtpAddress(text), match.Result("${name}"));
			ExTraceGlobals.SharingTracer.TraceDebug<string, string>(0L, "PublicUrl.TryParse(): The url {0} is parsed as PublicUrl {1}.", url, publicUrl.TraceInfo);
			return true;
		}

		private static string CalculateIdentity(SharingDataType dataType, string folderName)
		{
			Util.ThrowOnNullArgument(dataType, "dataType");
			Util.ThrowOnNullOrEmptyArgument(folderName, "folderName");
			return dataType.PublishResourceName + "\\" + Uri.EscapeDataString(folderName);
		}

		internal override SharingAnonymousIdentityCacheKey CreateKey()
		{
			return new PublicUrlKey(this);
		}

		private const string PublicUrlFormat = "{0}/{1}/{2}/{3}/{1}";

		private const string SpecialCharacters = ".*";

		private const string ReservedCharacters = "$&+,/:;=?@";

		private const string UnsafeCharacters = "\"\\<>#%{}|\\^~[]`";

		private const string RemoveCharacters = ".*$&+,/:;=?@\"\\<>#%{}|\\^~[]`";

		private const string DefaultFolderName = "MyCalendar";

		private const string FolderNameWithPostfix = "{0}({1})";

		private const int MaxNumberForPostFix = 50;

		private static readonly Regex PublicUrlRegex = new Regex("^/owa/(?<datatype>calendar|contacts)/(?<address>[^/ ]+)/(?<name>[^/ ]+)/(calendar|contacts)(.html|.ics)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private readonly SmtpAddress smtpAddress;

		private readonly string folderName;
	}
}
