using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.ActivityLog
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ClientId
	{
		private bool IsValidValue
		{
			get
			{
				int num = this.id & int.MaxValue;
				return num >= ClientId.Min.ToInt() && num <= ClientId.Max.ToInt();
			}
		}

		public bool LoggedViaServerSideInstrumentation
		{
			get
			{
				return (this.id & int.MinValue) != 0;
			}
		}

		private ClientId(int id)
		{
			this.id = id;
		}

		public static bool operator ==(ClientId id1, ClientId id2)
		{
			return object.ReferenceEquals(id1, id2) || (id1 != null && id2 != null && id1.Equals(id2, false));
		}

		public static bool operator !=(ClientId id1, ClientId id2)
		{
			return !(id1 == id2);
		}

		public static bool operator <(ClientId id1, ClientId id2)
		{
			return !object.ReferenceEquals(id1, id2) && (id1 == null || (!(id2 == null) && id1.id < id2.id));
		}

		public static bool operator >(ClientId id1, ClientId id2)
		{
			return !(id1 == id2) && !(id1 < id2);
		}

		internal static ClientId FromInt(int intId)
		{
			ClientId clientId = new ClientId(intId);
			if (!clientId.IsValidValue)
			{
				return null;
			}
			return clientId;
		}

		internal static ClientId FromString(string s, bool ignoreCase = false)
		{
			if (string.IsNullOrEmpty(s))
			{
				return null;
			}
			int intId;
			if (!int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out intId))
			{
				return null;
			}
			return ClientId.FromInt(intId);
		}

		public override int GetHashCode()
		{
			return this.id.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj, false);
		}

		public bool Equals(object obj, bool ignoreSsi)
		{
			return obj != null && obj is ClientId && this.Equals((ClientId)obj, ignoreSsi);
		}

		public bool Equals(ClientId obj, bool ignoreSsi = false)
		{
			if (obj == null)
			{
				return false;
			}
			int num = this.id ^ obj.id;
			if (ignoreSsi)
			{
				num &= int.MaxValue;
			}
			return num == 0;
		}

		public ClientId GetServerSideInstrumentationVariant(bool isLoggedViaServerSideInstrumentation)
		{
			if (isLoggedViaServerSideInstrumentation)
			{
				if (this.LoggedViaServerSideInstrumentation)
				{
					return this;
				}
				return ClientId.FromInt(this.id | int.MinValue);
			}
			else
			{
				if (this.LoggedViaServerSideInstrumentation)
				{
					return ClientId.FromInt(this.id & int.MaxValue);
				}
				return this;
			}
		}

		public override string ToString()
		{
			return this.id.ToString("X8");
		}

		internal int ToInt()
		{
			return this.id;
		}

		private const int SsiBit = -2147483648;

		public static readonly ClientId Min = new ClientId(0);

		public static readonly ClientId Web = new ClientId(1);

		public static readonly ClientId Mobile = new ClientId(2);

		public static readonly ClientId Tablet = new ClientId(3);

		public static readonly ClientId Desktop = new ClientId(4);

		public static readonly ClientId Exchange = new ClientId(5);

		public static readonly ClientId Outlook = new ClientId(6);

		public static readonly ClientId MacOutlook = new ClientId(7);

		public static readonly ClientId POP3 = new ClientId(8);

		public static readonly ClientId IMAP4 = new ClientId(9);

		public static readonly ClientId Other = new ClientId(10);

		public static readonly ClientId Lync = new ClientId(11);

		public static readonly ClientId Max = ClientId.Lync;

		private readonly int id;
	}
}
