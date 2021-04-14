using System;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal abstract class AmDbState : IAmDbState, IDisposable
	{
		internal static string ConstructLastLogTimeStampProperty(string prefix)
		{
			return prefix + "_Time";
		}

		internal AmDbState()
		{
		}

		public void Dispose()
		{
			if (!this.m_fDisposed)
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
		}

		public void Dispose(bool disposing)
		{
			lock (this)
			{
				if (!this.m_fDisposed)
				{
					if (disposing)
					{
						this.CloseHandles();
					}
					this.m_fDisposed = true;
				}
			}
		}

		public void Write(AmDbStateInfo state)
		{
			this.Write(state, false);
		}

		internal bool Write(AmDbStateInfo state, bool isBestEffort)
		{
			bool result = false;
			try
			{
				ActiveManagerServerPerfmon.DatabaseStateInfoWrites.Increment();
				ActiveManagerServerPerfmon.DatabaseStateInfoWritesPerSec.Increment();
				this.WriteInternal(state.DatabaseGuid.ToString(), state.ToString(), state.ActiveServer);
				result = true;
			}
			catch (ClusterException ex)
			{
				AmTrace.Error("Attempt write persistent database state for Database '{0}' failed with error: {1}", new object[]
				{
					state.DatabaseGuid,
					ex
				});
				if (!isBestEffort)
				{
					throw;
				}
			}
			return result;
		}

		public AmDbStateInfo Read(Guid dbGuid)
		{
			return this.Read(dbGuid, false);
		}

		public AmDbStateInfo[] ReadAll()
		{
			return this.ReadAll(false);
		}

		public string ReadStateString(Guid dbGuid)
		{
			string result;
			this.ReadInternal(dbGuid.ToString(), out result);
			return result;
		}

		internal AmDbStateInfo[] ReadAll(bool isBestEffort)
		{
			return this.ReadAllInternal(isBestEffort);
		}

		internal AmDbStateInfo Read(Guid dbGuid, bool isBestEffort)
		{
			AmDbStateInfo result = new AmDbStateInfo(dbGuid);
			string empty = string.Empty;
			try
			{
				bool flag = this.ReadInternal(dbGuid.ToString(), out empty);
				if (flag && !string.IsNullOrEmpty(empty))
				{
					result = AmDbStateInfo.Parse(dbGuid, empty);
				}
			}
			catch (FormatException ex)
			{
				AmTrace.Error("Failed to parse the state info string '{1}' for Database '{0}'. Error: {2}", new object[]
				{
					dbGuid,
					empty,
					ex
				});
				if (!isBestEffort)
				{
					throw new AmInvalidDbStateException(dbGuid, empty, ex);
				}
			}
			catch (ClusterException ex2)
			{
				AmTrace.Error("Attempt read persistent database state for Database '{0}' failed with error: {1}", new object[]
				{
					dbGuid,
					ex2
				});
				if (!isBestEffort)
				{
					throw;
				}
			}
			return result;
		}

		internal void Delete(Guid dbGuid)
		{
			this.DeleteInternal(dbGuid.ToString());
		}

		public void SetLastLogGenerationNumber(Guid dbGuid, long generation)
		{
			this.SetLastLogPropertyInternal(dbGuid.ToString(), generation.ToString());
		}

		public bool GetLastLogGenerationNumber(Guid dbGuid, out long lastLogGenNumber)
		{
			lastLogGenNumber = long.MaxValue;
			string text = dbGuid.ToString();
			string text2;
			bool lastLogPropertyInternal = this.GetLastLogPropertyInternal(text, out text2);
			if (lastLogPropertyInternal && !long.TryParse(text2, out lastLogGenNumber))
			{
				AmTrace.Error("GetLastLogPropertyInternal() returned a value that could not be parsed. DB: '{0}'; Value: {1}", new object[]
				{
					text,
					lastLogGenNumber
				});
				throw new AmLastLogPropertyCorruptedException(text, text2);
			}
			return lastLogPropertyInternal;
		}

		public void SetLastLogGenerationTimeStamp(Guid dbGuid, ExDateTime timeStamp)
		{
			string name = AmDbState.ConstructLastLogTimeStampProperty(dbGuid.ToString());
			this.SetLastLogPropertyInternal(name, timeStamp.ToString("s"));
		}

		public bool GetLastLogGenerationTimeStamp(Guid dbGuid, out ExDateTime lastLogGenTimeStamp)
		{
			string text = AmDbState.ConstructLastLogTimeStampProperty(dbGuid.ToString());
			string empty = string.Empty;
			lastLogGenTimeStamp = ExDateTime.MinValue;
			bool lastLogPropertyInternal = this.GetLastLogPropertyInternal(text, out empty);
			if (lastLogPropertyInternal && !ExDateTime.TryParse(empty, out lastLogGenTimeStamp))
			{
				AmTrace.Error("GetLastLogPropertyInternal() returned a value that could not be parsed. DB: '{0}'; Value: {1}", new object[]
				{
					text,
					empty
				});
				throw new AmLastLogPropertyCorruptedException(text, empty);
			}
			return lastLogPropertyInternal;
		}

		public bool GetLastServerTimeStamp(string serverName, out ExDateTime lastServerTimeStamp)
		{
			string empty = string.Empty;
			lastServerTimeStamp = ExDateTime.MinValue;
			bool lastLogPropertyInternal = this.GetLastLogPropertyInternal(serverName, out empty);
			if (lastLogPropertyInternal && !ExDateTime.TryParse(empty, out lastServerTimeStamp))
			{
				AmTrace.Error("GetLastLogPropertyInternal() returned a value that could not be parsed. serverName: '{0}'; Value: {1}", new object[]
				{
					serverName,
					empty
				});
				throw new AmLastServerTimeStampCorruptedException(serverName, empty);
			}
			return lastLogPropertyInternal;
		}

		public T GetDebugOption<T>(AmServerName serverName, string propertyName, T defaultValue)
		{
			bool flag = false;
			return this.GetDebugOption<T>(serverName, propertyName, defaultValue, out flag);
		}

		public T GetDebugOption<T>(AmServerName serverName, AmDebugOptions dbgOption, T defaultValue)
		{
			bool flag = false;
			return this.GetDebugOption<T>(serverName, dbgOption.ToString(), defaultValue, out flag);
		}

		public T GetDebugOption<T>(AmServerName serverName, string propertyName, T defaultValue, out bool doesValueExist)
		{
			string serverName2 = (serverName != null) ? serverName.NetbiosName : null;
			if (defaultValue is bool)
			{
				int debugOptionInternal = this.GetDebugOptionInternal<int>(serverName2, propertyName, Convert.ToInt32(defaultValue), out doesValueExist);
				return (T)((object)(debugOptionInternal > 0));
			}
			return this.GetDebugOptionInternal<T>(serverName2, propertyName, defaultValue, out doesValueExist);
		}

		internal bool SetDebugOption<T>(AmServerName serverName, string propertyName, T propertyValue)
		{
			string serverName2 = (serverName != null) ? serverName.NetbiosName : null;
			if (propertyValue is bool)
			{
				return this.SetDebugOptionInternal<int>(serverName2, propertyName, Convert.ToInt32(propertyValue));
			}
			return this.SetDebugOptionInternal<T>(serverName2, propertyName, propertyValue);
		}

		internal Guid[] ConvertGuidStringsToGuids(string[] dbGuidStrings)
		{
			Guid[] array = null;
			if (dbGuidStrings != null)
			{
				array = new Guid[dbGuidStrings.Length];
				for (int i = 0; i < dbGuidStrings.Length; i++)
				{
					array[i] = new Guid(dbGuidStrings[i]);
				}
			}
			return array;
		}

		protected abstract void InitializeHandles();

		protected abstract void CloseHandles();

		protected abstract void WriteInternal(string guidStr, string stateInfoStr, AmServerName activeServerName);

		protected abstract bool ReadInternal(string guidStr, out string stateInfoStr);

		protected abstract Guid[] ReadDatabaseGuids(bool isBestEffort);

		protected abstract void DeleteInternal(string guidStr);

		protected abstract void SetLastLogPropertyInternal(string name, string value);

		protected abstract bool GetLastLogPropertyInternal(string name, out string value);

		protected abstract T GetDebugOptionInternal<T>(string serverName, string propertyName, T defaultValue, out bool doesValueExist);

		protected abstract bool SetDebugOptionInternal<T>(string serverName, string propertyName, T propertyValue);

		protected abstract AmDbStateInfo[] ReadAllInternal(bool isBestEffort);

		protected const string DbStateKeyName = "DbState";

		protected const string DebugOptionKeyName = "DebugOption";

		private bool m_fDisposed;
	}
}
