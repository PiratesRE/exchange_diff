using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class LocationIdentifier
	{
		public LocationIdentifier(uint id) : this(id, LastChangeAction.None)
		{
		}

		public LocationIdentifier(byte[] byteArray)
		{
			this.ByteArray = byteArray;
		}

		internal LocationIdentifier(uint id, LastChangeAction action)
		{
			this.identifier = id;
			this.action = action;
		}

		private LocationIdentifier() : this(0U)
		{
		}

		public static int ByteArraySize
		{
			get
			{
				if (LocationIdentifier.byteArraySize == -1)
				{
					LocationIdentifier.byteArraySize = 8;
				}
				return LocationIdentifier.byteArraySize;
			}
		}

		public uint Identifier
		{
			get
			{
				return this.identifier;
			}
		}

		public byte[] ByteArray
		{
			get
			{
				byte[] array = new byte[LocationIdentifier.ByteArraySize];
				byte[] bytes = BitConverter.GetBytes(this.identifier);
				byte[] bytes2 = BitConverter.GetBytes((int)this.action);
				Array.Copy(bytes, array, bytes.Length);
				Array.Copy(bytes2, 0, array, bytes.Length, bytes2.Length);
				return array;
			}
			set
			{
				this.identifier = BitConverter.ToUInt32(value, 0);
				int num = BitConverter.ToInt32(value, 4);
				this.action = (LastChangeAction)num;
			}
		}

		internal LastChangeAction Action
		{
			get
			{
				return this.action;
			}
		}

		public static LocationIdentifier Parse(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				throw new ArgumentNullException("str", "Value cannot be null.");
			}
			string[] array = str.Split(new char[]
			{
				':'
			});
			if (array.Length != 2)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "'{0}' is not a valid Location Identifier representation.", new object[]
				{
					str
				}), "str");
			}
			string text = array[0];
			uint num;
			try
			{
				num = uint.Parse(text, CultureInfo.InvariantCulture);
			}
			catch (ArgumentException innerException)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "'{0}' is not a valid Location Identifier.", new object[]
				{
					text
				}), "str", innerException);
			}
			string text2 = array[1];
			LastChangeAction lastChangeAction;
			try
			{
				lastChangeAction = (LastChangeAction)Enum.Parse(typeof(LastChangeAction), text2);
			}
			catch (ArgumentException innerException2)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "'{0}' is not a valid Change Action.", new object[]
				{
					text2
				}), "str", innerException2);
			}
			LocationIdentifier result;
			try
			{
				result = new LocationIdentifier(num, lastChangeAction);
			}
			catch (FormatException innerException3)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "'{0}' is not a valid Location Identifier identifier.", new object[]
				{
					num
				}), "str", innerException3);
			}
			catch (OverflowException innerException4)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "'{0}' is not a valid Location Identifier identifier.", new object[]
				{
					num
				}), "str", innerException4);
			}
			return result;
		}

		public override int GetHashCode()
		{
			return this.identifier.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is LocationIdentifier && obj.GetHashCode() == this.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
			{
				this.identifier,
				this.action
			});
		}

		private const int NotCalculated = -1;

		private static int byteArraySize = -1;

		private uint identifier;

		private LastChangeAction action;
	}
}
