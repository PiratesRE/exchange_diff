using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal interface IViewDataSource : IDisposable
	{
		int GetPosition();

		int GetRowCount();

		void SetColumns(NativeStorePropertyDefinition[] propertyDefinitions, PropertyTag[] propertyTags);

		PropertyValue[][] GetRows(int rowCount, QueryRowsFlags flags);

		int SeekRow(BookmarkOrigin bookmarkOrigin, int rowCount);

		bool FindRow(Restriction restriction, uint bookmark, bool useForwardDirection);

		bool FindRow(Restriction restriction, byte[] bookmark, bool useForwardDirection);

		PropertyValue[][] ExpandRow(int maxRows, StoreId categoryId, out int rowsInExpandedCategory);

		int CollapseRow(StoreId categoryId);

		byte[] CreateBookmark();

		void FreeBookmark(byte[] bookmark);

		int SeekRowBookmark(byte[] bookmark, int rowCount, bool wantMoveCount, out bool soughtLess, out bool positionChanged);

		PropertyTag[] QueryColumnsAll();

		byte[] GetCollapseState(byte[] instanceKey);

		byte[] SetCollapseState(byte[] collapseState);

		IQueryResult QueryResult { get; }
	}
}
