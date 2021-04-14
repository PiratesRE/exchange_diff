using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal class RpcHttpConnectionRegistration
	{
		internal RpcHttpConnectionRegistration()
		{
			this.connectionRegistrationCache = new Dictionary<Guid, RpcHttpConnectionRegistrationCacheEntry>();
		}

		public int CacheSize
		{
			get
			{
				int count;
				lock (this.cacheLock)
				{
					count = this.connectionRegistrationCache.Count;
				}
				return count;
			}
		}

		internal static RpcHttpConnectionRegistration Instance
		{
			get
			{
				return RpcHttpConnectionRegistration.instance;
			}
		}

		public void Register(Guid associationGroupId, ClientSecurityContext clientSecurityContext, string authIdentifier, string serverTarget, string sessionCookie, string clientIp, Guid requestId)
		{
			lock (this.cacheLock)
			{
				RpcHttpConnectionRegistrationCacheEntry rpcHttpConnectionRegistrationCacheEntry = null;
				if (!this.connectionRegistrationCache.TryGetValue(associationGroupId, out rpcHttpConnectionRegistrationCacheEntry))
				{
					rpcHttpConnectionRegistrationCacheEntry = new RpcHttpConnectionRegistrationCacheEntry(associationGroupId, clientSecurityContext, authIdentifier, serverTarget, sessionCookie, clientIp);
					this.connectionRegistrationCache.Add(associationGroupId, rpcHttpConnectionRegistrationCacheEntry);
				}
				rpcHttpConnectionRegistrationCacheEntry.AddRequest(requestId, clientSecurityContext);
			}
		}

		public bool TryRegisterAdditionalConnection(Guid associationGroupId, string authIdentifier, Guid requestId)
		{
			bool flag = false;
			string arg = string.Empty;
			lock (this.cacheLock)
			{
				RpcHttpConnectionRegistrationCacheEntry rpcHttpConnectionRegistrationCacheEntry = null;
				if (this.connectionRegistrationCache.TryGetValue(associationGroupId, out rpcHttpConnectionRegistrationCacheEntry))
				{
					if (rpcHttpConnectionRegistrationCacheEntry.AuthIdentifier.Equals(authIdentifier, StringComparison.InvariantCultureIgnoreCase))
					{
						rpcHttpConnectionRegistrationCacheEntry.AddRequest(requestId);
						return true;
					}
					arg = rpcHttpConnectionRegistrationCacheEntry.AuthIdentifier;
					flag = true;
				}
			}
			if (flag)
			{
				throw new ConnectionRegistrationException(string.Format("Association GUID {0} cannot be shared between auth identifiers {1} and {2}", associationGroupId.ToString(), arg, authIdentifier));
			}
			return false;
		}

		public void Unregister(Guid associationGroupId, Guid requestId)
		{
			DateTime utcNow = DateTime.UtcNow;
			lock (this.cacheLock)
			{
				RpcHttpConnectionRegistrationCacheEntry rpcHttpConnectionRegistrationCacheEntry = null;
				if (!this.connectionRegistrationCache.TryGetValue(associationGroupId, out rpcHttpConnectionRegistrationCacheEntry))
				{
					string message = string.Format("Association Group ID '{0}' does not exist in RpcHttpConnectionRegistration cache.", associationGroupId);
					throw new NotFoundException(message);
				}
				rpcHttpConnectionRegistrationCacheEntry.RemoveRequest(requestId);
				if (rpcHttpConnectionRegistrationCacheEntry.RequestIds.Count == 0)
				{
					this.connectionRegistrationCache.Remove(associationGroupId);
					rpcHttpConnectionRegistrationCacheEntry.Dispose();
				}
			}
		}

		public void Clear()
		{
			Dictionary<Guid, RpcHttpConnectionRegistrationCacheEntry> dictionary = new Dictionary<Guid, RpcHttpConnectionRegistrationCacheEntry>();
			Dictionary<Guid, RpcHttpConnectionRegistrationCacheEntry> dictionary2;
			lock (this.cacheLock)
			{
				dictionary2 = this.connectionRegistrationCache;
				this.connectionRegistrationCache = dictionary;
			}
			foreach (RpcHttpConnectionRegistrationCacheEntry rpcHttpConnectionRegistrationCacheEntry in dictionary2.Values)
			{
				rpcHttpConnectionRegistrationCacheEntry.Dispose();
			}
			dictionary2.Clear();
		}

		public bool TryGet(Guid associationGroupId, out ClientSecurityContext clientSecurityContext, out RpcHttpConnectionProperties connectionProperties)
		{
			DateTime utcNow = DateTime.UtcNow;
			clientSecurityContext = null;
			connectionProperties = null;
			bool result;
			lock (this.cacheLock)
			{
				RpcHttpConnectionRegistrationCacheEntry rpcHttpConnectionRegistrationCacheEntry = null;
				if (this.connectionRegistrationCache.TryGetValue(associationGroupId, out rpcHttpConnectionRegistrationCacheEntry))
				{
					string[] requestIds = (from requestId in rpcHttpConnectionRegistrationCacheEntry.RequestIds
					select requestId.ToString()).ToArray<string>();
					connectionProperties = new RpcHttpConnectionProperties(rpcHttpConnectionRegistrationCacheEntry.ClientIp, rpcHttpConnectionRegistrationCacheEntry.ServerTarget, rpcHttpConnectionRegistrationCacheEntry.SessionCookie, requestIds);
					clientSecurityContext = rpcHttpConnectionRegistrationCacheEntry.GetClientSecurityContext();
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		private static readonly RpcHttpConnectionRegistration instance = new RpcHttpConnectionRegistration();

		private readonly object cacheLock = new object();

		private Dictionary<Guid, RpcHttpConnectionRegistrationCacheEntry> connectionRegistrationCache;
	}
}
