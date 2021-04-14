using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum LastAction
	{
		Open,
		VotingOptionMin,
		VotingOptionMax = 31,
		First = 100,
		ReplyToSender = 102,
		ReplyToAll,
		Forward,
		ReplyToFolder = 108
	}
}
