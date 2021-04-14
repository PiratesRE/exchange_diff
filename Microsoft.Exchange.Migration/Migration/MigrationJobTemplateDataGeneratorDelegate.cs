using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Migration
{
	internal delegate IDictionary<string, string> MigrationJobTemplateDataGeneratorDelegate(MigrationJobReportingCursor migrationReportData, string successReportLink, string failureReportLink);
}
