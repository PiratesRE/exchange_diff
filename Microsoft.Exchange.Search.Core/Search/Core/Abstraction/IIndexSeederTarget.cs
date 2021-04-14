using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IIndexSeederTarget : IDisposable
	{
		string GetSeedingEndPoint();
	}
}
