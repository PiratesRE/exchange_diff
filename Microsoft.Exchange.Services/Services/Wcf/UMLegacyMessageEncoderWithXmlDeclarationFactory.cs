using System;
using System.ServiceModel.Channels;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class UMLegacyMessageEncoderWithXmlDeclarationFactory : MessageEncoderWithXmlDeclarationFactory
	{
		public UMLegacyMessageEncoderWithXmlDeclarationFactory(MessageEncoderWithXmlDeclarationBindingElement bindingElement) : base(bindingElement)
		{
		}

		public override MessageEncoder Encoder
		{
			get
			{
				if (this.encoder == null)
				{
					this.encoder = new UMLegacyMessageEncoderWithXmlDeclaration(this);
				}
				return this.encoder;
			}
		}
	}
}
