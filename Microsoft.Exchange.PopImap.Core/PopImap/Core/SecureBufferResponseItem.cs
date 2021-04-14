using System;

namespace Microsoft.Exchange.PopImap.Core
{
	internal class SecureBufferResponseItem : BufferResponseItem
	{
		public SecureBufferResponseItem(byte[] buf) : base(buf)
		{
			if (buf.Length < 2 || buf[buf.Length - 2] != 13 || buf[buf.Length - 1] != 10)
			{
				byte[] array = new byte[buf.Length + 2];
				Array.Copy(buf, array, buf.Length);
				array[buf.Length] = 13;
				array[buf.Length + 1] = 10;
				base.BindData(array, 0, array.Length, null);
				Array.Clear(buf, 0, buf.Length);
			}
		}

		public override int GetNextChunk(BaseSession session, out byte[] buffer, out int offset)
		{
			if (base.DataSent)
			{
				base.ClearData();
				buffer = null;
				offset = 0;
				return 0;
			}
			return base.GetNextChunk(session, out buffer, out offset);
		}
	}
}
