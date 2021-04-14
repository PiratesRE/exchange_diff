using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	public class InfobarMessage
	{
		public static InfobarMessage CreateLocalized(Strings.IDs stringId, InfobarMessageType type)
		{
			return new InfobarMessage(SanitizedHtmlString.FromStringId(stringId), type);
		}

		public static InfobarMessage CreateLocalized(Strings.IDs stringId, InfobarMessageType type, string id)
		{
			return new InfobarMessage(SanitizedHtmlString.FromStringId(stringId), type, id);
		}

		public static InfobarMessage CreateText(string messageText, InfobarMessageType type)
		{
			return new InfobarMessage(new SanitizedHtmlString(messageText), type);
		}

		public static InfobarMessage CreateText(string messageText, InfobarMessageType type, string id)
		{
			return new InfobarMessage(new SanitizedHtmlString(messageText), type, id);
		}

		public static InfobarMessage CreateHtml(SanitizedHtmlString messageHtml, InfobarMessageType type)
		{
			if (messageHtml == null)
			{
				throw new ArgumentNullException("messageHtml");
			}
			return new InfobarMessage(messageHtml, type);
		}

		public static InfobarMessage CreateErrorMessageFromException(Exception e, UserContext userContext)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			ErrorInformation exceptionHandlingInformation = Utilities.GetExceptionHandlingInformation(e, userContext.MailboxIdentity);
			return InfobarMessage.CreateText(exceptionHandlingInformation.Message, InfobarMessageType.Error);
		}

		public static InfobarMessage CreatePromptHtml(SanitizedHtmlString messageHtml, SanitizedHtmlString bodyHtml, SanitizedHtmlString footerHtml)
		{
			return new InfobarMessage(messageHtml, bodyHtml, footerHtml);
		}

		public static InfobarMessage CreateExpandingHtml(SanitizedHtmlString messageHtml, SanitizedHtmlString expandSectionHtml, bool isExpanding)
		{
			return new InfobarMessage(messageHtml, InfobarMessageType.Expanding, expandSectionHtml, isExpanding);
		}

		private InfobarMessage(SanitizedHtmlString message, InfobarMessageType type)
		{
			this.message = message;
			this.type = type;
		}

		private InfobarMessage(SanitizedHtmlString message, InfobarMessageType type, string id)
		{
			this.message = message;
			this.type = type;
			this.tagId = id;
		}

		private InfobarMessage(SanitizedHtmlString messageHtml, InfobarMessageType type, SanitizedHtmlString expandSectionHtml, bool isExpanding)
		{
			this.message = messageHtml;
			this.expandSectionHtml = expandSectionHtml;
			this.type = type;
			this.isExpanding = isExpanding;
		}

		private InfobarMessage(SanitizedHtmlString messageHtml, SanitizedHtmlString bodyHtml, SanitizedHtmlString footerHtml)
		{
			this.message = messageHtml;
			this.type = InfobarMessageType.Prompt;
			this.bodyHtml = bodyHtml;
			this.footerHtml = footerHtml;
		}

		public static void PutExceptionInfoIntoContextInfobarMessage(Exception e, OwaContext owaContext)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			owaContext[OwaContextProperty.InfobarMessage] = InfobarMessage.CreateErrorMessageFromException(e, owaContext.UserContext);
		}

		public SanitizedHtmlString BodyHtml
		{
			get
			{
				return this.bodyHtml;
			}
		}

		public SanitizedHtmlString FooterHtml
		{
			get
			{
				return this.footerHtml;
			}
		}

		public InfobarMessageType Type
		{
			get
			{
				return this.type;
			}
		}

		public string TagId
		{
			get
			{
				return this.tagId;
			}
		}

		public SanitizedHtmlString ExpandSectionHtml
		{
			get
			{
				return this.expandSectionHtml;
			}
		}

		public bool IsExpanding
		{
			get
			{
				return this.isExpanding;
			}
		}

		public bool IsActionResult
		{
			get
			{
				return this.isActionResult;
			}
			set
			{
				this.isActionResult = value;
			}
		}

		public void RenderMessageString(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write(this.message);
		}

		private SanitizedHtmlString message;

		private SanitizedHtmlString bodyHtml;

		private SanitizedHtmlString footerHtml;

		private InfobarMessageType type = InfobarMessageType.Informational;

		private SanitizedHtmlString expandSectionHtml;

		private bool isExpanding;

		private string tagId;

		private bool isActionResult;
	}
}
