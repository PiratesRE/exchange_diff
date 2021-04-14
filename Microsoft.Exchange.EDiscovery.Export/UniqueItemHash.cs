using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal class UniqueItemHash
	{
		public UniqueItemHash(string internetMsgId, string topic, BodyTagInfo btInfo, bool sentItems)
		{
			if (string.IsNullOrEmpty(internetMsgId))
			{
				throw new ArgumentNullException("internetMsgId");
			}
			this.internetMessageId = internetMsgId;
			this.conversationTopic = topic;
			this.bodyTagInfo = btInfo;
			this.isSentItems = sentItems;
		}

		public string InternetMessageId
		{
			get
			{
				return this.internetMessageId;
			}
		}

		public string ConversationTopic
		{
			get
			{
				return this.conversationTopic;
			}
		}

		public bool IsSentItems
		{
			get
			{
				return this.isSentItems;
			}
		}

		public BodyTagInfo BodyTagInfo
		{
			get
			{
				return this.bodyTagInfo;
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
			if (num2 == 0)
			{
				throw new ArgumentException("Internet message id value read from serialized item is empty.");
			}
			num += 4;
			string internetMsgId = serializedUniqueItemHash.Substring(num, num2);
			num += num2;
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
			stringBuilder.AppendFormat("{0:X4}", this.internetMessageId.Length);
			stringBuilder.Append(this.internetMessageId);
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
				return this.internetMessageId.GetHashCode();
			}
			return this.internetMessageId.GetHashCode() ^ this.conversationTopic.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			UniqueItemHash uniqueItemHash = obj as UniqueItemHash;
			if (uniqueItemHash == null)
			{
				return false;
			}
			if (!this.internetMessageId.Equals(uniqueItemHash.internetMessageId, StringComparison.OrdinalIgnoreCase) || !UniqueItemHash.CompareTopics(this.conversationTopic, uniqueItemHash.conversationTopic))
			{
				return false;
			}
			if (this.bodyTagInfo != null && uniqueItemHash.bodyTagInfo != null)
			{
				return this.bodyTagInfo.Equals(uniqueItemHash.bodyTagInfo);
			}
			return (!(this.bodyTagInfo == null) || !(uniqueItemHash.bodyTagInfo == null)) && ((this.bodyTagInfo == null && this.isSentItems) || (uniqueItemHash.bodyTagInfo == null && uniqueItemHash.isSentItems));
		}

		private static bool CompareTopics(string incomingTopic, string foundTopic)
		{
			return (string.IsNullOrEmpty(foundTopic) && string.IsNullOrEmpty(incomingTopic)) || (foundTopic != null && incomingTopic != null && 0 == string.Compare(incomingTopic, foundTopic, StringComparison.OrdinalIgnoreCase));
		}

		private static string SerializeBodyTagInfoToString(BodyTagInfo bodyTagInfo)
		{
			if (bodyTagInfo == null)
			{
				return null;
			}
			byte[] bytes = bodyTagInfo.ToByteArray();
			return UniqueItemHash.asciiEncoding.GetString(bytes);
		}

		private static BodyTagInfo DeserializeBodyTagInfoFromString(string bodyTagInfoString)
		{
			if (string.IsNullOrEmpty(bodyTagInfoString))
			{
				return null;
			}
			byte[] bytes = UniqueItemHash.asciiEncoding.GetBytes(bodyTagInfoString);
			return BodyTagInfo.FromByteArray(bytes);
		}

		private static readonly Encoding asciiEncoding = new ASCIIEncoding();

		private readonly string internetMessageId;

		private readonly string conversationTopic;

		private readonly BodyTagInfo bodyTagInfo;

		private readonly bool isSentItems;
	}
}
