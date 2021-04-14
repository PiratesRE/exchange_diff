using System;
using System.ServiceModel.Configuration;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class OWAWebHttpBehaviorExtensionElement : BehaviorExtensionElement
	{
		public override Type BehaviorType
		{
			get
			{
				return typeof(OWAWebHttpBehavior);
			}
		}

		protected override object CreateBehavior()
		{
			return new OWAWebHttpBehavior();
		}
	}
}
