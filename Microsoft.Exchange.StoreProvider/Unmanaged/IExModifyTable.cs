using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IExModifyTable : IExInterface, IDisposeTrackable, IDisposable
	{
		int GetTable(int ulFlags, out IExMapiTable iMAPITable);

		int ModifyTable(int ulFlags, ICollection<RowEntry> lpRowList);
	}
}
