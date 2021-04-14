using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class StreamCopier
	{
		public StreamCopier(int bufferSize)
		{
			this.bufferSize = bufferSize;
		}

		public CopyStreamResult CopyStream(Stream input, Stream output)
		{
			return this.CopyStream(input, output, null, delegate()
			{
			});
		}

		public CopyStreamResult CopyStream(Stream input, Stream output, HashAlgorithm hashAlgorithm, Action abortFileOperation)
		{
			Stopwatch stopwatch = new Stopwatch();
			Stopwatch stopwatch2 = new Stopwatch();
			long num = 0L;
			byte[] array = new byte[this.bufferSize];
			for (;;)
			{
				abortFileOperation();
				stopwatch.Start();
				int num2 = input.Read(array, 0, array.Length);
				stopwatch.Stop();
				if (num2 == 0)
				{
					break;
				}
				if (hashAlgorithm != null)
				{
					hashAlgorithm.TransformBlock(array, 0, num2, array, 0);
				}
				stopwatch2.Start();
				output.Write(array, 0, num2);
				stopwatch2.Stop();
				num += (long)num2;
			}
			if (hashAlgorithm != null)
			{
				hashAlgorithm.TransformFinalBlock(array, 0, 0);
			}
			return new CopyStreamResult(stopwatch.Elapsed, stopwatch2.Elapsed, num);
		}

		private readonly int bufferSize;
	}
}
