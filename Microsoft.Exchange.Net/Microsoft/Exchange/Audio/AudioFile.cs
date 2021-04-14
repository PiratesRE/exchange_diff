using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.Audio
{
	internal abstract class AudioFile
	{
		internal static bool IsMp3(string fileName)
		{
			return AudioFile.HasExtension(fileName, ".mp3");
		}

		internal static bool IsProtectedMp3(string fileName)
		{
			return AudioFile.HasExtension(fileName, ".umrmmp3");
		}

		internal static bool IsWma(string fileName)
		{
			return AudioFile.HasExtension(fileName, ".wma");
		}

		internal static bool IsProtectedWma(string fileName)
		{
			return AudioFile.HasExtension(fileName, ".umrmwma");
		}

		internal static bool IsWav(string fileName)
		{
			return AudioFile.HasExtension(fileName, ".wav");
		}

		internal static bool IsProtectedWav(string fileName)
		{
			return AudioFile.HasExtension(fileName, ".umrmwav");
		}

		internal static bool IsProtectedVoiceAttachment(string fileName)
		{
			return AudioFile.IsProtectedMp3(fileName) || AudioFile.IsProtectedWav(fileName) || AudioFile.IsProtectedWma(fileName);
		}

		internal static bool TryGetDRMFileExtension(string extension, out string drmExtension)
		{
			drmExtension = null;
			if (AudioFile.HasExtension(extension, ".wav"))
			{
				drmExtension = ".umrmwav";
			}
			else if (AudioFile.HasExtension(extension, ".wma"))
			{
				drmExtension = ".umrmwma";
			}
			else if (AudioFile.HasExtension(extension, ".mp3"))
			{
				drmExtension = ".umrmmp3";
			}
			return drmExtension != null;
		}

		internal static bool TryGetNonDRMFileNameFromDRM(string drmFileName, out string nonDRMFileName)
		{
			nonDRMFileName = null;
			string text = null;
			string oldValue = null;
			if (AudioFile.IsProtectedMp3(drmFileName))
			{
				text = ".mp3";
				oldValue = ".umrmmp3";
			}
			else if (AudioFile.IsProtectedWma(drmFileName))
			{
				text = ".wma";
				oldValue = ".umrmwma";
			}
			else if (AudioFile.IsProtectedWav(drmFileName))
			{
				text = ".wav";
				oldValue = ".umrmwav";
			}
			else
			{
				ExTraceGlobals.UtilTracer.TraceError<string>(0L, "Cannot get Non DRM filename for the following filename:{0},", drmFileName);
			}
			if (text != null)
			{
				nonDRMFileName = drmFileName.Replace(oldValue, text);
			}
			return nonDRMFileName != null;
		}

		private static bool HasExtension(string fileName, string extension)
		{
			return fileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase);
		}
	}
}
