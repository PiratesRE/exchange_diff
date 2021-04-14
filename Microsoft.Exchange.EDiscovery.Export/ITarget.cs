using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal interface ITarget
	{
		IExportContext ExportContext { get; }

		ExportSettings ExportSettings { get; set; }

		IItemIdList CreateItemIdList(string mailboxId, bool isUnsearchable);

		void RemoveItemIdList(string mailboxId, bool isUnsearchable);

		IContextualBatchDataWriter<List<ItemInformation>> CreateDataWriter(IProgressController progressController);

		void Rollback(SourceInformationCollection allSourceInformation);

		IStatusLog GetStatusLog();

		void CheckInitialStatus(SourceInformationCollection allSourceInformation, OperationStatus status);
	}
}
