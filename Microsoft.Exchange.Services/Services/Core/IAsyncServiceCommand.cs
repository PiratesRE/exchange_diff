using System;

namespace Microsoft.Exchange.Services.Core
{
	internal interface IAsyncServiceCommand
	{
		CompleteRequestAsyncCallback CompleteRequestAsyncCallback { get; set; }
	}
}
