using System;
using System.IO;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class EventWatermark
	{
		internal EventWatermark(Guid mdbGuid, long mapiWatermark, bool wasEventProcessed)
		{
			this.mdbGuid = mdbGuid;
			this.mapiWatermark = mapiWatermark;
			this.wasEventProcessed = wasEventProcessed;
		}

		internal long MapiWatermark
		{
			get
			{
				return this.mapiWatermark;
			}
		}

		internal bool WasEventProcessed
		{
			get
			{
				return this.wasEventProcessed;
			}
		}

		internal Guid MdbGuid
		{
			get
			{
				return this.mdbGuid;
			}
		}

		public static EventWatermark Deserialize(string base64String)
		{
			if (base64String == null)
			{
				throw new ArgumentNullException(base64String);
			}
			byte[] buffer = null;
			try
			{
				buffer = Convert.FromBase64String(base64String);
			}
			catch (FormatException innerException)
			{
				throw new CorruptDataException(ServerStrings.ExInvalidBase64StringFormat(base64String), innerException);
			}
			EventWatermark result;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						int num = binaryReader.ReadInt32();
						Guid empty;
						if (num == 0)
						{
							empty = Guid.Empty;
						}
						else
						{
							if (num != 1)
							{
								throw new CorruptDataException(ServerStrings.ExInvalidWatermarkString);
							}
							empty = new Guid(binaryReader.ReadBytes(16));
						}
						long num2 = binaryReader.ReadInt64();
						bool flag = binaryReader.ReadBoolean();
						result = new EventWatermark(empty, num2, flag);
					}
				}
			}
			catch (IOException innerException2)
			{
				throw new CorruptDataException(ServerStrings.ExInvalidWatermarkString, innerException2);
			}
			return result;
		}

		public string ToBase64String()
		{
			int num = 29;
			byte[] array = new byte[num];
			int num2 = 0;
			num2 += ExBitConverter.Write(1, array, num2);
			num2 += ExBitConverter.Write(this.mdbGuid, array, num2);
			num2 += ExBitConverter.Write(this.mapiWatermark, array, num2);
			array[num2++] = (this.wasEventProcessed ? 1 : 0);
			return Convert.ToBase64String(array);
		}

		private const int SizeOfGuidByteArray = 16;

		private readonly long mapiWatermark;

		private readonly bool wasEventProcessed;

		private readonly Guid mdbGuid;
	}
}
