using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Storage.ActiveManager;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmSystemSwitchoverOnAdminMove : AmSystemMoveBase
	{
		internal AmSystemSwitchoverOnAdminMove(AmDbMoveArguments args) : base(args.SourceServer)
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
			ReplayCrimsonEvents.InitiatingServerMoveAllDatabases.Log<AmServerName>(this.m_nodeName);
		}

		protected override void LogCompletionInternal()
		{
			AmTrace.Debug("Finished {0} for {1}", new object[]
			{
				base.GetType().Name,
				this.m_nodeName
			});
			ReplayCrimsonEvents.CompletedServerMoveAllDatabases.Log<AmServerName, int, string>(this.m_nodeName, this.m_moveRequests, this.Arguments.MoveComment);
		}

		protected override void RunInternal()
		{
			AmDbActionCode actionCode = new AmDbActionCode(AmDbActionInitiator.Admin, AmDbActionReason.Cmdlet, AmDbActionCategory.Move);
			base.MoveDatabases(actionCode);
		}

		protected override void PrepareMoveArguments(ref AmDbMoveArguments moveArgs)
		{
			moveArgs = this.Arguments;
		}
	}
}
