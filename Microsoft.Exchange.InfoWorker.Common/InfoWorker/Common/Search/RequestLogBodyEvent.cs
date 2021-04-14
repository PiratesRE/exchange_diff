using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.InfoWorker.Common.Search
{
	internal class RequestLogBodyEvent : EventArgs
	{
		internal RequestLogBodyEvent(Body itemBody)
		{
			this.itemBody = itemBody;
		}

		internal Body ItemBody
		{
			get
			{
				return this.itemBody;
			}
		}

		private Body itemBody;
	}
}
