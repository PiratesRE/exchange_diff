using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal enum FreeBusyStatus
	{
		Unknown = -1,
		Free,
		Tentative,
		Busy,
		Oof,
		WorkingElsewhere
	}
}
