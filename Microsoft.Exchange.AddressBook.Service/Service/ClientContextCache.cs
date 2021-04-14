using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AddressBook.Service;
using Microsoft.Exchange.Nspi;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess.Server;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal static class ClientContextCache
	{
		internal static NspiContext CreateContext(ClientBinding clientBinding)
		{
			NspiContext result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				ClientSecurityContext clientSecurityContext = null;
				bool isAnonymous = false;
				string text = null;
				string userDomain = null;
				RpcHttpConnectionProperties rpcHttpConnectionProperties = null;
				if (!RpcDispatch.TryGetAuthContextInfo(clientBinding, out clientSecurityContext, out isAnonymous, out text, out userDomain, out rpcHttpConnectionProperties))
				{
					ExTraceGlobals.NspiTracer.TraceError<Guid>(0L, "Could not resolve anonymous user for session id: {0}", clientBinding.AssociationGuid);
					throw new NspiException(NspiStatus.LogonFailed, "Could not resolve anonymous user.");
				}
				disposeGuard.Add<ClientSecurityContext>(clientSecurityContext);
				Guid empty = Guid.Empty;
				if (rpcHttpConnectionProperties != null && rpcHttpConnectionProperties.RequestIds.Length > 0)
				{
					Guid.TryParse(rpcHttpConnectionProperties.RequestIds[rpcHttpConnectionProperties.RequestIds.Length - 1], out empty);
				}
				NspiContext nspiContext = new NspiContext(clientSecurityContext, userDomain, clientBinding.ClientAddress, clientBinding.ServerAddress, clientBinding.ProtocolSequence, empty);
				disposeGuard.Add<NspiContext>(nspiContext);
				nspiContext.IsAnonymous = isAnonymous;
				if (!nspiContext.TryAcquireBudget())
				{
					ExTraceGlobals.NspiTracer.TraceError((long)nspiContext.ContextHandle, "Could not acquire budget");
					throw new NspiException(NspiStatus.GeneralFailure, "Failed to acquire budget.");
				}
				bool flag = false;
				lock (ClientContextCache.clientContextDictionaryLock)
				{
					flag = ClientContextCache.clientContextDictionary.ContainsKey(nspiContext.ContextHandle);
					if (!flag)
					{
						ClientContextCache.clientContextDictionary.Add(nspiContext.ContextHandle, nspiContext);
						AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.NspiConnectionsCurrent.RawValue = (long)ClientContextCache.clientContextDictionary.Count;
						AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.NspiConnectionsTotal.Increment();
						AddressBookPerformanceCountersWrapper.AddressBookPerformanceCounters.NspiConnectionsRate.Increment();
					}
				}
				if (flag)
				{
					ExTraceGlobals.NspiTracer.TraceError((long)nspiContext.ContextHandle, "Duplicate contextHandle found in context dictionary");
					throw new NspiException(NspiStatus.GeneralFailure, "Duplicate contextHandle found in context dictionary.");
				}
				disposeGuard.Success();
				result = nspiContext;
			}
			return result;
		}

		internal static bool TryGetContext(int contextHandle, out NspiContext context)
		{
			context = null;
			bool result;
			lock (ClientContextCache.clientContextDictionaryLock)
			{
				result = ClientContextCache.clientContextDictionary.TryGetValue(contextHandle, out context);
			}
			return result;
		}

		internal static NspiContext GetContext(int contextHandle)
		{
			NspiContext result = null;
			if (!ClientContextCache.TryGetContext(contextHandle, out result))
			{
				ExTraceGlobals.NspiTracer.TraceError((long)contextHandle, "Unable to find contextHandle in context dictionary");
				throw new InvalidHandleException("Failed to find contextHandle in context dictionary.");
			}
			return result;
		}

		internal static void DeleteContext(int contextHandle)
		{
			lock (ClientContextCache.clientContextDictionaryLock)
			{
				ClientContextCache.clientContextDictionary.Remove(contextHandle);
			}
		}

		private static object clientContextDictionaryLock = new object();

		private static Dictionary<int, NspiContext> clientContextDictionary = new Dictionary<int, NspiContext>(500);
	}
}
