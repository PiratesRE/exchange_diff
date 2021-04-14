using System;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public interface IColumnValueBag
	{
		object GetColumnValue(Context context, Column column);

		object GetOriginalColumnValue(Context context, Column column);

		bool IsColumnChanged(Context context, Column column);

		void SetInstanceNumber(Context context, object instanceNumber);
	}
}
