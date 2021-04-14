using System;
using System.IO;

namespace Microsoft.Exchange.Migration
{
	internal delegate MigrationJobReportingCursor MigrationJobReportWriterDelegate(MigrationJobReportingCursor cursorInitialPosition, StreamWriter successWriter, StreamWriter failureWriter);
}
