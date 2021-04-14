using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmDbTrace
	{
		internal AmDbTrace(IADDatabase db)
		{
			this.m_db = db;
		}

		internal void Debug(string format, params object[] args)
		{
			AmTrace.Debug(this.PrefixDatabase(format), args);
		}

		internal void Info(string format, params object[] args)
		{
			AmTrace.Info(this.PrefixDatabase(format), args);
		}

		internal void Warning(string format, params object[] args)
		{
			AmTrace.Warning(this.PrefixDatabase(format), args);
		}

		internal void Error(string format, params object[] args)
		{
			AmTrace.Error(this.PrefixDatabase(format), args);
		}

		internal void Entering(string format, params object[] args)
		{
			AmTrace.Entering(this.PrefixDatabase(format), args);
		}

		internal void Leaving(string format, params object[] args)
		{
			AmTrace.Leaving(this.PrefixDatabase(format), args);
		}

		private string PrefixDatabase(string format)
		{
			string result;
			if (this.m_db != null)
			{
				result = string.Format("{0} [DbGuid={1}]", format, this.m_db.Guid);
			}
			else
			{
				result = string.Format("{0} [DbGuid=<unknown>]", format);
			}
			return result;
		}

		private IADDatabase m_db;
	}
}
