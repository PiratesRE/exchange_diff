using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.Compliance;

namespace Microsoft.Exchange.Clients.Common
{
	public class CryptoMessage
	{
		private static byte[] Init()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 112, "Init", "f:\\15.00.1497\\sources\\dev\\clients\\src\\common\\CryptoMessage.cs");
			byte[] array = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest().ObjectGuid.ToByteArray();
			byte[] array2 = topologyConfigurationSession.GetDatabasesContainerId().ObjectGuid.ToByteArray();
			byte[] array3 = new byte[array.Length + array2.Length];
			array.CopyTo(array3, 0);
			array2.CopyTo(array3, array.Length);
			if (ExTraceGlobals.CryptoTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				using (SHA256Cng sha256Cng = new SHA256Cng())
				{
					byte[] bytes = sha256Cng.ComputeHash(array3);
					ExTraceGlobals.CryptoTracer.TraceDebug<string, string, string>(0L, "{0}.{1}: adObjectIdsBinaryHash={2}", "Clients.Common.CryptoMessage", "CryptoMessage()", CryptoMessage.GetHexString(bytes));
					sha256Cng.Clear();
				}
			}
			return array3;
		}

		public CryptoMessage(byte[] message, byte[] hiddenMessage)
		{
			this.CreateSignedMessage(message, hiddenMessage);
		}

		public CryptoMessage(DateTime timeStamp, string message, Guid userContextId, string logonUniqueKey)
		{
			byte[] bytes = BitConverter.GetBytes(timeStamp.Ticks);
			byte[] array = CryptoMessage.EncodeToByteArray(message, CryptoMessage.EncodingKind.UTF8);
			this.CreateSignedMessage(CryptoMessage.MergeArrays(new byte[][]
			{
				bytes,
				array
			}), CryptoMessage.GetHiddenMessage(userContextId, logonUniqueKey));
		}

		public static byte[] EncodeToByteArray(string message, CryptoMessage.EncodingKind encodingKind)
		{
			if (message == null)
			{
				message = string.Empty;
			}
			byte[] array = new byte[]
			{
				(byte)encodingKind
			};
			byte[] bytes;
			switch (encodingKind)
			{
			case CryptoMessage.EncodingKind.UTF8:
				bytes = new UTF8Encoding().GetBytes(message);
				goto IL_43;
			}
			bytes = new UnicodeEncoding().GetBytes(message);
			IL_43:
			return CryptoMessage.MergeArrays(new byte[][]
			{
				array,
				bytes
			});
		}

		public static string DecodeToString(byte[] message, bool legacy)
		{
			if (message != null)
			{
				if (legacy)
				{
					if (message.Length > 0)
					{
						return new UnicodeEncoding().GetString(message);
					}
				}
				else if (message.Length > 1)
				{
					switch (message[0])
					{
					case 1:
						return new UTF8Encoding().GetString(message, 1, message.Length - 1);
					}
					return new UnicodeEncoding().GetString(message, 1, message.Length - 1);
				}
			}
			return null;
		}

		private static string GetHexString(byte[] bytes)
		{
			if (!ExTraceGlobals.CryptoTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				return null;
			}
			if (bytes == null)
			{
				return "NULL_BYTE_ARRAY";
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in bytes)
			{
				stringBuilder.AppendFormat("{0:x2}", b);
			}
			return stringBuilder.ToString();
		}

		private static byte[] ComputeHash(byte[] messageBinary, byte[] privateKeyBinary)
		{
			byte[] result;
			using (HMACSHA256Cng hmacsha256Cng = new HMACSHA256Cng(privateKeyBinary))
			{
				byte[] array = hmacsha256Cng.ComputeHash(messageBinary);
				hmacsha256Cng.Clear();
				result = array;
			}
			return result;
		}

		private static bool AreEqualTimeSafe(byte[] a, byte[] b)
		{
			if (a == null || b == null || a.Length != b.Length)
			{
				return false;
			}
			int num = 0;
			for (int i = 0; i < a.Length; i++)
			{
				num |= (int)(a[i] ^ b[i]);
			}
			return num == 0;
		}

		private static byte[] Clone(byte[] byteArray)
		{
			byte[] array = new byte[(byteArray == null) ? 0 : byteArray.Length];
			if (array.Length > 0)
			{
				Array.Copy(byteArray, array, array.Length);
			}
			return array;
		}

		public static string Encode(byte[] byteArray)
		{
			int num = (byteArray.Length + 2 - (byteArray.Length + 2) % 3) / 3 * 4;
			char[] array = new char[num];
			int num2 = Convert.ToBase64CharArray(byteArray, 0, byteArray.Length, array, 0);
			for (int i = 0; i < num2; i++)
			{
				char c = array[i];
				if (c != '+')
				{
					if (c != '/')
					{
						if (c == '=')
						{
							array[i] = '.';
						}
					}
					else
					{
						array[i] = '_';
					}
				}
				else
				{
					array[i] = '-';
				}
			}
			return new string(array);
		}

		public static byte[] Decode(string stringToDecode)
		{
			char[] array = stringToDecode.ToCharArray();
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				char c = array[i];
				switch (c)
				{
				case '-':
					array[i] = '+';
					break;
				case '.':
					array[i] = '=';
					break;
				default:
					if (c == '_')
					{
						array[i] = '/';
					}
					break;
				}
			}
			return Convert.FromBase64CharArray(array, 0, num);
		}

		public static bool ParseMessage(byte[] hashAndMessageBinary, byte[] hiddenMessageBinary, out byte[] messageBinary)
		{
			hashAndMessageBinary = (hashAndMessageBinary ?? CryptoMessage.zeroArray);
			hiddenMessageBinary = (hiddenMessageBinary ?? CryptoMessage.zeroArray);
			int num = hashAndMessageBinary.Length - 32;
			if (num < 0)
			{
				messageBinary = CryptoMessage.zeroArray;
				return false;
			}
			messageBinary = new byte[num];
			Array.Copy(hashAndMessageBinary, 32, messageBinary, 0, num);
			CryptoMessage cryptoMessage = new CryptoMessage(messageBinary, hiddenMessageBinary);
			return CryptoMessage.AreEqualTimeSafe(cryptoMessage.hashAndMessageBinary, hashAndMessageBinary);
		}

		public static byte[] GetHiddenMessage(Guid userContextId, string logonUniqueKey)
		{
			byte[] array = userContextId.ToByteArray();
			byte[] bytes = new UnicodeEncoding().GetBytes(logonUniqueKey ?? string.Empty);
			return CryptoMessage.MergeArrays(new byte[][]
			{
				array,
				bytes
			});
		}

		private void CreateSignedMessage(byte[] message, byte[] hiddenMessage)
		{
			this.messageBinary = CryptoMessage.Clone(message);
			this.hiddenMessageBinary = CryptoMessage.Clone(hiddenMessage);
			this.privateKeyBinary = CryptoMessage.MergeArrays(new byte[][]
			{
				CryptoMessage.adObjectIdsBinary,
				this.hiddenMessageBinary
			});
			this.hashBinary = CryptoMessage.ComputeHash(this.messageBinary, this.privateKeyBinary);
			this.hashAndMessageBinary = CryptoMessage.MergeArrays(new byte[][]
			{
				this.hashBinary,
				this.messageBinary
			});
			this.hashAndMessageString = CryptoMessage.Encode(this.hashAndMessageBinary);
		}

		public static byte[] MergeArrays(params byte[][] inpArrays)
		{
			if (inpArrays == null || inpArrays.Length == 0)
			{
				return null;
			}
			int num = 0;
			foreach (byte[] array in inpArrays)
			{
				num += array.Length;
			}
			byte[] array2 = new byte[num];
			int num2 = 0;
			foreach (byte[] array3 in inpArrays)
			{
				Array.Copy(array3, 0, array2, num2, array3.Length);
				num2 += array3.Length;
			}
			return array2;
		}

		public static bool ParseMessage(string hashAndMessage, byte[] hiddenMessageBinary, out DateTime timeStamp, out byte[] message)
		{
			return CryptoMessage.ParseMessage(hashAndMessage, hiddenMessageBinary, false, out timeStamp, out message);
		}

		public static bool ParseMessage(string hashAndMessage, byte[] hiddenMessageBinary, bool ignoreHmac, out DateTime timeStamp, out byte[] message)
		{
			timeStamp = DateTime.MinValue;
			bool flag = false;
			message = null;
			try
			{
				byte[] array = CryptoMessage.Decode(hashAndMessage ?? string.Empty);
				byte[] array2;
				flag = CryptoMessage.ParseMessage(array, hiddenMessageBinary, out array2);
				if (flag || ignoreHmac)
				{
					long ticks = BitConverter.ToInt64(array2, 0);
					timeStamp = new DateTime(ticks, DateTimeKind.Utc);
					int num = array2.Length - 8;
					message = new byte[num];
					Array.Copy(array2, 8, message, 0, num);
				}
				else
				{
					ExTraceGlobals.CryptoTracer.TraceDebug(2L, "{0}.{1}: failed: messageString={2}, hiddenMessage={3}", new object[]
					{
						"Clients.Common.CryptoMessage",
						"ParseMessage",
						hashAndMessage,
						CryptoMessage.GetHexString(hiddenMessageBinary)
					});
				}
			}
			catch (Exception ex)
			{
				flag = false;
				ExTraceGlobals.CryptoTracer.TraceDebug(3L, "{0}.{1}: Exception: messageString={2}, hiddenMessage={3}, exception={3}", new object[]
				{
					"Clients.Common.CryptoMessage",
					"ParseMessage",
					hashAndMessage,
					CryptoMessage.GetHexString(hiddenMessageBinary),
					ex
				});
			}
			return flag;
		}

		public static string ExtractUrl(string hashAndMessage, bool legacyFormat)
		{
			DateTime dateTime;
			byte[] message;
			CryptoMessage.ParseMessage(hashAndMessage, null, true, out dateTime, out message);
			return CryptoMessage.DecodeToString(message, legacyFormat);
		}

		public override string ToString()
		{
			return this.hashAndMessageString;
		}

		private const string ThisClassName = "Clients.Common.CryptoMessage";

		private const int HashLength = 32;

		private const int TimeStampLength = 8;

		private static byte[] zeroArray = new byte[0];

		private static byte[] adObjectIdsBinary = CryptoMessage.Init();

		private string hashAndMessageString;

		private byte[] hashAndMessageBinary;

		private byte[] hashBinary;

		private byte[] messageBinary;

		private byte[] hiddenMessageBinary;

		private byte[] privateKeyBinary;

		public enum EncodingKind : byte
		{
			Unicode,
			UTF8
		}
	}
}
