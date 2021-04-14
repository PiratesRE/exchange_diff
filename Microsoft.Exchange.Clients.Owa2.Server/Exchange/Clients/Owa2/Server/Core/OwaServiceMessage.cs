using System;
using System.ServiceModel.Channels;
using System.Web;
using System.Xml;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class OwaServiceMessage : Message
	{
		public OwaServiceMessage(HttpRequest httpRequest, object request)
		{
			this.HttpRequest = httpRequest;
			this.Request = request;
			this.messageProperties = new MessageProperties();
			this.messageHeaders = new MessageHeaders(MessageVersion.None);
		}

		public HttpRequest HttpRequest { get; private set; }

		public object Request { get; private set; }

		public override MessageHeaders Headers
		{
			get
			{
				return this.messageHeaders;
			}
		}

		protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
		{
		}

		public override MessageProperties Properties
		{
			get
			{
				return this.messageProperties;
			}
		}

		public override MessageVersion Version
		{
			get
			{
				return MessageVersion.None;
			}
		}

		private MessageProperties messageProperties;

		private MessageHeaders messageHeaders;
	}
}
