using System;
using System.Text;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class ReplicationCursor
	{
		public ReplicationCursor()
		{
		}

		public ReplicationCursor(Guid sourceInvocationId, long upToDatenessUsn, DateTime lastSuccessfulSyncTime, ADObjectId sourceDsa)
		{
			this.sourceInvocationId = sourceInvocationId;
			this.upToDatenessUsn = upToDatenessUsn;
			this.lastSuccessfulSyncTime = lastSuccessfulSyncTime;
			this.sourceDsa = sourceDsa;
		}

		public Guid SourceInvocationId
		{
			get
			{
				return this.sourceInvocationId;
			}
		}

		public long UpToDatenessUsn
		{
			get
			{
				return this.upToDatenessUsn;
			}
		}

		public DateTime LastSuccessfulSyncTime
		{
			get
			{
				return this.lastSuccessfulSyncTime;
			}
		}

		public ADObjectId SourceDsa
		{
			get
			{
				return this.sourceDsa;
			}
		}

		public static ReplicationCursor Parse(byte[] value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.Length < 36)
			{
				throw new ArgumentException("value");
			}
			byte[] array = new byte[16];
			byte[] array2 = new byte[8];
			byte[] array3 = new byte[8];
			byte[] array4 = new byte[4];
			Array.Copy(value, 0, array, 0, 16);
			Guid guid = new Guid(array);
			Array.Copy(value, 16, array2, 0, 8);
			long num = BitConverter.ToInt64(array2, 0);
			Array.Copy(value, 24, array3, 0, 8);
			uint num2 = BitConverter.ToUInt32(array3, 0);
			uint num3 = BitConverter.ToUInt32(array3, 4);
			ulong fileTime = ((ulong)num3 << 32) + (ulong)num2;
			DateTime dateTime = DateTime.FromFileTimeUtc((long)fileTime);
			Array.Copy(value, 32, array4, 0, 4);
			int num4 = BitConverter.ToInt32(array4, 0);
			ADObjectId adobjectId = null;
			if (num4 > 36)
			{
				byte[] array5 = new byte[value.Length - num4];
				Array.Copy(value, num4, array5, 0, value.Length - num4);
				string text = Encoding.Unicode.GetString(array5);
				string text2 = text;
				char[] trimChars = new char[1];
				text = text2.Trim(trimChars);
				adobjectId = new ADObjectId(text);
			}
			return new ReplicationCursor(guid, num, dateTime, adobjectId);
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}", this.SourceInvocationId, this.UpToDatenessUsn);
		}

		private Guid sourceInvocationId;

		private long upToDatenessUsn;

		private DateTime lastSuccessfulSyncTime;

		private ADObjectId sourceDsa;
	}
}
