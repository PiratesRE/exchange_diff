using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class MessageInfo
	{
		public MessageInfo(string checkTitle, string instanceIdentity, string message, bool isException) : this(checkTitle, instanceIdentity, message, isException, null)
		{
		}

		public MessageInfo(string checkTitle, string instanceIdentity, string message, bool isException, uint? dbFailureEventId) : this(checkTitle, instanceIdentity, message, isException, dbFailureEventId, true)
		{
		}

		public MessageInfo(string checkTitle, string instanceIdentity, string message, bool isException, uint? dbFailureEventId, bool isTransitioningState)
		{
			this.m_checkTitle = checkTitle;
			this.m_instanceIdentity = instanceIdentity;
			this.m_message = message;
			this.m_isException = isException;
			this.m_dbFailureEventId = dbFailureEventId;
			this.m_isTransitioningState = isTransitioningState;
		}

		public string InstanceIdentity
		{
			get
			{
				return this.m_instanceIdentity;
			}
		}

		public string Message
		{
			get
			{
				return this.m_message;
			}
		}

		public bool IsException
		{
			get
			{
				return this.m_isException;
			}
		}

		public uint? DbFailureEventId
		{
			get
			{
				return this.m_dbFailureEventId;
			}
		}

		public string CheckTitle
		{
			get
			{
				return this.m_checkTitle;
			}
		}

		public bool IsTransitioningState
		{
			get
			{
				return this.m_isTransitioningState;
			}
		}

		private readonly string m_message;

		private readonly bool m_isException;

		private readonly string m_instanceIdentity;

		private readonly uint? m_dbFailureEventId;

		private readonly string m_checkTitle;

		private readonly bool m_isTransitioningState;
	}
}
