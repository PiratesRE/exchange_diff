using System;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SessionContextIdentifier
	{
		public SessionContextIdentifier()
		{
			this.contextId = Interlocked.Increment(ref SessionContextIdentifier.nextContextId);
			this.contextCookie = SessionContextIdentifier.BuildContextCookie(SessionContextIdentifier.CookieInstancePrefix, this.contextId);
			this.sequenceCounter = 0;
			this.nextSequenceCookie = SessionContextIdentifier.GenerateSequenceCookie(this.sequenceCounter);
			this.previousSequenceCookie = string.Empty;
			this.currentSequenceCookie = null;
		}

		public long Id
		{
			get
			{
				return this.contextId;
			}
		}

		public string Cookie
		{
			get
			{
				return this.contextCookie;
			}
		}

		public string NextSequenceCookie
		{
			get
			{
				return this.nextSequenceCookie;
			}
		}

		private static Random Random
		{
			get
			{
				if (SessionContextIdentifier.random == null)
				{
					SessionContextIdentifier.random = new Random();
				}
				return SessionContextIdentifier.random;
			}
		}

		private static byte[] CookieInstancePrefix
		{
			get
			{
				if (SessionContextIdentifier.cookieInstancePrefix == null)
				{
					lock (SessionContextIdentifier.cookieInstancePrefixLock)
					{
						if (SessionContextIdentifier.cookieInstancePrefix == null)
						{
							SessionContextIdentifier.cookieInstancePrefix = SessionContextIdentifier.BuildCookieInstancePrefix(Environment.MachineName, DateTime.UtcNow, SessionContextIdentifier.Random.Next());
						}
					}
				}
				return SessionContextIdentifier.cookieInstancePrefix;
			}
		}

		public static bool TryGetIdFromCookie(string cookie, out long id, out Exception failureException)
		{
			id = 0L;
			failureException = null;
			byte[] array;
			try
			{
				array = Convert.FromBase64String(cookie);
			}
			catch (FormatException)
			{
				failureException = ProtocolException.FromResponseCode((LID)54688, "Context cookie is not proper Base64 encoding.", ResponseCode.InvalidContextCookie, null);
				return false;
			}
			byte[] array2 = SessionContextIdentifier.CookieInstancePrefix;
			if (array.Length < array2.Length + 8)
			{
				if (!SessionContextIdentifier.TryGetRoutingFailure(array, out failureException))
				{
					failureException = ProtocolException.FromResponseCode((LID)42400, "Context cookie not found.", ResponseCode.ContextNotFound, null);
				}
				return false;
			}
			for (int i = 0; i < array2.Length; i++)
			{
				if (array[i] != array2[i])
				{
					if (!SessionContextIdentifier.TryGetRoutingFailure(array, out failureException))
					{
						failureException = ProtocolException.FromResponseCode((LID)58784, "Context cookie not found.", ResponseCode.ContextNotFound, null);
					}
					return false;
				}
			}
			id = BitConverter.ToInt64(array, array2.Length);
			return true;
		}

		public string BeginSequenceOperation(string sequenceCookie)
		{
			string result;
			lock (this.sequenceLock)
			{
				if (this.currentSequenceCookie != null)
				{
					throw ProtocolException.FromResponseCode((LID)44316, string.Format("Currently processing a sequenced operation; found={0}, current={1}.", sequenceCookie, this.currentSequenceCookie), ResponseCode.InvalidSequence, null);
				}
				if (string.Compare(sequenceCookie, this.nextSequenceCookie, true) != 0)
				{
					if (string.Compare(sequenceCookie, this.previousSequenceCookie, true) == 0)
					{
						throw ProtocolException.FromResponseCode((LID)39456, string.Format("Request contains previous sequence cookie; found={0}, expected={1}.", sequenceCookie, this.nextSequenceCookie), ResponseCode.InvalidSequence, null);
					}
					throw ProtocolException.FromResponseCode((LID)39456, string.Format("Request contains wrong sequence cookie; found={0}, expected={1}.", sequenceCookie, this.nextSequenceCookie), ResponseCode.InvalidSequence, null);
				}
				else
				{
					this.sequenceCounter++;
					this.previousSequenceCookie = this.nextSequenceCookie;
					this.currentSequenceCookie = this.nextSequenceCookie;
					this.nextSequenceCookie = SessionContextIdentifier.GenerateSequenceCookie(this.sequenceCounter);
					result = this.nextSequenceCookie;
				}
			}
			return result;
		}

		public void EndSequenceOperation()
		{
			this.currentSequenceCookie = null;
		}

		internal static byte[] BuildCookieInstancePrefix(string machineName, DateTime creationTime, int randomNumber)
		{
			Util.ThrowOnNullArgument(machineName, "machineName");
			byte[] array = SessionContextIdentifier.Scramble(new ArraySegment<byte>(Encoding.UTF8.GetBytes(string.Format("{0}#{1}#{2}", machineName, creationTime.ToString("u"), randomNumber))));
			byte[] bytes = BitConverter.GetBytes(0);
			byte[] array2 = new byte[SessionContextIdentifier.cookieInstanceSignature.Length + bytes.Length + array.Length];
			Array.Copy(SessionContextIdentifier.cookieInstanceSignature, 0, array2, 0, SessionContextIdentifier.cookieInstanceSignature.Length);
			Array.Copy(bytes, 0, array2, SessionContextIdentifier.cookieInstanceSignature.Length, bytes.Length);
			Array.Copy(array, 0, array2, SessionContextIdentifier.cookieInstanceSignature.Length + bytes.Length, array.Length);
			return array2;
		}

		internal static string BuildContextCookie(byte[] cookieInstancePrefix, long contextId)
		{
			Util.ThrowOnNullArgument(cookieInstancePrefix, "cookieInstancePrefix");
			byte[] bytes = BitConverter.GetBytes(contextId);
			byte[] array = new byte[cookieInstancePrefix.Length + bytes.Length];
			Array.Copy(cookieInstancePrefix, 0, array, 0, cookieInstancePrefix.Length);
			Array.Copy(bytes, 0, array, cookieInstancePrefix.Length, bytes.Length);
			return Convert.ToBase64String(array);
		}

		private static string GenerateSequenceCookie(int sequenceCounter)
		{
			return string.Format("{0}-{1}", sequenceCounter, Convert.ToBase64String(BitConverter.GetBytes(SessionContextIdentifier.Random.Next())));
		}

		private static bool TryGetRoutingInfo(byte[] cookieBytes, out string backendServer)
		{
			backendServer = null;
			if (cookieBytes.Length <= SessionContextIdentifier.cookieInstanceSignature.Length + 4 + 8)
			{
				return false;
			}
			for (int i = 0; i < SessionContextIdentifier.cookieInstanceSignature.Length; i++)
			{
				if (cookieBytes[i] != SessionContextIdentifier.cookieInstanceSignature[i])
				{
					return false;
				}
			}
			try
			{
				int num = BitConverter.ToInt32(cookieBytes, SessionContextIdentifier.cookieInstanceSignature.Length);
				if (num != 0)
				{
					return false;
				}
			}
			catch (Exception)
			{
				return false;
			}
			bool result;
			try
			{
				string @string = Encoding.UTF8.GetString(SessionContextIdentifier.Unscramble(new ArraySegment<byte>(cookieBytes, SessionContextIdentifier.cookieInstanceSignature.Length + 4, cookieBytes.Length - (SessionContextIdentifier.cookieInstanceSignature.Length + 4 + 8))));
				string[] array = @string.Split(SessionContextIdentifier.cookieSeparator, StringSplitOptions.RemoveEmptyEntries);
				if (array == null || array.Length < 1)
				{
					result = false;
				}
				else
				{
					backendServer = array[0];
					result = true;
				}
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		private static bool TryGetRoutingFailure(byte[] cookieBytes, out Exception failureException)
		{
			failureException = null;
			string text;
			if (!SessionContextIdentifier.TryGetRoutingInfo(cookieBytes, out text))
			{
				return false;
			}
			if (Environment.MachineName != text)
			{
				failureException = ProtocolException.FromResponseCode((LID)55068, string.Format("Context cookie for a different BE machine [{0}]; possible failover detected.", text), ResponseCode.ContextNotFound, null);
				return true;
			}
			failureException = ProtocolException.FromResponseCode((LID)63228, "Context cookie for previous instance on this BE machine; possible application pool recycle detected.", ResponseCode.ContextNotFound, null);
			return true;
		}

		private static byte[] Scramble(ArraySegment<byte> data)
		{
			if (data.Count == 0)
			{
				return Array<byte>.Empty;
			}
			byte[] array = new byte[data.Count];
			Array.Copy(data.Array, data.Offset, array, 0, array.Length);
			array[0] = (array[0] ^ 165);
			int i = 0;
			int num = 1;
			while (i < array.Length - 1)
			{
				array[num] ^= array[num - 1];
				num++;
				i++;
			}
			return array;
		}

		private static byte[] Unscramble(ArraySegment<byte> data)
		{
			if (data.Count == 0)
			{
				return Array<byte>.Empty;
			}
			byte[] array = new byte[data.Count];
			Array.Copy(data.Array, data.Offset, array, 0, array.Length);
			int i = 0;
			int num = array.Length - 1;
			while (i < array.Length - 1)
			{
				array[num] ^= array[num - 1];
				num--;
				i++;
			}
			array[0] = (array[0] ^ 165);
			return array;
		}

		private const int CurrentCookieVersion = 0;

		private static readonly byte[] cookieInstanceSignature = new byte[]
		{
			48,
			3,
			200
		};

		private static readonly char[] cookieSeparator = new char[]
		{
			'#'
		};

		private static readonly object cookieInstancePrefixLock = new object();

		private static byte[] cookieInstancePrefix = null;

		private static long nextContextId = 0L;

		[ThreadStatic]
		private static Random random = null;

		private readonly string contextCookie;

		private readonly long contextId;

		private readonly object sequenceLock = new object();

		private int sequenceCounter;

		private string currentSequenceCookie;

		private string previousSequenceCookie;

		private string nextSequenceCookie;
	}
}
