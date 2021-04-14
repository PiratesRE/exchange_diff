using System;
using System.IO;
using System.Security;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal static class LocalFileHelper
	{
		public static void CallFileOperation(Action action, ExportErrorType errorType)
		{
			Exception ex = null;
			try
			{
				action();
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (SecurityException ex3)
			{
				ex = ex3;
			}
			catch (UnauthorizedAccessException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				Tracer.TraceError("LocalFileHelper.CallFileOperation: Failed with file operation, Exception: {0}", new object[]
				{
					ex
				});
				throw new ExportException(errorType, ex);
			}
		}

		public static void RemoveFile(string filePath, ExportErrorType errorType)
		{
			LocalFileHelper.CallFileOperation(delegate
			{
				if (File.Exists(filePath))
				{
					File.Delete(filePath);
				}
			}, errorType);
		}
	}
}
