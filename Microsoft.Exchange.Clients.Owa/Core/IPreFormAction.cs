using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal interface IPreFormAction
	{
		PreFormActionResponse Execute(OwaContext owaContext, out ApplicationElement applicationElement, out string type, out string state, out string action);
	}
}
