using System;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[Flags]
	internal enum ContentRight
	{
		None = 0,
		View = 1,
		Edit = 2,
		Print = 4,
		Extract = 8,
		ObjectModel = 16,
		Owner = 32,
		ViewRightsData = 64,
		Forward = 128,
		Reply = 256,
		ReplyAll = 512,
		Sign = 1024,
		DocumentEdit = 2048,
		Export = 4096,
		EditRightsData = 8192
	}
}
