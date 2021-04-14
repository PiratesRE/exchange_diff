using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace Microsoft.Exchange.Services.Wcf
{
	public class SecurityBindingElementExtension : BindingElementExtensionElement
	{
		public override Type BindingElementType
		{
			get
			{
				return typeof(SecurityBindingElement);
			}
		}

		protected override BindingElement CreateBindingElement()
		{
			return new SecurityBindingElement();
		}
	}
}
