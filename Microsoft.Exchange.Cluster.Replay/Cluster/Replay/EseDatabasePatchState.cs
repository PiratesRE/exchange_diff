using System;
using System.Text;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	[Serializable]
	internal class EseDatabasePatchState : IEquatable<EseDatabasePatchState>
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.IncrementalReseederTracer;
			}
		}

		public Version Version { get; private set; }

		public EseDatabasePatchComponent ComponentId { get; private set; }

		public Guid DatabaseGuid { get; private set; }

		public string ServerName { get; private set; }

		public long PageSizeBytes { get; private set; }

		public bool IsPatchCompleted { get; set; }

		public int NumPagesToPatch { get; set; }

		public long FirstDivergedLogGen { get; set; }

		public long LowestRequiredLogGen { get; set; }

		public long HighestRequiredLogGen { get; set; }

		public EseDatabasePatchState.PatchPhase CurrentPatchPhase { get; set; }

		public EseDatabasePatchState(Guid dbGuid, EseDatabasePatchComponent patchComponent, long pageSizeBytes)
		{
			this.Version = EseDatabasePatchState.VersionNumber;
			this.ServerName = Environment.MachineName;
			this.ComponentId = patchComponent;
			this.DatabaseGuid = dbGuid;
			this.PageSizeBytes = pageSizeBytes;
		}

		public bool Equals(EseDatabasePatchState other)
		{
			return other != null && (object.ReferenceEquals(other, this) || (this.Version.Equals(other.Version) && this.ComponentId == other.ComponentId && this.DatabaseGuid.Equals(other.DatabaseGuid) && SharedHelper.StringIEquals(this.ServerName, other.ServerName) && this.PageSizeBytes == other.PageSizeBytes && this.CurrentPatchPhase == other.CurrentPatchPhase && this.IsPatchCompleted == other.IsPatchCompleted && this.NumPagesToPatch == other.NumPagesToPatch && this.FirstDivergedLogGen == other.FirstDivergedLogGen && this.LowestRequiredLogGen == other.LowestRequiredLogGen && this.HighestRequiredLogGen == other.HighestRequiredLogGen));
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(200);
			stringBuilder.AppendFormat("DatabaseGuid={0}, ", this.DatabaseGuid);
			stringBuilder.AppendFormat("Version={0}, ", this.Version);
			stringBuilder.AppendFormat("ComponentId={0}, ", this.ComponentId);
			stringBuilder.AppendFormat("ServerName={0}, ", this.ServerName);
			stringBuilder.AppendFormat("PageSizeBytes={0}, ", this.PageSizeBytes);
			stringBuilder.AppendFormat("CurrentPatchPhase={0}, ", this.CurrentPatchPhase);
			stringBuilder.AppendFormat("IsPatchCompleted={0}, ", this.IsPatchCompleted);
			stringBuilder.AppendFormat("NumPagesToPatch={0}, ", this.NumPagesToPatch);
			stringBuilder.AppendFormat("FirstDivergedLogGen={0}, ", this.FirstDivergedLogGen);
			stringBuilder.AppendFormat("LowestRequiredLogGen={0}, ", this.LowestRequiredLogGen);
			stringBuilder.AppendFormat("HighestRequiredLogGen={0}", this.HighestRequiredLogGen);
			return stringBuilder.ToString();
		}

		public void Validate(Guid dbGuid, long pageSizeBytes, string patchFile)
		{
			bool flag = true;
			if (flag && (this.Version.Major != EseDatabasePatchState.VersionNumber.Major || this.Version.Minor > EseDatabasePatchState.VersionNumber.Minor))
			{
				EseDatabasePatchState.Tracer.TraceError<Version, Version>((long)this.GetHashCode(), "Patch file validation failed! Version number is not supported. Actual: {0}; Supported: {1}", this.Version, EseDatabasePatchState.VersionNumber);
				flag = false;
			}
			if (flag && !SharedHelper.StringIEquals(this.ServerName, Environment.MachineName))
			{
				EseDatabasePatchState.Tracer.TraceError<string, string>((long)this.GetHashCode(), "Patch file validation failed! Actual ServerName: {0}; Expected: {1}", this.ServerName, Environment.MachineName);
				flag = false;
			}
			if (flag && this.PageSizeBytes != pageSizeBytes)
			{
				EseDatabasePatchState.Tracer.TraceError<long, long>((long)this.GetHashCode(), "Patch file validation failed! Actual PageSizeBytes: {0}; Expected: {1}", this.PageSizeBytes, pageSizeBytes);
				flag = false;
			}
			if (flag && !dbGuid.Equals(this.DatabaseGuid))
			{
				EseDatabasePatchState.Tracer.TraceError<Guid, Guid>((long)this.GetHashCode(), "Patch file validation failed! Actual DatabaseGuid: {0}; Expected: {1}", this.DatabaseGuid, dbGuid);
				flag = false;
			}
			if (!flag)
			{
				throw new PagePatchInvalidFileException(patchFile);
			}
		}

		public static readonly Version VersionNumber = new Version(1, 0);

		public enum PatchPhase
		{
			GatheringPatchData,
			GatheringComplete,
			PatchingPages,
			PatchingPagesComplete,
			MovingOldLogs,
			MovingNewLogs,
			LogReplacementComplete
		}
	}
}
