using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Migration.DataAccessLayer;

namespace Microsoft.Exchange.Migration
{
	internal class MrsAccessorCommand
	{
		protected MrsAccessorCommand(string cmdletName, ICollection<Type> ignoredExceptions, ICollection<Type> transientExceptions)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(cmdletName, "name");
			this.arguments = new Dictionary<string, object>();
			this.cmdletName = cmdletName;
			this.IgnoreExceptions = ignoredExceptions;
			this.TransientExceptions = transientExceptions;
		}

		public PSCommand Command
		{
			get
			{
				PSCommand pscommand = new PSCommand().AddCommand(this.cmdletName);
				foreach (KeyValuePair<string, object> keyValuePair in this.arguments)
				{
					pscommand.AddParameter(keyValuePair.Key, keyValuePair.Value);
				}
				foreach (string parameterName in this.switchParameters)
				{
					pscommand.AddParameter(parameterName);
				}
				return pscommand;
			}
		}

		public object Identity
		{
			get
			{
				return this.arguments["Identity"];
			}
			set
			{
				object value2 = value;
				MRSSubscriptionId mrssubscriptionId = value as MRSSubscriptionId;
				if (mrssubscriptionId != null)
				{
					value2 = mrssubscriptionId.GetIdParameter();
				}
				this.AddParameter("Identity", value2);
			}
		}

		public ICollection<Type> IgnoreExceptions { get; protected set; }

		public ICollection<Type> TransientExceptions { get; protected set; }

		public bool IncludeReport
		{
			set
			{
				this.AddParameter("IncludeReport", value);
			}
		}

		internal bool WhatIf
		{
			set
			{
				this.AddParameter("WhatIf", value);
			}
		}

		public override string ToString()
		{
			return MigrationRunspaceProxy.GetCommandString(this.Command);
		}

		protected virtual void UpdateSubscriptionSettings(ExchangeOutlookAnywhereEndpoint endpoint, ExchangeJobSubscriptionSettings jobSettings, ExchangeJobItemSubscriptionSettings jobItemSettings)
		{
			this.AddParameter("RemoteCredential", endpoint.Credentials);
			string value = string.IsNullOrEmpty(jobItemSettings.RPCProxyServer) ? endpoint.RpcProxyServer.ToString() : jobItemSettings.RPCProxyServer;
			this.AddParameter("OutlookAnywhereHostName", value);
			this.AddParameter("RemoteSourceMailboxServerLegacyDN", jobItemSettings.ExchangeServerDN);
		}

		protected void AddParameter(string parameterName)
		{
			this.switchParameters.Add(parameterName);
		}

		protected void AddParameter(string parameterName, object value)
		{
			this.arguments[parameterName] = value;
		}

		internal const string IdentityParameter = "Identity";

		internal const string IncludeReportParameter = "IncludeReport";

		internal const string MRSInitialConnectionValidation = "InitialConnectionValidation";

		internal const string MRSServerParameter = "MRSServer";

		internal const string NameParameter = "Name";

		internal const string OutlookAnywhereHostNameParameter = "OutlookAnywhereHostName";

		internal const string RemoteCredentialsParameter = "RemoteCredential";

		internal const string RemoteSourceMailboxLegacyDNParameter = "RemoteSourceMailboxLegacyDN";

		internal const string RemoteSourceServerLegacyDNParameter = "RemoteSourceMailboxServerLegacyDN";

		internal const string SkipMergingParameter = "SkipMerging";

		internal const string SuspendWhenReadyToCompleteParameter = "SuspendWhenReadyToComplete";

		internal const string WhatIfParameter = "WhatIf";

		private readonly Dictionary<string, object> arguments;

		private readonly List<string> switchParameters = new List<string>();

		private readonly string cmdletName;
	}
}
