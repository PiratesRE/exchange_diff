using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public class PSVariables : IVariableDictionary
	{
		public PSVariables(PSVariableIntrinsics psVariables)
		{
			this.variables = psVariables;
		}

		public object this[string name]
		{
			get
			{
				return this.variables.GetValue(name);
			}
			set
			{
				this.variables.Set(name, value);
			}
		}

		public void Set(string name, object value, VariableScopedOptions scope)
		{
			this.variables.Set(new PSVariable(name, value, (ScopedItemOptions)scope));
		}

		public bool ContainsName(string name)
		{
			return this.variables.Get(name) != null;
		}

		public bool TryGetValue(string name, out object value)
		{
			value = null;
			PSVariable psvariable = this.variables.Get(name);
			if (psvariable != null)
			{
				value = psvariable.Value;
			}
			return psvariable != null;
		}

		public void Remove(string name)
		{
			this.variables.Remove(name);
		}

		private readonly PSVariableIntrinsics variables;
	}
}
