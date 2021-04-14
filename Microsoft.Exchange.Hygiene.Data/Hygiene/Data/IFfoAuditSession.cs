using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal interface IFfoAuditSession
	{
		IEnumerable<AuditProperty> FindAuditPropertiesByInstance(Guid partitionId, Guid instanceId, string entityName);

		IEnumerable<AuditProperty> FindAuditPropertiesByAuditId(Guid partitionId, Guid auditId);

		IEnumerable<AuditHistoryResult> FindAuditHistory(string entityName, Guid? entityInstanceId, Guid partitionId, DateTime startTime, DateTime? endTime);

		IEnumerable<AuditHistoryResult> SearchAuditHistory(string entityName, string searchString, Guid? entityInstanceId, Guid partitionId, DateTime startTime, DateTime? endTime);

		void SetEntityData(Guid partitionId, string tableName, string columnName, string condition, string newValue);
	}
}
