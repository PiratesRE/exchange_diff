using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IQueryResult : IDisposable
	{
		object[][] GetRows(int rowCount, out bool mightBeMoreRows);

		object[][] GetRows(int rowCount, QueryRowsFlags flags, out bool mightBeMoreRows);

		void SetTableColumns(ICollection<PropertyDefinition> propertyDefinitions);

		int SeekToOffset(SeekReference reference, int offset);

		bool SeekToCondition(SeekReference reference, QueryFilter seekFilter, SeekToConditionFlags flags);

		bool SeekToCondition(SeekReference reference, QueryFilter seekFilter);

		bool SeekToCondition(uint bookMark, bool useForwardDirection, QueryFilter seekFilter, SeekToConditionFlags flags);

		object[][] ExpandRow(int rowCount, long categoryId, out int rowsInExpandedCategory);

		int CollapseRow(long categoryId);

		uint CreateBookmark();

		void FreeBookmark(uint bookmarkPosition);

		int SeekRowBookmark(uint bookmarkPosition, int rowCount, bool wantRowsSought, out bool soughtLess, out bool positionChanged);

		NativeStorePropertyDefinition[] GetAllPropertyDefinitions(params PropertyTagPropertyDefinition[] excludeProperties);

		byte[] GetCollapseState(byte[] instanceKey);

		uint SetCollapseState(byte[] collapseState);

		object Advise(SubscriptionSink subscriptionSink, bool asyncMode);

		void Unadvise(object notificationHandle);

		IStorePropertyBag[] GetPropertyBags(int rowCount);

		StoreSession StoreSession { get; }

		ColumnPropertyDefinitions Columns { get; }

		int CurrentRow { get; }

		int EstimatedRowCount { get; }

		bool IsDisposed { get; }
	}
}
