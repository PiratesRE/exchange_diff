using System;
using System.Net;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	internal class WorkItemData
	{
		public WorkItemData()
		{
			this.workType = WorkItemType.InvalidType;
			this.blockPeriod = 0;
			this.blockComment = string.Empty;
		}

		public WorkItemPriority Priority
		{
			get
			{
				return this.priority;
			}
			set
			{
				this.priority = value;
			}
		}

		public IPAddress SenderAddress
		{
			get
			{
				return this.senderAddress;
			}
			set
			{
				this.senderAddress = value;
			}
		}

		public int BlockPeriod
		{
			get
			{
				return this.blockPeriod;
			}
			set
			{
				this.blockPeriod = value;
			}
		}

		public string BlockComment
		{
			get
			{
				return this.blockComment;
			}
			set
			{
				this.blockComment = value;
			}
		}

		public WorkItemType WorkType
		{
			get
			{
				return this.workType;
			}
			set
			{
				this.workType = value;
			}
		}

		public DateTime InsertTime
		{
			get
			{
				return this.insertTime;
			}
			set
			{
				this.insertTime = value;
			}
		}

		private WorkItemPriority priority;

		private IPAddress senderAddress;

		private DateTime insertTime;

		private int blockPeriod;

		private string blockComment;

		private WorkItemType workType;
	}
}
