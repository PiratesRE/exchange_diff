using System;

namespace Microsoft.Exchange.Services.Core.Search
{
	internal class BasePageResult
	{
		public BasePageResult(BaseQueryView view)
		{
			this.view = view;
		}

		public BaseQueryView View
		{
			get
			{
				return this.view;
			}
		}

		private BaseQueryView view;
	}
}
