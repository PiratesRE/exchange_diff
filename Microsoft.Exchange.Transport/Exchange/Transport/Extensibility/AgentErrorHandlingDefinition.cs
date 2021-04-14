using System;

namespace Microsoft.Exchange.Transport.Extensibility
{
	internal class AgentErrorHandlingDefinition
	{
		public AgentErrorHandlingDefinition(string name, AgentErrorHandlingCondition condition, IErrorHandlingAction action)
		{
			this.Name = name;
			this.Condition = condition;
			this.Action = action;
		}

		public string Name { get; private set; }

		public AgentErrorHandlingCondition Condition { get; private set; }

		public IErrorHandlingAction Action { get; private set; }
	}
}
