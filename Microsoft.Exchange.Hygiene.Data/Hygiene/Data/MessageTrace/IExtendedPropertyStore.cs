using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal interface IExtendedPropertyStore<T> where T : PropertyBase
	{
		int ExtendedPropertiesCount { get; }

		bool TryGetExtendedProperty(string nameSpace, string name, out T extendedProperty);

		T GetExtendedProperty(string nameSpace, string name);

		IEnumerable<T> GetExtendedPropertiesEnumerable();

		void AddExtendedProperty(T extendedProperty);
	}
}
