using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace Microsoft.Exchange.Services.Wcf
{
	public class UMLegacyMessageEncoderWithXmlDeclarationSoap11BindingElementExtension : BindingElementExtensionElement
	{
		public override Type BindingElementType
		{
			get
			{
				return typeof(UMLegacyMessageEncoderWithXmlDeclarationBindingElement);
			}
		}

		protected override BindingElement CreateBindingElement()
		{
			return new UMLegacyMessageEncoderWithXmlDeclarationBindingElement(MessageVersion.Soap11);
		}
	}
}
