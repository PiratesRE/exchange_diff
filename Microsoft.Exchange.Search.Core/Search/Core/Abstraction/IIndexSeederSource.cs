using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IIndexSeederSource : IDisposable
	{
		string SeedToEndPoint(string seedingEndPoint, string reason);

		int GetProgress(string identifier);

		void Cancel(string identifier, string reason);
	}
}
