using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	public class LogSearchCursor : IDisposable
	{
		public LogSearchCursor(CsvTable table, string server, ServerVersion version, string log, LogQuery query, IProgressReport progressReport)
		{
			if (version == null)
			{
				throw new ArgumentNullException("version must not be null.");
			}
			this.searchStream = new LogSearchStream(server, version, log, query, progressReport);
			this.cursor = new CsvFieldCache(table, this.searchStream, 32768);
			this.responseContainsSchema = (version >= LogSearchConstants.LowestModernInterfaceBuildVersion);
			this.serverSchemaMatchesLocalSchema = (CsvFieldCache.LocalVersion == version);
		}

		public bool MoveNext()
		{
			if (!this.cursor.MoveNext(false))
			{
				return false;
			}
			if (this.responseContainsSchema && !this.firstRowHasBeenProcessed)
			{
				if (!this.serverSchemaMatchesLocalSchema)
				{
					this.cursor.SetMappingTableBasedOnCurrentRecord();
				}
				this.firstRowHasBeenProcessed = true;
				return this.cursor.MoveNext(false);
			}
			return true;
		}

		public object GetField(int idx)
		{
			return this.cursor.GetField(idx);
		}

		public void Cancel()
		{
			this.searchStream.Cancel();
		}

		public void Close()
		{
			this.searchStream.Close();
		}

		public void Dispose()
		{
			this.Close();
		}

		private LogSearchStream searchStream;

		private CsvFieldCache cursor;

		private bool serverSchemaMatchesLocalSchema;

		private bool responseContainsSchema;

		private bool firstRowHasBeenProcessed;
	}
}
