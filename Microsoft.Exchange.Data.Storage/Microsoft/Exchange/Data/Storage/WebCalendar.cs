using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class WebCalendar
	{
		static WebCalendar()
		{
			WebCalendar.RegisterPrefixes();
		}

		public static void RegisterPrefixes()
		{
			WebRequest.RegisterPrefix("webcal", new WebCalendar.WebCalRequestCreator(Uri.UriSchemeHttp));
			WebRequest.RegisterPrefix("webcals", new WebCalendar.WebCalRequestCreator(Uri.UriSchemeHttps));
		}

		public static SubscribeResultsWebCal Subscribe(MailboxSession mailboxSession, string iCalUrlString, string folderName = null)
		{
			Util.ThrowOnNullArgument(mailboxSession, "mailboxSession");
			Util.ThrowOnNullOrEmptyArgument(iCalUrlString, "iCalUrlString");
			Uri iCalUrl = null;
			if (!PublishingUrl.IsAbsoluteUriString(iCalUrlString, out iCalUrl) || !Array.Exists<string>(WebCalendar.validWebCalendarSchemes, (string scheme) => StringComparer.OrdinalIgnoreCase.Equals(scheme, iCalUrl.Scheme)))
			{
				throw new InvalidSharingDataException("iCalUrlString", iCalUrlString);
			}
			string text;
			if ((text = folderName) == null && (text = WebCalendar.GetFolderNameFromInternetCalendar(iCalUrl)) == null)
			{
				text = (WebCalendar.GetFolderNameFromUrl(iCalUrl) ?? ClientStrings.Calendar.ToString(mailboxSession.InternalPreferedCulture));
			}
			folderName = text;
			PublishingSubscriptionData newSubscription = WebCalendar.CreateSubscriptionData(iCalUrl, folderName);
			return WebCalendar.InternalSubscribe(mailboxSession, newSubscription, null, null);
		}

		internal static SubscribeResultsWebCal InternalSubscribe(MailboxSession mailboxSession, PublishingSubscriptionData newSubscription, string initiatorSmtpAddress, string initiatorName)
		{
			if (mailboxSession.MailboxOwner.MailboxInfo.Location.ServerVersion < Server.E14SP1MinVersion)
			{
				throw new NotSupportedWithMailboxVersionException();
			}
			StoreObjectId storeObjectId = null;
			SubscribeResultsWebCal result;
			using (PublishingSubscriptionManager publishingSubscriptionManager = new PublishingSubscriptionManager(mailboxSession))
			{
				PublishingFolderManager publishingFolderManager = new PublishingFolderManager(mailboxSession);
				PublishingSubscriptionData existing = publishingSubscriptionManager.GetExisting(newSubscription.Key);
				PublishingSubscriptionData publishingSubscriptionData = existing ?? newSubscription;
				IdAndName idAndName = publishingFolderManager.EnsureFolder(publishingSubscriptionData);
				if (publishingSubscriptionData.LocalFolderId == null || !publishingSubscriptionData.LocalFolderId.Equals(idAndName.Id))
				{
					storeObjectId = (publishingSubscriptionData.LocalFolderId = idAndName.Id);
				}
				PublishingSubscriptionData publishingSubscriptionData2 = publishingSubscriptionManager.CreateOrUpdate(publishingSubscriptionData, false);
				if (!publishingSubscriptionData.LocalFolderId.Equals(publishingSubscriptionData2.LocalFolderId))
				{
					idAndName = publishingFolderManager.GetFolder(publishingSubscriptionData2);
				}
				ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, StoreObjectId>(0L, "{0}: WebCalendar.InternalSubscribe will request a sync for folder id {1}.", mailboxSession.MailboxOwner, idAndName.Id);
				SyncAssistantInvoker.SyncFolder(mailboxSession, idAndName.Id);
				result = new SubscribeResultsWebCal(SharingDataType.Calendar, initiatorSmtpAddress, initiatorName, publishingSubscriptionData.RemoteFolderName, publishingSubscriptionData.PublishingUrl, idAndName.Id, storeObjectId != null, idAndName.Name);
			}
			return result;
		}

		private static PublishingSubscriptionData CreateSubscriptionData(Uri iCalUrl, string folderName)
		{
			return new PublishingSubscriptionData(SharingDataType.Calendar.PublishName, iCalUrl, folderName, null);
		}

		private static string GetFolderNameFromInternetCalendar(Uri iCalUrl)
		{
			if (iCalUrl.Scheme == "holidays")
			{
				return null;
			}
			HttpWebRequest httpWebRequest = WebRequest.Create(iCalUrl) as HttpWebRequest;
			httpWebRequest.AddRange(0, 511);
			httpWebRequest.Timeout = 10000;
			try
			{
				using (WebResponse response = httpWebRequest.GetResponse())
				{
					using (Stream responseStream = response.GetResponseStream())
					{
						byte[] array = new byte[512];
						int newSize = responseStream.Read(array, 0, 512);
						Array.Resize<byte>(ref array, newSize);
						string @string = Encoding.UTF8.GetString(array);
						int num = @string.IndexOf("X-WR-CALNAME:", StringComparison.InvariantCultureIgnoreCase);
						if (num > 0)
						{
							int num2 = @string.IndexOf(Environment.NewLine, num);
							if (num2 > num)
							{
								num += "X-WR-CALNAME:".Length;
								return @string.Substring(num, num2 - num).Trim();
							}
						}
					}
				}
			}
			catch (WebException arg)
			{
				ExTraceGlobals.SharingTracer.TraceError<WebException>(0L, "WebCalendar.GetFolderNameFromInternetCalendar: Unable to determine the calendar folder name due to {0}.", arg);
			}
			return null;
		}

		private static string GetFolderNameFromUrl(Uri iCalUrl)
		{
			string input = iCalUrl.LocalPath.Replace("+", " ").Replace(" ", "_");
			Match match = WebCalendar.regex.Match(input);
			if (match.Success)
			{
				return match.Result("${name}");
			}
			return null;
		}

		public const string UriSchemeWebCal = "webcal";

		public const string UriSchemeWebCals = "webcals";

		public const string UriSchemeHolidayCalendars = "holidays";

		private const string IcsCalendarNameHeader = "X-WR-CALNAME:";

		private const int IcsMaximumBytesToRead = 512;

		private const int IcsRequestTimeout = 10000;

		private static readonly string[] validWebCalendarSchemes = new string[]
		{
			Uri.UriSchemeHttp,
			Uri.UriSchemeHttps,
			"webcal",
			"webcals",
			"holidays"
		};

		private static readonly Regex regex = new Regex("/(?<name>[^/ ]+).ics$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private class WebCalRequestCreator : IWebRequestCreate
		{
			public WebCalRequestCreator(string newUriScheme)
			{
				this.NewUriScheme = newUriScheme;
			}

			public WebRequest Create(Uri uri)
			{
				return WebRequest.Create(new UriBuilder(uri)
				{
					Scheme = this.NewUriScheme
				}.Uri);
			}

			private string NewUriScheme { get; set; }
		}
	}
}
