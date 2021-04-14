using System;
using System.Collections;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.ClientAccess.Messages;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class UMPromptRpcRequestHandler : RequestHandler
	{
		protected static PromptPreviewRpcResponse GenerateAndEncode(ArrayList prompts)
		{
			PromptPreviewRpcResponse result;
			using (ITempFile tempFile = UMPromptRpcRequestHandler.ToPcm(prompts))
			{
				using (ITempFile tempFile2 = UMPromptRpcRequestHandler.ToCodec(tempFile.FilePath, AudioCodecEnum.Mp3))
				{
					result = UMPromptRpcRequestHandler.GenerateResponse(tempFile2.FilePath);
				}
			}
			return result;
		}

		protected static ITempFile ToPcm(ArrayList prompts)
		{
			UMPromptRpcRequestHandler.LogPrompts(prompts);
			return Platform.Utilities.SynthesizePromptsToPcmWavFile(prompts);
		}

		protected static ITempFile ToCodec(ITempFile pcmFile, AudioCodecEnum codec)
		{
			return MediaMethods.FromPcm(pcmFile, codec);
		}

		protected static ITempFile ToCodec(string pcmFilePath, AudioCodecEnum codec)
		{
			return MediaMethods.FromPcm(pcmFilePath, codec);
		}

		protected static PromptPreviewRpcResponse GenerateResponse(string filePath)
		{
			FileInfo fileInfo = new FileInfo(filePath);
			if (fileInfo.Length > 5242880L)
			{
				throw new AudioDataIsOversizeException(5, 5L);
			}
			return new PromptPreviewRpcResponse(filePath);
		}

		protected static ITempFile ChangeFileNameForTests(ITempFile tempFile, string newFileName, out string filename)
		{
			ITempFile tempFile2 = TempFileFactory.CreateTempDir();
			string text = Path.Combine(tempFile2.FilePath, Path.GetFileName(newFileName));
			text += ".wav";
			File.Copy(tempFile.FilePath, text);
			tempFile.Dispose();
			filename = text;
			return tempFile2;
		}

		protected override ResponseBase Execute(RequestBase requestBase)
		{
			return UMPromptRpcRequestHandler.GenerateAndEncode(this.GetPrompts(requestBase));
		}

		protected abstract ArrayList GetPrompts(RequestBase requestBase);

		private static void LogPrompts(ArrayList prompts)
		{
			if (prompts.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < prompts.Count; i++)
				{
					stringBuilder.Append("\n");
					stringBuilder.Append(prompts[i].ToString());
				}
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_PromptsPlayed, null, new object[]
				{
					Guid.NewGuid().ToString(),
					stringBuilder.ToString()
				});
			}
		}
	}
}
