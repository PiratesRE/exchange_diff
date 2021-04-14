using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.LazyIndexing;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class LogicalContext : IndexingContext
	{
		internal new static LogicalContext Create(ExecutionDiagnostics executionDiagnostics, ClientSecurityContext securityContext, ClientType clientType, CultureInfo culture)
		{
			return new LogicalContext(executionDiagnostics, securityContext, clientType, culture);
		}

		internal LogicalContext(ExecutionDiagnostics executionDiagnostics) : base(executionDiagnostics)
		{
		}

		internal LogicalContext(ExecutionDiagnostics executionDiagnostics, ClientSecurityContext securityContext, ClientType clientType, CultureInfo culture) : base(executionDiagnostics, securityContext, clientType, culture)
		{
		}

		public override void OnBeforeTableAccess(Connection.OperationType operationType, Table table, IList<object> partitionValues)
		{
			if (!base.IsSharedMailboxOperation)
			{
				return;
			}
			switch (operationType)
			{
			case Connection.OperationType.Query:
			{
				ReceiveFolderTable receiveFolderTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.ReceiveFolderTable(base.Database);
				ReceiveFolder2Table receiveFolder2Table = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.ReceiveFolder2Table(base.Database);
				if (table.Equals(Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.MessageTable(base.Database).Table) || table.Equals(Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.FolderTable(base.Database).Table) || table.Equals(Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.EventsTable(base.Database).Table) || table.Equals(Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.DeliveredToTable(base.Database).Table) || table.Equals(Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.AttachmentTable(base.Database).Table) || table.Equals(Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.InferenceLogTable(base.Database).Table) || (receiveFolderTable != null && table.Equals(receiveFolderTable.Table)) || (receiveFolder2Table != null && table.Equals(receiveFolder2Table.Table)) || table.Equals(Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.SearchQueueTable(base.Database).Table) || table.Equals(Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.TombstoneTable(base.Database).Table) || table.Equals(Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.WatermarksTable(base.Database).Table))
				{
					return;
				}
				break;
			}
			case Connection.OperationType.Insert:
				if (table.Equals(Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.EventsTable(base.Database).Table))
				{
					return;
				}
				if (table.Equals(Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.AttachmentTable(base.Database).Table) || table.Equals(Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.TombstoneTable(base.Database).Table))
				{
					return;
				}
				break;
			case Connection.OperationType.Update:
				if (table.Equals(Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.AttachmentTable(base.Database).Table) || table.Equals(Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.TombstoneTable(base.Database).Table))
				{
					return;
				}
				if (table.Equals(Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.FolderTable(base.Database).Table) && base.IsUserExclusiveLocked(Context.UserLockCheckFrame.Scope.SearchFolder))
				{
					return;
				}
				break;
			}
			base.OnBeforeTableAccess(operationType, table, partitionValues);
		}
	}
}
