using System;
using System.ServiceModel.Channels;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class MessageEncoderWithXmlDeclarationFactory : MessageEncoderFactory
	{
		public MessageEncoderWithXmlDeclarationFactory(MessageEncoderWithXmlDeclarationBindingElement bindingElement)
		{
			this.version = bindingElement.MessageVersion;
		}

		public override MessageEncoder Encoder
		{
			get
			{
				if (this.encoder == null)
				{
					this.encoder = new MessageEncoderWithXmlDeclaration(this);
				}
				return this.encoder;
			}
		}

		public override MessageVersion MessageVersion
		{
			get
			{
				return this.version;
			}
		}

		protected MessageEncoderWithXmlDeclaration encoder;

		private MessageVersion version;
	}
}
