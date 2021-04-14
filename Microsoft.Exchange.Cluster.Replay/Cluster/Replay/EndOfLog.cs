using System;
using Microsoft.Exchange.Data.Serialization;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class EndOfLog
	{
		public EndOfLog(long gen, DateTime utc)
		{
			this.m_generationNum = gen;
			this.m_writeTimeUtc = utc;
		}

		public EndOfLog()
		{
			this.m_generationNum = 0L;
			this.m_writeTimeUtc = DateTime.FromFileTimeUtc(0L);
		}

		public long Generation
		{
			get
			{
				return this.m_generationNum;
			}
		}

		public DateTime Utc
		{
			get
			{
				return this.m_writeTimeUtc;
			}
		}

		public void SetValue(long newGenNum, DateTime? writeTimeUtc)
		{
			lock (this)
			{
				this.m_generationNum = newGenNum;
				if (writeTimeUtc != null)
				{
					this.m_writeTimeUtc = writeTimeUtc.Value;
				}
			}
		}

		public void Serialize(byte[] buf, ref int bytePos)
		{
			lock (this)
			{
				Serialization.SerializeUInt64(buf, ref bytePos, (ulong)this.m_generationNum);
				Serialization.SerializeDateTime(buf, ref bytePos, this.m_writeTimeUtc);
			}
		}

		public void Deserialize(byte[] buf, ref int bytePos)
		{
			lock (this)
			{
				this.m_generationNum = (long)Serialization.DeserializeUInt64(buf, ref bytePos);
				this.m_writeTimeUtc = Serialization.DeserializeDateTime(buf, ref bytePos);
			}
		}

		public const int SerializationLength = 16;

		private long m_generationNum;

		private DateTime m_writeTimeUtc;
	}
}
