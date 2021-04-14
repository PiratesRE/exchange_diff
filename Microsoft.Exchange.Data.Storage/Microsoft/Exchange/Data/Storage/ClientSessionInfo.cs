using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class ClientSessionInfo
	{
		private ClientSessionInfo()
		{
		}

		public LogonType EffectiveLogonType { get; private set; }

		public string LogonUserSid { get; private set; }

		public string LogonUserDisplayName { get; private set; }

		public string ClientInfoString { get; private set; }

		public static byte[] WrapInfoForRemoteServer(MailboxSession session)
		{
			Util.ThrowOnNullArgument(session, "session");
			if (!session.IsRemote)
			{
				throw new InvalidOperationException("Only remote session supports ClientSessionInfo.");
			}
			if (!session.MailboxOwner.MailboxInfo.Configuration.IsMailboxAuditEnabled)
			{
				return null;
			}
			IdentityPair identityPair = IdentityHelper.GetIdentityPair(session);
			ClientSessionInfo clientSessionInfo = new ClientSessionInfo
			{
				EffectiveLogonType = COWAudit.ResolveEffectiveLogonType(session, null, null),
				ClientInfoString = session.ClientInfoString,
				LogonUserSid = identityPair.LogonUserSid,
				LogonUserDisplayName = identityPair.LogonUserDisplayName
			};
			return clientSessionInfo.ToBytes();
		}

		public static ClientSessionInfo FromBytes(byte[] blob)
		{
			if (blob == null)
			{
				return null;
			}
			ClientSessionInfo result;
			try
			{
				BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(ClientSessionInfo.serializationBinder);
				using (MemoryStream memoryStream = new MemoryStream(blob))
				{
					ClientSessionInfo clientSessionInfo = (ClientSessionInfo)binaryFormatter.Deserialize(memoryStream);
					result = clientSessionInfo;
				}
			}
			catch (FormatException)
			{
				throw new ClientSessionInfoParseException();
			}
			return result;
		}

		public byte[] ToBytes()
		{
			BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream(2048))
			{
				binaryFormatter.Serialize(memoryStream, this);
				byte[] array = memoryStream.ToArray();
				result = array;
			}
			return result;
		}

		internal static byte[] InternalTestGetFakeSerializedBlob()
		{
			ClientSessionInfo clientSessionInfo = new ClientSessionInfo
			{
				EffectiveLogonType = LogonType.BestAccess,
				ClientInfoString = "FakeTestSession",
				LogonUserSid = "S-Fake-Test-SID",
				LogonUserDisplayName = "Fake User Display Name"
			};
			return clientSessionInfo.ToBytes();
		}

		private const int MaxBlobSize = 2048;

		private static SerializationBinder serializationBinder = new ClientSessionInfo.ClientSessionInfoDeserializationBinder();

		private sealed class ClientSessionInfoDeserializationBinder : SerializationBinder
		{
			public override Type BindToType(string assemblyName, string typeName)
			{
				Type type = null;
				if (typeName == typeof(ClientSessionInfo).FullName)
				{
					type = typeof(ClientSessionInfo);
				}
				else if (typeName == typeof(LogonType).FullName)
				{
					type = typeof(LogonType);
				}
				if (type == null)
				{
					throw new ClientSessionInfoTypeParseException(typeName, assemblyName);
				}
				return type;
			}
		}
	}
}
