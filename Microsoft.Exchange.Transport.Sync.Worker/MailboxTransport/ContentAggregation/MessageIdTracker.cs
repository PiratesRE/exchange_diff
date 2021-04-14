using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MessageIdTracker
	{
		internal MessageIdTracker(int capacity)
		{
			this.list = new List<int>(capacity);
			this.mode = MessageIdTracker.IdTrackerMode.List;
		}

		internal MessageIdTracker(params int[] messageIds) : this(messageIds)
		{
		}

		internal MessageIdTracker(ICollection<int> messageIds)
		{
			if (messageIds == null)
			{
				throw new ArgumentNullException("messageIds");
			}
			this.list = new List<int>(messageIds.Count);
			this.list.AddRange(messageIds);
			this.mode = MessageIdTracker.IdTrackerMode.List;
		}

		internal MessageIdTracker(int minId, int maxId, bool backward)
		{
			if (minId <= 0)
			{
				throw new ArgumentOutOfRangeException("minId");
			}
			if (maxId < minId)
			{
				throw new ArgumentOutOfRangeException("maxId");
			}
			this.minId = minId;
			this.maxId = maxId;
			this.backward = backward;
			this.current = (this.backward ? this.maxId : this.minId);
			this.mode = MessageIdTracker.IdTrackerMode.Range;
		}

		internal int Current
		{
			get
			{
				switch (this.mode)
				{
				case MessageIdTracker.IdTrackerMode.Range:
					if (!this.backward)
					{
						return this.current - 1;
					}
					return this.current + 1;
				case MessageIdTracker.IdTrackerMode.List:
					return this.list[this.current - 1];
				default:
					throw new InvalidOperationException("The message id tracker mode is invalid.");
				}
			}
		}

		public override string ToString()
		{
			switch (this.mode)
			{
			case MessageIdTracker.IdTrackerMode.Range:
				return string.Format(CultureInfo.InvariantCulture, "{0} ({1} to {2})", new object[]
				{
					this.Current,
					this.minId,
					this.maxId
				});
			case MessageIdTracker.IdTrackerMode.List:
				return string.Format(CultureInfo.InvariantCulture, "{0} ({1} of {2})", new object[]
				{
					(this.current == 0) ? 0 : this.Current,
					this.current,
					this.list.Count
				});
			default:
				return "The message id tracker mode is invalid.";
			}
		}

		internal void Add(int id)
		{
			if (this.mode != MessageIdTracker.IdTrackerMode.List)
			{
				throw new InvalidOperationException("The message id tracker is not in valid mode.");
			}
			this.list.Add(id);
		}

		internal bool MoveNext()
		{
			switch (this.mode)
			{
			case MessageIdTracker.IdTrackerMode.Range:
				if (this.backward ? (this.current <= 0) : (this.current > this.maxId))
				{
					return false;
				}
				break;
			case MessageIdTracker.IdTrackerMode.List:
				if (this.current >= this.list.Count)
				{
					return false;
				}
				break;
			}
			this.current = (this.backward ? (this.current - 1) : (this.current + 1));
			return true;
		}

		internal bool Contains(int id)
		{
			switch (this.mode)
			{
			case MessageIdTracker.IdTrackerMode.Range:
				return id >= this.minId && id <= this.maxId;
			case MessageIdTracker.IdTrackerMode.List:
				return this.list.Contains(id);
			default:
				throw new InvalidOperationException("The message id tracker mode is invalid.");
			}
		}

		internal void Remove(int id)
		{
			if (!this.Contains(id))
			{
				throw new InvalidOperationException("The specified id is not being tracked.");
			}
			if (this.mode == MessageIdTracker.IdTrackerMode.Range)
			{
				throw new InvalidOperationException("The message id tracker is not in valid mode.");
			}
			int num = this.list.IndexOf(id);
			if (this.current > num)
			{
				this.current--;
			}
			this.list.Remove(id);
		}

		internal void Reset()
		{
			switch (this.mode)
			{
			case MessageIdTracker.IdTrackerMode.Range:
				this.current = (this.backward ? this.maxId : this.minId);
				return;
			case MessageIdTracker.IdTrackerMode.List:
				this.current = 0;
				return;
			default:
				throw new InvalidOperationException("The message id tracker mode is invalid.");
			}
		}

		private List<int> list;

		private int minId;

		private int maxId;

		private bool backward;

		private int current;

		private MessageIdTracker.IdTrackerMode mode;

		private enum IdTrackerMode
		{
			Range,
			List
		}
	}
}
