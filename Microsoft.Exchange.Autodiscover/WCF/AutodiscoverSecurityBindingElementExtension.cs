using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	public class AutodiscoverSecurityBindingElementExtension : BindingElementExtensionElement
	{
		public override Type BindingElementType
		{
			get
			{
				return typeof(AutodiscoverSecurityBindingElement);
			}
		}

		protected override BindingElement CreateBindingElement()
		{
			return new AutodiscoverSecurityBindingElement();
		}
	}
}
