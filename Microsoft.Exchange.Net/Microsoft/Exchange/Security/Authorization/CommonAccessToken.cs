using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Security.Authorization
{
	public sealed class CommonAccessToken
	{
		static CommonAccessToken()
		{
			Type typeFromHandle = typeof(ExtensionDataKey);
			FieldInfo[] fields = typeFromHandle.GetFields(BindingFlags.Static | BindingFlags.Public);
			if (fields != null)
			{
				CommonAccessToken.ValidExtensionDataKeys = new List<string>(from f in fields
				select f.GetValue(null).ToString());
			}
		}

		public CommonAccessToken(AccessTokenType accessTokenType) : this(accessTokenType, false)
		{
		}

		public CommonAccessToken(AccessTokenType accessTokenType, bool shouldCompress)
		{
			this.Version = 1;
			this.TokenType = accessTokenType.ToString();
			this.IsCompressed = shouldCompress;
			this.ExtensionData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		}

		public CommonAccessToken(WindowsIdentity windowsIdentity) : this(windowsIdentity, false)
		{
		}

		public CommonAccessToken(WindowsIdentity windowsIdentity, bool shouldCompress)
		{
			if (windowsIdentity == null)
			{
				throw new ArgumentNullException("windowsIdentity");
			}
			this.Version = 1;
			this.TokenType = AccessTokenType.Windows.ToString();
			this.IsCompressed = shouldCompress;
			this.ExtensionData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			using (ClientSecurityContext clientSecurityContext = new ClientSecurityContext(windowsIdentity))
			{
				try
				{
					this.WindowsAccessToken = new WindowsAccessToken(windowsIdentity.GetSafeName(false), windowsIdentity.AuthenticationType, clientSecurityContext, this);
				}
				catch (SystemException innerException)
				{
					throw new CommonAccessTokenException(0, AuthorizationStrings.LogonNameIsMissing, innerException);
				}
			}
		}

		internal CommonAccessToken(string logonName, string authenticationType, ClientSecurityContext clientSecurityContext, bool shouldCompress)
		{
			this.Version = 1;
			this.TokenType = AccessTokenType.Windows.ToString();
			this.IsCompressed = shouldCompress;
			this.ExtensionData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			this.WindowsAccessToken = new WindowsAccessToken(logonName, authenticationType, clientSecurityContext, this);
		}

		private CommonAccessToken(Stream stream)
		{
			this.Deserialize(stream);
		}

		public string TokenType { get; private set; }

		[CLSCompliant(false)]
		public ushort Version { get; set; }

		public bool IsCompressed { get; internal set; }

		public Dictionary<string, string> ExtensionData { get; private set; }

		public WindowsAccessToken WindowsAccessToken { get; private set; }

		public static CommonAccessToken Deserialize(string token)
		{
			if (token == null)
			{
				throw new ArgumentNullException("token");
			}
			CommonAccessToken result;
			try
			{
				using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(token)))
				{
					result = new CommonAccessToken(memoryStream);
				}
			}
			catch (FormatException innerException)
			{
				throw new CommonAccessTokenException(0, AuthorizationStrings.InvalidCommonAccessTokenString, innerException);
			}
			return result;
		}

		public string Serialize()
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					this.Serialize(binaryWriter);
				}
				result = Convert.ToBase64String(memoryStream.ToArray());
			}
			return result;
		}

		internal void ReadAndValidateFieldType(BinaryReader reader, char fieldType, LocalizedString errorMessage)
		{
			char c = this.BinaryRead<char>(new Func<char>(reader.ReadChar), errorMessage);
			if (c != fieldType)
			{
				throw new CommonAccessTokenException((int)this.Version, errorMessage);
			}
		}

		internal T BinaryRead<T>(Func<T> readMethod, LocalizedString errorMessage)
		{
			T result;
			try
			{
				result = readMethod();
			}
			catch (EndOfStreamException innerException)
			{
				throw new CommonAccessTokenException((int)this.Version, errorMessage, innerException);
			}
			return result;
		}

		internal void WriteExtensionData(BinaryWriter writer)
		{
			if (this.ExtensionData != null)
			{
				writer.Write('E');
				writer.Write(this.ExtensionData.Count);
				foreach (KeyValuePair<string, string> keyValuePair in this.ExtensionData)
				{
					writer.Write(keyValuePair.Key);
					writer.Write(keyValuePair.Value ?? string.Empty);
				}
			}
		}

		internal void ReadExtensionData(BinaryReader reader)
		{
			int num = this.BinaryRead<int>(new Func<int>(reader.ReadInt32), AuthorizationStrings.InvalidExtensionDataLength);
			this.ExtensionData = new Dictionary<string, string>(num, StringComparer.OrdinalIgnoreCase);
			for (int i = 0; i < num; i++)
			{
				string text = this.BinaryRead<string>(new Func<string>(reader.ReadString), AuthorizationStrings.InvalidExtensionDataKey);
				if (string.IsNullOrEmpty(text))
				{
					throw new CommonAccessTokenException((int)this.Version, AuthorizationStrings.InvalidExtensionDataKey);
				}
				string value = this.BinaryRead<string>(new Func<string>(reader.ReadString), AuthorizationStrings.InvalidExtensionDataValue);
				this.ExtensionData[text] = value;
			}
		}

		private void Serialize(BinaryWriter writer)
		{
			writer.Write('V');
			writer.Write(this.Version);
			writer.Write('T');
			writer.Write(this.TokenType);
			writer.Write('C');
			writer.Write(this.IsCompressed);
			byte[] tokenBytes = this.GetTokenBytes();
			if (this.IsCompressed)
			{
				using (GZipStream gzipStream = new GZipStream(writer.BaseStream, CompressionMode.Compress, true))
				{
					gzipStream.Write(tokenBytes, 0, tokenBytes.Length);
					goto IL_77;
				}
			}
			writer.Write(tokenBytes);
			IL_77:
			writer.Flush();
		}

		private byte[] GetTokenBytes()
		{
			if (!this.TokenType.Equals(AccessTokenType.Windows.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				byte[] result;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
					{
						this.WriteExtensionData(binaryWriter);
						binaryWriter.Flush();
					}
					result = memoryStream.ToArray();
				}
				return result;
			}
			if (this.WindowsAccessToken != null)
			{
				return this.WindowsAccessToken.GetSerializedToken();
			}
			throw new CommonAccessTokenException((int)this.Version, AuthorizationStrings.ExpectingWindowsAccessToken);
		}

		private void Deserialize(Stream stream)
		{
			using (BinaryReader binaryReader = new BinaryReader(stream))
			{
				this.ReadAndValidateFieldType(binaryReader, 'V', AuthorizationStrings.MissingVersion);
				this.Version = this.BinaryRead<ushort>(new Func<ushort>(binaryReader.ReadUInt16), AuthorizationStrings.MissingVersion);
				this.ReadAndValidateFieldType(binaryReader, 'T', AuthorizationStrings.MissingTokenType);
				this.TokenType = this.BinaryRead<string>(new Func<string>(binaryReader.ReadString), AuthorizationStrings.MissingTokenType);
				this.ReadAndValidateFieldType(binaryReader, 'C', AuthorizationStrings.MissingIsCompressed);
				this.IsCompressed = this.BinaryRead<bool>(new Func<bool>(binaryReader.ReadBoolean), AuthorizationStrings.MissingIsCompressed);
				int num = (int)(binaryReader.BaseStream.Length - binaryReader.BaseStream.Position);
				if (num > 0)
				{
					byte[] array = binaryReader.ReadBytes(num);
					if (this.IsCompressed)
					{
						array = this.Decompress(array);
					}
					if (this.TokenType.Equals(AccessTokenType.Windows.ToString(), StringComparison.OrdinalIgnoreCase))
					{
						this.WindowsAccessToken = new WindowsAccessToken(this);
						this.WindowsAccessToken.DeserializeFromToken(array);
					}
					else
					{
						this.ReadExtensionData(array);
					}
				}
				else if (this.TokenType.Equals(AccessTokenType.Windows.ToString(), StringComparison.OrdinalIgnoreCase))
				{
					throw new CommonAccessTokenException((int)this.Version, AuthorizationStrings.InvalidWindowsAccessToken);
				}
			}
		}

		private byte[] Decompress(byte[] compressedBytes)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (MemoryStream memoryStream2 = new MemoryStream(compressedBytes))
				{
					using (GZipStream gzipStream = new GZipStream(memoryStream2, CompressionMode.Decompress))
					{
						gzipStream.CopyTo(memoryStream);
					}
				}
				result = memoryStream.ToArray();
			}
			return result;
		}

		private void ReadExtensionData(byte[] bytes)
		{
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				using (BinaryReader binaryReader = new BinaryReader(memoryStream))
				{
					this.ReadAndValidateFieldType(binaryReader, 'E', AuthorizationStrings.InvalidFieldType);
					this.ReadExtensionData(binaryReader);
				}
			}
		}

		public const string HttpHeaderName = "X-CommonAccessToken";

		public const string HttpContextItemKey = "Item-CommonAccessToken";

		private const ushort CurrentVersion = 1;

		private const ushort UnknownVersion = 0;

		private static readonly List<string> ValidExtensionDataKeys;
	}
}
