using System;
using System.Text;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class ReplicationNeighbor
	{
		public ReplicationNeighbor()
		{
		}

		private ReplicationNeighbor(ADObjectId sourceDsa)
		{
			if (sourceDsa == null)
			{
				throw new ArgumentNullException("sourceDsa");
			}
			this.SourceDsa = sourceDsa;
		}

		public ADObjectId SourceDsa { get; private set; }

		public static ReplicationNeighbor Parse(byte[] value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.Length < 36)
			{
				throw new ArgumentException("value");
			}
			byte[] array = new byte[4];
			Array.Copy(value, 4, array, 0, 4);
			int num = BitConverter.ToInt32(array, 0);
			ADObjectId adobjectId = null;
			if (num >= 120)
			{
				byte[] array2 = new byte[value.Length - num];
				Array.Copy(value, num, array2, 0, value.Length - num);
				string text = Encoding.Unicode.GetString(array2);
				string text2 = text;
				char[] separator = new char[1];
				text = text2.Split(separator)[0];
				adobjectId = new ADObjectId(text);
			}
			if (adobjectId != null)
			{
				return new ReplicationNeighbor(adobjectId);
			}
			return null;
		}

		public override string ToString()
		{
			return this.SourceDsa.ToString();
		}
	}
}
