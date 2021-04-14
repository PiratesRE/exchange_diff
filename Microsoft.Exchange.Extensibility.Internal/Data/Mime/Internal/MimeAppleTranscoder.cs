using System;
using System.IO;
using Microsoft.Exchange.Data.Transport.Email;

namespace Microsoft.Exchange.Data.Mime.Internal
{
	public static class MimeAppleTranscoder
	{
		public static Stream ExtractDataFork(Stream macBinStream)
		{
			return MimeAppleTranscoder.ExtractDataFork(macBinStream);
		}

		public static bool IsMacBinStream(Stream stream)
		{
			return MimeAppleTranscoder.IsMacBinStream(stream);
		}

		public static void WriteWholeApplefile(Stream dataForkStream, Stream outStream)
		{
			MimeAppleTranscoder.WriteWholeApplefile(dataForkStream, outStream);
		}

		public static void WriteWholeApplefile(Stream applefileStream, Stream dataForkStream, Stream outStream)
		{
			MimeAppleTranscoder.WriteWholeApplefile(applefileStream, dataForkStream, outStream);
		}

		public static void MacBinToApplefile(Stream macBinStream, Stream outStream, out string fileName, out byte[] additionalInfo)
		{
			MimeAppleTranscoder.MacBinToApplefile(macBinStream, outStream, out fileName, out additionalInfo);
		}

		public static void ApplesingleToMacBin(Stream applesingleStream, Stream outAttachMacInfo, Stream outMacBinStream, out string fileName, out byte[] additionalInfo)
		{
			MimeAppleTranscoder.ApplesingleToMacBin(applesingleStream, outAttachMacInfo, outMacBinStream, out fileName, out additionalInfo);
		}

		public static void AppledoubleToMacBin(Stream resourceForkStream, Stream dataForkStream, Stream outMacBinStream, out string fileName, out byte[] additionalInfo)
		{
			MimeAppleTranscoder.AppledoubleToMacBin(resourceForkStream, dataForkStream, outMacBinStream, out fileName, out additionalInfo);
		}
	}
}
