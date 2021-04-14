using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics.Components.Data;

namespace Microsoft.Exchange.Data.QueueViewer
{
	[Serializable]
	public class MessageIdentity : ObjectId
	{
		public MessageIdentity(long identity)
		{
			this.internalId = identity;
			if (identity <= 0L)
			{
				throw new ArgumentNullException("Identity");
			}
		}

		public MessageIdentity(long identity, QueueIdentity queueIdentity) : this(identity)
		{
			if (queueIdentity == null)
			{
				throw new ArgumentNullException("QueueIdentity");
			}
			this.queueIdentity = queueIdentity;
		}

		public MessageIdentity(QueueIdentity queueId)
		{
			this.queueIdentity = queueId;
		}

		private MessageIdentity()
		{
			this.queueIdentity = QueueIdentity.Empty;
		}

		private MessageIdentity(PropertyStreamReader reader)
		{
			KeyValuePair<string, object> item;
			reader.Read(out item);
			if (!string.Equals("NumProperties", item.Key, StringComparison.OrdinalIgnoreCase))
			{
				throw new SerializationException(string.Format("Cannot deserialize MessageIdentity. Expected property NumProperties, but found property '{0}'", item.Key));
			}
			int value = PropertyStreamReader.GetValue<int>(item);
			for (int i = 0; i < value; i++)
			{
				reader.Read(out item);
				if (string.Equals("InternalId", item.Key, StringComparison.OrdinalIgnoreCase))
				{
					this.internalId = PropertyStreamReader.GetValue<long>(item);
				}
				else if (string.Equals("QueueIdentity", item.Key, StringComparison.OrdinalIgnoreCase))
				{
					this.queueIdentity = QueueIdentity.Create(reader);
				}
				else
				{
					ExTraceGlobals.SerializationTracer.TraceWarning<string>(0L, "Ignoring unknown property '{0} in messageIdentity", item.Key);
				}
			}
		}

		public static MessageIdentity Empty
		{
			get
			{
				return MessageIdentity.empty;
			}
		}

		public long InternalId
		{
			get
			{
				return this.internalId;
			}
		}

		public QueueIdentity QueueIdentity
		{
			get
			{
				return this.queueIdentity;
			}
		}

		public bool IsFullySpecified
		{
			get
			{
				return this.internalId != 0L && this.queueIdentity.IsFullySpecified;
			}
		}

		public override string ToString()
		{
			return this.queueIdentity.ToString() + "\\" + this.internalId.ToString(NumberFormatInfo.InvariantInfo);
		}

		public override bool Equals(object obj)
		{
			return this == obj as MessageIdentity;
		}

		public override int GetHashCode()
		{
			return this.internalId.GetHashCode() ^ this.queueIdentity.GetHashCode();
		}

		public override byte[] GetBytes()
		{
			if (this.internalId != 0L)
			{
				return BitConverter.GetBytes(this.internalId);
			}
			return new byte[0];
		}

		public bool Match(MessageIdentity matchPattern, MatchOptions matchOptions)
		{
			return this.internalId == matchPattern.internalId && this.queueIdentity.Match(matchPattern.queueIdentity, matchOptions);
		}

		internal void ToByteArray(Version targetVersion, ref byte[] bytes, ref int offset)
		{
			if (targetVersion <= MessageIdentity.messageIdAsTextVersion)
			{
				PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.Identity.Name, StreamPropertyType.String, this.ToString(), ref bytes, ref offset);
				return;
			}
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.Identity.Name, StreamPropertyType.Int32, 1, ref bytes, ref offset);
			this.ToByteArray(ref bytes, ref offset);
		}

		internal void ToByteArray(ref byte[] bytes, ref int offset)
		{
			int num = 0;
			PropertyStreamWriter.WritePropertyKeyValue("NumProperties", StreamPropertyType.Int32, 2, ref bytes, ref offset);
			PropertyStreamWriter.WritePropertyKeyValue("InternalId", StreamPropertyType.Int64, this.internalId, ref bytes, ref offset);
			num++;
			PropertyStreamWriter.WritePropertyKeyValue("QueueIdentity", StreamPropertyType.Int32, 1, ref bytes, ref offset);
			this.queueIdentity.ToByteArray(ref bytes, ref offset);
			num++;
		}

		internal static MessageIdentity Create(Version sourceVersion, KeyValuePair<string, object> pair, PropertyStreamReader reader)
		{
			if (!(sourceVersion <= MessageIdentity.messageIdAsTextVersion))
			{
				return MessageIdentity.Create(reader);
			}
			return MessageIdentity.Parse(PropertyStreamReader.GetValue<string>(pair));
		}

		internal static MessageIdentity Create(PropertyStreamReader reader)
		{
			return new MessageIdentity(reader);
		}

		public static MessageIdentity Parse(string identity)
		{
			return MessageIdentity.InternalParse(identity, false);
		}

		internal static MessageIdentity InternalParse(string identity, bool queuePartAlwaysAsDomain)
		{
			int num = identity.LastIndexOf('\\');
			string text = null;
			string text2;
			if (num == -1)
			{
				text2 = identity;
			}
			else
			{
				text2 = identity.Substring(num + 1);
				text = identity.Substring(0, num);
			}
			if (string.IsNullOrEmpty(text2))
			{
				throw new ArgumentNullException("Identity");
			}
			QueueIdentity queueIdentity;
			if (!string.IsNullOrEmpty(text))
			{
				queueIdentity = QueueIdentity.InternalParse(text, queuePartAlwaysAsDomain, false);
			}
			else
			{
				queueIdentity = QueueIdentity.Empty;
			}
			long identity2;
			if (long.TryParse(text2, out identity2))
			{
				return new MessageIdentity(identity2, queueIdentity);
			}
			throw new ArgumentException(DataStrings.ExceptionParseInternalMessageId, "Identity");
		}

		public static MessageIdentity ParsePattern(string identity, ref MatchOptions matchOptions)
		{
			MessageIdentity messageIdentity = MessageIdentity.InternalParse(identity, true);
			messageIdentity.QueueIdentity.ParseDomain(ref matchOptions);
			return messageIdentity;
		}

		public static implicit operator long(MessageIdentity messageIdentity)
		{
			return messageIdentity.internalId;
		}

		public static int Compare(ObjectId a, ObjectId b)
		{
			MessageIdentity messageIdentity = (MessageIdentity)a;
			MessageIdentity messageIdentity2 = (MessageIdentity)b;
			if (messageIdentity == messageIdentity2)
			{
				return 0;
			}
			if (messageIdentity == null && messageIdentity2 != null)
			{
				return -1;
			}
			if (messageIdentity != null && messageIdentity2 == null)
			{
				return 1;
			}
			int num = messageIdentity.internalId.CompareTo(messageIdentity2.internalId);
			if (num != 0)
			{
				return num;
			}
			return QueueIdentity.Compare(messageIdentity.queueIdentity, messageIdentity2.queueIdentity);
		}

		public static bool operator ==(MessageIdentity v1, MessageIdentity v2)
		{
			return MessageIdentity.Compare(v1, v2) == 0;
		}

		public static bool operator !=(MessageIdentity v1, MessageIdentity v2)
		{
			return !(v1 == v2);
		}

		private const string NumPropertiesKey = "NumProperties";

		private const string InternalIdKey = "InternalId";

		private const string QueueIdentityKey = "QueueIdentity";

		private static readonly Version messageIdAsTextVersion = new Version(1, 0);

		private long internalId;

		private QueueIdentity queueIdentity = QueueIdentity.Empty;

		private static readonly MessageIdentity empty = new MessageIdentity();
	}
}
