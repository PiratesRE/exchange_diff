using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class Breadcrumbs
	{
		internal static ITempFile GenerateDump()
		{
			ITempFile tempFile = TempFileFactory.CreateTempFile();
			using (FileStream fileStream = new FileStream(tempFile.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
			{
				using (StreamWriter streamWriter = new StreamWriter(fileStream))
				{
					int num = Breadcrumbs.sequenceNumber + 1 & 2047;
					streamWriter.WriteLine();
					streamWriter.WriteLine("--- Dumping breadcrumbs --- ");
					streamWriter.WriteLine("{0}, {1} \r\n", Breadcrumbs.initialTickCount, Breadcrumbs.initialDateTime);
					for (int i = 0; i < 2048; i++)
					{
						string value = Breadcrumbs.breadcrumbs[num];
						if (!string.IsNullOrEmpty(value))
						{
							streamWriter.WriteLine(value);
						}
						num = (num + 1 & 2047);
					}
					streamWriter.WriteLine();
				}
			}
			return tempFile;
		}

		internal static void AddBreadcrumb(string crumb)
		{
			if (!string.IsNullOrEmpty(crumb))
			{
				int num = Interlocked.Increment(ref Breadcrumbs.sequenceNumber);
				int num2 = num & 2047;
				if (crumb.Length > 1024)
				{
					crumb = crumb.Substring(0, 1024);
				}
				crumb = string.Format(CultureInfo.InvariantCulture, "{0}, TID:{1} {2}", new object[]
				{
					Environment.TickCount & int.MaxValue,
					Thread.CurrentThread.ManagedThreadId,
					crumb
				});
				Breadcrumbs.breadcrumbs[num2] = crumb;
			}
		}

		private const int NumCrumbs = 2048;

		private const int IndexMask = 2047;

		private static int sequenceNumber = 0;

		private static string[] breadcrumbs = new string[2048];

		private static int initialTickCount = Environment.TickCount & int.MaxValue;

		private static ExDateTime initialDateTime = ExDateTime.UtcNow;
	}
}
