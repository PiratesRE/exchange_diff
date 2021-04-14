using System;
using System.IO;

namespace Microsoft.Exchange.UM.ClientAccess.Messages
{
	[Serializable]
	public class PromptPreviewRpcResponse : ResponseBase
	{
		public PromptPreviewRpcResponse(string filePath)
		{
			FileInfo fileInfo = new FileInfo(filePath);
			using (FileStream fileStream = fileInfo.OpenRead())
			{
				byte[] array = new byte[fileStream.Length];
				int i = (int)fileStream.Length;
				int num = 0;
				while (i > 0)
				{
					int num2 = fileStream.Read(array, num, i);
					if (num2 == 0)
					{
						break;
					}
					num += num2;
					i -= num2;
				}
				this.AudioData = array;
			}
		}

		public PromptPreviewRpcResponse()
		{
		}

		public byte[] AudioData { get; private set; }
	}
}
