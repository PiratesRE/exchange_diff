using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	internal class RpsCmdletLoggerBuffer
	{
		internal Dictionary<Enum, object> MetadataLogCache
		{
			get
			{
				return this.metadataLogCache;
			}
		}

		internal Dictionary<string, string> GenericInfoLogCache
		{
			get
			{
				return this.genericInfoLogCache;
			}
		}

		internal Dictionary<string, string> GenericErrorLogCache
		{
			get
			{
				return this.genericErrorLogCache;
			}
		}

		internal Dictionary<Enum, double> LatencyLogCache
		{
			get
			{
				return this.latencyLogCache;
			}
		}

		internal Dictionary<Enum, Dictionary<string, string>> GenericColumnLogCache
		{
			get
			{
				return this.genericColumnLogCache;
			}
		}

		internal static RpsCmdletLoggerBuffer Get(Guid cmdletUniqueId)
		{
			if (cmdletUniqueId == Guid.Empty && !CmdletThreadStaticData.TryGetCurrentCmdletUniqueId(out cmdletUniqueId))
			{
				return null;
			}
			RpsCmdletLoggerBuffer rpsCmdletLoggerBuffer;
			if (!CmdletStaticDataWithUniqueId<RpsCmdletLoggerBuffer>.TryGet(cmdletUniqueId, out rpsCmdletLoggerBuffer))
			{
				rpsCmdletLoggerBuffer = new RpsCmdletLoggerBuffer();
				CmdletStaticDataWithUniqueId<RpsCmdletLoggerBuffer>.Set(cmdletUniqueId, rpsCmdletLoggerBuffer);
			}
			return rpsCmdletLoggerBuffer;
		}

		internal void AddMetadataLog(Enum key, object value)
		{
			this.metadataLogCache[key] = value;
		}

		internal void AppendGenericInfo(string key, string value)
		{
			this.genericInfoLogCache[key] = value;
		}

		internal void AppendGenericError(string key, string value)
		{
			this.genericErrorLogCache[key] = value;
		}

		internal void UpdateLatency(Enum latencyMetadata, double latencyInMilliseconds)
		{
			this.latencyLogCache[latencyMetadata] = latencyInMilliseconds;
		}

		internal void AppendColumn(Enum column, string key, string value)
		{
			if (!this.genericColumnLogCache.ContainsKey(column))
			{
				this.genericColumnLogCache[column] = new Dictionary<string, string>();
			}
			this.genericColumnLogCache[column][key] = value;
		}

		internal void Reset()
		{
			this.MetadataLogCache.Clear();
			this.LatencyLogCache.Clear();
			this.GenericInfoLogCache.Clear();
			this.GenericErrorLogCache.Clear();
			this.GenericColumnLogCache.Clear();
		}

		private readonly Dictionary<Enum, object> metadataLogCache = new Dictionary<Enum, object>();

		private readonly Dictionary<string, string> genericInfoLogCache = new Dictionary<string, string>();

		private readonly Dictionary<string, string> genericErrorLogCache = new Dictionary<string, string>();

		private readonly Dictionary<Enum, double> latencyLogCache = new Dictionary<Enum, double>();

		private readonly Dictionary<Enum, Dictionary<string, string>> genericColumnLogCache = new Dictionary<Enum, Dictionary<string, string>>();
	}
}
