using System;

namespace Microsoft.Exchange.Data.Directory
{
	public interface IADRawEntry : IConfigurable, IPropertyBag, IReadOnlyPropertyBag
	{
		ADObjectId Id { get; }
	}
}
