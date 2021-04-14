using System;
using System.Text;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core
{
	[Serializable]
	internal sealed class SerializedSecurityAccessToken : ISecurityAccessToken
	{
		public SidStringAndAttributes[] GroupSids { get; set; }

		public SidStringAndAttributes[] RestrictedGroupSids { get; set; }

		public string UserSid { get; set; }

		public string SmtpAddress { get; set; }

		public static SerializedSecurityAccessToken FromBytes(byte[] serializedTokenBytes)
		{
			SerializedSecurityAccessToken serializedSecurityAccessToken = new SerializedSecurityAccessToken();
			int num = 0;
			if (serializedTokenBytes.Length < SerializedSecurityAccessToken.serializedSecurityAccessTokenCookie.Length + 1)
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidSerializedAccessTokenException(), FaultParty.Sender);
			}
			for (int i = 0; i < SerializedSecurityAccessToken.serializedSecurityAccessTokenCookie.Length; i++)
			{
				if (serializedTokenBytes[num++] != SerializedSecurityAccessToken.serializedSecurityAccessTokenCookie[i])
				{
					throw FaultExceptionUtilities.CreateFault(new InvalidSerializedAccessTokenException(), FaultParty.Sender);
				}
			}
			if (serializedTokenBytes[num++] != 1)
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidSerializedAccessTokenException(), FaultParty.Sender);
			}
			serializedSecurityAccessToken.UserSid = SerializedSecurityAccessToken.DeserializeStringFromByteArray(serializedTokenBytes, ref num);
			serializedSecurityAccessToken.GroupSids = SerializedSecurityAccessToken.DeserializeSidArrayFromByteArray(serializedTokenBytes, ref num);
			serializedSecurityAccessToken.RestrictedGroupSids = SerializedSecurityAccessToken.DeserializeSidArrayFromByteArray(serializedTokenBytes, ref num);
			if (num < serializedTokenBytes.Length)
			{
				serializedSecurityAccessToken.SmtpAddress = SerializedSecurityAccessToken.DeserializeStringFromByteArray(serializedTokenBytes, ref num);
			}
			return serializedSecurityAccessToken;
		}

		public byte[] GetBytes()
		{
			byte[] array = new byte[this.GetByteCountToSerializeToken()];
			int num = 0;
			SerializedSecurityAccessToken.serializedSecurityAccessTokenCookie.CopyTo(array, num);
			num += SerializedSecurityAccessToken.serializedSecurityAccessTokenCookie.Length;
			array[num++] = 1;
			SerializedSecurityAccessToken.SerializeStringToByteArray(this.UserSid, array, ref num);
			SerializedSecurityAccessToken.SerializeSidArrayToByteArray(this.GroupSids, array, ref num);
			SerializedSecurityAccessToken.SerializeSidArrayToByteArray(this.RestrictedGroupSids, array, ref num);
			if (!string.IsNullOrEmpty(this.SmtpAddress))
			{
				SerializedSecurityAccessToken.SerializeStringToByteArray(this.SmtpAddress, array, ref num);
			}
			return array;
		}

		private static int GetByteCountToSerializeSidArray(SidStringAndAttributes[] sidArray)
		{
			int num = 0;
			num += 4;
			if (sidArray == null)
			{
				return num;
			}
			foreach (SidStringAndAttributes sidStringAndAttributes in sidArray)
			{
				num += 4;
				num += Encoding.UTF8.GetByteCount(sidStringAndAttributes.SecurityIdentifier);
				num += 4;
			}
			return num;
		}

		internal static void SerializeStringToByteArray(string stringToSerialize, byte[] byteArray, ref int byteIndex)
		{
			int index = byteIndex;
			byteIndex += 4;
			int bytes = Encoding.UTF8.GetBytes(stringToSerialize, 0, stringToSerialize.Length, byteArray, byteIndex);
			byteIndex += bytes;
			BitConverter.GetBytes(bytes).CopyTo(byteArray, index);
		}

		private static void SerializeSidArrayToByteArray(SidStringAndAttributes[] sidArray, byte[] byteArray, ref int byteIndex)
		{
			if (sidArray == null || sidArray.Length == 0)
			{
				for (int i = 0; i < 4; i++)
				{
					byteArray[byteIndex++] = 0;
				}
				return;
			}
			BitConverter.GetBytes(sidArray.Length).CopyTo(byteArray, byteIndex);
			byteIndex += 4;
			foreach (SidStringAndAttributes sidStringAndAttributes in sidArray)
			{
				SerializedSecurityAccessToken.SerializeStringToByteArray(sidStringAndAttributes.SecurityIdentifier, byteArray, ref byteIndex);
				BitConverter.GetBytes(sidStringAndAttributes.Attributes).CopyTo(byteArray, byteIndex);
				byteIndex += 4;
			}
		}

		internal static int ReadInt32(byte[] byteArray, ref int byteIndex)
		{
			if (byteArray.Length < byteIndex + 4)
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidSerializedAccessTokenException(), FaultParty.Sender);
			}
			int result = BitConverter.ToInt32(byteArray, byteIndex);
			byteIndex += 4;
			return result;
		}

		private static uint ReadUInt32(byte[] byteArray, ref int byteIndex)
		{
			if (byteArray.Length < byteIndex + 4)
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidSerializedAccessTokenException(), FaultParty.Sender);
			}
			uint result = BitConverter.ToUInt32(byteArray, byteIndex);
			byteIndex += 4;
			return result;
		}

		internal static string DeserializeStringFromByteArray(byte[] byteArray, ref int byteIndex)
		{
			int num = SerializedSecurityAccessToken.ReadInt32(byteArray, ref byteIndex);
			if (byteArray.Length < byteIndex + num)
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidSerializedAccessTokenException(), FaultParty.Sender);
			}
			string @string = Encoding.UTF8.GetString(byteArray, byteIndex, num);
			byteIndex += num;
			return @string;
		}

		private static SidStringAndAttributes[] DeserializeSidArrayFromByteArray(byte[] byteArray, ref int byteIndex)
		{
			int num = SerializedSecurityAccessToken.ReadInt32(byteArray, ref byteIndex);
			if (num == 0)
			{
				return null;
			}
			SidStringAndAttributes[] array = new SidStringAndAttributes[num];
			for (int i = 0; i < num; i++)
			{
				string identifier = SerializedSecurityAccessToken.DeserializeStringFromByteArray(byteArray, ref byteIndex);
				uint attribute = SerializedSecurityAccessToken.ReadUInt32(byteArray, ref byteIndex);
				array[i] = new SidStringAndAttributes(identifier, attribute);
			}
			return array;
		}

		private int GetByteCountToSerializeToken()
		{
			int num = 0;
			num += SerializedSecurityAccessToken.serializedSecurityAccessTokenCookie.Length;
			num++;
			num += 4;
			num += Encoding.UTF8.GetByteCount(this.UserSid);
			num += SerializedSecurityAccessToken.GetByteCountToSerializeSidArray(this.GroupSids);
			num += SerializedSecurityAccessToken.GetByteCountToSerializeSidArray(this.RestrictedGroupSids);
			if (!string.IsNullOrEmpty(this.SmtpAddress))
			{
				num += 4;
				num += Encoding.UTF8.GetByteCount(this.SmtpAddress);
			}
			return num;
		}

		private const int SerializedSecurityAccessTokenVersion = 1;

		private static byte[] serializedSecurityAccessTokenCookie = new byte[]
		{
			83,
			83,
			65,
			84
		};
	}
}
