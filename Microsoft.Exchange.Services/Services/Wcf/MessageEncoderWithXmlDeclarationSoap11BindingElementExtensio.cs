using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace Microsoft.Exchange.Services.Wcf
{
	public class MessageEncoderWithXmlDeclarationSoap11BindingElementExtension : BindingElementExtensionElement
	{
		public override Type BindingElementType
		{
			get
			{
				return typeof(MessageEncoderWithXmlDeclarationBindingElement);
			}
		}

		protected override BindingElement CreateBindingElement()
		{
			return new MessageEncoderWithXmlDeclarationBindingElement(MessageVersion.Soap11);
		}
	}
}
