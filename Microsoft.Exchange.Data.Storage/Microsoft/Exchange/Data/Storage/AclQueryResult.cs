using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AclQueryResult : DisposableObject, IQueryResult, IDisposable
	{
		internal AclQueryResult(StoreSession session, IList<AclTableEntry> tableEntries, bool allowExtendedPermissionInformationQuery, bool removeFreeBusyRights, ICollection<PropertyDefinition> columns)
		{
			this.session = session;
			this.tableEntries = tableEntries;
			this.allowExtendedPermissionInformationQuery = allowExtendedPermissionInformationQuery;
			this.removeFreeBusyRights = removeFreeBusyRights;
			this.columns = new List<PropertyDefinition>(columns);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<AclQueryResult>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			base.InternalDispose(disposing);
		}

		public object[][] GetRows(int rowCount, out bool mightBeMoreRows)
		{
			this.CheckDisposed(null);
			return this.GetRows(rowCount, QueryRowsFlags.None, out mightBeMoreRows);
		}

		public object[][] GetRows(int rowCount, QueryRowsFlags flags, out bool mightBeMoreRows)
		{
			this.CheckDisposed(null);
			EnumValidator.ThrowIfInvalid<QueryRowsFlags>(flags, "flags");
			List<object[]> list = new List<object[]>();
			int num = 0;
			int num2 = 0;
			while (num2 < rowCount && this.currentRowIndex + num < this.tableEntries.Count)
			{
				object[] array = new object[this.columns.Count];
				for (int i = 0; i < this.columns.Count; i++)
				{
					if (this.columns[i] == InternalSchema.InstanceKey)
					{
						array[i] = BitConverter.GetBytes(this.tableEntries[this.currentRowIndex + num].MemberId);
					}
					else if (this.columns[i] == PermissionSchema.MemberId)
					{
						array[i] = this.tableEntries[this.currentRowIndex + num].MemberId;
					}
					else if (this.columns[i] == PermissionSchema.MemberEntryId)
					{
						array[i] = this.tableEntries[this.currentRowIndex + num].MemberEntryId;
					}
					else if (this.columns[i] == PermissionSchema.MemberRights)
					{
						MemberRights memberRights = this.tableEntries[this.currentRowIndex + num].MemberRights;
						if (this.removeFreeBusyRights)
						{
							array[i] = (int)(memberRights & ~(MemberRights.FreeBusySimple | MemberRights.FreeBusyDetailed));
						}
						else
						{
							array[i] = (int)memberRights;
						}
					}
					else if (this.columns[i] == PermissionSchema.MemberName)
					{
						array[i] = this.tableEntries[this.currentRowIndex + num].MemberName;
					}
					else if (this.columns[i] == PermissionSchema.MemberSecurityIdentifier)
					{
						if (!this.allowExtendedPermissionInformationQuery)
						{
							throw new InvalidOperationException("QueryResult doesn't support MemberSecurityIdentifier property");
						}
						SecurityIdentifier securityIdentifier = this.tableEntries[this.currentRowIndex + num].SecurityIdentifier;
						byte[] array2 = new byte[securityIdentifier.BinaryLength];
						securityIdentifier.GetBinaryForm(array2, 0);
						array[i] = array2;
					}
					else if (this.columns[i] == PermissionSchema.MemberIsGroup)
					{
						if (!this.allowExtendedPermissionInformationQuery)
						{
							throw new InvalidOperationException("QueryResult doesn't support MemberIsGroup property");
						}
						array[i] = this.tableEntries[this.currentRowIndex + num].IsGroup;
					}
				}
				list.Add(array);
				num++;
				num2++;
			}
			if ((flags & QueryRowsFlags.NoAdvance) != QueryRowsFlags.NoAdvance)
			{
				this.currentRowIndex += num;
			}
			mightBeMoreRows = (list.Count > 0);
			return list.ToArray();
		}

		public void SetTableColumns(ICollection<PropertyDefinition> propertyDefinitions)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(propertyDefinitions, "propertyDefinitions");
			this.columns = new List<PropertyDefinition>(propertyDefinitions);
		}

		public int SeekToOffset(SeekReference reference, int offset)
		{
			this.CheckDisposed(null);
			EnumValidator.ThrowIfInvalid<SeekReference>(reference, "reference");
			reference &= ~SeekReference.SeekBackward;
			int num;
			if (reference == SeekReference.OriginBeginning)
			{
				num = 0;
			}
			else if (reference == SeekReference.OriginEnd)
			{
				num = ((this.tableEntries.Count > 0) ? this.tableEntries.Count : 0);
			}
			else
			{
				num = this.currentRowIndex;
			}
			int num2 = num + offset;
			if (offset > 0 && num2 >= this.tableEntries.Count)
			{
				num2 = this.tableEntries.Count;
			}
			else if (offset < 0 && num2 < 0)
			{
				num2 = 0;
			}
			int result = Math.Abs(num2 - this.currentRowIndex);
			this.currentRowIndex = num2;
			return result;
		}

		public bool SeekToCondition(SeekReference reference, QueryFilter seekFilter, SeekToConditionFlags flags)
		{
			this.CheckDisposed(null);
			EnumValidator.ThrowIfInvalid<SeekReference>(reference, "reference");
			throw new NotSupportedException();
		}

		public bool SeekToCondition(SeekReference reference, QueryFilter seekFilter)
		{
			this.CheckDisposed(null);
			EnumValidator.ThrowIfInvalid<SeekReference>(reference, "reference");
			throw new NotSupportedException();
		}

		public bool SeekToCondition(uint bookMark, bool useForwardDirection, QueryFilter seekFilter, SeekToConditionFlags flags)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(seekFilter, "seekFilter");
			EnumValidator.ThrowIfInvalid<SeekToConditionFlags>(flags, "flags");
			throw new NotSupportedException();
		}

		public object[][] ExpandRow(int rowCount, long categoryId, out int rowsInExpandedCategory)
		{
			this.CheckDisposed(null);
			throw new NotSupportedException();
		}

		public int CollapseRow(long categoryId)
		{
			this.CheckDisposed(null);
			throw new NotSupportedException();
		}

		public uint CreateBookmark()
		{
			this.CheckDisposed(null);
			throw new NotSupportedException();
		}

		public void FreeBookmark(uint bookmarkPosition)
		{
			this.CheckDisposed(null);
			throw new NotSupportedException();
		}

		public int SeekRowBookmark(uint bookmarkPosition, int rowCount, bool wantRowsSought, out bool soughtLess, out bool positionChanged)
		{
			this.CheckDisposed(null);
			throw new NotSupportedException();
		}

		public NativeStorePropertyDefinition[] GetAllPropertyDefinitions(params PropertyTagPropertyDefinition[] excludeProperties)
		{
			this.CheckDisposed(null);
			NativeStorePropertyDefinition[] array = this.allowExtendedPermissionInformationQuery ? AclQueryResult.availableExtendedPermissionQueryColumns : AclQueryResult.availableQueryColumns;
			if (excludeProperties != null && excludeProperties.Length != 0)
			{
				array = array.Except(excludeProperties).ToArray<NativeStorePropertyDefinition>();
			}
			return array;
		}

		public byte[] GetCollapseState(byte[] instanceKey)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(instanceKey, "instanceKey");
			throw new NotSupportedException();
		}

		public uint SetCollapseState(byte[] collapseState)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(collapseState, "collapseState");
			throw new NotSupportedException();
		}

		public object Advise(SubscriptionSink subscriptionSink, bool asyncMode)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(subscriptionSink, "subscriptionSink");
			throw new NotSupportedException();
		}

		public void Unadvise(object notificationHandle)
		{
			this.CheckDisposed(null);
			Util.ThrowOnNullArgument(notificationHandle, "notificationHandle");
			throw new NotSupportedException();
		}

		public IStorePropertyBag[] GetPropertyBags(int rowCount)
		{
			this.CheckDisposed(null);
			throw new NotSupportedException();
		}

		public StoreSession StoreSession
		{
			get
			{
				this.CheckDisposed(null);
				return this.session;
			}
		}

		public ColumnPropertyDefinitions Columns
		{
			get
			{
				this.CheckDisposed(null);
				throw new NotSupportedException();
			}
		}

		public int CurrentRow
		{
			get
			{
				this.CheckDisposed(null);
				return this.currentRowIndex;
			}
		}

		public int EstimatedRowCount
		{
			get
			{
				this.CheckDisposed(null);
				return this.tableEntries.Count;
			}
		}

		public new bool IsDisposed
		{
			get
			{
				return base.IsDisposed;
			}
		}

		private static readonly NativeStorePropertyDefinition[] availableQueryColumns = new NativeStorePropertyDefinition[]
		{
			InternalSchema.InstanceKey,
			InternalSchema.MemberId,
			InternalSchema.MemberName,
			InternalSchema.MemberEntryId,
			InternalSchema.MemberRights
		};

		private static readonly NativeStorePropertyDefinition[] availableExtendedPermissionQueryColumns = new NativeStorePropertyDefinition[]
		{
			InternalSchema.InstanceKey,
			InternalSchema.MemberId,
			InternalSchema.MemberName,
			InternalSchema.MemberEntryId,
			InternalSchema.MemberRights,
			InternalSchema.MemberSecurityIdentifier,
			InternalSchema.MemberIsGroup
		};

		private readonly StoreSession session;

		private readonly IList<AclTableEntry> tableEntries;

		private readonly bool allowExtendedPermissionInformationQuery;

		private readonly bool removeFreeBusyRights;

		private IList<PropertyDefinition> columns = Array<PropertyDefinition>.Empty;

		private int currentRowIndex;
	}
}
