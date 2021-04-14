using System;

namespace Microsoft.Exchange.Search.Mdb
{
	internal interface IMdbWatcher : IDisposable
	{
		event EventHandler Changed;

		IMdbCollection GetDatabases();

		bool Exists(Guid mdbGuid);
	}
}
