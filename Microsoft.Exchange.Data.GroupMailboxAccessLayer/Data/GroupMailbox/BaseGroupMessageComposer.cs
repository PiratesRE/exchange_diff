using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class BaseGroupMessageComposer : IMessageComposer
	{
		protected abstract ADUser[] Recipients { get; }

		protected abstract Participant FromParticipant { get; }

		protected abstract string Subject { get; }

		public virtual void WriteToMessage(MessageItem message)
		{
			ArgumentValidator.ThrowIfNull("message", message);
			foreach (ADUser aduser in this.Recipients)
			{
				BaseGroupMessageComposer.Tracer.TraceDebug<string, string, SmtpAddress>((long)this.GetHashCode(), "BaseGroupMessageComposer.WriteMessage: Adding recipient with ExternalId:{0}. Legacy DN:{1}, PrimarySmtpAddress: {2}", aduser.ExternalDirectoryObjectId, aduser.LegacyExchangeDN, aduser.PrimarySmtpAddress);
				Participant participant = new Participant(aduser.DisplayName, aduser.PrimarySmtpAddress.ToString(), "SMTP");
				message.Recipients.Add(participant, RecipientItemType.To);
			}
			message.AutoResponseSuppress = AutoResponseSuppress.All;
			message.From = this.FromParticipant;
			message.Subject = this.Subject;
			this.SetAdditionalMessageProperties(message);
			using (Stream stream = message.Body.OpenWriteStream(new BodyWriteConfiguration(BodyFormat.TextHtml, Charset.Unicode)))
			{
				using (StreamWriter streamWriter = new StreamWriter(stream, Encoding.Unicode))
				{
					this.WriteMessageBody(streamWriter);
				}
			}
			this.AddAttachments(message);
		}

		protected static CultureInfo GetPreferredCulture(params ADUser[] users)
		{
			foreach (ADUser aduser in users)
			{
				CultureInfo cultureInfo = aduser.Languages.FirstOrDefault((CultureInfo language) => BaseGroupMessageComposer.SupportedClientLanguages.Contains(language));
				if (cultureInfo != null)
				{
					BaseGroupMessageComposer.Tracer.TraceDebug<CultureInfo, string>(0L, "BaseGroupMessageComposer.GetPreferredCulture: language {0} is supported by {1}.", cultureInfo, aduser.Guid.ToString());
					return cultureInfo;
				}
			}
			if (BaseGroupMessageComposer.SupportedClientLanguages.Contains(CultureInfo.CurrentCulture))
			{
				BaseGroupMessageComposer.Tracer.TraceDebug<CultureInfo>(0L, "BaseGroupMessageComposer.GetPreferredCulture: no language of the provided users is supported - returning current culture: {0}.", CultureInfo.CurrentCulture);
				return CultureInfo.CurrentCulture;
			}
			BaseGroupMessageComposer.Tracer.TraceDebug<CultureInfo>(0L, "BaseGroupMessageComposer.GetPreferredCulture: Couldn't find a supported language, returning default culture: {0}.", BaseGroupMessageComposer.ProductDefaultCulture);
			return BaseGroupMessageComposer.ProductDefaultCulture;
		}

		protected abstract void WriteMessageBody(StreamWriter streamWriter);

		protected abstract void SetAdditionalMessageProperties(MessageItem message);

		protected virtual void AddAttachments(MessageItem message)
		{
			ArgumentValidator.ThrowIfNull("message", message);
			WelcomeMessageBodyBuilder.CalendarIcon.AddImageAsAttachment(message);
			WelcomeMessageBodyBuilder.ConversationIcon.AddImageAsAttachment(message);
			WelcomeMessageBodyBuilder.DocumentIcon.AddImageAsAttachment(message);
		}

		protected static readonly Trace Tracer = ExTraceGlobals.GroupEmailNotificationHandlerTracer;

		private static readonly CultureInfo ProductDefaultCulture = new CultureInfo("en-US");

		private static readonly HashSet<CultureInfo> SupportedClientLanguages = new HashSet<CultureInfo>(LanguagePackInfo.GetInstalledLanguagePackSpecificCultures(LanguagePackType.Client));
	}
}
