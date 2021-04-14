using System;
using System.Text;
using Microsoft.Exchange.Conversion;

namespace Microsoft.Exchange.AddressBook.Nspi
{
	internal class EntryId
	{
		internal EntryId(EntryId.DisplayType displayType, string dn)
		{
			if (dn == null)
			{
				throw new ArgumentNullException("dn");
			}
			this.displayType = displayType;
			this.dn = dn;
			this.providerGuid = EntryId.ExchangeProviderGuid;
		}

		internal EntryId(EntryId.DisplayType displayType, Guid providerGuid, int ephemeralId)
		{
			this.displayType = displayType;
			this.providerGuid = providerGuid;
			this.ephemeralId = ephemeralId;
		}

		internal string DistinguishedName
		{
			get
			{
				return this.dn;
			}
		}

		internal int EphemeralId
		{
			get
			{
				return this.ephemeralId;
			}
		}

		internal Guid ProviderGuid
		{
			get
			{
				return this.providerGuid;
			}
			set
			{
				this.InvalidateBytes();
				this.providerGuid = value;
			}
		}

		internal bool IsEphemeral
		{
			get
			{
				return this.dn == null;
			}
		}

		internal static bool TryParse(byte[] buffer, out EntryId entryId)
		{
			entryId = null;
			if (buffer.Length < 28)
			{
				return false;
			}
			EntryId.EntryIdFlags entryIdFlags = (EntryId.EntryIdFlags)BitConverter.ToInt32(buffer, 0);
			Guid a = ExBitConverter.ReadGuid(buffer, 4);
			int num = BitConverter.ToInt32(buffer, 20);
			EntryId.DisplayType displayType = (EntryId.DisplayType)BitConverter.ToInt32(buffer, 24);
			if (num != 1)
			{
				return false;
			}
			if (entryIdFlags == EntryId.EntryIdFlags.Permanent)
			{
				if (buffer.Length < 29 || buffer[buffer.Length - 1] != 0 || a != EntryId.ExchangeProviderGuid)
				{
					return false;
				}
				string @string = Encoding.ASCII.GetString(buffer, 28, buffer.Length - 28 - 1);
				entryId = new EntryId(displayType, @string);
				return true;
			}
			else
			{
				if (entryIdFlags != EntryId.EntryIdFlags.Ephemeral)
				{
					return false;
				}
				if (buffer.Length != 32)
				{
					return false;
				}
				entryId = new EntryId(displayType, a, BitConverter.ToInt32(buffer, 28));
				entryId.bytes = buffer;
				return true;
			}
		}

		internal byte[] ToByteArray()
		{
			if (this.bytes == null)
			{
				if (this.IsEphemeral)
				{
					int num = 32;
					this.bytes = new byte[num];
					this.WriteHeader(EntryId.EntryIdFlags.Ephemeral, this.bytes);
					ExBitConverter.Write(this.ephemeralId, this.bytes, 28);
				}
				else
				{
					byte[] array = Encoding.ASCII.GetBytes(this.dn);
					int num2 = 28 + array.Length + 1;
					this.bytes = new byte[num2];
					this.WriteHeader(EntryId.EntryIdFlags.Permanent, this.bytes);
					Array.Copy(array, 0, this.bytes, 28, array.Length);
				}
			}
			return this.bytes;
		}

		private void WriteHeader(EntryId.EntryIdFlags flags, byte[] buffer)
		{
			int num = 0;
			num += ExBitConverter.Write((int)flags, buffer, num);
			num += ExBitConverter.Write(this.providerGuid, buffer, num);
			num += ExBitConverter.Write(1, buffer, num);
			num += ExBitConverter.Write((int)this.displayType, buffer, num);
		}

		private void InvalidateBytes()
		{
			this.bytes = null;
		}

		public override string ToString()
		{
			return string.Format("DisplayType: {0}, MID: {1}, DN: {2}", this.displayType, this.ephemeralId, this.dn ?? "(null)");
		}

		private const int Version = 1;

		private const int BaseSize = 28;

		private const int EphemeralEntryIdSize = 32;

		public static readonly Guid ExchangeProviderGuid = new Guid("c840a7dc-42c0-1a10-b4b9-08002b2fe182");

		public static readonly byte[] ExchangeProviderGuidByteArray = EntryId.ExchangeProviderGuid.ToByteArray();

		public static readonly EntryId DefaultGalEntryId = new EntryId(EntryId.DisplayType.AbContainer, "/");

		private EntryId.DisplayType displayType;

		private string dn;

		private int ephemeralId;

		private byte[] bytes;

		private Guid providerGuid;

		internal enum DisplayType
		{
			MailUser,
			DistList,
			Forum,
			Agent,
			Organization,
			RemoteMailUser = 6,
			AbContainer = 256,
			AbAddressTemplate = 258
		}

		private enum EntryIdFlags
		{
			Permanent,
			Ephemeral = 135
		}
	}
}
