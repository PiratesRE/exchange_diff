using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UniqueItemHash
	{
		public UniqueItemHash(string internetMsgId, string topic, BodyTagInfo btInfo, bool sentItems)
		{
			this.internetMessageId = internetMsgId;
			this.conversationTopic = topic;
			this.bodyTagInfo = btInfo;
			this.isSentItems = sentItems;
		}

		public static UniqueItemHash Create(IStorePropertyBag propertyBag, bool isOnSentItems)
		{
			string internetMsgId = propertyBag.TryGetProperty(ItemSchema.InternetMessageId) as string;
			string text = propertyBag.TryGetProperty(ItemSchema.ConversationTopic) as string;
			byte[] array = propertyBag.TryGetProperty(ItemSchema.BodyTag) as byte[];
			return new UniqueItemHash(internetMsgId, string.IsNullOrEmpty(text) ? string.Empty : text, (array != null) ? BodyTagInfo.FromByteArray(array) : null, isOnSentItems);
		}

		internal string InternetMessageId
		{
			get
			{
				return this.internetMessageId;
			}
		}

		internal string ConversationTopic
		{
			get
			{
				return this.conversationTopic;
			}
		}

		public static UniqueItemHash Parse(string serializedUniqueItemHash)
		{
			if (string.IsNullOrEmpty(serializedUniqueItemHash))
			{
				return null;
			}
			bool sentItems = false;
			if (serializedUniqueItemHash.Substring(0, 1) == "1")
			{
				sentItems = true;
			}
			else if (serializedUniqueItemHash.Substring(0, 1) != "0")
			{
				throw new ArgumentException("Expected sent items serialized value is not either 1 or 0.");
			}
			int num = 1;
			int num2 = 0;
			if (!int.TryParse(serializedUniqueItemHash.Substring(num, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num2))
			{
				throw new ArgumentException("Cannot read the length indicator value of internet message id part.");
			}
			num += 4;
			string internetMsgId = null;
			if (num2 != 0)
			{
				internetMsgId = serializedUniqueItemHash.Substring(num, num2);
				num += num2;
			}
			if (!int.TryParse(serializedUniqueItemHash.Substring(num, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num2))
			{
				throw new ArgumentException("Cannot read the length indicator value of conversation topic part.");
			}
			num += 4;
			string topic = null;
			if (num2 != 0)
			{
				topic = serializedUniqueItemHash.Substring(num, num2);
				num += num2;
			}
			if (!int.TryParse(serializedUniqueItemHash.Substring(num, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num2))
			{
				throw new ArgumentException("Cannot read the length indicator value of body tag info part.");
			}
			num += 4;
			BodyTagInfo btInfo = null;
			if (num2 != 0)
			{
				string bodyTagInfoString = serializedUniqueItemHash.Substring(num, num2);
				btInfo = UniqueItemHash.DeserializeBodyTagInfoFromString(bodyTagInfoString);
				num += num2;
			}
			if (num != serializedUniqueItemHash.Length)
			{
				throw new ArgumentException(string.Format("The serialized unique item hash has not been completely parsed. Start index = {0}, Length = {1}", num, serializedUniqueItemHash.Length));
			}
			return new UniqueItemHash(internetMsgId, topic, btInfo, sentItems);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0}", this.isSentItems ? "1" : "0");
			if (!string.IsNullOrEmpty(this.internetMessageId))
			{
				stringBuilder.AppendFormat("{0:X4}", this.internetMessageId.Length);
				stringBuilder.Append(this.internetMessageId);
			}
			else
			{
				stringBuilder.AppendFormat("{0:X4}", 0);
			}
			if (!string.IsNullOrEmpty(this.conversationTopic))
			{
				stringBuilder.AppendFormat("{0:X4}", this.conversationTopic.Length);
				stringBuilder.Append(this.conversationTopic);
			}
			else
			{
				stringBuilder.AppendFormat("{0:X4}", 0);
			}
			string text = UniqueItemHash.SerializeBodyTagInfoToString(this.bodyTagInfo);
			if (!string.IsNullOrEmpty(text))
			{
				stringBuilder.AppendFormat("{0:X4}", text.Length);
				stringBuilder.Append(text);
			}
			else
			{
				stringBuilder.AppendFormat("{0:X4}", 0);
			}
			return stringBuilder.ToString();
		}

		public override int GetHashCode()
		{
			if (string.IsNullOrEmpty(this.conversationTopic))
			{
				if (!string.IsNullOrEmpty(this.internetMessageId))
				{
					return this.internetMessageId.GetHashCode();
				}
				return 0;
			}
			else
			{
				if (string.IsNullOrEmpty(this.internetMessageId))
				{
					return this.conversationTopic.GetHashCode();
				}
				return this.internetMessageId.GetHashCode() ^ this.conversationTopic.GetHashCode();
			}
		}

		public override bool Equals(object obj)
		{
			UniqueItemHash uniqueItemHash = obj as UniqueItemHash;
			if (uniqueItemHash == null)
			{
				return false;
			}
			if (!UniqueItemHash.CompareInternetMessageIds(this.internetMessageId, uniqueItemHash.internetMessageId) || !ConversationIndex.CompareTopics(this.conversationTopic, uniqueItemHash.conversationTopic))
			{
				return false;
			}
			if (this.bodyTagInfo != null && uniqueItemHash.bodyTagInfo != null)
			{
				return this.bodyTagInfo.Equals(uniqueItemHash.bodyTagInfo);
			}
			return (!(this.bodyTagInfo == null) || !(uniqueItemHash.bodyTagInfo == null)) && ((this.bodyTagInfo == null && this.isSentItems) || (uniqueItemHash.bodyTagInfo == null && uniqueItemHash.isSentItems));
		}

		private static bool CompareInternetMessageIds(string id, string otherId)
		{
			return !string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(otherId) && id.Equals(otherId, StringComparison.OrdinalIgnoreCase);
		}

		private static string SerializeBodyTagInfoToString(BodyTagInfo bodyTagInfo)
		{
			if (bodyTagInfo == null)
			{
				return null;
			}
			byte[] array = bodyTagInfo.ToByteArray();
			return CTSGlobals.AsciiEncoding.GetString(array, 0, array.Length);
		}

		private static BodyTagInfo DeserializeBodyTagInfoFromString(string bodyTagInfoString)
		{
			if (string.IsNullOrEmpty(bodyTagInfoString))
			{
				return null;
			}
			byte[] bytes = CTSGlobals.AsciiEncoding.GetBytes(bodyTagInfoString);
			return BodyTagInfo.FromByteArray(bytes);
		}

		private string internetMessageId;

		private string conversationTopic;

		private BodyTagInfo bodyTagInfo;

		private bool isSentItems;
	}
}
