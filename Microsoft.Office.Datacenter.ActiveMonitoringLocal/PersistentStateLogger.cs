using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal class PersistentStateLogger : DisposeTrackableBase
	{
		internal PersistentStateLogger(ILogConfiguration configuration)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			this.configuration = configuration;
			if (this.configuration.IsLoggingEnabled)
			{
				string version = PersistentStateLogger.assembly.GetName().Version.ToString();
				this.logSchema = new LogSchema("Microsoft Exchange Server", version, this.configuration.LogType, PersistentStateLogger.Fields);
				this.log = new Log(this.configuration.LogPrefix, null, this.configuration.LogComponent);
				this.log.Configure(this.configuration.LogPath, this.configuration.MaxLogAge, this.configuration.MaxLogDirectorySizeInBytes, this.configuration.MaxLogFileSizeInBytes, 0, new TimeSpan(0, 1, 0));
			}
		}

		public void SetForeLogFileRollOver(bool flag)
		{
			if (!this.configuration.IsLoggingEnabled)
			{
				return;
			}
			this.log.TestHelper_ForceLogFileRollOver = flag;
		}

		public string GetLogFile()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(this.configuration.LogPath);
			IEnumerable<FileInfo> enumerable = (from f in directoryInfo.GetFiles()
			orderby f.LastWriteTime descending
			select f).Take(3);
			int num = 0;
			int num2 = 0;
			if (enumerable.Count<FileInfo>() <= 0)
			{
				return null;
			}
			foreach (FileInfo fileInfo in enumerable)
			{
				using (StreamReader streamReader = new StreamReader(fileInfo.FullName))
				{
					string text = string.Empty;
					while ((text = streamReader.ReadLine()) != null)
					{
						if (text.Contains(LocalDataAccess.PersistentStateIdentity))
						{
							bool resultSize = this.GetResultSize(text, out num2);
							if (resultSize && num2 != 0 && num == num2)
							{
								return fileInfo.FullName;
							}
							break;
						}
						else
						{
							num++;
						}
					}
				}
			}
			return null;
		}

		public void LogEvent(string tempResult)
		{
			if (!this.configuration.IsLoggingEnabled)
			{
				return;
			}
			LogRowFormatter row = this.CreateRow(tempResult);
			this.log.Append(row, -1);
		}

		public bool GetResultSize(string str, out int resultSize)
		{
			string[] array = str.Split(new char[]
			{
				'|'
			});
			return int.TryParse(array[11], out resultSize);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PersistentStateLogger>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.log != null)
			{
				this.log.Flush();
				this.log.Close();
			}
		}

		private LogRowFormatter CreateRow(string data)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.logSchema, true);
			logRowFormatter[0] = data;
			return logRowFormatter;
		}

		private const string SoftwareName = "Microsoft Exchange Server";

		private static readonly string[] Fields = Enum.GetNames(typeof(PersistentStateLogger.Field));

		private static readonly Assembly assembly = Assembly.GetExecutingAssembly();

		private readonly LogSchema logSchema;

		private readonly Log log;

		private readonly ILogConfiguration configuration;

		private enum Field
		{
			Data
		}
	}
}
