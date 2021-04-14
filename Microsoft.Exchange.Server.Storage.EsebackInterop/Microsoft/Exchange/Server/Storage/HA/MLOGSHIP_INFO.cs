using System;

namespace Microsoft.Exchange.Server.Storage.HA
{
	public struct MLOGSHIP_INFO
	{
		public ESE_LOGSHIP Type
		{
			get
			{
				return this.<backing_store>Type;
			}
			set
			{
				this.<backing_store>Type = value;
			}
		}

		public string Name
		{
			get
			{
				return this.<backing_store>Name;
			}
			set
			{
				this.<backing_store>Name = value;
			}
		}

		private ESE_LOGSHIP <backing_store>Type;

		private string <backing_store>Name;
	}
}
