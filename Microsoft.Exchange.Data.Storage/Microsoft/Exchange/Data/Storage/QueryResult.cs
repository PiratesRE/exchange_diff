using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class QueryResult : ITableView, IPagedView, IQueryResult, IDisposeTrackable, IDisposable, INotificationSource
	{
		internal QueryResult(MapiTable mapiTable, ICollection<PropertyDefinition> propertyDefinitions, IList<PropTag> alteredProperties, StoreSession session, bool isTableOwned) : this(mapiTable, propertyDefinitions, alteredProperties, session, isTableOwned, null)
		{
		}

		internal QueryResult(MapiTable mapiTable, ICollection<PropertyDefinition> propertyDefinitions, IList<PropTag> alteredProperties, StoreSession session, bool isTableOwned, SortOrder sortOrder)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				StorageGlobals.TraceConstructIDisposable(this);
				this.disposeTracker = this.GetDisposeTracker();
				this.mapiTable = mapiTable;
				this.propertyDefinitions = propertyDefinitions;
				this.storeSession = session;
				this.isTableOwned = isTableOwned;
				this.alteredProperties = alteredProperties;
				this.isAtTheBeginningOfTable = true;
				this.SetTableColumns(propertyDefinitions);
				this.SortTable(sortOrder);
				disposeGuard.Success();
			}
		}

		public static PropTag[] GetColumnPropertyTags(StoreSession session, MapiProp propertyReference, ICollection<PropertyDefinition> dataColumns, out NativeStorePropertyDefinition[] nativeColumns)
		{
			ICollection<NativeStorePropertyDefinition> nativePropertyDefinitions = StorePropertyDefinition.GetNativePropertyDefinitions<PropertyDefinition>(PropertyDependencyType.NeedForRead, dataColumns);
			nativeColumns = nativePropertyDefinitions.ToArray<NativeStorePropertyDefinition>();
			return PropertyTagCache.Cache.PropTagsFromPropertyDefinitions(propertyReference, session, nativeColumns).ToArray<PropTag>();
		}

		public virtual DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<QueryResult>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		private static bool SeekReferenceToBookmark(SeekReference reference, out BookMark bookMark)
		{
			bookMark = (BookMark)(reference & ~SeekReference.SeekBackward);
			return (reference & SeekReference.SeekBackward) != SeekReference.SeekBackward;
		}

		private static void ExcludeProperties(PropertyTagPropertyDefinition[] excludeProperties, ref PropTag[] propertyTags)
		{
			if (excludeProperties == null || excludeProperties.Length == 0)
			{
				return;
			}
			List<PropTag> list = new List<PropTag>();
			list.AddRange(from propTag in propertyTags
			where null == excludeProperties.FirstOrDefault((PropertyTagPropertyDefinition excludedProperty) => (excludedProperty.PropertyTag & 4294901760U) == (uint)(propTag & (PropTag)4294901760U))
			select propTag);
			propertyTags = list.ToArray();
		}

		protected void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			StorageGlobals.TraceDispose(this, this.isDisposed, disposing);
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.InternalDispose(disposing);
			}
		}

		protected virtual void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.isTableOwned)
				{
					this.mapiTable.Dispose();
				}
				if (this.OnDisposing != null)
				{
					this.OnDisposing();
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
		}

		public int EstimatedRowCount
		{
			get
			{
				this.CheckDisposed("RowCount::get");
				StoreSession storeSession = this.storeSession;
				bool flag = false;
				int rowCount;
				try
				{
					if (storeSession != null)
					{
						storeSession.BeginMapiCall();
						storeSession.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					rowCount = this.mapiTable.GetRowCount();
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetRowCount, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Folder::RowCount::get.", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetRowCount, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Folder::RowCount::get.", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (storeSession != null)
						{
							storeSession.EndMapiCall();
							if (flag)
							{
								storeSession.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
				return rowCount;
			}
		}

		public int CurrentRow
		{
			get
			{
				this.CheckDisposed("CurrentRow::get");
				if (this.isAtTheBeginningOfTable)
				{
					return 0;
				}
				StoreSession storeSession = this.storeSession;
				bool flag = false;
				int result;
				try
				{
					if (storeSession != null)
					{
						storeSession.BeginMapiCall();
						storeSession.BeginServerHealthCall();
						flag = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					result = this.mapiTable.QueryPosition();
				}
				catch (MapiPermanentException ex)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetCurrentRow, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Folder::CurrentRow::get.", new object[0]),
						ex
					});
				}
				catch (MapiRetryableException ex2)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetCurrentRow, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Folder::CurrentRow::get.", new object[0]),
						ex2
					});
				}
				finally
				{
					try
					{
						if (storeSession != null)
						{
							storeSession.EndMapiCall();
							if (flag)
							{
								storeSession.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
				return result;
			}
		}

		public bool IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
		}

		public bool SeekToCondition(SeekReference reference, QueryFilter seekFilter, SeekToConditionFlags flags)
		{
			this.CheckDisposed("SeekToCondition");
			EnumValidator.ThrowIfInvalid<SeekReference>(reference, "reference");
			BookMark bookMark;
			bool useForwardDirection = QueryResult.SeekReferenceToBookmark(reference, out bookMark);
			if ((flags & SeekToConditionFlags.AllowExtendedSeekReferences) == SeekToConditionFlags.None && reference != SeekReference.OriginCurrent && reference != SeekReference.OriginBeginning)
			{
				throw new NotSupportedException("Seek references other than forward-from-current/beginning require explicit enabling through SeekToConditionFlags");
			}
			return this.SeekToCondition((uint)bookMark, useForwardDirection, seekFilter, flags);
		}

		public bool SeekToCondition(uint bookMark, bool useForwardDirection, QueryFilter seekFilter, SeekToConditionFlags flags)
		{
			this.CheckDisposed("SeekToCondition");
			Util.ThrowOnNullArgument(seekFilter, "seekFilter");
			EnumValidator.ThrowIfInvalid<SeekToConditionFlags>(flags, "flags");
			if ((flags & SeekToConditionFlags.AllowExtendedFilters) == SeekToConditionFlags.None && !(seekFilter is ComparisonFilter))
			{
				throw new NotSupportedException("Filters that are more complex that simple property comparisons require explicit enabling through SeekToConditionFlags");
			}
			Restriction restriction = FilterRestrictionConverter.CreateRestriction(this.storeSession, this.storeSession.ExTimeZone, this.storeSession.Mailbox.MapiStore, seekFilter);
			if (this.isAtTheBeginningOfTable && useForwardDirection && bookMark == 1U)
			{
				bookMark = 0U;
			}
			bool flag = false;
			StoreSession storeSession = this.storeSession;
			bool flag2 = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag2 = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				flag = this.mapiTable.FindRow(restriction, (BookMark)bookMark, useForwardDirection ? FindRowFlag.None : FindRowFlag.FindBackward);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotFindRow, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Folder::SeekToCondition failed.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotFindRow, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Folder::SeekToCondition failed.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag2)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			if (!flag && (flags & SeekToConditionFlags.KeepCursorPositionWhenNoMatch) == SeekToConditionFlags.None)
			{
				StoreSession storeSession2 = this.storeSession;
				bool flag3 = false;
				try
				{
					if (storeSession2 != null)
					{
						storeSession2.BeginMapiCall();
						storeSession2.BeginServerHealthCall();
						flag3 = true;
					}
					if (StorageGlobals.MapiTestHookBeforeCall != null)
					{
						StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
					}
					this.mapiTable.SeekRow(useForwardDirection ? BookMark.End : BookMark.Beginning, 0);
				}
				catch (MapiPermanentException ex3)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSeekRow, ex3, storeSession2, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Folder::SeekToCondition failed.", new object[0]),
						ex3
					});
				}
				catch (MapiRetryableException ex4)
				{
					throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSeekRow, ex4, storeSession2, this, "{0}. MapiException = {1}.", new object[]
					{
						string.Format("Folder::SeekToCondition failed.", new object[0]),
						ex4
					});
				}
				finally
				{
					try
					{
						if (storeSession2 != null)
						{
							storeSession2.EndMapiCall();
							if (flag3)
							{
								storeSession2.EndServerHealthCall();
							}
						}
					}
					finally
					{
						if (StorageGlobals.MapiTestHookAfterCall != null)
						{
							StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
						}
					}
				}
				this.isAtTheBeginningOfTable = false;
			}
			else if (flag)
			{
				this.isAtTheBeginningOfTable = false;
			}
			return flag;
		}

		public bool SeekToCondition(SeekReference reference, QueryFilter seekFilter)
		{
			return this.SeekToCondition(reference, seekFilter, SeekToConditionFlags.None);
		}

		public int SeekToOffset(SeekReference reference, int offset)
		{
			this.CheckDisposed("SeekToOffset");
			EnumValidator.ThrowIfInvalid<SeekReference>(reference, "reference");
			BookMark bookmark;
			QueryResult.SeekReferenceToBookmark(reference, out bookmark);
			int result = 0;
			StoreSession storeSession = this.storeSession;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				result = this.mapiTable.SeekRow(bookmark, offset);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSeekRow, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Folder::SeekToOffset.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSeekRow, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Folder::SeekToOffset.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			if (reference == SeekReference.OriginBeginning && offset == 0)
			{
				this.isAtTheBeginningOfTable = true;
			}
			else
			{
				this.isAtTheBeginningOfTable = false;
			}
			return result;
		}

		public void SetTableColumns(ICollection<PropertyDefinition> propertyDefinitions)
		{
			this.CheckDisposed("SetTableColumns");
			Util.ThrowOnNullArgument(propertyDefinitions, "propertyDefinitions");
			this.columns = this.SetTableColumns(propertyDefinitions, this.alteredProperties);
		}

		public object[][] GetRows(int rowCount)
		{
			return this.GetRows(rowCount, QueryRowsFlags.None);
		}

		public object[][] GetRows(int rowCount, QueryRowsFlags flags)
		{
			bool flag;
			return this.GetRows(rowCount, flags, out flag);
		}

		public object[][] GetRows(int rowCount, out bool mightBeMoreRows)
		{
			return this.GetRows(rowCount, QueryRowsFlags.None, out mightBeMoreRows);
		}

		public virtual object[][] GetRows(int rowCount, QueryRowsFlags flags, out bool mightBeMoreRows)
		{
			this.CheckDisposed("GetRows");
			EnumValidator.ThrowIfInvalid<QueryRowsFlags>(flags, "flags");
			PropValue[][] array = this.Fetch(rowCount, flags);
			mightBeMoreRows = (array.Length > 0);
			return this.PropValuesToObjectArray(array);
		}

		public virtual object[][] ExpandRow(int rowCount, long categoryId, out int rowsInExpandedCategory)
		{
			this.CheckDisposed("ExpandRow");
			if (rowCount < 0)
			{
				throw new ArgumentOutOfRangeException("rowCount", ServerStrings.ExInvalidRowCount);
			}
			PropValue[][] propertyValues = null;
			StoreSession storeSession = this.storeSession;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				propertyValues = this.mapiTable.ExpandRow(categoryId, rowCount, 0, out rowsInExpandedCategory);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotExpandRow, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("QueryResult::ExpandRow. categoryId = {0}, rowCount = {1}.", categoryId, rowCount),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotExpandRow, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("QueryResult::ExpandRow. categoryId = {0}, rowCount = {1}.", categoryId, rowCount),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			return this.PropValuesToObjectArray(propertyValues);
		}

		public virtual int CollapseRow(long categoryId)
		{
			this.CheckDisposed("CollapseRow");
			StoreSession storeSession = this.storeSession;
			bool flag = false;
			int result;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				result = this.mapiTable.CollapseRow(categoryId, 0);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCollapseRow, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("QueryResult::CollapseRow. categoryId = {0}.", categoryId),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCollapseRow, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("QueryResult::CollapseRow. categoryId = {0}.", categoryId),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			return result;
		}

		public uint CreateBookmark()
		{
			this.CheckDisposed("CreateBookmark");
			StoreSession storeSession = this.storeSession;
			bool flag = false;
			uint result;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				result = (uint)this.mapiTable.CreateBookmark();
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateBookmark, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("QueryResult::CreateBookmark failed.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotCreateBookmark, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("QueryResult::CreateBookmark failed.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			return result;
		}

		public void FreeBookmark(uint bookmarkPosition)
		{
			this.CheckDisposed("FreeBookmark");
			StoreSession storeSession = this.storeSession;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				this.mapiTable.FreeBookmark((BookMark)bookmarkPosition);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotFreeBookmark, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("QueryResult::FreeBookmark failed.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotFreeBookmark, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("QueryResult::FreeBookmark failed.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
		}

		public int SeekRowBookmark(uint bookmarkPosition, int rowCount, bool wantRowsSought, out bool soughtLess, out bool positionChanged)
		{
			this.CheckDisposed("SeekRowBookmark");
			StoreSession storeSession = this.storeSession;
			bool flag = false;
			int result;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				result = this.mapiTable.SeekRowBookmark((BookMark)bookmarkPosition, rowCount, wantRowsSought, out soughtLess, out positionChanged);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSeekRowBookmark, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("QueryResult::SeekRowBookmark failed.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSeekRowBookmark, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("QueryResult::SeekRowBookmark failed.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			return result;
		}

		public NativeStorePropertyDefinition[] GetAllPropertyDefinitions(params PropertyTagPropertyDefinition[] excludeProperties)
		{
			this.CheckDisposed("GetAllPropertyDefinitions");
			StoreSession storeSession = this.storeSession;
			bool flag = false;
			PropTag[] propTags;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				propTags = this.mapiTable.QueryColumns(QueryColumnsFlags.AllColumns);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotQueryColumns, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("QueryResult::QueryColumns", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotQueryColumns, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("QueryResult::QueryColumns", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			QueryResult.ExcludeProperties(excludeProperties, ref propTags);
			NativeStorePropertyDefinition[] array = PropertyTagCache.Cache.PropertyDefinitionsFromPropTags(NativeStorePropertyDefinition.TypeCheckingFlag.DoNotCreateInvalidType, this.StoreSession.Mailbox.MapiStore, this.StoreSession, propTags);
			List<NativeStorePropertyDefinition> list = new List<NativeStorePropertyDefinition>();
			foreach (NativeStorePropertyDefinition nativeStorePropertyDefinition in array)
			{
				if (nativeStorePropertyDefinition != null)
				{
					list.Add(nativeStorePropertyDefinition);
				}
			}
			return list.ToArray();
		}

		public virtual byte[] GetCollapseState(byte[] instanceKey)
		{
			this.CheckDisposed("GetCollapseState");
			Util.ThrowOnNullArgument(instanceKey, "instanceKey");
			byte[] result = Array<byte>.Empty;
			StoreSession storeSession = this.storeSession;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				result = this.mapiTable.GetCollapseState(instanceKey);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetCollapseState, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("QueryResult::GetCollapseState. failed.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotGetCollapseState, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("QueryResult::GetCollapseState. failed.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			return result;
		}

		public virtual uint SetCollapseState(byte[] collapseState)
		{
			this.CheckDisposed("SetCollapseState");
			Util.ThrowOnNullArgument(collapseState, "collapseState");
			StoreSession storeSession = this.storeSession;
			bool flag = false;
			uint result;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				result = (uint)this.mapiTable.SetCollapseState(collapseState);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetCollapseState, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("QueryResult::SetCollapseState failed.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetCollapseState, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("QueryResult::SetCollapseState failed.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			this.isAtTheBeginningOfTable = false;
			return result;
		}

		public object Advise(SubscriptionSink subscriptionSink, bool asyncMode)
		{
			this.CheckDisposed("Advise");
			Util.ThrowOnNullArgument(subscriptionSink, "subscriptionSink");
			StoreSession storeSession = this.StoreSession;
			bool flag = false;
			object result;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				result = this.MapiTable.Advise(new MapiNotificationHandler(subscriptionSink.OnNotify), asyncMode ? NotificationCallbackMode.Async : NotificationCallbackMode.Sync);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotAddNotification, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("QueryResult::Advise failed.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotAddNotification, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("QueryResult::Advise failed.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			return result;
		}

		public void Unadvise(object notificationHandle)
		{
			this.CheckDisposed("Unadvise");
			Util.ThrowOnNullArgument(notificationHandle, "notificationHandle");
			StoreSession storeSession = this.StoreSession;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				if (this.MapiTable != null && !this.MapiTable.IsDisposed)
				{
					this.MapiTable.Unadvise((MapiNotificationHandle)notificationHandle);
				}
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotRemoveNotification, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("QueryResult::Unadvise failed.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotRemoveNotification, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("QueryResult::Unadvise failed.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
		}

		public virtual IStorePropertyBag[] GetPropertyBags(int rowCount)
		{
			this.CheckDisposed("GetPropertyBags");
			if (rowCount < 0)
			{
				throw new ArgumentOutOfRangeException("rowCount", ServerStrings.ExInvalidRowCount);
			}
			PropValue[][] array = this.Fetch(rowCount);
			IStorePropertyBag[] array2 = new IStorePropertyBag[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				QueryResultPropertyBag queryResultPropertyBag = new QueryResultPropertyBag(this.HeaderPropBag);
				queryResultPropertyBag.SetQueryResultRow(array[i]);
				array2[i] = queryResultPropertyBag.AsIStorePropertyBag();
			}
			return array2;
		}

		protected QueryResultPropertyBag HeaderPropBag
		{
			get
			{
				if (this.headerPropBag == null)
				{
					this.headerPropBag = new QueryResultPropertyBag(this.storeSession, this.columns.SimplePropertyDefinitions);
				}
				return this.headerPropBag;
			}
		}

		protected ICollection<PropertyDefinition> PropertyDefinitions
		{
			get
			{
				return this.propertyDefinitions;
			}
		}

		protected Schema Schema
		{
			get
			{
				return this.storeSession.Schema;
			}
		}

		public StoreSession StoreSession
		{
			get
			{
				this.CheckDisposed("StoreSession");
				return this.storeSession;
			}
		}

		public ColumnPropertyDefinitions Columns
		{
			get
			{
				return this.columns;
			}
		}

		internal static bool DoPropertyValuesMatchColumns(ColumnPropertyDefinitions columns, PropValue[] values)
		{
			if (columns.PropertyTags.Count != values.Length)
			{
				return false;
			}
			int num = 0;
			foreach (PropTag propTag in columns.PropertyTags)
			{
				if (propTag.Id() != values[num++].PropTag.Id())
				{
					return false;
				}
			}
			return true;
		}

		bool INotificationSource.IsDisposedOrDead
		{
			get
			{
				return this.IsDisposed || this.mapiTable.IsDisposed;
			}
		}

		private MapiTable MapiTable
		{
			get
			{
				return this.mapiTable;
			}
		}

		private ColumnPropertyDefinitions SetTableColumns(ICollection<PropertyDefinition> propertyDefinitions, IList<PropTag> alteredProperties)
		{
			ICollection<NativeStorePropertyDefinition> nativePropertyDefinitions = StorePropertyDefinition.GetNativePropertyDefinitions<PropertyDefinition>(PropertyDependencyType.NeedForRead, propertyDefinitions);
			ICollection<PropTag> collection = PropertyTagCache.Cache.PropTagsFromPropertyDefinitions(this.storeSession.Mailbox.MapiStore, this.storeSession, nativePropertyDefinitions);
			HashSet<int> hashSet = new HashSet<int>(collection.Count);
			int num = 0;
			foreach (PropTag item in collection)
			{
				if (hashSet.Contains((int)item))
				{
					num++;
					if (num > 6)
					{
						throw new ArgumentException(ServerStrings.ExTooManyDuplicateDataColumns(6), "propertyDefinitions");
					}
				}
				else
				{
					hashSet.Add((int)item);
				}
			}
			if (alteredProperties != null && alteredProperties.Count > 0 && collection.Count > 0)
			{
				PropTag[] array = new PropTag[collection.Count];
				collection.CopyTo(array, 0);
				for (int i = 0; i < array.Length; i++)
				{
					for (int j = 0; j < alteredProperties.Count; j++)
					{
						if (array[i] == alteredProperties[j])
						{
							array[i] |= (PropTag)12288U;
						}
					}
				}
				collection = array;
			}
			ColumnPropertyDefinitions columnPropertyDefinitions = new ColumnPropertyDefinitions(propertyDefinitions, nativePropertyDefinitions.ToArray<NativeStorePropertyDefinition>(), collection);
			StoreSession storeSession = this.storeSession;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				this.mapiTable.SetColumns(columnPropertyDefinitions.PropertyTags);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetTableColumns, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Folder::SetTableColumns. Failed to set columns to the table.", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSetTableColumns, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Folder::SetTableColumns. Failed to set columns to the table.", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			return columnPropertyDefinitions;
		}

		private void SortTable(SortOrder sortOrder)
		{
			if (sortOrder == null)
			{
				return;
			}
			StoreSession storeSession = this.storeSession;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				this.mapiTable.SortTable(sortOrder, SortTableFlags.Batch);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSortTable, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Folder::SortTable. Failed due to sort a table. sortOrder = {0}; SortTableFlags = {1}.", sortOrder, SortTableFlags.Batch),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotSortTable, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Folder::SortTable. Failed due to sort a table. sortOrder = {0}; SortTableFlags = {1}.", sortOrder, SortTableFlags.Batch),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
		}

		protected PropValue[][] Fetch(int rowCount)
		{
			return this.Fetch(rowCount, QueryRowsFlags.None);
		}

		protected PropValue[][] Fetch(int rowCount, QueryRowsFlags flags)
		{
			QueryRowsFlags flags2 = ((flags & QueryRowsFlags.NoAdvance) == QueryRowsFlags.NoAdvance) ? QueryRowsFlags.NoAdvance : QueryRowsFlags.None;
			PropValue[][] result = null;
			StoreSession storeSession = this.storeSession;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				result = this.mapiTable.QueryRows(rowCount, flags2);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotQueryRows, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("QueryResult::Fetch. rowCount = {0}.", rowCount),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotQueryRows, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("QueryResult::Fetch. rowCount = {0}.", rowCount),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			this.isAtTheBeginningOfTable = false;
			return result;
		}

		private object[][] PropValuesToObjectArray(PropValue[][] propertyValues)
		{
			object[][] array = new object[propertyValues.Length][];
			if (propertyValues.Length > 0)
			{
				ColumnPropertyDefinitions columnPropertyDefinitions = this.Columns;
				QueryResultPropertyBag queryResultPropertyBag = new QueryResultPropertyBag(this.storeSession, columnPropertyDefinitions.SimplePropertyDefinitions);
				for (int i = 0; i < propertyValues.Length; i++)
				{
					queryResultPropertyBag.SetQueryResultRow(propertyValues[i]);
					array[i] = queryResultPropertyBag.GetProperties(columnPropertyDefinitions.PropertyDefinitions);
				}
			}
			return array;
		}

		private const int MaxDuplicateTableColumns = 6;

		public const int MaxRows = 10000;

		private readonly MapiTable mapiTable;

		private readonly bool isTableOwned = true;

		private readonly StoreSession storeSession;

		private readonly IList<PropTag> alteredProperties;

		private readonly DisposeTracker disposeTracker;

		private bool isDisposed;

		private ICollection<PropertyDefinition> propertyDefinitions;

		private ColumnPropertyDefinitions columns;

		private bool isAtTheBeginningOfTable;

		private QueryResultPropertyBag headerPropBag;

		public Action OnDisposing;
	}
}
