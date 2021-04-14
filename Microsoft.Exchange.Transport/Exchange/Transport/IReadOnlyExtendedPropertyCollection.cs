using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Microsoft.Exchange.Transport
{
	internal interface IReadOnlyExtendedPropertyCollection
	{
		int Count { get; }

		bool Contains(string name);

		T GetValue<T>(string name, T defaultValue);

		void Serialize(Stream stream);

		bool TryGetListValue<ItemT>(string name, out ReadOnlyCollection<ItemT> value);

		bool TryGetValue<T>(string name, out T value);
	}
}
