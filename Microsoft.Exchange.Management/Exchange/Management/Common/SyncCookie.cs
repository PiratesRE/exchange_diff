using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Common
{
	[Serializable]
	internal sealed class SyncCookie
	{
		public SyncCookie(Guid domainController, WatermarkMap lowWatermarks, WatermarkMap highWatermarks, byte[] pageCookie) : this(domainController, lowWatermarks, highWatermarks, pageCookie, null)
		{
		}

		public SyncCookie(Guid domainController, WatermarkMap lowWatermarks, WatermarkMap highWatermarks, byte[] pageCookie, byte[] pageCookie2)
		{
			this.CheckNullArgument("lowWatermarks", lowWatermarks);
			this.CheckNullArgument("highWatermarks", highWatermarks);
			this.DomainController = domainController;
			this.LowWatermarks = lowWatermarks;
			this.HighWatermarks = highWatermarks;
			this.PageCookie = pageCookie;
			this.PageCookie2 = pageCookie2;
			this.Version = SyncCookie.CurrentVersion;
		}

		public int Version { get; private set; }

		public Guid DomainController { get; private set; }

		public WatermarkMap LowWatermarks { get; private set; }

		public WatermarkMap HighWatermarks { get; private set; }

		public byte[] PageCookie { get; private set; }

		public byte[] PageCookie2 { get; private set; }

		public long LowWatermark
		{
			get
			{
				if (!this.LowWatermarks.ContainsKey(this.DomainController))
				{
					return 0L;
				}
				return this.LowWatermarks[this.DomainController];
			}
		}

		public long HighWatermark
		{
			get
			{
				if (!this.HighWatermarks.ContainsKey(this.DomainController))
				{
					return 0L;
				}
				return this.HighWatermarks[this.DomainController];
			}
		}

		public byte[] ToBytes()
		{
			string text = string.Empty;
			if (this.PageCookie != null)
			{
				text = Convert.ToBase64String(this.PageCookie, Base64FormattingOptions.None);
			}
			string s = string.Join(SyncCookie.Delimiter, new string[]
			{
				this.Version.ToString(),
				this.DomainController.ToString(),
				this.LowWatermarks.SerializeToString(),
				this.HighWatermarks.SerializeToString(),
				text,
				text
			});
			return Encoding.UTF8.GetBytes(s);
		}

		public static bool TryFromBytes(byte[] cookieData, out SyncCookie cookie, out Exception ex)
		{
			ex = null;
			cookie = null;
			if (cookieData == null)
			{
				ex = new ArgumentNullException("cookieData");
				return false;
			}
			try
			{
				string @string = Encoding.UTF8.GetString(cookieData);
				string[] array = @string.Split(new string[]
				{
					SyncCookie.Delimiter
				}, StringSplitOptions.None);
				if (array.Length < 1)
				{
					ex = new InvalidCookieException();
					return false;
				}
				int num = int.Parse(array[0]);
				if (num != 1 && num != 2 && num != 3)
				{
					ex = new CookieVersionUnsupportedException(num);
					return false;
				}
				if (((num == 1 || num == 2) && array.Length != 5) || (num == 3 && array.Length != 6))
				{
					ex = new InvalidCookieException();
					return false;
				}
				Guid guid = new Guid(array[1]);
				WatermarkMap watermarkMap = WatermarkMap.Empty;
				if (num == 1)
				{
					long value = long.Parse(array[2]);
					watermarkMap.Add(guid, value);
				}
				else
				{
					watermarkMap = WatermarkMap.Parse(array[2]);
				}
				WatermarkMap watermarkMap2 = WatermarkMap.Empty;
				if (num == 1)
				{
					long value2 = long.Parse(array[3]);
					watermarkMap2.Add(guid, value2);
				}
				else
				{
					watermarkMap2 = WatermarkMap.Parse(array[3]);
				}
				byte[] pageCookie = null;
				if (!string.IsNullOrEmpty(array[4]))
				{
					pageCookie = Convert.FromBase64String(array[4]);
				}
				byte[] pageCookie2 = null;
				if (num == 3 && !string.IsNullOrEmpty(array[5]))
				{
					pageCookie2 = Convert.FromBase64String(array[5]);
				}
				cookie = new SyncCookie(guid, watermarkMap, watermarkMap2, pageCookie, pageCookie2);
			}
			catch (DecoderFallbackException innerException)
			{
				ex = new InvalidCookieException(innerException);
				return false;
			}
			catch (FormatException innerException2)
			{
				ex = new InvalidCookieException(innerException2);
				return false;
			}
			catch (OverflowException innerException3)
			{
				ex = new InvalidCookieException(innerException3);
				return false;
			}
			return true;
		}

		public static SyncCookie FromBytes(byte[] cookieData)
		{
			Exception ex = null;
			SyncCookie result = null;
			if (!SyncCookie.TryFromBytes(cookieData, out result, out ex))
			{
				throw ex;
			}
			return result;
		}

		private static string FormatWatermarks(WatermarkMap watermarks)
		{
			StringBuilder stringBuilder = new StringBuilder(watermarks.Count * 56);
			foreach (KeyValuePair<Guid, long> keyValuePair in watermarks)
			{
				stringBuilder.AppendFormat("{0}:{1};", keyValuePair.Key, keyValuePair.Value);
			}
			return stringBuilder.ToString();
		}

		private static WatermarkMap ParseWatermarks(string rawstring)
		{
			WatermarkMap empty = WatermarkMap.Empty;
			string[] array = rawstring.Split(new string[]
			{
				";"
			}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text in array)
			{
				string[] array3 = text.Split(new string[]
				{
					":"
				}, StringSplitOptions.None);
				if (array3.Length != 2)
				{
					throw new FormatException();
				}
				Guid key = new Guid(array3[0]);
				long value = long.Parse(array3[1]);
				if (!empty.ContainsKey(key))
				{
					empty.Add(key, value);
				}
			}
			return empty;
		}

		private void CheckNullArgument(string name, object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(name);
			}
		}

		private static readonly int CurrentVersion = 3;

		private static readonly string Delimiter = "\n";
	}
}
