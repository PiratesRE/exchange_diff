using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class ItemDisabledPreFormAction : IPreFormAction
	{
		public PreFormActionResponse Execute(OwaContext owaContext, out ApplicationElement applicationElement, out string type, out string state, out string action)
		{
			applicationElement = ApplicationElement.Item;
			type = "Disabled";
			action = owaContext.FormsRegistryContext.Action;
			state = string.Empty;
			return null;
		}
	}
}
