using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
	public class ConditionParameterName : Attribute
	{
		public ConditionParameterName(string name)
		{
			this.name = name;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private readonly string name;
	}
}
