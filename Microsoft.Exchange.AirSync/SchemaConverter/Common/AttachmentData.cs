using System;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	[Serializable]
	internal struct AttachmentData
	{
		public AttachmentData(string attName, long attSize, int attMethod, string displayName)
		{
			this.AttName = attName;
			this.AttSize = attSize;
			this.AttMethod = attMethod;
			this.DisplayName = displayName;
		}

		public int AttMethod;

		public string AttName;

		public long AttSize;

		public string DisplayName;
	}
}
