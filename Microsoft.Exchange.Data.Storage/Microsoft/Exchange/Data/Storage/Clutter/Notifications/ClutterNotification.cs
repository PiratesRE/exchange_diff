using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage.Clutter.Notifications
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ClutterNotification
	{
		protected ClutterNotification(MailboxSession session, VariantConfigurationSnapshot snapshot, IFrontEndLocator frontEndLocator)
		{
			ArgumentValidator.ThrowIfNull("session", session);
			ArgumentValidator.ThrowIfNull("snapshot", snapshot);
			if (frontEndLocator == null)
			{
				InferenceDiagnosticsLog.Log("ClutterNotification.ctor", "FrontEndLocator was not provided (it must be dependency injected). Using default OWA path.");
			}
			this.Session = session;
			this.Snapshot = snapshot;
			this.FrontEndLocator = frontEndLocator;
			this.Culture = ClutterNotification.GetPreferredCulture(this.Session);
		}

		private protected MailboxSession Session { protected get; private set; }

		private protected VariantConfigurationSnapshot Snapshot { protected get; private set; }

		private protected IFrontEndLocator FrontEndLocator { protected get; private set; }

		private protected CultureInfo Culture { protected get; private set; }

		public MessageItem Compose(DefaultFolderType folder)
		{
			bool flag = false;
			MessageItem messageItem = null;
			MessageItem result;
			try
			{
				messageItem = MessageItem.Create(this.Session, this.Session.GetDefaultFolderId(folder));
				Participant participant = new Participant(this.Session.MailboxOwner);
				messageItem.Recipients.Add(participant, RecipientItemType.To);
				messageItem.AutoResponseSuppress = AutoResponseSuppress.All;
				messageItem.From = this.GetFrom();
				messageItem.Subject = this.GetSubject().ToString(this.Culture);
				messageItem.IsDraft = false;
				messageItem.IsRead = false;
				messageItem.Importance = this.GetImportance();
				messageItem[MessageItemSchema.InferenceMessageIdentifier] = Guid.NewGuid();
				this.WriteMessageProperties(messageItem);
				using (Stream stream = messageItem.Body.OpenWriteStream(new BodyWriteConfiguration(BodyFormat.TextHtml, Charset.Unicode)))
				{
					using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.Unicode))
					{
						streamWriter.Write("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">");
						streamWriter.Write("<html>");
						streamWriter.Write("<head>");
						streamWriter.Write("<meta name='ProgId' content='Word.Document'>");
						streamWriter.Write("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=us-ascii\">");
						streamWriter.Write("<meta content=\"text/html; charset=US-ASCII\">");
						streamWriter.Write("</head>");
						streamWriter.Write("<body>");
						this.WriteMessageBody(streamWriter);
						streamWriter.Write("</body>");
						streamWriter.Write("</html>");
					}
				}
				messageItem.Save(SaveMode.NoConflictResolutionForceSave);
				messageItem.Load();
				flag = true;
				result = messageItem;
			}
			finally
			{
				if (!flag && messageItem != null)
				{
					messageItem.Dispose();
					messageItem = null;
				}
			}
			return result;
		}

		protected abstract LocalizedString GetSubject();

		protected virtual Importance GetImportance()
		{
			return Importance.Normal;
		}

		protected virtual void WriteMessageProperties(MessageItem message)
		{
		}

		protected abstract void WriteMessageBody(StreamWriter streamWriter);

		protected void WriteHeader(StreamWriter streamWriter, LocalizedString text)
		{
			streamWriter.Write("<div style='font-family: ");
			streamWriter.Write(ClientStrings.ClutterNotificationHeaderFont.ToString(this.Culture));
			streamWriter.Write("; font-size: 42px; color: #0072C6; line-height: normal; margin-top: 0; margin-bottom: 20px;'>");
			streamWriter.Write(text.ToString(this.Culture));
			streamWriter.Write("</div>");
		}

		protected void WriteSubHeader(StreamWriter streamWriter, LocalizedString text)
		{
			streamWriter.Write("<div style='font-family: ");
			streamWriter.Write(ClientStrings.ClutterNotificationBodyFont.ToString(this.Culture));
			streamWriter.Write("; font-size: 21px; color: #323232; line-height: normal; margin-top: 0; margin-bottom: 10px;'>");
			streamWriter.Write(text.ToString(this.Culture));
			streamWriter.Write("</div>");
		}

		protected void WriteParagraph(StreamWriter streamWriter, LocalizedString text)
		{
			this.WriteParagraph(streamWriter, text, 10U);
		}

		protected void WriteParagraph(StreamWriter streamWriter, LocalizedString text, uint marginBottom)
		{
			streamWriter.Write("<div style='font-family: ");
			streamWriter.Write(ClientStrings.ClutterNotificationBodyFont.ToString(this.Culture));
			streamWriter.Write("; font-size: 14px; color: #323232; line-height: 20px; margin-top: 0; margin-bottom: ");
			streamWriter.Write(marginBottom.ToString());
			streamWriter.Write("px;'>");
			streamWriter.Write(text.ToString(this.Culture));
			streamWriter.Write("</div>");
		}

		protected void WriteTurnOnInstructions(StreamWriter streamWriter)
		{
			this.WriteEnablementInstructions(streamWriter, true);
		}

		protected void WriteTurnOffInstructions(StreamWriter streamWriter)
		{
			this.WriteEnablementInstructions(streamWriter, false);
		}

		protected void WriteSurveyInstructions(StreamWriter streamWriter)
		{
			this.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationTakeSurveyDeepLink(this.GetOptionsDeepLink()));
		}

		protected void WriteSteps(StreamWriter streamWriter, params LocalizedString[] steps)
		{
			streamWriter.Write("<table style='font-family: ");
			streamWriter.Write(ClientStrings.ClutterNotificationBodyFont.ToString(this.Culture));
			streamWriter.Write("; font-size: 14px; margin-top: 0; margin-bottom: 10px; margin-left: 20px;' cellpadding='0' cellspacing='0' border='0'>");
			for (int i = 0; i < steps.Length; i++)
			{
				this.WriteStep(streamWriter, i + 1, steps[i], (i != steps.Length - 1) ? 0U : 10U);
			}
			streamWriter.Write("</table>");
		}

		protected Uri GetOwaUrl()
		{
			if (this.FrontEndLocator != null)
			{
				return this.FrontEndLocator.GetOwaUrl(this.Session.MailboxOwner);
			}
			return new Uri(ClutterNotification.Office365OwaUrl);
		}

		protected string GetOptionsDeepLink()
		{
			UriBuilder uriBuilder = new UriBuilder(this.GetOwaUrl());
			NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(uriBuilder.Query);
			nameValueCollection[ClutterNotification.OwaOptionsDeepLinkParam] = ClutterNotification.OwaClutterOptionsDeepLinkPath;
			if (!this.Snapshot.OwaClient.Options.Enabled)
			{
				nameValueCollection[ClutterNotification.OwaLayoutParam] = ClutterNotification.OwaLayoutMouseValue;
			}
			uriBuilder.Query = nameValueCollection.ToString();
			return uriBuilder.Uri.AbsoluteUri;
		}

		private void WriteEnablementInstructions(StreamWriter streamWriter, bool turnOn)
		{
			if (turnOn)
			{
				this.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationEnableDeepLink(this.GetOptionsDeepLink()));
				return;
			}
			this.WriteParagraph(streamWriter, ClientStrings.ClutterNotificationDisableDeepLink(this.GetOptionsDeepLink()));
		}

		private void WriteStep(StreamWriter streamWriter, int step, LocalizedString content, uint marginBottom)
		{
			streamWriter.Write("<tr><td style='font-family: ");
			streamWriter.Write(ClientStrings.ClutterNotificationBodyFont.ToString(this.Culture));
			streamWriter.Write("; font-size: 14px; color: #323232; line-height: 20px; margin-top: 0; margin-bottom: ");
			streamWriter.Write(marginBottom.ToString());
			streamWriter.Write("px; width: 35px; vertical-align: top;'>");
			streamWriter.Write(step.ToString(this.Culture));
			streamWriter.Write(".&nbsp;</td><td style='font-family: ");
			streamWriter.Write(ClientStrings.ClutterNotificationBodyFont.ToString(this.Culture));
			streamWriter.Write("; font-size: 14px; color: #323232; line-height: 20px; margin-top: 0; margin-bottom: ");
			streamWriter.Write(marginBottom.ToString());
			streamWriter.Write("px;'>");
			streamWriter.Write(content.ToString(this.Culture));
			streamWriter.Write("</td></tr>");
		}

		private Participant GetFrom()
		{
			return new Participant(ClientStrings.ClutterNotificationO365DisplayName.ToString(this.Culture), ClutterNotification.EmailFromSmtp, "SMTP");
		}

		private static CultureInfo GetPreferredCulture(MailboxSession session)
		{
			foreach (CultureInfo cultureInfo in session.MailboxOwner.PreferredCultures.Concat(new CultureInfo[]
			{
				session.InternalPreferedCulture
			}))
			{
				if (ClutterNotification.SupportedClientLanguages.Contains(cultureInfo))
				{
					return cultureInfo;
				}
			}
			if (ClutterNotification.SupportedClientLanguages.Contains(CultureInfo.CurrentCulture))
			{
				return CultureInfo.CurrentCulture;
			}
			InferenceDiagnosticsLog.Log("ClutterNotification.GetPreferredCulture", string.Format("No supported culture could be found for mailbox '{0}'. Falling back to default {1}.", session.MailboxGuid, ClutterNotification.ProductDefaultCulture.Name));
			return ClutterNotification.ProductDefaultCulture;
		}

		public static readonly string Office365OwaUrl = "http://outlook.office365.com/owa/";

		public static readonly string OwaOptionsDeepLinkParam = "path";

		public static readonly string AnnouncementUrl = "http://blogs.office.com/2014/03/31/the-evolution-of-email/";

		public static readonly string OwaClutterOptionsDeepLinkPath = "/options/clutter";

		public static readonly string OwaLayoutParam = "layout";

		public static readonly string OwaLayoutMouseValue = "mouse";

		public static readonly string LearnMoreUrl = "http://go.microsoft.com/fwlink/?LinkId=506974";

		public static readonly string FeedbackMailtoUrl = "mailto:ExClutterFeedback@microsoft.com?subject=Clutter%20feedback";

		private static readonly string EmailFromSmtp = "no-reply@office365.com";

		private static readonly CultureInfo ProductDefaultCulture = new CultureInfo("en-US");

		private static readonly HashSet<CultureInfo> SupportedClientLanguages = new HashSet<CultureInfo>(LanguagePackInfo.GetInstalledLanguagePackSpecificCultures(LanguagePackType.Client));
	}
}
