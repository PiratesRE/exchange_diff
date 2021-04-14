using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CrossServerConnectionPolicy : ICrossServerConnectionPolicy
	{
		public CrossServerConnectionPolicy(ICrossServerDiagnostics crossServerDiagnostics, IClientBehaviorOverrides clientBehaviorOverrides)
		{
			if (crossServerDiagnostics == null)
			{
				throw new ArgumentNullException("crossServerDiagnostics");
			}
			if (clientBehaviorOverrides == null)
			{
				throw new ArgumentNullException("clientBehaviorOverrides");
			}
			this.crossServerDiagnostics = crossServerDiagnostics;
			this.clientBehaviorOverrides = clientBehaviorOverrides;
			this.InitializeCrossServerBehaviorDictionary();
		}

		private void CreateAndAddCrossServerBehaviorToDictionary(string clientId)
		{
			CrossServerBehavior crossServerBehavior = new CrossServerBehavior(clientId, true, false, false, false);
			this.crossServerBehaviors[new Tuple<string, bool>(crossServerBehavior.ClientId.ToLower(), crossServerBehavior.PreExchange15)] = crossServerBehavior;
			crossServerBehavior = new CrossServerBehavior(clientId, false, true, false, false);
			this.crossServerBehaviors[new Tuple<string, bool>(crossServerBehavior.ClientId.ToLower(), crossServerBehavior.PreExchange15)] = crossServerBehavior;
		}

		private void InitializeCrossServerBehaviorDictionary()
		{
			this.CreateAndAddCrossServerBehaviorToDictionary("activesync");
			this.CreateAndAddCrossServerBehaviorToDictionary("approvalapi");
			this.CreateAndAddCrossServerBehaviorToDictionary("as");
			this.CreateAndAddCrossServerBehaviorToDictionary("ci");
			this.CreateAndAddCrossServerBehaviorToDictionary("eba");
			this.CreateAndAddCrossServerBehaviorToDictionary("ediscoverysearch");
			this.CreateAndAddCrossServerBehaviorToDictionary("elc");
			this.CreateAndAddCrossServerBehaviorToDictionary("hub");
			this.CreateAndAddCrossServerBehaviorToDictionary("hub transport");
			this.CreateAndAddCrossServerBehaviorToDictionary("management");
			this.CreateAndAddCrossServerBehaviorToDictionary("monitoring");
			this.CreateAndAddCrossServerBehaviorToDictionary("msexchangemigration");
			this.CreateAndAddCrossServerBehaviorToDictionary("msexchangerpc");
			this.CreateAndAddCrossServerBehaviorToDictionary("msexchangesimplemigration");
			this.CreateAndAddCrossServerBehaviorToDictionary("outlookservice");
			this.CreateAndAddCrossServerBehaviorToDictionary("owa");
			this.CreateAndAddCrossServerBehaviorToDictionary("pop3/imap4");
			this.CreateAndAddCrossServerBehaviorToDictionary("publicfoldersystem");
			this.CreateAndAddCrossServerBehaviorToDictionary("tba");
			this.CreateAndAddCrossServerBehaviorToDictionary("teammailbox");
			this.CreateAndAddCrossServerBehaviorToDictionary("transportsync");
			this.CreateAndAddCrossServerBehaviorToDictionary("um");
			this.CreateAndAddCrossServerBehaviorToDictionary("unifiedpolicy");
			this.CreateAndAddCrossServerBehaviorToDictionary("webservices");
		}

		public void Apply(ExRpcConnectionInfo connectionInfo)
		{
			CrossServerBehavior crossServerBehavior = null;
			CrossServerBehavior crossServerBehavior2 = null;
			if (connectionInfo == null)
			{
				throw new ArgumentNullException("connectionInfo");
			}
			if ((connectionInfo.ConnectFlags & ConnectFlag.ConnectToExchangeRpcServerOnly) == ConnectFlag.ConnectToExchangeRpcServerOnly)
			{
				return;
			}
			if (ExEnvironment.IsTestProcess)
			{
				return;
			}
			if (!connectionInfo.IsCrossServer)
			{
				return;
			}
			bool flag = (connectionInfo.ConnectFlags & ConnectFlag.IsPreExchange15) == ConnectFlag.IsPreExchange15;
			string normalizedClientInfoWithoutPrefix = connectionInfo.ApplicationId.GetNormalizedClientInfoWithoutPrefix();
			if (this.CheckAndBlockMonitoringMailboxes(connectionInfo))
			{
				return;
			}
			CrossServerBehavior crossServerBehavior3 = flag ? CrossServerConnectionPolicy.defaultE14CrossServerConnectionBehavior : CrossServerConnectionPolicy.defaultE15CrossServerConnectionBehavior;
			if (!this.crossServerBehaviors.TryGetValue(new Tuple<string, bool>(normalizedClientInfoWithoutPrefix, flag), out crossServerBehavior))
			{
				crossServerBehavior = crossServerBehavior3;
			}
			if (this.clientBehaviorOverrides.TryGetClientSpecificOverrides(normalizedClientInfoWithoutPrefix, crossServerBehavior3, out crossServerBehavior2))
			{
				crossServerBehavior = crossServerBehavior2;
			}
			if (crossServerBehavior.ShouldTrace)
			{
				this.crossServerDiagnostics.TraceCrossServerCall(connectionInfo.ServerDn);
			}
			if (crossServerBehavior.ShouldLogInfoWatson)
			{
				this.crossServerDiagnostics.LogInfoWatson(connectionInfo);
			}
			if (crossServerBehavior.ShouldBlock)
			{
				this.crossServerDiagnostics.BlockCrossServerCall(connectionInfo);
			}
		}

		private bool CheckAndBlockMonitoringMailboxes(ExRpcConnectionInfo connectionInfo)
		{
			if (connectionInfo.IsCrossServer && (connectionInfo.ConnectFlags & ConnectFlag.MonitoringMailbox) == ConnectFlag.MonitoringMailbox && connectionInfo.ApplicationId.ClientType != MapiClientType.ContentIndexing)
			{
				this.crossServerDiagnostics.BlockMonitoringCrossServerCall(connectionInfo);
				return true;
			}
			return false;
		}

		private ICrossServerDiagnostics crossServerDiagnostics;

		private IClientBehaviorOverrides clientBehaviorOverrides;

		private static CrossServerBehavior defaultE14CrossServerConnectionBehavior = new CrossServerBehavior(string.Empty, true, false, false, false);

		private static CrossServerBehavior defaultE15CrossServerConnectionBehavior = new CrossServerBehavior(string.Empty, false, true, true, true);

		private Dictionary<Tuple<string, bool>, CrossServerBehavior> crossServerBehaviors = new Dictionary<Tuple<string, bool>, CrossServerBehavior>();
	}
}
