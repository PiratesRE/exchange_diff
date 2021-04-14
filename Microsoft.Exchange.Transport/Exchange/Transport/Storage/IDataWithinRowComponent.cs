using System;

namespace Microsoft.Exchange.Transport.Storage
{
	internal interface IDataWithinRowComponent : IDataObjectComponent
	{
		void LoadFromParentRow(DataTableCursor cursor);

		void SaveToParentRow(DataTableCursor cursor, Func<bool> checkpointCallback);

		void Cleanup();
	}
}
