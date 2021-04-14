using System;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.LogSearch;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	internal class LogSearchClient : LogSearchRpcClient
	{
		public LogSearchClient(string server, ServerVersion version) : base(server)
		{
			if (version == null)
			{
				throw new ArgumentNullException("version must not be null.");
			}
			this.remoteServerVersion = version;
		}

		public int Search(string logName, LogQuery query, bool continueInBackground, byte[] results, out Guid sessionId, out bool more, out int progress)
		{
			byte[] query2 = LogSearchClient.SerializeQuery(query);
			sessionId = Guid.Empty;
			more = false;
			progress = 0;
			if (this.remoteServerVersion >= LogSearchConstants.LowestModernInterfaceBuildVersion)
			{
				return base.SearchExtensibleSchema(CsvFieldCache.LocalVersion.ToString(), logName, query2, continueInBackground, results, ref sessionId, ref more, ref progress);
			}
			return base.Search(logName, query2, continueInBackground, results, ref sessionId, ref more, ref progress);
		}

		public new int Continue(Guid sessionId, bool continueInBackground, byte[] results, out bool more, out int progress)
		{
			more = false;
			progress = 0;
			return base.Continue(sessionId, continueInBackground, results, ref more, ref progress);
		}

		private static byte[] SerializeQuery(LogQuery query)
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.OmitXmlDeclaration = true;
			xmlWriterSettings.Encoding = Encoding.UTF8;
			xmlWriterSettings.Indent = false;
			byte[] result = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings))
				{
					LogSearchClient.logQuerySerializer.Serialize(xmlWriter, query);
					result = memoryStream.ToArray();
				}
			}
			return result;
		}

		private static LogQuerySerializer logQuerySerializer = new LogQuerySerializer();

		private ServerVersion remoteServerVersion;
	}
}
