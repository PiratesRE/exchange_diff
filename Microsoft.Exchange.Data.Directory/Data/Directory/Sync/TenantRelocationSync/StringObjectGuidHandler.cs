using System;

namespace Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync
{
	internal class StringObjectGuidHandler : ObjectGuidHandler<string>
	{
		private StringObjectGuidHandler()
		{
		}

		public static StringObjectGuidHandler Instance
		{
			get
			{
				if (StringObjectGuidHandler.instance == null)
				{
					StringObjectGuidHandler.instance = new StringObjectGuidHandler();
				}
				return StringObjectGuidHandler.instance;
			}
		}

		private static StringObjectGuidHandler instance;
	}
}
