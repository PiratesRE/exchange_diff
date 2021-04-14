using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.Data;
using Microsoft.Exchange.Rpc.QueueViewer;

namespace Microsoft.Exchange.Data.QueueViewer
{
	[Serializable]
	public class QueueIdentity : ObjectId, IComparable
	{
		private QueueIdentity()
		{
		}

		private QueueIdentity(QueueType queueType)
		{
			if (queueType == QueueType.Delivery || queueType == QueueType.Shadow)
			{
				throw new InvalidOperationException("Cannot create Delivery or Shadow queue using this constructor");
			}
			this.server = QueueIdentity.LocalHostName;
			this.queueType = queueType;
		}

		public QueueIdentity(QueueType queueType, long queueRowId, string nextHopDomain)
		{
			if (queueRowId <= 0L)
			{
				throw new ExArgumentNullException("queueRowId");
			}
			if (queueType != QueueType.Shadow && queueType != QueueType.Delivery)
			{
				throw new ArgumentException(string.Format("QueueType '{0}' not allowed in this constructor", queueType));
			}
			this.server = QueueIdentity.LocalHostName;
			this.queueType = queueType;
			this.queueRowId = queueRowId;
			this.nextHopDomain = nextHopDomain;
		}

		public QueueIdentity(ExtensibleQueueInfo queue) : this()
		{
			QueueIdentity queueIdentity = (QueueIdentity)queue.Identity;
			this.server = queueIdentity.server;
			this.queueType = queueIdentity.queueType;
			this.queueRowId = queueIdentity.queueRowId;
			this.nextHopDomain = queueIdentity.nextHopDomain;
		}

		private QueueIdentity(PropertyStreamReader reader)
		{
			KeyValuePair<string, object> item;
			reader.Read(out item);
			if (!string.Equals("NumProperties", item.Key, StringComparison.OrdinalIgnoreCase))
			{
				throw new SerializationException(string.Format("Cannot deserialize QueueIdentity. Expected property NumProperties, but found property '{0}'", item.Key));
			}
			int value = PropertyStreamReader.GetValue<int>(item);
			for (int i = 0; i < value; i++)
			{
				reader.Read(out item);
				if (string.Equals("RowId", item.Key, StringComparison.OrdinalIgnoreCase))
				{
					this.queueRowId = PropertyStreamReader.GetValue<long>(item);
				}
				else if (string.Equals("Type", item.Key, StringComparison.OrdinalIgnoreCase))
				{
					this.queueType = (QueueType)PropertyStreamReader.GetValue<int>(item);
				}
				else if (string.Equals("NextHopDomain", item.Key, StringComparison.OrdinalIgnoreCase))
				{
					this.nextHopDomain = PropertyStreamReader.GetValue<string>(item);
				}
				else if (string.Equals("Server", item.Key, StringComparison.OrdinalIgnoreCase))
				{
					this.server = PropertyStreamReader.GetValue<string>(item);
				}
				else
				{
					ExTraceGlobals.SerializationTracer.TraceWarning<string>(0L, "Ignoring unknown property '{0} in queueIdentity", item.Key);
				}
			}
		}

		public static QueueIdentity Empty
		{
			get
			{
				return QueueIdentity.empty;
			}
		}

		public static QueueIdentity PoisonQueueIdentity
		{
			get
			{
				return QueueIdentity.poisonQueueIdentity;
			}
		}

		public static QueueIdentity SubmissionQueueIdentity
		{
			get
			{
				return QueueIdentity.submissionQueueIdentity;
			}
		}

		public static QueueIdentity UnreachableQueueIdentity
		{
			get
			{
				return QueueIdentity.unreachableQueueIdentity;
			}
		}

		public QueueType Type
		{
			get
			{
				return this.queueType;
			}
		}

		public long RowId
		{
			get
			{
				return this.queueRowId;
			}
		}

