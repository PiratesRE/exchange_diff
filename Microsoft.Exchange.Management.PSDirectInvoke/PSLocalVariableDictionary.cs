using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.PSDirectInvoke
{
	internal class PSLocalVariableDictionary : IVariableDictionary
	{
		public object this[string name]
		{
			get
			{
				object result;
				this.variables.TryGetValue(name, out result);
				return result;
			}
			set
			{
				this.variables[name] = value;
			}
		}

		public bool ContainsName(string name)
		{
			return this.variables.ContainsKey(name);
		}

		public void Remove(string name)
		{
			this.variables.Remove(name);
		}

		public void Set(string name, object value, VariableScopedOptions scope)
		{
			this.variables[name] = value;
		}

		public bool TryGetValue(string name, out object value)
		{
			return this.variables.TryGetValue(name, out value);
		}

		private readonly Dictionary<string, object> variables = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
	}
}
