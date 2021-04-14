using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Microsoft.Exchange.Configuration.Core
{
	internal abstract class CrossAppDomainObjectBehavior : IDisposable
	{
		internal CrossAppDomainObjectBehavior(string namedPipeName, BehaviorDirection direction)
		{
			this.NamedPipeName = namedPipeName;
			this.Direction = direction;
		}

		~CrossAppDomainObjectBehavior()
		{
			this.Dispose(false);
		}

		internal string NamedPipeName { get; private set; }

		internal BehaviorDirection Direction { get; private set; }

		internal virtual bool IsActive
		{
			get
			{
				return true;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal static bool ConnectClientStream(NamedPipeClientStream clientStream, int timeOutInMilliseconds, string namePipeName, bool swallowKnownException = true)
		{
			try
			{
				clientStream.Connect(timeOutInMilliseconds);
				return true;
			}
			catch (Exception ex)
			{
				if (!swallowKnownException || (!(ex is IOException) && !(ex is TimeoutException)))
				{
					throw;
				}
			}
			return false;
		}

		internal static byte[] PackMessages(IEnumerable<string> messages)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				binaryFormatter.Serialize(memoryStream, messages);
				result = memoryStream.ToArray();
			}
			return result;
		}

		internal static IEnumerable<string> UnpackMessages(byte[] data)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			IEnumerable<string> result;
			using (MemoryStream memoryStream = new MemoryStream(data))
			{
				result = (binaryFormatter.Deserialize(memoryStream) as IEnumerable<string>);
			}
			return result;
		}

		internal static byte[] LoopReadData(CrossAppDomainObjectBehavior.SingleReadAction readAction)
		{
			List<byte[]> list = new List<byte[]>(5000);
			byte[] array = new byte[5000];
			int num;
			do
			{
				num = readAction(array, 0, 5000);
				if (num > 0)
				{
					byte[] array2 = new byte[num];
					Buffer.BlockCopy(array, 0, array2, 0, num);
					list.Add(array2);
				}
			}
			while (num == 5000);
			return CrossAppDomainObjectBehavior.MergeData(list);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		private static byte[] MergeData(List<byte[]> dataList)
		{
			byte[] array = new byte[dataList.Sum((byte[] a) => a.Length)];
			int num = 0;
			foreach (byte[] array2 in dataList)
			{
				Buffer.BlockCopy(array2, 0, array, num, array2.Length);
				num += array2.Length;
			}
			return array;
		}

		protected const int MaxBytesSizeSentInNamedPipe = 5000;

		internal delegate int SingleReadAction(byte[] buffer, int offset, int count);
	}
}
