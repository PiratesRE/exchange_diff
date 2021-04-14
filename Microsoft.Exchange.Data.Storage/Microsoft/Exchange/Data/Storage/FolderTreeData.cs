using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Storage.OutlookClassIds;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class FolderTreeData : MessageItem, IFolderTreeData, IMessageItem, IToDoItem, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		protected static VersionedId FindFirstRowMatchingFilter(MailboxSession session, PropertyDefinition[] filterProperties, FolderTreeData.RowMatchesFilterDelegate rowMatchesFilter)
		{
			bool flag;
			return FolderTreeData.GetValueFromFirstMatchingRow<VersionedId>(session, null, rowMatchesFilter, filterProperties, ItemSchema.Id, out flag);
		}

		protected static byte[] GetOrdinalValueOfFirstMatchingNode(MailboxSession session, SortBy[] sortOrder, FolderTreeData.RowMatchesFilterDelegate rowMatchesFilter, PropertyDefinition[] filterProperties, out bool noRowsFound)
		{
			return FolderTreeData.GetValueFromFirstMatchingRow<byte[]>(session, sortOrder, rowMatchesFilter, filterProperties, FolderTreeDataSchema.Ordinal, out noRowsFound);
		}

		protected static T GetValueFromFirstMatchingRow<T>(MailboxSession session, SortBy[] sortOrder, FolderTreeData.RowMatchesFilterDelegate rowMatchesFilter, PropertyDefinition[] filterProperties, PropertyDefinition propertyToRetrieve, out bool noRowsFound)
		{
			noRowsFound = true;
			List<PropertyDefinition> list = new List<PropertyDefinition>();
			list.AddRange(filterProperties);
			list.Add(propertyToRetrieve);
			using (Folder folder = Folder.Bind(session, DefaultFolderType.CommonViews))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.Associated, null, sortOrder, list))
				{
					IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(10000);
					foreach (IStorePropertyBag storePropertyBag in propertyBags)
					{
						if (rowMatchesFilter(storePropertyBag))
						{
							noRowsFound = false;
							return storePropertyBag.GetValueOrDefault<T>(propertyToRetrieve, default(T));
						}
					}
				}
			}
			return default(T);
		}

		protected static Guid GetSafeGuidFromByteArray(byte[] guid)
		{
			if (guid != null && guid.Length == 16)
			{
				return new Guid(guid);
			}
			return Guid.Empty;
		}

		private static int GenerateNewOutlookTagId()
		{
			return FolderTreeData.outlookTagIdRandom.Next();
		}

		public FolderTreeData(ICoreItem coreItem) : base(coreItem, false)
		{
		}

		public byte[] NodeOrdinal
		{
			get
			{
				this.CheckDisposed("NodeOrdinal::get");
				return base.GetValueOrDefault<byte[]>(FolderTreeDataSchema.Ordinal);
			}
			private set
			{
				this.CheckDisposed("NodeOrdinal::set");
				this[FolderTreeDataSchema.Ordinal] = value;
			}
		}

		public int OutlookTagId
		{
			get
			{
				this.CheckDisposed("NodeOrdinal::get");
				return base.GetValueOrDefault<int>(FolderTreeDataSchema.OutlookTagId);
			}
			private set
			{
				this.CheckDisposed("NodeOrdinal::set");
				this[FolderTreeDataSchema.OutlookTagId] = value;
			}
		}

		public FolderTreeDataType FolderTreeDataType
		{
			get
			{
				this.CheckDisposed("FolderTreeDataType::get");
				return base.GetValueOrDefault<FolderTreeDataType>(FolderTreeDataSchema.Type, FolderTreeDataType.Undefined);
			}
			protected set
			{
				this.CheckDisposed("FolderTreeDataType::set");
				this[FolderTreeDataSchema.Type] = value;
			}
		}

		public FolderTreeDataFlags FolderTreeDataFlags
		{
			get
			{
				this.CheckDisposed("FolderTreeDataFlags::get");
				return base.GetValueOrDefault<FolderTreeDataFlags>(FolderTreeDataSchema.FolderTreeDataFlags, FolderTreeDataFlags.None);
			}
			protected set
			{
				this.CheckDisposed("FolderTreeDataFlags::set");
				this[FolderTreeDataSchema.FolderTreeDataFlags] = value;
			}
		}

		public MailboxSession MailboxSession { get; protected set; }

		public virtual void SetNodeOrdinal(byte[] nodeBefore, byte[] nodeAfter)
		{
			this.CheckDisposed("SetNodeOrdinal");
			if (base.IsNew)
			{
				throw new NotSupportedException("SetNodeOrdinal cannot be used for new NavigationNodes.");
			}
			this.SetNodeOrdinalInternal(nodeBefore, nodeAfter);
		}

		protected void SetNodeOrdinalInternal(byte[] nodeBefore, byte[] nodeAfter)
		{
			this.NodeOrdinal = FolderTreeData.BinaryOrdinalGenerator.GetInbetweenOrdinalValue(nodeBefore, nodeAfter);
		}

		protected override void OnBeforeSave()
		{
			base.OnBeforeSave();
			this.OutlookTagId = FolderTreeData.GenerateNewOutlookTagId();
		}

		protected static readonly Guid MyFoldersClassId = NavigationNodeParentGroup.MyFoldersClassId.AsGuid;

		protected static readonly Guid OtherFoldersClassId = NavigationNodeParentGroup.OtherFoldersClassId.AsGuid;

		protected static readonly Guid PeoplesFoldersClassId = NavigationNodeParentGroup.PeoplesFoldersClassId.AsGuid;

		private static readonly Random outlookTagIdRandom = new Random((int)ExDateTime.UtcNow.UtcTicks);

		protected delegate bool RowMatchesFilterDelegate(IStorePropertyBag row);

		private static class BinaryOrdinalGenerator
		{
			public static byte[] GetInbetweenOrdinalValue(byte[] beforeOrdinal, byte[] afterOrdinal)
			{
				int num = 0;
				int num2 = Math.Max((beforeOrdinal != null) ? beforeOrdinal.Length : 0, (afterOrdinal != null) ? afterOrdinal.Length : 0) + 1;
				byte[] array = new byte[num2];
				if (beforeOrdinal != null && afterOrdinal != null && ArrayComparer<byte>.Comparer.Compare(beforeOrdinal, afterOrdinal) >= 0)
				{
					throw new Exception("Previous ordinal value is greater then after ordinal value");
				}
				if (beforeOrdinal != null && FolderTreeData.BinaryOrdinalGenerator.CheckAllZero(beforeOrdinal))
				{
					beforeOrdinal = null;
				}
				if (afterOrdinal != null && FolderTreeData.BinaryOrdinalGenerator.CheckAllZero(afterOrdinal))
				{
					afterOrdinal = null;
				}
				byte beforeVal;
				byte afterVal;
				for (;;)
				{
					beforeVal = FolderTreeData.BinaryOrdinalGenerator.GetBeforeVal(num, beforeOrdinal);
					afterVal = FolderTreeData.BinaryOrdinalGenerator.GetAfterVal(num, afterOrdinal);
					if (afterVal > beforeVal + 1)
					{
						break;
					}
					array[num++] = beforeVal;
					if (beforeVal + 1 == afterVal)
					{
						afterOrdinal = null;
					}
				}
				array[num++] = (afterVal + beforeVal) / 2;
				byte[] array2 = new byte[num];
				Array.Copy(array, array2, num);
				return array2;
			}

			private static bool CheckAllZero(byte[] bytes)
			{
				if (bytes == null)
				{
					throw new ArgumentNullException("bytes");
				}
				foreach (byte b in bytes)
				{
					if (b != 0)
					{
						return false;
					}
				}
				return true;
			}

			private static byte GetValEx(int index, byte newValue, byte[] ordinal)
			{
				if (index >= ordinal.Length)
				{
					return newValue;
				}
				return ordinal[index];
			}

			private static byte GetBeforeVal(int index, byte[] beforeOrdinal)
			{
				if (beforeOrdinal == null)
				{
					return 0;
				}
				return FolderTreeData.BinaryOrdinalGenerator.GetValEx(index, 0, beforeOrdinal);
			}

			private static byte GetAfterVal(int index, byte[] afterOrdinal)
			{
				if (afterOrdinal == null)
				{
					return byte.MaxValue;
				}
				return FolderTreeData.BinaryOrdinalGenerator.GetValEx(index, byte.MaxValue, afterOrdinal);
			}

			private const byte MinValue = 0;

			private const byte MaxValue = 255;
		}
	}
}
