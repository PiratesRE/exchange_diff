using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay.DatabaseCopyLayout
{
	internal class DatabaseCopyLayoutEntry
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.DatabaseCopyLayoutTracer;
			}
		}

		public DatabaseCopyLayoutEntry(string databaseNamePrefix = "", string databaseNumberFormatSpecifier = "D3")
		{
			this.m_databaseNamePrefix = databaseNamePrefix;
			this.m_databaseNumberFormatSpecifier = databaseNumberFormatSpecifier;
		}

		public string GetDatabaseName(int databaseNumber)
		{
			return this.m_databaseNamePrefix + databaseNumber.ToString(this.m_databaseNumberFormatSpecifier);
		}

		private readonly string m_databaseNamePrefix;

		private readonly string m_databaseNumberFormatSpecifier;
	}
}
