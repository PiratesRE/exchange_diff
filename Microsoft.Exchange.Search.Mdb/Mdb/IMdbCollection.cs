using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Search.Mdb
{
	internal interface IMdbCollection
	{
		IEnumerable<MdbInfo> Databases { get; }

		void UpdateDatabasesIndexStatusInfo(int numberOfCopiesToIndexPerDatabase);

		void UpdateDatabasesCopyStatusInfo();
	}
}
