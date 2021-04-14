using System;
using System.IO;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal class Segmentator
	{
		public Segmentator(int numOfThreads)
		{
			if (numOfThreads < 1)
			{
				numOfThreads = 1;
			}
			this.numOfSegments = Math.Min(numOfThreads, 5);
		}

		public DownloadParameter[] SegmentTheFile(long fileSize)
		{
			Logger.LoggerMessage("Segmenting the file with size: " + fileSize);
			long num = fileSize / (long)this.numOfSegments;
			DownloadParameter[] array = new DownloadParameter[this.numOfSegments];
			for (int i = 0; i < this.numOfSegments; i++)
			{
				if (i == 0)
				{
					array[i].StartPosition = 0;
				}
				else
				{
					array[i].StartPosition = (int)((long)i * num) + 1;
				}
				if (i + 1 == this.numOfSegments)
				{
					array[i].EndPosition = (int)fileSize - 1;
				}
				else
				{
					array[i].EndPosition = (int)((long)(i + 1) * num);
				}
				array[i].PathToFile = Path.GetTempFileName();
			}
			return array;
		}

		private int numOfSegments;
	}
}
