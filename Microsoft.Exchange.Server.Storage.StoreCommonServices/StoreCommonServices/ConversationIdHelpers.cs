using System;
using System.Text;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public static class ConversationIdHelpers
	{
		public static byte[] GenerateNewConversationIndex()
		{
			return ConversationIdHelpers.GenerateConversationIndex(Guid.NewGuid().ToByteArray());
		}

		public static byte[] GenerateConversationIndex(byte[] conversationId)
		{
			byte[] array = new byte[22];
			array[0] = 1;
			long value = DateTime.UtcNow.ToFileTimeUtc();
			byte[] bytes = BitConverter.GetBytes(value);
			array[1] = bytes[6];
			array[2] = bytes[5];
			array[3] = bytes[4];
			array[4] = bytes[3];
			array[5] = bytes[2];
			Array.Copy(conversationId, 0, array, 6, conversationId.Length);
			return array;
		}

		public static byte[] FabricateConversationId(bool messageIsAssociated, byte[] conversationIndex, bool? conversationIndexTracking, string conversationTopic, string messageClass, int? documentId)
		{
			if (messageIsAssociated)
			{
				return null;
			}
			if (ConversationIdHelpers.MessageClassIsExcludedFromConversations(messageClass))
			{
				return null;
			}
			if (conversationIndexTracking == null || conversationIndexTracking == false)
			{
				if (MessageClassHelper.MatchingMessageClass(messageClass, "IPM.Contact") || MessageClassHelper.MatchingMessageClass(messageClass, "IPM.DistList"))
				{
					return ConversationIdHelpers.ComputeConversationIdForContact(documentId);
				}
				string conversationTopic2 = ConversationIdHelpers.NormalizeConversationTopic(conversationTopic);
				return ConversationIdHelpers.ComputeConversationTopicHash(conversationTopic2);
			}
			else
			{
				if (conversationIndex == null || conversationIndex.Length < 22)
				{
					return null;
				}
				byte[] array = new byte[16];
				Array.Copy(conversationIndex, 6, array, 0, 16);
				return array;
			}
		}

		private static byte[] ComputeConversationTopicHash(string conversationTopic)
		{
			return ConversationIdHelpers.ComputeMD5Hash(Encoding.Unicode.GetBytes(conversationTopic));
		}

		private static byte[] ComputeConversationIdForContact(int? documentId)
		{
			byte[] contactConversationIdSalt = ConversationIdHelpers.ContactConversationIdSalt;
			byte[] bytes = BitConverter.GetBytes((documentId != null) ? documentId.Value : 0);
			byte[] array = new byte[contactConversationIdSalt.Length + bytes.Length];
			Array.Copy(contactConversationIdSalt, array, contactConversationIdSalt.Length);
			Array.Copy(bytes, 0, array, contactConversationIdSalt.Length, bytes.Length);
			return ConversationIdHelpers.ComputeMD5Hash(array);
		}

		private static byte[] ComputeMD5Hash(byte[] buffer)
		{
			byte[] result;
			using (Md5Hasher md5Hasher = new Md5Hasher())
			{
				byte[] array = md5Hasher.ComputeHash(buffer);
				result = array;
			}
			return result;
		}

		private static string NormalizeConversationTopic(string conversationTopic)
		{
			if (conversationTopic != null)
			{
				return conversationTopic.ToUpperInvariant();
			}
			return string.Empty;
		}

		private static bool MessageClassIsExcludedFromConversations(string messageClass)
		{
			if (messageClass == null)
			{
				return false;
			}
			foreach (string desiredMessageClass in ConversationIdHelpers.excludedMessageClasses)
			{
				if (MessageClassHelper.MatchingMessageClass(messageClass, desiredMessageClass))
				{
					return true;
				}
			}
			return false;
		}

		private const int MinimumValidConversationIndexLength = 22;

		private const byte ConversationIndexReservedByte = 1;

		private const int OffsetOfGuidInConversationIndex = 6;

		private static readonly string[] excludedMessageClasses = new string[]
		{
			"IPM.Appointment",
			"IPM.Activity",
			"IPM.Organization",
			"IPM.ContentClassDef",
			"IPM.Microsoft.ScheduleData.FreeBusy",
			"IPM.InkNotes"
		};

		private static readonly byte[] ContactConversationIdSalt = new Guid(928611694, -11142, 19143, 165, 69, 2, 127, 102, 140, 2, 116).ToByteArray();
	}
}
