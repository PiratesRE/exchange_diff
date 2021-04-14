using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct RoutingTypeOptionsData
	{
		internal RoutingTypeOptionsData(byte[] messageData, byte[] recipientData, byte[] helpFileName, byte[] helpFileData)
		{
			this.MessageData = messageData;
			this.RecipientData = recipientData;
			this.HelpFileName = helpFileName;
			this.HelpFileData = helpFileData;
		}

		public readonly byte[] MessageData;

		public readonly byte[] RecipientData;

		public readonly byte[] HelpFileName;

		public readonly byte[] HelpFileData;
	}
}
