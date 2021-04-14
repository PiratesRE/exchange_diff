using System;
using System.Diagnostics;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc.ActiveManager;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmConfig
	{
		internal AmConfig() : this(ReplayStrings.ErrorAmConfigNotInitialized)
		{
		}

		internal AmConfig(string lastError)
		{
			this.Initialize(AmRole.Unknown, null, null, lastError);
		}

		internal AmConfig(AmRole role, IAmDbState dbState, AmDagConfig dagCfg, string lastError)
		{
			this.Initialize(role, dbState, dagCfg, lastError);
		}

		internal static AmConfig UnknownConfig
		{
			get
			{
				return AmConfig.sm_unknownConfig;
			}
		}

		internal bool IsInternalObjectsDisposed { get; set; }

		internal bool IsCurrentConfiguration { get; set; }

		internal AmRole Role { get; set; }

		internal IAmDbState DbState { get; set; }

		internal AmDagConfig DagConfig { get; set; }

		internal string LastError { get; set; }

		internal bool IsUnknownTriggeredByADError { get; set; }

		internal ExDateTime TimeCreated { get; private set; }

		internal ExDateTime TimeRoleLastChanged { get; set; }

		internal Stopwatch PeriodicEventWatch { get; private set; }

		internal bool IsPAM
		{
			get
			{
				return this.Role == AmRole.PAM;
			}
		}

		internal bool IsSAM
		{
			get
			{
				return this.Role == AmRole.SAM;
			}
		}

		internal bool IsStandalone
		{
			get
			{
				return this.Role == AmRole.Standalone;
			}
		}

		internal bool IsDecidingAuthority
		{
			get
			{
				return this.IsPAM || this.IsStandalone;
			}
		}

		internal bool IsPamOrSam
		{
			get
			{
				return this.IsPAM || this.IsSAM;
			}
		}

		internal bool IsUnknown
		{
			get
			{
				return this.Role == AmRole.Unknown;
			}
		}

		internal static AmConfigChangedFlags CheckForChanges(AmConfig left, AmConfig right)
		{
			AmConfigChangedFlags amConfigChangedFlags = AmConfigChangedFlags.None;
			if (left.Role != right.Role)
			{
				amConfigChangedFlags |= AmConfigChangedFlags.Role;
			}
			if (!object.ReferenceEquals(left.DbState, right.DbState))
			{
				amConfigChangedFlags |= AmConfigChangedFlags.DbState;
			}
			if (!SharedHelper.StringIEquals(left.LastError, right.LastError))
			{
				amConfigChangedFlags |= AmConfigChangedFlags.LastError;
			}
			if ((left.DagConfig == null && right.DagConfig != null) || (left.DagConfig != null && right.DagConfig == null))
			{
				amConfigChangedFlags |= AmConfigChangedFlags.DagConfig;
			}
			if (left.DagConfig != null && right.DagConfig != null)
			{
				if (!left.DagConfig.Id.Equals(right.DagConfig.Id))
				{
					amConfigChangedFlags |= AmConfigChangedFlags.DagId;
				}
				if (!AmServerName.IsArrayEquals(left.DagConfig.MemberServers, right.DagConfig.MemberServers))
				{
					amConfigChangedFlags |= AmConfigChangedFlags.MemberServers;
				}
				if (!AmServerName.IsEqual(left.DagConfig.CurrentPAM, right.DagConfig.CurrentPAM))
				{
					amConfigChangedFlags |= AmConfigChangedFlags.CurrentPAM;
				}
				if (!object.ReferenceEquals(left.DagConfig.Cluster, right.DagConfig.Cluster))
				{
					amConfigChangedFlags |= AmConfigChangedFlags.Cluster;
				}
			}
			return amConfigChangedFlags;
		}

		private void Initialize(AmRole role, IAmDbState dbState, AmDagConfig dagCfg, string lastError)
		{
			this.Role = role;
			this.DbState = dbState;
			this.DagConfig = dagCfg;
			this.LastError = lastError;
			this.TimeCreated = ExDateTime.Now;
			this.TimeRoleLastChanged = this.TimeCreated;
			this.PeriodicEventWatch = new Stopwatch();
			this.PeriodicEventWatch.Start();
			this.IsUnknownTriggeredByADError = false;
			this.IsCurrentConfiguration = true;
			this.IsInternalObjectsDisposed = false;
		}

		private static AmConfig sm_unknownConfig = new AmConfig();
	}
}
