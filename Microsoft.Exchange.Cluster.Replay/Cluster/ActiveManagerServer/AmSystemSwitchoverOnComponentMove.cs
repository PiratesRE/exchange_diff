using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmSystemSwitchoverOnComponentMove : AmSystemMoveBase
	{
		internal AmSystemSwitchoverOnComponentMove(AmDbMoveArguments args) : base(args.SourceServer)
		{
			this.Arguments = args;
		}

		internal AmDbMoveArguments Arguments { get; private set; }

		protected override void LogStartupInternal()
		{
			AmTrace.Debug("Starting {0} for {1}", new object[]
			{
				base.GetType().Name,
				this.m_nodeName
			});
			ReplayCrimsonEvents.InitiatingServerMoveAllDatabasesByComponentRequest.Log<AmServerName, string>(this.m_nodeName, this.Arguments.ComponentName);
		}

		protected override void LogCompletionInternal()
		{
			AmTrace.Debug("Finished {0} for {1}", new object[]
			{
				base.GetType().Name,
				this.m_nodeName
			});
			ReplayCrimsonEvents.CompletedServerMoveAllDatabasesByComponentRequest.Log<AmServerName, int, string, string>(this.m_nodeName, this.m_moveRequests, this.Arguments.MoveComment, this.Arguments.ComponentName);
		}

		protected override void RunInternal()
		{
			base.MoveDatabases(this.Arguments.ActionCode);
		}

		protected override void PrepareMoveArguments(ref AmDbMoveArguments moveArgs)
		{
			moveArgs = this.Arguments;
		}
	}
}
