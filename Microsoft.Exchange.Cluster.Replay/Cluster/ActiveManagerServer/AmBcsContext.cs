using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AmBcsContext
	{
		public AmBcsContext(Guid dbGuid, AmServerName sourceServerName, IAmBcsErrorLogger errorLogger)
		{
			this.DatabaseGuid = dbGuid;
			this.SourceServerName = sourceServerName;
			this.IsSourceServerAllowedForMount = false;
			this.ErrorLogger = errorLogger;
			this.StatusTable = new Dictionary<AmServerName, RpcDatabaseCopyStatus2>();
		}

		public Guid DatabaseGuid { get; set; }

		public IADDatabase Database
		{
			get
			{
				return this.m_database;
			}
			set
			{
				this.m_database = value;
			}
		}

		public bool DatabaseNeverMounted { get; set; }

		public bool SortCopiesByActivationPreference { get; set; }

		public AmBcsSkipFlags SkipValidationChecks { get; set; }

		public bool IsSourceServerAllowedForMount { get; set; }

		public AmDbActionCode ActionCode { get; set; }

		public AmServerName SourceServerName { get; set; }

		public Dictionary<AmServerName, RpcDatabaseCopyStatus2> StatusTable { get; set; }

		public ComponentStateWrapper ComponentStateWrapper { get; set; }

		public IAmBcsErrorLogger ErrorLogger { get; set; }

		public string InitiatingComponent { get; set; }

		public AmDbAction.PrepareSubactionArgsDelegate PrepareSubaction { get; set; }

		public bool ShouldLogSubactionEvent
		{
			get
			{
				return this.PrepareSubaction != null;
			}
		}

		public string GetDatabaseNameOrGuid()
		{
			if (this.Database == null)
			{
				return this.DatabaseGuid.ToString();
			}
			return this.Database.Name;
		}

		private IADDatabase m_database;
	}
}
