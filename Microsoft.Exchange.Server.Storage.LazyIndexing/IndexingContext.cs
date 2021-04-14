using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public class IndexingContext : Context
	{
		internal new static IndexingContext Create(ExecutionDiagnostics executionDiagnostics, ClientSecurityContext securityContext, ClientType clientType, CultureInfo culture)
		{
			return new IndexingContext(executionDiagnostics, securityContext, clientType, culture);
		}

		internal IndexingContext(ExecutionDiagnostics executionDiagnostics) : base(executionDiagnostics)
		{
		}

		internal IndexingContext(ExecutionDiagnostics executionDiagnostics, ClientSecurityContext securityContext, ClientType clientType, CultureInfo culture) : base(executionDiagnostics, securityContext, clientType, culture)
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
				if (table.Equals(DatabaseSchema.PseudoIndexMaintenanceTable(base.Database).Table) || table.Equals(DatabaseSchema.PseudoIndexControlTable(base.Database).Table) || table.Equals(DatabaseSchema.PseudoIndexDefinitionTable(base.Database).Table) || table.Name.StartsWith("pi", StringComparison.Ordinal))
				{
					return;
				}
				break;
			case Connection.OperationType.Insert:
				if ((table.Equals(DatabaseSchema.PseudoIndexMaintenanceTable(base.Database).Table) || table.Name.StartsWith("pi", StringComparison.Ordinal)) && base.IsUserExclusiveLocked(Context.UserLockCheckFrame.Scope.LogicalIndex))
				{
					return;
				}
				break;
			case Connection.OperationType.Update:
				if ((table.Equals(DatabaseSchema.PseudoIndexControlTable(base.Database).Table) || table.Name.StartsWith("pi", StringComparison.Ordinal)) && base.IsUserExclusiveLocked(Context.UserLockCheckFrame.Scope.LogicalIndex))
				{
					return;
				}
				break;
			case Connection.OperationType.Delete:
				if (table.Name.StartsWith("pi", StringComparison.Ordinal) && base.IsUserExclusiveLocked(Context.UserLockCheckFrame.Scope.LogicalIndex))
				{
					return;
				}
				break;
			case Connection.OperationType.CreateTable:
				if (base.IsUserExclusiveLocked(Context.UserLockCheckFrame.Scope.LogicalIndex))
				{
					return;
				}
				break;
			}
			base.OnBeforeTableAccess(operationType, table, partitionValues);
		}
	}
}
