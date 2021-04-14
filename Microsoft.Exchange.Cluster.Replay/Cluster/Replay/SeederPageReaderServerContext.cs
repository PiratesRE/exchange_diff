using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class SeederPageReaderServerContext
	{
		public SeederPageReaderServerContext(IEseDatabaseReader localEseDatabaseReader, string targetServerName)
		{
			this.m_localEseDatabaseReader = localEseDatabaseReader;
			this.m_targetServerName = targetServerName;
			ReplayCrimsonEvents.IncReseedSourceBegin.Log<Guid, string, string, string>(this.m_localEseDatabaseReader.DatabaseGuid, this.m_localEseDatabaseReader.DatabaseName, this.m_targetServerName, string.Empty);
		}

		public IEseDatabaseReader DatabaseReader
		{
			get
			{
				return this.m_localEseDatabaseReader;
			}
		}

		public void Close()
		{
			if (this.m_localEseDatabaseReader != null)
			{
				this.m_localEseDatabaseReader.Dispose();
				ReplayCrimsonEvents.IncReseedSourceEnd.Log<Guid, string, string, string>(this.m_localEseDatabaseReader.DatabaseGuid, this.m_localEseDatabaseReader.DatabaseName, this.m_targetServerName, string.Empty);
			}
			this.m_localEseDatabaseReader = null;
		}

		private string m_targetServerName;

		private IEseDatabaseReader m_localEseDatabaseReader;
	}
}
