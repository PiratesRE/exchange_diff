using System;
using System.ServiceModel.Channels;

namespace Microsoft.Exchange.Services.Wcf
{
	public class UMLegacyMessageEncoderWithXmlDeclarationBindingElement : MessageEncoderWithXmlDeclarationBindingElement
	{
		public UMLegacyMessageEncoderWithXmlDeclarationBindingElement(MessageVersion version) : base(version)
		{
		}

		public UMLegacyMessageEncoderWithXmlDeclarationBindingElement(MessageEncoderWithXmlDeclarationBindingElement other) : base(other)
		{
		}

		public override BindingElement Clone()
		{
			return new UMLegacyMessageEncoderWithXmlDeclarationBindingElement(this);
		}

		public override MessageEncoderFactory CreateMessageEncoderFactory()
		{
			return new UMLegacyMessageEncoderWithXmlDeclarationFactory(this);
		}
	}
}
