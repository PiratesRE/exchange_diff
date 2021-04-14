using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	public class LegacyMessageEncoderBindingElementExtension : BindingElementExtensionElement
	{
		public override Type BindingElementType
		{
			get
			{
				return typeof(LegacyMessageEncoderBindingElement);
			}
		}

		protected override BindingElement CreateBindingElement()
		{
			return new LegacyMessageEncoderBindingElement();
		}
	}
}
