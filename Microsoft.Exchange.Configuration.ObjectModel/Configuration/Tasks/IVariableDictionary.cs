using System;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public interface IVariableDictionary
	{
		object this[string name]
		{
			get;
			set;
		}

		void Set(string name, object value, VariableScopedOptions scope);

		bool ContainsName(string name);

		bool TryGetValue(string name, out object value);

		void Remove(string name);
	}
}
