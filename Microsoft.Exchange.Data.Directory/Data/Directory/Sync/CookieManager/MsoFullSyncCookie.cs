using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.Sync.CookieManager
{
	[Serializable]
	internal sealed class MsoFullSyncCookie
	{
		public int Version { get; private set; }

		public DateTime WhenSyncRequested { get; set; }

		public DateTime WhenSyncStarted { get; set; }

		public string SyncRequestor
		{
			get
			{
				return this.syncRequestor;
			}
			set
			{
				this.syncRequestor = (value ?? string.Empty);
			}
		}

		public DateTime Timestamp { get; set; }

		public TenantSyncType SyncType { get; set; }

		public MsoFullSyncCookie(byte[] rawCookie, int cookieVersion)
		{
			this.RawCookie = rawCookie;
			this.WhenSyncRequested = DateTime.MinValue;
			this.WhenSyncStarted = DateTime.MinValue;
			this.SyncRequestor = string.Empty;
			this.Timestamp = DateTime.MinValue;
			this.Version = cookieVersion;
		}

		public byte[] RawCookie { get; private set; }

		public static MsoFullSyncCookie FromStorageCookie(byte[] storageCookie)
		{
			MsoFullSyncCookie result;
			Exception ex;
			if (!MsoFullSyncCookie.TryFromStorageCookie(storageCookie, out result, out ex))
			{
				throw ex;
			}
			return result;
		}

		public byte[] ToStorageCookie()
		{
			List<string> list = new List<string>(3);
			if (this.Version >= 1)
			{
				list.Add(this.GetCookieVersion1Data());
			}
			if (this.Version >= 2 || this.Version < 1)
			{
				list.Add(this.GetCookieVersion2Data());
			}
			if (this.Version >= 3)
			{
				list.Add(this.GetCookieVersion3Data());
			}
			string s = string.Join("\n", list);
			return Encoding.UTF8.GetBytes(s);
		}

		private static bool TryFromStorageCookie(byte[] storageCookie, out MsoFullSyncCookie cookie, out Exception ex)
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
					"\n"
				}, StringSplitOptions.None);
				byte[] rawCookie = Convert.FromBase64String(array[0]);
				cookie = new MsoFullSyncCookie(rawCookie, 3);
				if (array.Length == 1)
				{
					MsoFullSyncCookie.FillCookieWithVersion1Data(cookie);
				}
				if (array.Length >= 2)
				{
					MsoFullSyncCookie.FillCookieWithVersion1And2Data(cookie, array[1]);
				}
				if (array.Length >= 3)
				{
					MsoFullSyncCookie.FillCookieWithVersion3Data(cookie, array[2]);
				}
			}
			catch (DecoderFallbackException innerException)
			{
				ex = new InvalidTenantFullSyncCookieException(innerException);
			}
			catch (ArgumentException innerException2)
			{
				ex = new InvalidTenantFullSyncCookieException(innerException2);
			}
			catch (FormatException innerException3)
			{
				ex = new InvalidTenantFullSyncCookieException(innerException3);
			}
			catch (OverflowException innerException4)
			{
				ex = new InvalidTenantFullSyncCookieException(innerException4);
			}
			return null == ex;
		}

		private static void FillCookieWithVersion1Data(MsoFullSyncCookie cookie)
		{
			cookie.SyncType = TenantSyncType.Full;
			cookie.Version = 1;
			cookie.WhenSyncRequested = DateTime.UtcNow;
		}

		private static void FillCookieWithVersion1And2Data(MsoFullSyncCookie cookie, string data)
		{
			byte[] buffer = Convert.FromBase64String(data);
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					cookie.Version = binaryReader.ReadInt32();
					cookie.SyncType = (TenantSyncType)binaryReader.ReadByte();
					cookie.WhenSyncRequested = DateTime.FromBinary(binaryReader.ReadInt64());
					cookie.WhenSyncStarted = DateTime.FromBinary(binaryReader.ReadInt64());
					cookie.Timestamp = DateTime.FromBinary(binaryReader.ReadInt64());
				}
			}
		}

		private static void FillCookieWithVersion3Data(MsoFullSyncCookie cookie, string data)
		{
			byte[] buffer = Convert.FromBase64String(data);
			using (MemoryStream memoryStream = new MemoryStream(buffer))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					cookie.SyncRequestor = binaryReader.ReadString();
				}
			}
		}

		private string GetCookieVersion1Data()
		{
			if (this.RawCookie == null)
			{
				return string.Empty;
			}
			return Convert.ToBase64String(this.RawCookie, Base64FormattingOptions.None);
		}

		private string GetCookieVersion2Data()
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(this.Version);
					binaryWriter.Write((byte)this.SyncType);
					binaryWriter.Write(this.WhenSyncRequested.ToBinary());
					binaryWriter.Write(this.WhenSyncStarted.ToBinary());
					binaryWriter.Write(this.Timestamp.ToBinary());
					binaryWriter.Flush();
					result = Convert.ToBase64String(memoryStream.ToArray());
				}
			}
			return result;
		}

		private string GetCookieVersion3Data()
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(this.SyncRequestor);
					binaryWriter.Flush();
					result = Convert.ToBase64String(memoryStream.ToArray());
				}
			}
			return result;
		}

		public bool IsRawCookieNull
		{
			get
			{
				return this.RawCookie != null && this.RawCookie.Length > 0;
			}
		}

		private const string Delimiter = "\n";

		public const int CurrentCookieVersion = 3;

		private string syncRequestor;
	}
}
