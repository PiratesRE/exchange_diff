using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ReportingTask.Common
{
	public interface ITaskContext
	{
		Guid CurrentOrganizationGuid { get; }

		Guid CurrentOrganizationExternalDirectoryId { get; }

		bool IsCurrentOrganizationForestWide { get; }

		void WriteError(LocalizedException localizedException, ExchangeErrorCategory exchangeErrorCategory, object target);

		void WriteWarning(LocalizedString text);

		void WriteVerbose(LocalizedString text);
	}
}
