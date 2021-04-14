using System;
using System.IO;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OofHistoryWriter
	{
		public static void Reset(Stream oofHistoryStream)
		{
			oofHistoryStream.Seek(0L, SeekOrigin.Begin);
			oofHistoryStream.Write(OofHistoryWriter.InitialBytes, 0, 6);
			if (oofHistoryStream.Length > 6L)
			{
				oofHistoryStream.SetLength(6L);
			}
		}

		public void Initialize(Stream oofHistoryStream)
		{
			this.oofHistoryStream = oofHistoryStream;
			this.dataPosition = 0;
		}

		public void AppendEntry(int entryCount, byte[] senderAddress, byte[] globalRuleId)
		{
			if (this.data != null)
			{
				throw new InvalidOperationException("OOF history writer only supports one append operation.");
			}
			this.oofHistoryStream.Position = this.oofHistoryStream.Length;
			int num = 7 + senderAddress.Length + globalRuleId.Length;
			this.data = new byte[num];
			this.data[0] = 2;
			this.dataPosition = 1;
			this.AppendProperty(OofHistory.PropId.SenderAddress, senderAddress);
			this.AppendProperty(OofHistory.PropId.GlobalRuleId, globalRuleId);
			this.oofHistoryStream.Write(this.data, 0, this.dataPosition);
			this.oofHistoryStream.Seek(2L, SeekOrigin.Begin);
			ExBitConverter.Write(entryCount, this.data, 0);
			this.oofHistoryStream.Write(this.data, 0, 4);
			this.oofHistoryStream.Flush();
		}

		private void AppendProperty(OofHistory.PropId propId, byte[] propertyValue)
		{
			this.data[this.dataPosition++] = (byte)propId;
			ushort num = (ushort)propertyValue.Length;
			ExBitConverter.Write(num, this.data, this.dataPosition);
			this.dataPosition += 2;
			Buffer.BlockCopy(propertyValue, 0, this.data, this.dataPosition, (int)num);
			this.dataPosition += (int)num;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static OofHistoryWriter()
		{
			byte[] array = new byte[6];
			array[0] = 1;
			array[1] = 1;
			OofHistoryWriter.InitialBytes = array;
		}

		internal static readonly byte[] InitialBytes;

		private byte[] data;

		private int dataPosition;

		private Stream oofHistoryStream;
	}
}
