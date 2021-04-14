using System;
using System.Globalization;

namespace Microsoft.Exchange.MessageSecurity.EdgeSync
{
	public struct LeaseToken
	{
		public LeaseToken(string path, DateTime expiry, LeaseTokenType type, DateTime lastSync, DateTime alertTime, int version)
		{
			this = new LeaseToken(string.Empty, path, expiry, type, lastSync, alertTime, version);
			this.stringForm = this.ComputeString();
		}

		private LeaseToken(string stringForm, string path, DateTime expiry, LeaseTokenType type, DateTime lastSync, DateTime alertTime, int version)
		{
			this.stringForm = stringForm;
			this.path = path;
			this.expiry = expiry;
			this.type = type;
			this.lastSync = lastSync;
			this.alertTime = alertTime;
			this.version = version;
		}

		public string StringForm
		{
			get
			{
				return this.stringForm;
			}
		}

		public string Path
		{
			get
			{
				return this.path;
			}
		}

		public DateTime Expiry
		{
			get
			{
				return this.expiry;
			}
		}

		public LeaseTokenType Type
		{
			get
			{
				return this.type;
			}
		}

		public DateTime LastSync
		{
			get
			{
				return this.lastSync;
			}
		}

		public DateTime AlertTime
		{
			get
			{
				return this.alertTime;
			}
		}

		public bool NotHeld
		{
			get
			{
				return string.IsNullOrEmpty(this.path);
			}
		}

		public int Version
		{
			get
			{
				return this.version;
			}
		}

		public static LeaseToken Parse(string leaseString)
		{
			string[] array = leaseString.Split(new char[]
			{
				';'
			});
			string text = string.Empty;
			DateTime minValue = DateTime.MinValue;
			DateTime minValue2 = DateTime.MinValue;
			DateTime minValue3 = DateTime.MinValue;
			LeaseTokenType leaseTokenType = LeaseTokenType.Lock;
			int num = 0;
			if (string.IsNullOrEmpty(leaseString))
			{
				return LeaseToken.Empty;
			}
			if (array.Length < 3)
			{
				return LeaseToken.Empty;
			}
			text = array[0];
			if (!DateTime.TryParse(array[1], CultureInfo.InvariantCulture, DateTimeStyles.None, out minValue))
			{
				return LeaseToken.Empty;
			}
			if (!LeaseToken.TryStringToLeaseTokenType(array[2], out leaseTokenType))
			{
				return LeaseToken.Empty;
			}
			if (array.Length == 4 && !int.TryParse(array[3], NumberStyles.None, CultureInfo.InvariantCulture, out num))
			{
				return LeaseToken.Empty;
			}
			if (array.Length == 5)
			{
				if (!DateTime.TryParse(array[3], CultureInfo.InvariantCulture, DateTimeStyles.None, out minValue2))
				{
					return LeaseToken.Empty;
				}
				if (!DateTime.TryParse(array[4], CultureInfo.InvariantCulture, DateTimeStyles.None, out minValue3))
				{
					return LeaseToken.Empty;
				}
			}
			if (array.Length >= 6)
			{
				if (!DateTime.TryParse(array[3], CultureInfo.InvariantCulture, DateTimeStyles.None, out minValue2))
				{
					return LeaseToken.Empty;
				}
				if (!DateTime.TryParse(array[4], CultureInfo.InvariantCulture, DateTimeStyles.None, out minValue3))
				{
					return LeaseToken.Empty;
				}
				if (!int.TryParse(array[5], NumberStyles.None, CultureInfo.InvariantCulture, out num))
				{
					return LeaseToken.Empty;
				}
			}
			return new LeaseToken(leaseString, text, minValue, leaseTokenType, minValue2, minValue3, num);
		}

		private static string LeaseTokenTypeToString(LeaseTokenType type)
		{
			switch (type)
			{
			case LeaseTokenType.Lock:
				return "L";
			case LeaseTokenType.Option:
				return "O";
			case LeaseTokenType.None:
				return "N";
			default:
				throw new InvalidOperationException("LeaseTokenTypeToString");
			}
		}

		private static bool TryStringToLeaseTokenType(string s, out LeaseTokenType type)
		{
			if (s.StartsWith("L"))
			{
				type = LeaseTokenType.Lock;
				return true;
			}
			if (s.StartsWith("O"))
			{
				type = LeaseTokenType.Option;
				return true;
			}
			if (s.StartsWith("N"))
			{
				type = LeaseTokenType.None;
				return true;
			}
			type = LeaseTokenType.None;
			return false;
		}

		private string ComputeString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0};{1};{2};{3};{4};{5}", new object[]
			{
				this.path,
				this.expiry.ToString(CultureInfo.InvariantCulture),
				LeaseToken.LeaseTokenTypeToString(this.type),
				this.lastSync.ToString(CultureInfo.InvariantCulture),
				this.alertTime.ToString(CultureInfo.InvariantCulture),
				this.version.ToString(CultureInfo.InvariantCulture)
			});
		}

		public static readonly LeaseToken Empty = new LeaseToken(string.Empty, DateTime.MinValue, LeaseTokenType.None, DateTime.MinValue, DateTime.MinValue, 0);

		private readonly string stringForm;

		private readonly string path;

		private readonly DateTime expiry;

		private readonly DateTime lastSync;

		private readonly DateTime alertTime;

		private readonly LeaseTokenType type;

		private readonly int version;
	}
}
