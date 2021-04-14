using System;
using System.IO;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.External
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class Streams
	{
		internal static Stream CreateSuppressCloseWrapper(Stream baseStream)
		{
			return new SuppressCloseStream(baseStream);
		}

		internal static void ConfigureTempStorage(int defaultMaximumSize, int defaultBlockSize, string defaultPath, Func<int, byte[]> defaultAcquireBuffer, Action<byte[]> defaultReleaseBuffer)
		{
			TemporaryDataStorage.Configure(defaultMaximumSize, defaultBlockSize, defaultPath, defaultAcquireBuffer, defaultReleaseBuffer);
		}

		internal static string GetCtsTempPath()
		{
			return TemporaryDataStorage.GetTempPath();
		}

		internal static Stream CreateTemporaryStorageStream()
		{
			return ApplicationServices.Provider.CreateTemporaryStorage();
		}

		internal static Stream CloneTemporaryStorageStream(Stream originalStream)
		{
			ICloneableStream cloneableStream = originalStream as ICloneableStream;
			if (cloneableStream == null)
			{
				throw new ArgumentException("This stream cannot be cloned", "originalStream");
			}
			return cloneableStream.Clone();
		}

		internal static Stream CreateTemporaryStorageStream(Func<int, byte[]> acquireBuffer, Action<byte[]> releaseBuffer)
		{
			return DefaultApplicationServices.CreateTemporaryStorage(acquireBuffer, releaseBuffer);
		}

		internal static Stream CreateAutoPositionedStream(Stream baseStream)
		{
			return new AutoPositionReadOnlyStream(baseStream, true);
		}
	}
}
