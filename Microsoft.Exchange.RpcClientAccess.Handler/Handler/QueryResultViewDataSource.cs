using System;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	internal sealed class QueryResultViewDataSource : BaseObject, IViewDataSource, IDisposeTrackable, IDisposable
	{
		internal QueryResultViewDataSource(StoreSession session, PropertyTag[] propertyTags, IQueryResult queryResult, bool useUnicodeForRestrictions)
		{
			this.session = session;
			this.propertyTags = propertyTags;
			this.queryResult = queryResult;
			this.useUnicodeForRestrictions = useUnicodeForRestrictions;
		}

		public void SetColumns(NativeStorePropertyDefinition[] propertyDefinitions, PropertyTag[] propertyTags)
		{
			this.propertyTags = propertyTags;
			this.queryResult.SetTableColumns(propertyDefinitions);
		}

		public int GetPosition()
		{
			return this.queryResult.CurrentRow;
		}

		public int GetRowCount()
		{
			return this.queryResult.EstimatedRowCount;
		}

		public PropertyValue[][] GetRows(int rowCount, QueryRowsFlags flags)
		{
			bool flag;
			object[][] xsoRows = ((byte)(flags & QueryRowsFlags.DoNotAdvance) == 1) ? this.queryResult.GetRows(rowCount, QueryRowsFlags.NoAdvance, out flag) : this.queryResult.GetRows(rowCount, QueryRowsFlags.None, out flag);
			return this.ConvertFromXsoRows(xsoRows);
		}

		public int SeekRow(BookmarkOrigin bookmarkOrigin, int rowCount)
		{
			SeekReference reference;
			QueryResultViewDataSource.BookmarkAndDirectionToSeekReference((uint)bookmarkOrigin, rowCount >= 0, out reference);
			return this.queryResult.SeekToOffset(reference, rowCount);
		}

		public bool FindRow(Restriction restriction, byte[] bookmark, bool useForwardDirection)
		{
			Util.ThrowOnNullArgument(bookmark, "bookmark");
			uint bookmark2 = QueryResultViewDataSource.BookmarkToBookmarkPosition(bookmark);
			return this.FindRow(restriction, bookmark2, useForwardDirection);
		}

		public bool FindRow(Restriction restriction, uint bookmark, bool useForwardDirection)
		{
			FilterRestrictionTranslator filterRestrictionTranslator = new FilterRestrictionTranslator(this.session);
			QueryFilter seekFilter = filterRestrictionTranslator.Translate(restriction);
			if ((bookmark & 4U) == 4U)
			{
				throw new ArgumentOutOfRangeException("bookmark");
			}
			return this.queryResult.SeekToCondition(bookmark, useForwardDirection, seekFilter, SeekToConditionFlags.AllowExtendedFilters | SeekToConditionFlags.AllowExtendedSeekReferences | SeekToConditionFlags.KeepCursorPositionWhenNoMatch);
		}

		public PropertyValue[][] ExpandRow(int maxRows, StoreId categoryId, out int rowsInExpandedCategory)
		{
			object[][] xsoRows = this.queryResult.ExpandRow(maxRows, categoryId, out rowsInExpandedCategory);
			return this.ConvertFromXsoRows(xsoRows);
		}

		public int CollapseRow(StoreId categoryId)
		{
			return this.queryResult.CollapseRow(categoryId);
		}

		public byte[] CreateBookmark()
		{
			uint bookmarkPosition = this.queryResult.CreateBookmark();
			return QueryResultViewDataSource.BookmarkPositionToBookmark(bookmarkPosition);
		}

		public void FreeBookmark(byte[] bookmark)
		{
			Util.ThrowOnNullArgument(bookmark, "bookmark");
			uint bookmarkPosition = QueryResultViewDataSource.BookmarkToBookmarkPosition(bookmark);
			this.queryResult.FreeBookmark(bookmarkPosition);
		}

		public int SeekRowBookmark(byte[] bookmark, int rowCount, bool wantMoveCount, out bool soughtLess, out bool positionChanged)
		{
			Util.ThrowOnNullArgument(bookmark, "bookmark");
			uint bookmarkPosition = QueryResultViewDataSource.BookmarkToBookmarkPosition(bookmark);
			return this.queryResult.SeekRowBookmark(bookmarkPosition, rowCount, wantMoveCount, out soughtLess, out positionChanged);
		}

		public PropertyTag[] QueryColumnsAll()
		{
			bool useUnicodeType = true;
			return MEDSPropertyTranslator.PropertyTagsFromPropertyDefinitions<NativeStorePropertyDefinition>(this.session, this.QueryResult.GetAllPropertyDefinitions(new PropertyTagPropertyDefinition[]
			{
				CoreItemSchema.XMsExchOrganizationAVStampMailbox
			}), useUnicodeType).ToArray<PropertyTag>();
		}

		public byte[] GetCollapseState(byte[] instanceKey)
		{
			return this.queryResult.GetCollapseState(instanceKey);
		}

		public byte[] SetCollapseState(byte[] collapseState)
		{
			uint bookmarkPosition = this.queryResult.SetCollapseState(collapseState);
			return QueryResultViewDataSource.BookmarkPositionToBookmark(bookmarkPosition);
		}

		public IQueryResult QueryResult
		{
			get
			{
				return this.queryResult;
			}
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<QueryResultViewDataSource>(this);
		}

		protected override void InternalDispose()
		{
			this.queryResult.Dispose();
			base.InternalDispose();
		}

		private static void BookmarkAndDirectionToSeekReference(uint bookmark, bool useForwardDirection, out SeekReference seekReference)
		{
			seekReference = (SeekReference)bookmark;
			if ((seekReference & SeekReference.SeekBackward) == SeekReference.SeekBackward)
			{
				throw new ArgumentOutOfRangeException("bookmark");
			}
			if (!useForwardDirection)
			{
				seekReference |= SeekReference.SeekBackward;
			}
		}

		private static uint BookmarkToBookmarkPosition(byte[] bookmark)
		{
			if (bookmark.Length != 4)
			{
				throw new RopExecutionException(string.Format("Invalid bookmark {0}.", new ArrayTracer<byte>(bookmark)), (ErrorCode)2147746821U);
			}
			uint num = BitConverter.ToUInt32(bookmark, 0);
			if ((num & 7U) != 0U)
			{
				throw new RopExecutionException(string.Format("Invalid bookmark {0}.", new ArrayTracer<byte>(bookmark)), (ErrorCode)2147746821U);
			}
			return num;
		}

		private static byte[] BookmarkPositionToBookmark(uint bookmarkPosition)
		{
			return BitConverter.GetBytes(bookmarkPosition);
		}

		private PropertyValue[][] ConvertFromXsoRows(object[][] xsoRows)
		{
			PropertyValue[][] array = new PropertyValue[xsoRows.Length][];
			for (int i = 0; i < xsoRows.Length; i++)
			{
				array[i] = MEDSPropertyTranslator.TranslatePropertyValues(this.session, this.propertyTags, xsoRows[i], this.useUnicodeForRestrictions);
			}
			return array;
		}

		private readonly StoreSession session;

		private readonly IQueryResult queryResult;

		private readonly bool useUnicodeForRestrictions;

		private PropertyTag[] propertyTags;
	}
}