		public string NextHopDomain
		{
			get
			{
				if (!string.IsNullOrEmpty(this.nextHopDomain))
				{
					return this.nextHopDomain;
				}
				if (this.queueType == QueueType.Submission)
				{
					return DataStrings.SubmissionQueueNextHopDomain;
				}
				if (this.queueType == QueueType.Poison)
				{
					return DataStrings.PoisonQueueNextHopDomain;
				}
				if (this.queueType == QueueType.Unreachable)
				{
					return DataStrings.UnreachableQueueNextHopDomain;
				}
				throw new Exception("Unexpected queue type");
			}
			set
			{
				this.nextHopDomain = value;
			}
		}

		public string Server
		{
			get
			{
				if (string.IsNullOrEmpty(this.server))
				{
					return QueueIdentity.LocalHostName;
				}
				return this.server;
			}
			private set
			{
				if (RoutingAddress.IsValidDomain(value))
				{
					this.server = value;
					return;
				}
				throw new ArgumentException(DataStrings.ExceptionInvalidServerName(value), "Identity");
			}
		}

		public bool IsLocal
		{
			get
			{
				return string.IsNullOrEmpty(this.server) || string.Compare(this.server, QueueIdentity.LocalHostName, StringComparison.OrdinalIgnoreCase) == 0;
			}
		}

