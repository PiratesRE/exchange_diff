using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal class LogonMap : BaseObject, IParseLogonTracker
	{
		internal LogonMap(IConnectionHandler handler)
		{
			this.handler = handler;
			this.logonMap = new Dictionary<byte, LogonMap.Logon>();
			this.parseLogonMap = new Dictionary<byte, LogonFlags>();
			this.parseHandleTable = new Dictionary<byte, byte>();
			this.parseTrackingEnabled = false;
		}

		internal int LogonCount
		{
			get
			{
				return this.logonMap.Count;
			}
		}

		private IRopHandler RopHandler
		{
			get
			{
				return this.handler.RopHandler;
			}
		}

		internal ServerObjectMap CreateLogon(byte logonIndex, LogonFlags logonFlags)
		{
			LogonMap.Logon logon = new LogonMap.Logon(logonIndex, logonFlags);
			if (this.logonMap.ContainsKey(logonIndex))
			{
				this.ReleaseLogon(logonIndex);
			}
			this.logonMap.Add(logonIndex, logon);
			return logon.ServerObjectMap;
		}

		internal void ReleaseLogon(byte logonIndex)
		{
			LogonMap.Logon logon;
			if (this.logonMap.TryGetValue(logonIndex, out logon))
			{
				logon.ServerObjectMap.ReleaseAll(this.RopHandler);
			}
			this.logonMap.Remove(logonIndex);
		}

		internal void ReleaseHandle(byte logonIndex, ServerObjectHandle handleToRelease)
		{
			if (handleToRelease.IsLogonHandle(logonIndex))
			{
				this.ReleaseLogon(logonIndex);
				return;
			}
			LogonMap.Logon logon;
			if (!this.logonMap.TryGetValue(logonIndex, out logon))
			{
				return;
			}
			logon.ServerObjectMap.ReleaseAndRemove(this.RopHandler, handleToRelease);
		}

		internal bool TryGetServerObjectMap(byte logonIndex, out ServerObjectMap serverObjectMap, out ErrorCode errorCode)
		{
			LogonMap.Logon logon;
			if (this.logonMap.TryGetValue(logonIndex, out logon))
			{
				errorCode = ErrorCode.None;
				serverObjectMap = logon.ServerObjectMap;
				return true;
			}
			errorCode = ErrorCode.NoSuchLogon;
			serverObjectMap = null;
			return false;
		}

		internal bool TryGetServerObject(byte logonIndex, ServerObjectHandle handle, out IServerObject serverObject, out ErrorCode errorCode)
		{
			serverObject = null;
			ServerObjectMap serverObjectMap;
			return this.TryGetServerObjectMap(logonIndex, out serverObjectMap, out errorCode) && serverObjectMap.TryGetValue(handle, out serverObject, out errorCode);
		}

		public void ParseBegin(ServerObjectHandleTable serverObjectHandleTable)
		{
			this.parseLogonMap.Clear();
			this.parseHandleTable.Clear();
			this.parseTrackingEnabled = true;
			Dictionary<ServerObjectHandle, byte> dictionary = new Dictionary<ServerObjectHandle, byte>(this.logonMap.Count);
			foreach (KeyValuePair<byte, LogonMap.Logon> keyValuePair in this.logonMap)
			{
				byte key = keyValuePair.Key;
				LogonMap.Logon value = keyValuePair.Value;
				dictionary.Add(new ServerObjectHandle(key, 0U), key);
				this.parseLogonMap.Add(key, value.LogonFlags);
			}
			int highestIndexAccessed = serverObjectHandleTable.HighestIndexAccessed;
			byte b = 0;
			while ((int)b < serverObjectHandleTable.LastIndex)
			{
				byte value2;
				if (dictionary.TryGetValue(serverObjectHandleTable[(int)b], out value2))
				{
					this.parseHandleTable.Add(b, value2);
				}
				b += 1;
			}
			serverObjectHandleTable.HighestIndexAccessed = highestIndexAccessed;
		}

		public void ParseEnd()
		{
			this.parseLogonMap.Clear();
			this.parseHandleTable.Clear();
			this.parseTrackingEnabled = false;
		}

		public void ParseRecordLogon(byte logonIndex, byte handleTableIndex, LogonFlags logonFlags)
		{
			if (!this.parseTrackingEnabled)
			{
				throw new InvalidOperationException("Not currently in parsing state");
			}
			this.parseHandleTable[handleTableIndex] = logonIndex;
			this.parseLogonMap[logonIndex] = logonFlags;
		}

		public void ParseRecordRelease(byte handleTableIndex)
		{
			if (!this.parseTrackingEnabled)
			{
				throw new InvalidOperationException("Not currently in parsing state");
			}
			byte key;
			if (this.parseHandleTable.TryGetValue(handleTableIndex, out key))
			{
				this.parseLogonMap.Remove(key);
				this.parseHandleTable.Remove(handleTableIndex);
			}
		}

		public void ParseRecordInputOutput(byte handleTableIndex)
		{
			if (!this.parseTrackingEnabled)
			{
				throw new InvalidOperationException("Not currently in parsing state");
			}
			this.parseHandleTable.Remove(handleTableIndex);
		}

		public bool ParseIsValidLogon(byte logonIndex)
		{
			if (!this.parseTrackingEnabled)
			{
				throw new InvalidOperationException("Not currently in parsing state");
			}
			return this.parseLogonMap.ContainsKey(logonIndex);
		}

		public bool ParseIsPublicLogon(byte logonIndex)
		{
			if (!this.parseTrackingEnabled)
			{
				throw new InvalidOperationException("Not currently in parsing state");
			}
			LogonFlags logonFlags;
			return this.parseLogonMap.TryGetValue(logonIndex, out logonFlags) && (byte)(logonFlags & LogonFlags.Private) == 0;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<LogonMap>(this);
		}

		protected override void InternalDispose()
		{
			foreach (byte key in this.logonMap.Keys)
			{
				LogonMap.Logon logon;
				if (this.logonMap.TryGetValue(key, out logon))
				{
					logon.ServerObjectMap.ReleaseAll(this.RopHandler);
				}
			}
			base.InternalDispose();
		}

		private readonly IConnectionHandler handler;

		private readonly Dictionary<byte, LogonMap.Logon> logonMap;

		private readonly Dictionary<byte, LogonFlags> parseLogonMap;

		private readonly Dictionary<byte, byte> parseHandleTable;

		private bool parseTrackingEnabled;

		private class Logon
		{
			internal Logon(byte logonIndex, LogonFlags logonFlags)
			{
				this.serverObjectMap = new ServerObjectMap(logonIndex);
				this.logonFlags = logonFlags;
			}

			internal ServerObjectMap ServerObjectMap
			{
				get
				{
					return this.serverObjectMap;
				}
			}

			internal LogonFlags LogonFlags
			{
				get
				{
					return this.logonFlags;
				}
			}

			private readonly ServerObjectMap serverObjectMap;

			private readonly LogonFlags logonFlags;
		}
	}
}
