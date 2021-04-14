using System;

namespace Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync
{
	internal class BinaryObjectGuidHandler : ObjectGuidHandler<byte[]>
	{
		private BinaryObjectGuidHandler()
		{
		}

		public static BinaryObjectGuidHandler Instance
		{
			get
			{
				if (BinaryObjectGuidHandler.instance == null)
				{
					BinaryObjectGuidHandler.instance = new BinaryObjectGuidHandler();
				}
				return BinaryObjectGuidHandler.instance;
			}
		}

		private static BinaryObjectGuidHandler instance;
	}
}
