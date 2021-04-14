using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct EntitiesTags
	{
		public const int Common = 0;

		public const int Converters = 1;

		public const int ReliableActions = 2;

		public const int Serialization = 3;

		public const int AttachmentDataProvider = 4;

		public const int CreateAttachment = 5;

		public const int ReadAttachment = 6;

		public const int UpdateAttachment = 7;

		public const int DeleteAttachment = 8;

		public const int FindAttachments = 9;

		public static Guid guid = new Guid("B3FC667F-1AD2-4377-AC4D-3AE344A739D4");
	}
}
