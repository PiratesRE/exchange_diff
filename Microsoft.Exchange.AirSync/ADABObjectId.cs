using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ABProviderFramework;

namespace Microsoft.Exchange.AirSync
{
	[Serializable]
	internal sealed class ADABObjectId : ABObjectId
	{
		public ADABObjectId(ADObjectId activeDirectoryObjectId)
		{
			if (activeDirectoryObjectId == null)
			{
				throw new ArgumentNullException("activeDirectoryObjectId");
			}
			this.activeDirectoryObjectId = activeDirectoryObjectId;
		}

		public ADObjectId NativeId
		{
			get
			{
				return this.activeDirectoryObjectId;
			}
		}

		public override byte[] GetBytes()
		{
			return this.activeDirectoryObjectId.GetBytes();
		}

		private ADObjectId activeDirectoryObjectId;
	}
}