		public bool IsFullySpecified
		{
			get
			{
				return this.queueType != QueueType.Undefined;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.queueType == QueueType.Undefined && string.IsNullOrEmpty(this.nextHopDomain) && this.queueRowId == 0L;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(50);
			if (!string.IsNullOrEmpty(this.server))
			{
				stringBuilder.AppendFormat("{0}\\", this.server);
			}
			switch (this.queueType)
			{
			case QueueType.Poison:
				stringBuilder.Append(QueueIdentity.PoisonIdentityString);
				break;
			case QueueType.Submission:
				stringBuilder.Append(QueueIdentity.SubmissionIdentityString);
				break;
			case QueueType.Unreachable:
				stringBuilder.Append(QueueIdentity.UnreachableIdentityString);
				break;
			case QueueType.Shadow:
				if (this.queueRowId > 0L)
				{
					stringBuilder.AppendFormat("{0}\\{1}", QueueIdentity.ShadowIdentityString, this.queueRowId.ToString(NumberFormatInfo.InvariantInfo));
				}
				else
				{
					stringBuilder.AppendFormat("{0}\\{1}", QueueIdentity.ShadowIdentityString, this.nextHopDomain);
				}
				break;
			default:
				if (this.queueRowId > 0L)
				{
					stringBuilder.Append(this.queueRowId.ToString(NumberFormatInfo.InvariantInfo));
				}
				else
				{
					stringBuilder.Append(this.nextHopDomain);
				}
				break;
			}
			return stringBuilder.ToString();
		}

		public override bool Equals(object obj)
		{
			return this == obj as QueueIdentity;
		}

		public override int GetHashCode()
		{
			return this.queueType.GetHashCode() ^ ((this.queueRowId != 0L) ? this.queueRowId.GetHashCode() : ((!string.IsNullOrEmpty(this.nextHopDomain)) ? this.nextHopDomain.GetHashCode() : 0));
		}

		public override byte[] GetBytes()
		{
			return new byte[0];
		}

		internal void ToByteArray(ref byte[] bytes, ref int offset)
		{
			int num = 0;
			PropertyStreamWriter.WritePropertyKeyValue("NumProperties", StreamPropertyType.Int32, 4, ref bytes, ref offset);
			PropertyStreamWriter.WritePropertyKeyValue("RowId", StreamPropertyType.Int64, this.queueRowId, ref bytes, ref offset);
			num++;
			PropertyStreamWriter.WritePropertyKeyValue("Type", StreamPropertyType.Int32, (int)this.queueType, ref bytes, ref offset);
			num++;
			PropertyStreamWriter.WritePropertyKeyValue("NextHopDomain", StreamPropertyType.String, this.nextHopDomain, ref bytes, ref offset);
			num++;
			PropertyStreamWriter.WritePropertyKeyValue("Server", StreamPropertyType.String, this.server, ref bytes, ref offset);
			num++;
		}

		internal static QueueIdentity Create(PropertyStreamReader reader)
		{
			return new QueueIdentity(reader);
		}

		public static QueueIdentity Parse(string identity)
		{
			return QueueIdentity.InternalParse(identity, false, false);
		}

		public static QueueIdentity Parse(string identity, bool implicitShadow)
		{
			return QueueIdentity.InternalParse(identity, false, implicitShadow);
		}

		internal static QueueIdentity InternalParse(string identity, bool queuePartAlwaysAsDomain, bool implicitShadow)
		{
			int num = identity.IndexOf('\\');
			string text = null;
			string text2;
			if (num == -1)
			{
				text2 = identity;
			}
			else
			{
				text = identity.Substring(0, num);
				if (string.Equals(text, QueueIdentity.ShadowIdentityString, StringComparison.OrdinalIgnoreCase))
				{
					text = null;
					text2 = identity;
				}
				else
				{
					text2 = identity.Substring(num + 1);
				}
			}
			if (string.IsNullOrEmpty(text2))
			{
				throw new ExArgumentNullException("QueueIdentity");
			}
			string text3 = QueueIdentity.ShadowIdentityString + "\\";
			bool flag = text2.StartsWith(text3, StringComparison.OrdinalIgnoreCase);
			if (flag)
			{
				text2 = text2.Substring(text3.Length);
			}
			QueueIdentity queueIdentity = new QueueIdentity();
			int num2;
			if (!queuePartAlwaysAsDomain && int.TryParse(text2, out num2))
			{
				queueIdentity.queueType = ((flag || implicitShadow) ? QueueType.Shadow : QueueType.Delivery);
				queueIdentity.queueRowId = (long)num2;
			}
			else if (!queuePartAlwaysAsDomain && string.Compare(text2, QueueIdentity.PoisonIdentityString, StringComparison.OrdinalIgnoreCase) == 0)
			{
				queueIdentity.queueType = QueueType.Poison;
			}
			else if (!queuePartAlwaysAsDomain && string.Compare(text2, QueueIdentity.SubmissionIdentityString, StringComparison.OrdinalIgnoreCase) == 0)
			{
				queueIdentity.queueType = QueueType.Submission;
			}
			else if (!queuePartAlwaysAsDomain && string.Compare(text2, QueueIdentity.UnreachableIdentityString, StringComparison.OrdinalIgnoreCase) == 0)
			{
				queueIdentity.queueType = QueueType.Unreachable;
			}
			else
			{
				if (!text2.StartsWith("*", StringComparison.OrdinalIgnoreCase) && !text2.EndsWith("*", StringComparison.OrdinalIgnoreCase))
				{
					queueIdentity.queueType = (flag ? QueueType.Shadow : QueueType.Delivery);
				}
				queueIdentity.nextHopDomain = text2;
			}
			if (!string.IsNullOrEmpty(text))
			{
				queueIdentity.Server = text;
			}
			return queueIdentity;
		}

		public static QueueIdentity ParsePattern(string identity, ref MatchOptions matchOptions)
		{
			QueueIdentity queueIdentity = QueueIdentity.InternalParse(identity, true, false);
			queueIdentity.ParseDomain(ref matchOptions);
			return queueIdentity;
		}

		public void ParseDomain(ref MatchOptions matchOptions)
		{
			matchOptions = MatchOptions.FullString;
			if (!string.IsNullOrEmpty(this.nextHopDomain))
			{
				if (this.nextHopDomain.StartsWith("*", StringComparison.Ordinal))
				{
					matchOptions = MatchOptions.Suffix;
					this.nextHopDomain = this.nextHopDomain.Substring(1, this.nextHopDomain.Length - 1);
				}
				if (this.nextHopDomain.EndsWith("*", StringComparison.Ordinal))
				{
					if (matchOptions == MatchOptions.Suffix)
					{
						matchOptions = MatchOptions.SubString;
					}
					else
					{
						matchOptions = MatchOptions.Prefix;
					}
					this.nextHopDomain = this.nextHopDomain.Substring(0, this.nextHopDomain.Length - 1);
				}
			}
			if (string.IsNullOrEmpty(this.nextHopDomain))
			{
				matchOptions = MatchOptions.SubString;
				this.nextHopDomain = string.Empty;
			}
		}

		public bool Match(QueueIdentity matchPattern, MatchOptions matchOptions)
		{
			if (!PagedObjectSchema.MatchString(this.NextHopDomain, matchPattern.nextHopDomain, matchOptions))
			{
				return false;
			}
			if (string.IsNullOrEmpty(this.server))
			{
				throw new InvalidOperationException();
			}
			return string.IsNullOrEmpty(matchPattern.server) || this.server.Equals(matchPattern.server, StringComparison.OrdinalIgnoreCase);
		}

		public static int Compare(ObjectId a, ObjectId b)
		{
			QueueIdentity queueIdentity = (QueueIdentity)a;
			QueueIdentity queueIdentity2 = (QueueIdentity)b;
			if (queueIdentity == queueIdentity2)
			{
				return 0;
			}
			if (queueIdentity == null && queueIdentity2 != null)
			{
				return -1;
			}
			if (queueIdentity != null && queueIdentity2 == null)
			{
				return 1;
			}
			if (queueIdentity.IsEmpty || queueIdentity2.IsEmpty)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INCOMPLETE_IDENTITY);
			}
			int num2;
			if (queueIdentity.queueType != QueueType.Undefined && queueIdentity2.queueType != QueueType.Undefined)
			{
				int num = (int)queueIdentity.queueType;
				num2 = num.CompareTo((int)queueIdentity2.queueType);
				if (num2 != 0)
				{
					return num2;
				}
			}
			if (queueIdentity.queueRowId != 0L && queueIdentity2.queueRowId != 0L)
			{
				num2 = queueIdentity.queueRowId.CompareTo(queueIdentity2.queueRowId);
			}
			else
			{
				num2 = string.Compare(queueIdentity.nextHopDomain, queueIdentity2.nextHopDomain, StringComparison.OrdinalIgnoreCase);
			}
			if (num2 != 0)
			{
				return num2;
			}
			if (string.IsNullOrEmpty(queueIdentity.server) || string.IsNullOrEmpty(queueIdentity2.server))
			{
				return 0;
			}
			return string.Compare(queueIdentity.server, queueIdentity2.server, StringComparison.OrdinalIgnoreCase);
		}

