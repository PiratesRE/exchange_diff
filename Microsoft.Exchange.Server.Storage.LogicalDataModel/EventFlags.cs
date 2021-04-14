using System;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	[Flags]
	public enum EventFlags
	{
		None = 0,
		Associated = 1,
		ContentOnly = 16,
		ModifiedByMove = 128,
		Source = 256,
		Destination = 512,
		SearchFolder = 2048,
		Conversation = 4096,
		CommonCategorizationPropertyChanged = 8192
	}
}
