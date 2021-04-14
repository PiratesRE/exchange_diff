using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc.Common;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal static class RpcUpdateLastLogImpl
	{
		public static void HandleRequest(RpcGenericRequestInfo requestInfo, ref RpcGenericReplyInfo replyInfo)
		{
			RpcGenericReplyInfo tmpReplyInfo = null;
			RpcUpdateLastLogImpl.Request req = null;
			RpcUpdateLastLogImpl.Reply rep = new RpcUpdateLastLogImpl.Reply();
			Exception ex = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
			{
				req = ActiveManagerGenericRpcHelper.ValidateAndGetAttachedRequest<RpcUpdateLastLogImpl.Request>(requestInfo, 1, 0);
				ExDateTime exDateTime = ExDateTime.MinValue;
				AmCachedLastLogUpdater pamCachedLastLogUpdater = AmSystemManager.Instance.PamCachedLastLogUpdater;
				if (pamCachedLastLogUpdater != null)
				{
					exDateTime = pamCachedLastLogUpdater.AddEntries(req.ServerName, req.InitiatedTimeUtc, req.LastLogEntries);
				}
				rep.LastSuccessfulUpdateTimeUtc = exDateTime.UniversalTime;
				tmpReplyInfo = ActiveManagerGenericRpcHelper.PrepareServerReply(requestInfo, rep, 1, 0);
			});
			if (tmpReplyInfo != null)
			{
				replyInfo = tmpReplyInfo;
			}
			if (ex != null)
			{
				throw new AmServerException(ex.Message, ex);
			}
		}

		internal static DateTime Send(AmServerName originatingServer, AmServerName targetServer, Dictionary<string, string> dbLastLogMap)
		{
			RpcUpdateLastLogImpl.Request attachedRequest = new RpcUpdateLastLogImpl.Request(originatingServer.Fqdn, dbLastLogMap);
			RpcGenericRequestInfo requestInfo = ActiveManagerGenericRpcHelper.PrepareClientRequest(attachedRequest, 2, 1, 0);
			RpcUpdateLastLogImpl.Reply reply = ActiveManagerGenericRpcHelper.RunRpcAndGetReply<RpcUpdateLastLogImpl.Reply>(requestInfo, targetServer.Fqdn, RegistryParameters.PamLastLogRpcTimeoutInMsec);
			return reply.LastSuccessfulUpdateTimeUtc;
		}

		internal static KeyValuePair<Guid, long>[] ConvertLastLogDictionaryToKeyValuePairArray(Dictionary<string, string> dbLastLogMap)
		{
			List<KeyValuePair<Guid, long>> list = new List<KeyValuePair<Guid, long>>();
			foreach (KeyValuePair<string, string> keyValuePair in dbLastLogMap)
			{
				Guid key;
				long value;
				if (Guid.TryParse(keyValuePair.Key, out key) && long.TryParse(keyValuePair.Value, out value))
				{
					list.Add(new KeyValuePair<Guid, long>(key, value));
				}
			}
			return list.ToArray();
		}

		public const int MajorVersion = 1;

		public const int MinorVersion = 0;

		public const int CommandCode = 2;

		[Serializable]
		internal class Request
		{
			public Request(string serverFqdn, Dictionary<string, string> dbLastLogMap)
			{
				this.ServerName = serverFqdn;
				this.LastLogEntries = RpcUpdateLastLogImpl.ConvertLastLogDictionaryToKeyValuePairArray(dbLastLogMap);
				this.InitiatedTimeUtc = DateTime.UtcNow;
			}

			public string ServerName { get; private set; }

			public DateTime InitiatedTimeUtc { get; set; }

			public KeyValuePair<Guid, long>[] LastLogEntries { get; private set; }

			public override string ToString()
			{
				return string.Format("ServerName: '{0}' Count: '{1}'", this.ServerName, this.LastLogEntries.Length);
			}
		}

		[Serializable]
		internal class Reply
		{
			public DateTime LastSuccessfulUpdateTimeUtc { get; set; }

			public override string ToString()
			{
				return string.Format("LastSuccessfulUpdateTimeUtc: '{0}'", this.LastSuccessfulUpdateTimeUtc);
			}
		}
	}
}
