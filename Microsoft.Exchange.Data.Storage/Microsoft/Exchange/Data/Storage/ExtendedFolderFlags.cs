using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	public enum ExtendedFolderFlags
	{
		ShowUnread = 1,
		ShowTotal = 2,
		IsSharepointFolder = 4,
		NoShowPolicy = 32,
		ReadOnly = 64,
		HasRssItems = 128,
		WebCalFolder = 256,
		ICalFolder = 512,
		SharedIn = 1024,
		SharedOut = 2048,
		PersonalShare = 8192,
		SharedViaExchange = 32768,
		SFDoNotDelete = 4194304,
		ExclusivelyBound = 33554432,
		RemoteHierarchy = 67108864,
		ExchangeConsumerShareFolder = 536870912,
		ExchangeCrossOrgShareFolder = 1073741824,
		ExchangePublishedCalendar = -2147483648
	}
}
