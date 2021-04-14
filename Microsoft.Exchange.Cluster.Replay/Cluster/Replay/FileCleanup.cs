using System;
using System.IO;
using System.Security;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class FileCleanup
	{
		public static void TryDelete(string fName)
		{
			Exception ex = FileCleanup.Delete(fName);
			if (ex != null)
			{
				ExTraceGlobals.LogCopyTracer.TraceError<Exception, string>(0L, "FileCleanup: Ignoring exception during file delete for '{1}': {0}", ex, fName);
			}
		}

		public static Exception Delete(string fileFullPath)
		{
			Exception result = null;
			try
			{
				if (File.Exists(fileFullPath))
				{
					File.Delete(fileFullPath);
				}
			}
			catch (FileNotFoundException ex)
			{
				result = ex;
			}
			catch (IOException ex2)
			{
				result = ex2;
			}
			catch (SecurityException ex3)
			{
				result = ex3;
			}
			catch (UnauthorizedAccessException ex4)
			{
				result = ex4;
			}
			return result;
		}

		public static void DisposeProperly(FileStream wfs)
		{
			try
			{
				wfs.Dispose();
			}
			catch (IOException arg)
			{
				ExTraceGlobals.LogCopyTracer.TraceError<IOException>(0L, "DisposeProperly: Ignoring exception during file close: {0}", arg);
			}
		}
	}
}
