using System;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class UMIPGatewayMwiErrorStrategy : MwiFailureEventLogStrategy
	{
		internal override void LogFailure(MwiMessage message, Exception ex)
		{
			string obj = this.ConstructErrorMessage(message, ex);
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_MwiMessageDeliveryFailed, null, new object[]
			{
				message.UnreadVoicemailCount,
				message.TotalVoicemailCount - message.UnreadVoicemailCount,
				message.MailboxDisplayName,
				message.UserExtension,
				CommonUtil.ToEventLogString(obj)
			});
		}
	}
}
