using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.ContentAggregation.Schema;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class Pop3ResultData
	{
		internal ExDateTime FirstReceivedDate
		{
			set
			{
				this.firstReceivedDate = new ExDateTime?(value);
			}
		}

		internal ExDateTime LastReceivedDate
		{
			set
			{
				this.lastReceivedDate = new ExDateTime?(value);
			}
		}

		internal bool ReceivedDateDescends
		{
			get
			{
				return this.firstReceivedDate == null || this.lastReceivedDate == null || (this.firstReceivedDate == ExDateTime.MinValue || this.lastReceivedDate == ExDateTime.MinValue) || this.lastReceivedDate <= this.firstReceivedDate;
			}
		}

		internal int EmailDropCount
		{
			get
			{
				return this.emailDropCount;
			}
			set
			{
				this.emailDropCount = value;
			}
		}

		internal Pop3Email Email
		{
			get
			{
				return this.email;
			}
			set
			{
				this.email = value;
			}
		}

		internal int? RetentionDays
		{
			get
			{
				return this.retentionDays;
			}
			set
			{
				this.retentionDays = value;
			}
		}

		internal bool UidlCommandSupported
		{
			get
			{
				return this.uidlCommandSupported;
			}
			set
			{
				this.uidlCommandSupported = value;
			}
		}

		internal IEnumerable<string> DeletedMessageUniqueIds
		{
			get
			{
				if (this.deletedMessageIds != null)
				{
					foreach (int messageId in this.deletedMessageIds)
					{
						yield return this.uniqueIds[messageId - 1];
					}
				}
				yield break;
			}
		}

		internal int this[string uniqueId]
		{
			get
			{
				return this.uniqueId2IdMap[uniqueId];
			}
		}

		internal bool Contains(string uniqueId)
		{
			return this.uniqueId2IdMap != null && this.uniqueId2IdMap.ContainsKey(uniqueId);
		}

		internal void SetEmailSize(int id, long size)
		{
			this.emailSizes[id - 1] = size;
		}

		internal long GetEmailSize(int id)
		{
			return this.emailSizes[id - 1];
		}

		internal bool HasUniqueId(int id)
		{
			return this.uniqueIds[id - 1] != null;
		}

		internal void AllocateUniqueIds()
		{
			this.uniqueIds = new string[this.emailDropCount];
		}

		internal void AllocateEmailSizes()
		{
			this.emailSizes = new long[this.emailDropCount];
		}

		internal void SetUniqueId(int id, string uniqueId)
		{
			this.uniqueIds[id - 1] = uniqueId;
		}

		internal string GetUniqueId(int id)
		{
			return this.uniqueIds[id - 1];
		}

		internal void AddDeletedMessageId(int id)
		{
			if (this.deletedMessageIds == null)
			{
				this.deletedMessageIds = new List<int>(this.emailDropCount);
			}
			this.deletedMessageIds.Add(id);
		}

		internal void AddUniqueId(string uniqueId, int id)
		{
			if (this.uniqueId2IdMap == null)
			{
				this.uniqueId2IdMap = new Dictionary<string, int>(this.emailDropCount);
			}
			this.uniqueId2IdMap.Add(uniqueId, id);
		}

		private Dictionary<string, int> uniqueId2IdMap;

		private List<int> deletedMessageIds;

		private Pop3Email email;

		private int emailDropCount;

		private long[] emailSizes;

		private string[] uniqueIds;

		private int? retentionDays;

		private bool uidlCommandSupported = true;

		private ExDateTime? firstReceivedDate;

		private ExDateTime? lastReceivedDate;
	}
}
