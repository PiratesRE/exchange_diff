using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal static class MessageDisposition
	{
		public static MessageDispositionType ConvertToEnum(string messageDispositionValue)
		{
			MessageDispositionType result = MessageDispositionType.SendOnly;
			if (messageDispositionValue != null)
			{
				if (!(messageDispositionValue == "SendOnly"))
				{
					if (!(messageDispositionValue == "SaveOnly"))
					{
						if (messageDispositionValue == "SendAndSaveCopy")
						{
							result = MessageDispositionType.SendAndSaveCopy;
						}
					}
					else
					{
						result = MessageDispositionType.SaveOnly;
					}
				}
				else
				{
					result = MessageDispositionType.SendOnly;
				}
			}
			return result;
		}

		public const string SendOnlyValue = "SendOnly";

		public const string SaveOnlyValue = "SaveOnly";

		public const string SendAndSaveCopyValue = "SendAndSaveCopy";
	}
}
