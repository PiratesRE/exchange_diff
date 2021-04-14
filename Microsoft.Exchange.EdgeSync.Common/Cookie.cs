using System;
using System.Globalization;

namespace Microsoft.Exchange.EdgeSync
{
	[Serializable]
	public class Cookie
	{
		public Cookie(string baseDN)
		{
			this.baseDN = baseDN;
			this.lastUpdated = DateTime.MinValue;
			this.cookieValue = null;
			this.domainController = null;
		}

		public string BaseDN
		{
			get
			{
				return this.baseDN;
			}
			set
			{
				this.baseDN = value;
			}
		}

		public byte[] CookieValue
		{
			get
			{
				return this.cookieValue;
			}
			set
			{
				this.cookieValue = value;
			}
		}

		public string DomainController
		{
			get
			{
				return this.domainController;
			}
			set
			{
				this.domainController = value;
			}
		}

		public DateTime LastUpdated
		{
			get
			{
				return this.lastUpdated;
			}
			set
			{
				this.lastUpdated = value;
			}
		}

		public static bool TryDeserialize(string record, out Cookie cookie)
		{
			cookie = null;
			string[] array = record.Split(Cookie.RecordSeparator, 4);
			if (array.Length != 4)
			{
				return false;
			}
			DateTime dateTime;
			if (!DateTime.TryParse(array[2], CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
			{
				return false;
			}
			byte[] array2;
			try
			{
				array2 = Convert.FromBase64String(array[3]);
			}
			catch (FormatException)
			{
				return false;
			}
			cookie = new Cookie(array[0]);
			cookie.DomainController = array[1];
			cookie.LastUpdated = dateTime;
			cookie.CookieValue = array2;
			return true;
		}

		public static Cookie Deserialize(string record)
		{
			Cookie result;
			if (!Cookie.TryDeserialize(record, out result))
			{
				throw new FormatException("Cookie is not correctly formatted or is corrupted: " + record);
			}
			return result;
		}

		public static Cookie Clone(Cookie cookie)
		{
			Cookie cookie2 = new Cookie(cookie.baseDN);
			cookie2.DomainController = cookie.DomainController;
			cookie2.LastUpdated = cookie.LastUpdated;
			if (cookie.CookieValue != null && cookie.CookieValue.Length > 0)
			{
				cookie2.CookieValue = new byte[cookie.CookieValue.Length];
				Buffer.BlockCopy(cookie.CookieValue, 0, cookie2.CookieValue, 0, cookie.CookieValue.Length);
			}
			return cookie2;
		}

		public string Serialize()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0};{1};{2};{3}", new object[]
			{
				this.baseDN,
				this.domainController,
				this.lastUpdated,
				Convert.ToBase64String(this.cookieValue)
			});
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "DN=<{0}>; DC=<{1}>; LastUpdated=<{2}>; CookieLength=<{3}>", new object[]
			{
				this.baseDN,
				this.domainController,
				this.lastUpdated,
				(this.cookieValue == null) ? "<null>" : this.cookieValue.Length.ToString()
			});
		}

		private static readonly char[] RecordSeparator = new char[]
		{
			';'
		};

		private string baseDN;

		private byte[] cookieValue;

		private string domainController;

		private DateTime lastUpdated;
	}
}
