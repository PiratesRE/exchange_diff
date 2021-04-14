using System;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccessJet
{
	internal interface IJetRecordCounter
	{
		int GetCount();

		int GetOrdinalPosition(SortOrder sortOrder, StartStopKey stopKey, CompareInfo compareInfo);
	}
}
