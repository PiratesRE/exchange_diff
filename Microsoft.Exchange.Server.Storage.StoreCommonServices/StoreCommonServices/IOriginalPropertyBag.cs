using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface IOriginalPropertyBag : ISimpleReadOnlyPropertyBag, ISimpleReadOnlyPropertyStorage, ITWIR
	{
	}
}
