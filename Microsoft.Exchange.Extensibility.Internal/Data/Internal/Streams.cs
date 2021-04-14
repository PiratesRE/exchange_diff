using System;
using System.IO;

namespace Microsoft.Exchange.Data.Internal
{
	internal static class Streams
	{
		public static Stream CreateSuppressCloseWrapper(Stream baseStream)
		{
			return new SuppressCloseStream(baseStream);
		}

		public static void ConfigureTempStorage(int defaultMaximumSize, int defaultBlockSize, string defaultPath, Func<int, byte[]> defaultAcquireBuffer, Action<byte[]> defaultReleaseBuffer)
		{
			TemporaryDataStorage.Configure(defaultMaximumSize, defaultBlockSize, defaultPath, defaultAcquireBuffer, defaultReleaseBuffer);
		}

		public static string GetCtsTempPath()
		{
			return TemporaryDataStorage.GetTempPath();
		}

		public static Stream CreateTemporaryStorageStream()
		{
			return ApplicationServices.Provider.CreateTemporaryStorage();
		}

		public static Stream CloneTemporaryStorageStream(Stream originalStream)
		{
			ICloneableStream cloneableStream = originalStream as ICloneableStream;
			if (cloneableStream == null)
			{
				throw new ArgumentException("This stream cannot be cloned", "originalStream");
			}
			return cloneableStream.Clone();
		}

		public static Stream CreateTemporaryStorageStream(Func<int, byte[]> acquireBuffer, Action<byte[]> releaseBuffer)
		{
			return DefaultApplicationServices.CreateTemporaryStorage(acquireBuffer, releaseBuffer);
		}

		public static Stream CreateAutoPositionedStream(Stream baseStream)
		{
			return new AutoPositionReadOnlyStream(baseStream, true);
		}
	}
}
