using System;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal sealed class MsiRecord : MsiBase
	{
		public MsiRecord(MsiView view)
		{
			this.FetchNextRecord(view);
		}

		public void SaveStream(uint field, string fileName)
		{
			byte[] array = new byte[256];
			int num = array.Length;
			using (FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
			{
				for (;;)
				{
					uint errorCode = MsiNativeMethods.RecordReadStream(base.Handle, field, array, ref num);
					MsiHelper.ThrowIfNotSuccess(errorCode);
					if (num == 0)
					{
						break;
					}
					fileStream.Write(array, 0, num);
				}
			}
		}

		public string GetString(uint field)
		{
			string empty = string.Empty;
			uint dataSize = this.GetDataSize(field);
			if (dataSize == 0U)
			{
				throw new MsiException(Strings.InvalidFieldDataSize(field));
			}
			StringBuilder stringBuilder = new StringBuilder((int)(dataSize + 1U));
			uint capacity = (uint)stringBuilder.Capacity;
			uint errorCode = MsiNativeMethods.RecordGetString(base.Handle, field, stringBuilder, ref capacity);
			MsiHelper.ThrowIfNotSuccess(errorCode);
			return stringBuilder.ToString();
		}

		private void FetchNextRecord(MsiView view)
		{
			ValidationHelper.ThrowIfNull(view, "view");
			SafeMsiHandle handle;
			uint errorCode = MsiNativeMethods.ViewFetch(view.Handle, out handle);
			MsiHelper.ThrowIfNotSuccess(errorCode);
			base.Handle = handle;
		}

		private uint GetDataSize(uint field)
		{
			return MsiNativeMethods.RecordDataSize(base.Handle, field);
		}

		private const int MaxBufferSize = 256;

		public enum Fields
		{
			Property = 1,
			Stream
		}
	}
}
