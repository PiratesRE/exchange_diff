using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public class InfobarMessage
	{
		internal InfobarMessage(SanitizedHtmlString messageHtml, InfobarMessageType type)
		{
			this.message = messageHtml;
			this.type = type;
		}

		internal InfobarMessage(SanitizedHtmlString messageHtml, InfobarMessageType type, string id)
		{
			this.message = messageHtml;
			this.type = type;
			this.tagId = id;
		}

		internal InfobarMessage(SanitizedHtmlString messageHtml, InfobarMessageType type, string id, bool hideMessage)
		{
			this.message = messageHtml;
			this.type = type;
			this.tagId = id;
			this.hideMessage = hideMessage;
		}

		internal InfobarMessage(SanitizedHtmlString messageHtml, InfobarMessageType type, string id, SanitizedHtmlString linkText, SanitizedHtmlString expandSection)
		{
			this.message = messageHtml;
			this.linkText = linkText;
			this.tagId = id;
			this.expandSection = expandSection;
			this.type = type;
		}

		internal SanitizedHtmlString Message
		{
			get
			{
				return this.message;
			}
		}

		internal InfobarMessageType Type
		{
			get
			{
				return this.type;
			}
		}

		internal string TagId
		{
			get
			{
				return this.tagId;
			}
		}

		internal SanitizedHtmlString LinkText
		{
			get
			{
				return this.linkText;
			}
		}

		internal SanitizedHtmlString ExpandSection
		{
			get
			{
				return this.expandSection;
			}
		}

		internal bool HideMessage
		{
			get
			{
				return this.hideMessage;
			}
		}

		private SanitizedHtmlString message;

		private SanitizedHtmlString linkText;

		private SanitizedHtmlString expandSection;

		private InfobarMessageType type = InfobarMessageType.Informational;

		private string tagId;

		private bool hideMessage;
	}
}
