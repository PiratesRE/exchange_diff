using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class PrimaryHierarchyAclModifyTable : DisposableObject, IModifyTable, IDisposable
	{
		public PrimaryHierarchyAclModifyTable(RPCPrimaryHierarchyProvider primaryHierarchyProvider, CoreFolder coreFolder, IModifyTable permissionsTable, ModifyTableOptions options)
		{
			this.coreFolder = coreFolder;
			this.options = options;
			this.primaryHierarchyProvider = primaryHierarchyProvider;
			this.currentPermissionsTable = permissionsTable;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<PrimaryHierarchyAclModifyTable>(this);
		}

		public void Clear()
		{
			this.CheckDisposed(null);
			this.replaceAllRows = true;
			this.pendingModifyOperations.Clear();
		}

		public void AddRow(params PropValue[] propValues)
		{
			this.CheckDisposed(null);
			this.AddPendingChange(ModifyTableOperationType.Add, propValues);
		}

		public void ModifyRow(params PropValue[] propValues)
		{
			this.CheckDisposed(null);
			this.AddPendingChange(ModifyTableOperationType.Modify, propValues);
		}

		public void RemoveRow(params PropValue[] propValues)
		{
			this.CheckDisposed(null);
			this.AddPendingChange(ModifyTableOperationType.Remove, propValues);
		}

		public IQueryResult GetQueryResult(QueryFilter queryFilter, ICollection<PropertyDefinition> columns)
		{
			throw new NotSupportedException("GetQueryResult not supported on PrimaryHierarchyAclModifyTable");
		}

		public void ApplyPendingChanges()
		{
			this.CheckDisposed(null);
			AclTableEntry.ModifyOperation[] array = (from op in this.pendingModifyOperations
			select AclTableEntry.ModifyOperation.FromModifyTableOperation(op)).ToArray<AclTableEntry.ModifyOperation>();
			using (IQueryResult queryResult = this.currentPermissionsTable.GetQueryResult(null, new PropertyDefinition[]
			{
				PermissionSchema.MemberId,
				PermissionSchema.MemberEntryId
			}))
			{
				bool flag;
				object[][] rows = queryResult.GetRows(queryResult.EstimatedRowCount, out flag);
				for (int i = 0; i < array.Length; i++)
				{
					AclTableEntry.ModifyOperation modifyOperation = array[i];
					switch (modifyOperation.Operation)
					{
					case ModifyTableOperationType.Modify:
					case ModifyTableOperationType.Remove:
						if (modifyOperation.Entry.MemberId != -1L && modifyOperation.Entry.MemberId != 0L)
						{
							array[i] = new AclTableEntry.ModifyOperation(array[i].Operation, new AclTableEntry(array[i].Entry.MemberId, PrimaryHierarchyAclModifyTable.GetMemberEntryId(modifyOperation.Entry.MemberId, rows), array[i].Entry.MemberName, array[i].Entry.MemberRights));
						}
						break;
					}
				}
			}
			this.primaryHierarchyProvider.ModifyPermissions(this.coreFolder.Id, array, this.options, this.replaceAllRows);
			this.replaceAllRows = false;
			this.pendingModifyOperations.Clear();
		}

		public void SuppressRestriction()
		{
			throw new NotSupportedException("SuppressRestriction not supported on PrimaryHierarchyAclModifyTable");
		}

		public StoreSession Session
		{
			get
			{
				this.CheckDisposed(null);
				return this.coreFolder.Session;
			}
		}

		private static byte[] GetMemberEntryId(long memberId, object[][] permissionRows)
		{
			foreach (object[] array in permissionRows)
			{
				if ((long)array[0] == memberId)
				{
					return (byte[])array[1];
				}
			}
			throw new ObjectNotFoundException(new LocalizedString(string.Format("Missing AclTableEntry with MemberId - {0}", memberId)));
		}

		private void AddPendingChange(ModifyTableOperationType operation, PropValue[] propValues)
		{
			this.pendingModifyOperations.Add(new ModifyTableOperation(operation, propValues));
		}

		private readonly RPCPrimaryHierarchyProvider primaryHierarchyProvider;

		private readonly IModifyTable currentPermissionsTable;

		private readonly CoreFolder coreFolder;

		private readonly ModifyTableOptions options;

		private readonly List<ModifyTableOperation> pendingModifyOperations = new List<ModifyTableOperation>();

		private bool replaceAllRows;
	}
}
