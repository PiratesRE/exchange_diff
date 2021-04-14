using System;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class PingOwa : ServiceCommand<int>
	{
		public PingOwa(CallContext callContext) : base(callContext)
		{
		}

		protected override int InternalExecute()
		{
			return 0;
		}
	}
}
