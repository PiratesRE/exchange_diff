using System;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmDbMoveArguments
	{
		public AmDbMoveArguments(AmDbActionCode actionCode)
		{
			this.SourceServer = AmServerName.Empty;
			this.TargetServer = AmServerName.Empty;
			this.MountFlags = MountFlags.None;
			this.DismountFlags = UnmountFlags.SkipCacheFlush;
			this.MoveComment = ReplayStrings.AmBcsNoneSpecified;
			this.MountDialOverride = DatabaseMountDialOverride.Lossless;
			this.TryOtherHealthyServers = true;
			this.SkipValidationChecks = AmBcsSkipFlags.None;
			this.ActionCode = actionCode;
		}

		internal AmServerName SourceServer { get; set; }

		internal AmServerName TargetServer { get; set; }

		internal MountFlags MountFlags { get; set; }

		internal UnmountFlags DismountFlags { get; set; }

		internal DatabaseMountDialOverride MountDialOverride { get; set; }

		internal bool TryOtherHealthyServers { get; set; }

		internal AmBcsSkipFlags SkipValidationChecks { get; set; }

		internal AmDbActionCode ActionCode { get; set; }

		internal string MoveComment
		{
			get
			{
				return this.m_moveComment;
			}
			set
			{
				this.m_moveComment = value;
				if (string.IsNullOrEmpty(this.m_moveComment))
				{
					this.m_moveComment = ReplayStrings.AmBcsNoneSpecified;
				}
			}
		}

		internal string ComponentName { get; set; }

		private string m_moveComment;
	}
}