		public static bool operator ==(QueueIdentity v1, QueueIdentity v2)
		{
			return QueueIdentity.Compare(v1, v2) == 0;
		}

		public static bool operator !=(QueueIdentity a, QueueIdentity b)
		{
			return !(a == b);
		}

		public int CompareTo(object obj)
		{
			QueueIdentity queueIdentity = obj as QueueIdentity;
			if (queueIdentity == null)
			{
				throw new ArgumentException(DataStrings.ExceptionQueueIdentityCompare(obj.GetType().FullName));
			}
			return QueueIdentity.Compare(this, queueIdentity);
		}

		private const string NumPropertiesKey = "NumProperties";

		private const string RowIdKey = "RowId";

		private const string TypeKey = "Type";

		private const string NextHopDomainKey = "NextHopDomain";

		private const string ServerKey = "Server";

		public static readonly string LocalHostName = Dns.GetHostName();

		private string server = string.Empty;

		private QueueType queueType;

		private long queueRowId;

		private string nextHopDomain = string.Empty;

		private static readonly QueueIdentity empty = new QueueIdentity();

		private static readonly QueueIdentity poisonQueueIdentity = new QueueIdentity(QueueType.Poison);

		private static readonly QueueIdentity submissionQueueIdentity = new QueueIdentity(QueueType.Submission);

		private static readonly QueueIdentity unreachableQueueIdentity = new QueueIdentity(QueueType.Unreachable);

		private static readonly string PoisonIdentityString = QueueType.Poison.ToString();

		private static readonly string SubmissionIdentityString = QueueType.Submission.ToString();

		private static readonly string UnreachableIdentityString = QueueType.Unreachable.ToString();

		private static readonly string ShadowIdentityString = QueueType.Shadow.ToString();
	}
}
