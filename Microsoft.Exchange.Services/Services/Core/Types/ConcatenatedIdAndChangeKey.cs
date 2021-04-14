using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal struct ConcatenatedIdAndChangeKey
	{
		public ConcatenatedIdAndChangeKey(string id, string changeKey)
		{
			this.id = id;
			this.changeKey = changeKey;
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		public string ChangeKey
		{
			get
			{
				return this.changeKey;
			}
		}

		private string id;

		private string changeKey;
	}
}
