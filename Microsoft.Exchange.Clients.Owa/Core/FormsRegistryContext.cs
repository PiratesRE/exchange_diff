using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class FormsRegistryContext
	{
		public FormsRegistryContext(ApplicationElement applicationElement, string type, string state, string action)
		{
			this.ApplicationElement = applicationElement;
			this.Type = type;
			this.State = state;
			this.Action = action;
		}

		public ApplicationElement ApplicationElement;

		public string Type;

		public string State;

		public string Action;
	}
}
