using System;
using System.IO;

namespace Microsoft.Exchange.Transport
{
	internal interface IExtendedPropertyCollection : IReadOnlyExtendedPropertyCollection
	{
		void Deserialize(Stream stream, int numberOfExtendedPropertiesToFetch, bool doNotAddPropertyIfPresent);

		bool Remove(string name);

		void SetValue<T>(string name, T value);

		void Clear();
	}
}
