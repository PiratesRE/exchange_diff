using System;
using System.Text;

namespace Microsoft.Exchange.Common.HA
{
	internal class NotificationEventInfo
	{
		internal NotificationEventInfo()
		{
		}

		internal NotificationEventInfo(int eventId)
		{
			this.EventId = eventId;
		}

		internal NotificationEventInfo(int eventId, string[] parameters)
		{
			this.EventId = eventId;
			this.Parameters = parameters;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (base.GetType() != obj.GetType())
			{
				return false;
			}
			NotificationEventInfo notificationEventInfo = obj as NotificationEventInfo;
			if (!this.EventId.Equals(notificationEventInfo.EventId))
			{
				return false;
			}
			if ((this.Parameters == null || notificationEventInfo.Parameters == null) && (this.Parameters != null || notificationEventInfo.Parameters != null))
			{
				return false;
			}
			if (this.Parameters != null && notificationEventInfo.Parameters != null)
			{
				if (this.Parameters.Length != notificationEventInfo.Parameters.Length)
				{
					return false;
				}
				for (int i = 0; i < this.Parameters.Length; i++)
				{
					if (!object.Equals(this.Parameters[i], notificationEventInfo.Parameters[i]))
					{
						return false;
					}
				}
			}
			return true;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			stringBuilder.AppendFormat("EventId={0} ", this.EventId);
			stringBuilder.AppendLine();
			if (this.Parameters != null && this.Parameters.Length > 0)
			{
				stringBuilder.Append("Parameters={");
				foreach (string arg in this.Parameters)
				{
					stringBuilder.AppendFormat("{0},", arg);
				}
				stringBuilder.Append("} ");
				stringBuilder.AppendLine();
			}
			return stringBuilder.ToString();
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		internal int EventId
		{
			get
			{
				return this.eventId;
			}
			set
			{
				this.eventId = value;
			}
		}

		internal string[] Parameters
		{
			get
			{
				return this.parameters;
			}
			set
			{
				this.parameters = value;
			}
		}

		internal const int InvalidEventId = -1;

		private int eventId;

		private string[] parameters;
	}
}
