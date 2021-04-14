using System;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.Sync.CookieManager
{
	[Serializable]
	internal class MsoMainStreamCookie
	{
		public MsoMainStreamCookie(string serviceInstanceName, byte[] rawCookie, DateTime timeStamp, ServerVersion syncPropertySetVersion, bool isSyncPropertySetUpgrading)
		{
			if (string.IsNullOrEmpty(serviceInstanceName))
			{
				throw new ArgumentNullException("serviceInstanceName");
			}
			if (rawCookie == null || rawCookie.Length == 0)
			{
				throw new ArgumentNullException("rawCookie");
			}
			this.ServiceInstanceName = serviceInstanceName;
			this.RawCookie = rawCookie;
			this.TimeStamp = timeStamp;
			this.SyncPropertySetVersion = syncPropertySetVersion;
			this.IsSyncPropertySetUpgrading = isSyncPropertySetUpgrading;
		}

		public string ServiceInstanceName { get; private set; }

		public byte[] RawCookie { get; private set; }

		public DateTime TimeStamp { get; private set; }

		public ServerVersion SyncPropertySetVersion { get; private set; }

		public bool IsSyncPropertySetUpgrading { get; private set; }

		public static MsoMainStreamCookie FromStorageCookie(byte[] storageCookie)
		{
			Exception ex = null;
			MsoMainStreamCookie result = null;
			if (!MsoMainStreamCookie.TryFromStorageCookie(storageCookie, out result, out ex))
			{
				throw ex;
			}
			return result;
		}

		public static bool TryFromStorageCookie(byte[] storageCookie, out MsoMainStreamCookie cookie, out Exception ex)
		{
			ex = null;
			cookie = null;
			if (storageCookie == null || storageCookie.Length == 0)
			{
				ex = new ArgumentNullException("storageCookie");
				return false;
			}
			try
			{
				string @string = Encoding.UTF8.GetString(storageCookie);
				string[] array = @string.Split(new string[]
				{
					MsoMainStreamCookie.Delimiter
				}, StringSplitOptions.None);
				if (array.Length != 2 && array.Length != 4 && array.Length != 6)
				{
					ex = new InvalidMainStreamCookieException();
					return false;
				}
				string serviceInstanceName = string.Empty;
				DateTime timeStamp = DateTime.UtcNow;
				ServerVersion syncPropertySetVersion = SyncPropertyDefinition.InitialSyncPropertySetVersion;
				bool isSyncPropertySetUpgrading = false;
				int num;
				if (array.Length == 2)
				{
					num = 1;
					serviceInstanceName = array[0];
				}
				else
				{
					num = Convert.ToInt32(array[0]);
					serviceInstanceName = array[1];
					timeStamp = DateTime.FromFileTimeUtc(long.Parse(array[2]));
					if (num >= MsoMainStreamCookie.Version)
					{
						syncPropertySetVersion = new ServerVersion(Convert.ToInt32(array[3]));
						isSyncPropertySetUpgrading = bool.Parse(array[4]);
					}
				}
				if (num > MsoMainStreamCookie.Version)
				{
					return false;
				}
				byte[] rawCookie = Convert.FromBase64String(array[array.Length - 1]);
				cookie = new MsoMainStreamCookie(serviceInstanceName, rawCookie, timeStamp, syncPropertySetVersion, isSyncPropertySetUpgrading);
			}
			catch (DecoderFallbackException innerException)
			{
				ex = new InvalidMainStreamCookieException(innerException);
				return false;
			}
			catch (ArgumentException innerException2)
			{
				ex = new InvalidMainStreamCookieException(innerException2);
			}
			catch (FormatException innerException3)
			{
				ex = new InvalidMainStreamCookieException(innerException3);
				return false;
			}
			catch (OverflowException innerException4)
			{
				ex = new InvalidMainStreamCookieException(innerException4);
				return false;
			}
			return true;
		}

		public byte[] ToStorageCookie()
		{
			string text = Convert.ToBase64String(this.RawCookie, Base64FormattingOptions.None);
			string s = string.Empty;
			string[] value = new string[]
			{
				MsoMainStreamCookie.Version.ToString(),
				this.ServiceInstanceName,
				this.TimeStamp.ToFileTimeUtc().ToString(),
				this.SyncPropertySetVersion.ToInt().ToString(),
				this.IsSyncPropertySetUpgrading.ToString(),
				text
			};
			s = string.Join(MsoMainStreamCookie.Delimiter, value);
			return Encoding.UTF8.GetBytes(s);
		}

		public static readonly int Version = 2;

		private static readonly string Delimiter = "\n";
	}
}
