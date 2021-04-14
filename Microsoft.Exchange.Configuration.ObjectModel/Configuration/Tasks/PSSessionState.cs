using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public class PSSessionState : ISessionState
	{
		public PSSessionState(SessionState sessionState)
		{
			this.sessionState = sessionState;
			this.variables = new PSVariables(this.sessionState.PSVariable);
		}

		public string CurrentPath
		{
			get
			{
				return this.sessionState.Path.CurrentLocation.Path;
			}
		}

		public string CurrentPathProviderName
		{
			get
			{
				return this.sessionState.Path.CurrentLocation.Provider.Name;
			}
		}

		public IVariableDictionary Variables
		{
			get
			{
				return this.variables;
			}
		}

		private readonly SessionState sessionState;

		private readonly IVariableDictionary variables;
	}
}
