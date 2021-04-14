using System;

namespace Microsoft.Exchange.Data.Directory
{
	public interface IADObject : IADRawEntry, IConfigurable, IPropertyBag, IReadOnlyPropertyBag
	{
		Guid Guid { get; }

		string Name { get; }
	}
}
