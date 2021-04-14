using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AttachmentId : IEquatable<AttachmentId>
	{
		internal AttachmentId(byte[] attachKey)
		{
			this.AttachKey = attachKey;
		}

		public static AttachmentId Deserialize(byte[] byteArrayId)
		{
			if (byteArrayId == null)
			{
				throw new ArgumentNullException("byteArrayId");
			}
			AttachmentId result;
			if (AttachmentId.TryParse(byteArrayId, out result))
			{
				return result;
			}
			throw new CorruptDataException(ServerStrings.InvalidAttachmentId);
		}

		public static AttachmentId Deserialize(string base64Id)
		{
			if (base64Id == null)
			{
				throw new ArgumentNullException(base64Id);
			}
			byte[] byteArrayId = StoreId.Base64ToByteArray(base64Id);
			return AttachmentId.Deserialize(byteArrayId);
		}

		public int GetByteArrayLength()
		{
			short num = 0;
			if (this.attachKey != null)
			{
				num = (short)this.attachKey.Length;
			}
			return (int)(2 + num);
		}

		public byte[] ToByteArray()
		{
			short num = 0;
			if (this.attachKey != null)
			{
				num = (short)this.attachKey.Length;
			}
			int num2 = (int)(2 + num);
			byte[] array = new byte[num2];
			int num3 = 0;
			num3 += ExBitConverter.Write(num, array, num3);
			if (this.attachKey != null)
			{
				this.attachKey.CopyTo(array, num3);
			}
			return array;
		}

		public string ToBase64String()
		{
			return Convert.ToBase64String(this.ToByteArray());
		}

		public override bool Equals(object id)
		{
			AttachmentId id2 = id as AttachmentId;
			return this.Equals(id2);
		}

		public bool Equals(AttachmentId id)
		{
			return id != null && ArrayComparer<byte>.Comparer.Equals(this.attachKey, id.attachKey);
		}

		public override int GetHashCode()
		{
			if (this.hashCode == -1 && this.attachKey != null)
			{
				int num = 0;
				int num2 = 8;
				int num3 = this.attachKey.Length - 1;
				while (num3 >= 0 && num2 > 0)
				{
					num ^= ((int)this.attachKey[num3] << 8) * (num3 % 4);
					num2--;
					num3--;
				}
				this.hashCode = num;
			}
			return this.hashCode;
		}

		internal byte[] AttachKey
		{
			get
			{
				return this.attachKey;
			}
			private set
			{
				this.attachKey = value;
			}
		}

		internal static bool TryParse(byte[] idBytes, out AttachmentId id)
		{
			id = null;
			if (idBytes.Length < 2)
			{
				ExTraceGlobals.StorageTracer.TraceError<string>(-1L, "AttachmentId::TryParse. The index head of the attchment Id bytes is incomplete. id = {0}.", Convert.ToBase64String(idBytes));
				return false;
			}
			short num = BitConverter.ToInt16(idBytes, 0);
			int num2 = 2;
			byte[] destinationArray = null;
			if (num > 0)
			{
				if ((int)num + num2 > idBytes.Length)
				{
					return false;
				}
				destinationArray = new byte[(int)num];
				Array.Copy(idBytes, num2, destinationArray, 0, (int)num);
			}
			id = new AttachmentId(destinationArray);
			return true;
		}

		private int hashCode = -1;

		private byte[] attachKey;
	}
}
