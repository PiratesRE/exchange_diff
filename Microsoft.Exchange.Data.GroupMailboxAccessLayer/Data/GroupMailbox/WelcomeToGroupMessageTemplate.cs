using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Security.AntiXss;
using Microsoft.Exchange.Data.ApplicationLogic.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.GroupMailbox.Escalation;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.UnifiedGroups;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class WelcomeToGroupMessageTemplate : IMessageComposerBuilder
	{
		public WelcomeToGroupMessageTemplate(ADUser groupMailbox, IExchangePrincipal groupPrincipal, ADRecipient executingUser)
		{
			ArgumentValidator.ThrowIfNull("groupMailbox", groupMailbox);
			ArgumentValidator.ThrowIfNull("groupPrincipal", groupPrincipal);
			this.groupMailbox = groupMailbox;
			this.encodedGroupDescription = WelcomeToGroupMessageTemplate.GetGroupDescription(groupMailbox);
			this.encodedGroupDisplayName = AntiXssEncoder.HtmlEncode(groupMailbox.DisplayName, false);
			this.groupPrincipal = groupPrincipal;
			this.executingUser = executingUser;
		}

		public string EncodedGroupDisplayName
		{
			get
			{
				return this.encodedGroupDisplayName;
			}
		}

		public string EncodedGroupDescription
		{
			get
			{
				return this.encodedGroupDescription;
			}
		}

		public string GroupInboxUrl
		{
			get
			{
				return this.groupInboxUrl;
			}
		}

		public string GroupCalendarUrl
		{
			get
			{
				return this.groupCalendarUrl;
			}
		}

		public string GroupSharePointUrl
		{
			get
			{
				return this.groupSharePointUrl;
			}
		}

		public string SubscribeUrl
		{
			get
			{
				return this.subscribeUrl;
			}
		}

		public string UnsubscribeUrl
		{
			get
			{
				return this.unsubscribeUrl;
			}
		}

		public bool GroupIsAutoSubscribe
		{
			get
			{
				return this.Group.AutoSubscribeNewGroupMembers;
			}
		}

		public ADUser Group
		{
			get
			{
				return this.groupMailbox;
			}
		}

		public ImageAttachment GroupPhoto
		{
			get
			{
				return this.groupPhoto;
			}
		}

		public ImageAttachment ExecutingUserPhoto
		{
			get
			{
				return this.executingUserPhoto;
			}
		}

		public bool GroupHasPhoto
		{
			get
			{
				return this.GroupPhoto != null;
			}
		}

		public bool ExecutingUserHasPhoto
		{
			get
			{
				return this.ExecutingUserPhoto != null;
			}
		}

		public Participant EmailFrom
		{
			get
			{
				return this.emailFrom;
			}
		}

		public Participant EmailSender
		{
			get
			{
				return this.emailSender;
			}
		}

		public ADRecipient ExecutingUser
		{
			get
			{
				return this.executingUser;
			}
		}

		public string EncodedExecutingUserDisplayName
		{
			get
			{
				return this.encodedExecutingUserDisplayName;
			}
		}

		public IMessageComposer Build(ADUser recipient)
		{
			ArgumentValidator.ThrowIfNull("recipient", recipient);
			this.Initialize();
			return new WelcomeToGroupMessageComposer(this, recipient, this.groupMailbox);
		}

		private static ImageAttachment GetADThumbnailPhoto(ADRecipient adUser, string imageId, string imageName)
		{
			ImageAttachment result = null;
			if (adUser != null && adUser.ThumbnailPhoto != null && adUser.ThumbnailPhoto.Length > 0)
			{
				WelcomeToGroupMessageTemplate.Tracer.TraceDebug<ADRecipient>(0L, "WelcomeToGroupMessageTemplate.GetADThumbnailPhoto: Found thumbnail photo for {0}", adUser);
				result = new ImageAttachment(imageName, imageId, "image/jpeg", adUser.ThumbnailPhoto);
			}
			else
			{
				WelcomeToGroupMessageTemplate.Tracer.TraceDebug<ADRecipient>(0L, "WelcomeToGroupMessageTemplate.GetADThumbnailPhoto: No thumbnail photo found for {0}.", adUser);
			}
			return result;
		}

		private static string GetGroupDescription(ADUser groupMailbox)
		{
			string text = string.Empty;
			if (groupMailbox.Description != null && groupMailbox.Description.Count > 0)
			{
				string input = groupMailbox.Description[0];
				text = AntiXssEncoder.HtmlEncode(input, false);
				WelcomeToGroupMessageTemplate.Tracer.TraceDebug<string, string>(0L, "WelcomeToGroupMessageTemplate.GetGroupDescription: Found description for Group: {0}. Description {1}.", groupMailbox.ExternalDirectoryObjectId, text);
			}
			else
			{
				WelcomeToGroupMessageTemplate.Tracer.TraceDebug<string>(0L, "WelcomeToGroupMessageTemplate.GetGroupDescription: Couldn't find description for Group: {0}.", text);
			}
			return text;
		}

		private static string GetCalendarUrlForGroupMailbox(string calendarUrl)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(4);
			dictionary["src"] = "Mail";
			dictionary["to"] = "cal";
			dictionary["type"] = "MG";
			dictionary["exsvurl"] = "1";
			string text = WelcomeToGroupMessageTemplate.AddQueryParams(calendarUrl, dictionary);
			WelcomeToGroupMessageTemplate.Tracer.TraceDebug<string, string>(0L, "WelcomeToGroupMessageTemplate.GetCalendarUrlForGroupMailbox: Base URL: '{0}'. Calculated URL: '{1}'.", calendarUrl, text);
			return text;
		}

		private static string GetInboxUrlForGroupMailbox(string inboxUrl)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(4);
			dictionary["src"] = "Mail";
			dictionary["to"] = "conv";
			dictionary["type"] = "MG";
			dictionary["exsvurl"] = "1";
			string text = WelcomeToGroupMessageTemplate.AddQueryParams(inboxUrl, dictionary);
			WelcomeToGroupMessageTemplate.Tracer.TraceDebug<string, string>(0L, "WelcomeToGroupMessageTemplate.GetInboxUrlForGroupMailbox: Base URL: '{0}'. Calculated URL: '{1}'.", inboxUrl, text);
			return text;
		}

		private static string AddQueryParams(string originalUrl, Dictionary<string, string> queryParams)
		{
			Uri uri = new Uri(originalUrl);
			NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(uri.Query);
			foreach (KeyValuePair<string, string> keyValuePair in queryParams)
			{
				nameValueCollection[keyValuePair.Key] = keyValuePair.Value;
			}
			return new UriBuilder(originalUrl)
			{
				Query = nameValueCollection.ToString()
			}.Uri.AbsoluteUri;
		}

		private void Initialize()
		{
			MailboxUrls owaMailboxUrls = MailboxUrls.GetOwaMailboxUrls(this.groupPrincipal);
			EscalationLinkBuilder escalationLinkBuilder = new EscalationLinkBuilder(this.groupPrincipal, owaMailboxUrls);
			this.groupInboxUrl = WelcomeToGroupMessageTemplate.GetInboxUrlForGroupMailbox(owaMailboxUrls.InboxUrl);
			this.groupCalendarUrl = WelcomeToGroupMessageTemplate.GetCalendarUrlForGroupMailbox(owaMailboxUrls.CalendarUrl);
			this.subscribeUrl = escalationLinkBuilder.GetEscalationLink(EscalationLinkType.Subscribe);
			this.unsubscribeUrl = escalationLinkBuilder.GetEscalationLink(EscalationLinkType.Unsubscribe);
			this.groupSharePointUrl = (new SharePointUrlResolver(this.groupMailbox).GetDocumentsUrl() ?? string.Empty);
			this.groupPhoto = WelcomeToGroupMessageTemplate.GetADThumbnailPhoto(this.groupMailbox, WelcomeMessageBodyData.GroupPhotoImageId, WelcomeMessageBodyData.GroupPhotoImageId + ".jpg");
			this.executingUserPhoto = WelcomeToGroupMessageTemplate.GetADThumbnailPhoto(this.executingUser, WelcomeMessageBodyData.UserPhotoImageId, WelcomeMessageBodyData.UserPhotoImageId + ".jpg");
			if (this.executingUser != null)
			{
				WelcomeToGroupMessageTemplate.Tracer.TraceDebug((long)this.GetHashCode(), "WelcomeToGroupMessageTemplate.WelcomeToGroupMessageTemplate: Executing user is known. Setting Sender=Group, From-executingUser.");
				this.emailSender = new Participant(this.groupMailbox);
				this.emailFrom = new Participant(this.executingUser);
				this.encodedExecutingUserDisplayName = AntiXssEncoder.HtmlEncode(this.emailFrom.DisplayName, false);
				return;
			}
			WelcomeToGroupMessageTemplate.Tracer.TraceDebug((long)this.GetHashCode(), "WelcomeToGroupMessageTemplate.WelcomeToGroupMessageTemplate: Executing user is unknown. From-executingUser, sender won't be set.");
			this.emailFrom = new Participant(this.groupMailbox);
		}

		private const string SaveUrlOnLogoffParameter = "exsvurl";

		private const string SourceParameter = "src";

		private const string ToParameter = "to";

		private const string TypeParameter = "type";

		private const string Mail = "Mail";

		private const string SharePoint = "sp";

		private const string Calendar = "cal";

		private const string Conversations = "conv";

		private const string ModernGroup = "MG";

		private static readonly Trace Tracer = ExTraceGlobals.GroupEmailNotificationHandlerTracer;

		private readonly string encodedGroupDisplayName;

		private readonly string encodedGroupDescription;

		private readonly ADUser groupMailbox;

		private readonly ADRecipient executingUser;

		private string groupInboxUrl;

		private string groupCalendarUrl;

		private string groupSharePointUrl;

		private string subscribeUrl;

		private string unsubscribeUrl;

		private ImageAttachment groupPhoto;

		private ImageAttachment executingUserPhoto;

		private Participant emailFrom;

		private Participant emailSender;

		private string encodedExecutingUserDisplayName;

		private IExchangePrincipal groupPrincipal;
	}
}
