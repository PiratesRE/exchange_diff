using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.QueueProcessing;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;

namespace Microsoft.Exchange.MailboxLoadBalance.MailboxProcessors
{
	internal class GetMoveInfo : IGetMoveInfo
	{
		public GetMoveInfo(ILogger logger, CmdletExecutionPool cmdletPool)
		{
			this.logger = logger;
			this.cmdletPool = cmdletPool;
		}

		public MoveInfo GetInfo(DirectoryMailbox mailbox, IAnchorRunspaceProxy runspace)
		{
			GetMoveRequestStatistics getMoveRequestStatistics = new GetMoveRequestStatistics(mailbox, this.logger, this.cmdletPool);
			getMoveRequestStatistics.Process();
			if (getMoveRequestStatistics.Result == null)
			{
				return new MoveInfo(MoveStatus.MoveDoesNotExist, Guid.Empty);
			}
			MoveStatus status;
			if (getMoveRequestStatistics.Result.Status == RequestStatus.InProgress || getMoveRequestStatistics.Result.Status == RequestStatus.Queued)
			{
				status = MoveStatus.MoveExistsInProgress;
			}
			else
			{
				status = MoveStatus.MoveExistsNotInProgress;
			}
			return new MoveInfo(status, getMoveRequestStatistics.Result.RequestGuid);
		}

		private readonly ILogger logger;

		private readonly CmdletExecutionPool cmdletPool;
	}
}
