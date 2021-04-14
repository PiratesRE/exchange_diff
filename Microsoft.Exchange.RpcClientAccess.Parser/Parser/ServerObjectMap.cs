using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal class ServerObjectMap
	{
		public ServerObjectMap(byte logonIndex)
		{
			this.objectMap = new Dictionary<ServerObjectHandle, IServerObject>();
			this.logonIndex = logonIndex;
			this.counter = 0U;
		}

		public ServerObjectHandle Add(IServerObject serverObject)
		{
			if (serverObject == null)
			{
				throw new ArgumentNullException("serverObject");
			}
			ServerObjectHandle nextFreeHandle = this.GetNextFreeHandle();
			this.objectMap.Add(nextFreeHandle, serverObject);
			return nextFreeHandle;
		}

		public void Add(ServerObjectHandle handle, IServerObject serverObject)
		{
			if (serverObject == null)
			{
				throw new ArgumentNullException("serverObject");
			}
			if ((handle.HandleValue & 16777215U) == 16777215U)
			{
				throw new ArgumentOutOfRangeException("handle");
			}
			this.objectMap.Add(handle, serverObject);
		}

		public bool TryGetValue(ServerObjectHandle handle, out IServerObject serverObject, out ErrorCode errorCode)
		{
			if (handle == ServerObjectHandle.None)
			{
				errorCode = ErrorCode.NullObject;
				serverObject = null;
				return false;
			}
			if (handle.LogonIndex != this.logonIndex)
			{
				errorCode = (ErrorCode)2147942405U;
				serverObject = null;
				return false;
			}
			if (!this.objectMap.TryGetValue(handle, out serverObject) || serverObject == null)
			{
				errorCode = ErrorCode.NullObject;
				return false;
			}
			errorCode = ErrorCode.None;
			return true;
		}

		public void ReleaseAndRemove(IRopHandler handler, ServerObjectHandle handleToRemove)
		{
			IServerObject serverObject;
			if (this.objectMap.TryGetValue(handleToRemove, out serverObject))
			{
				if (handler != null)
				{
					handler.Release(serverObject);
				}
				this.objectMap.Remove(handleToRemove);
			}
		}

		public void ReleaseAll(IRopHandler handler)
		{
			ServerObjectHandle key = new ServerObjectHandle(this.logonIndex, 0U);
			IServerObject serverObject = null;
			if (this.objectMap.TryGetValue(key, out serverObject))
			{
				this.objectMap.Remove(key);
			}
			foreach (IServerObject serverObject2 in this.objectMap.Values)
			{
				handler.Release(serverObject2);
			}
			if (serverObject != null)
			{
				handler.Release(serverObject);
			}
		}

		internal int HandleCount
		{
			get
			{
				return this.objectMap.Count;
			}
		}

		internal IDictionary<ServerObjectHandle, IServerObject> ObjectMap
		{
			get
			{
				return this.objectMap;
			}
		}

		public ServerObjectHandle NextHandleValue
		{
			get
			{
				return new ServerObjectHandle(this.logonIndex, this.counter);
			}
		}

		public object LogonObject
		{
			get
			{
				return this.objectMap[this.LogonServerObjectHandle];
			}
		}

		private ServerObjectHandle LogonServerObjectHandle
		{
			get
			{
				if (this.logonServerObjectHandle == null)
				{
					this.logonServerObjectHandle = new ServerObjectHandle?(ServerObjectHandle.CreateLogonHandle(this.logonIndex));
				}
				return this.logonServerObjectHandle.Value;
			}
		}

		private void IncrementCount()
		{
			this.counter = (this.counter + 1U) % 16777214U;
		}

		private ServerObjectHandle GetNextFreeHandle()
		{
			for (uint num = 0U; num < 16777214U; num += 1U)
			{
				ServerObjectHandle nextHandleValue = this.NextHandleValue;
				this.IncrementCount();
				if (!this.objectMap.ContainsKey(nextHandleValue))
				{
					return nextHandleValue;
				}
			}
			throw new RopExecutionException("No more handles in the ServerObjectMap.", ErrorCode.Memory);
		}

		private const int InvalidCounter = 16777215;

		internal const int MaxCounter = 16777214;

		private readonly IDictionary<ServerObjectHandle, IServerObject> objectMap;

		private readonly byte logonIndex;

		private uint counter;

		private ServerObjectHandle? logonServerObjectHandle;
	}
}
