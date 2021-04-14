using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal sealed class ADNotificationEventArgs : EventArgs
	{
		public ADNotificationChangeType ChangeType
		{
			get
			{
				return this.changeType;
			}
		}

		public object Context
		{
			get
			{
				return this.context;
			}
		}

		public ADObjectId Id
		{
			get
			{
				return this.objectId;
			}
		}

		public ADObjectId LastKnownParent
		{
			get
			{
				return this.lastKnownParent;
			}
		}

		public Type Type
		{
			get
			{
				return this.type;
			}
		}

		internal ADNotificationEventArgs(ADNotificationChangeType changeType, object context, ADObjectId id, ADObjectId lastKnownParent, Type type)
		{
			this.changeType = changeType;
			this.context = context;
			this.objectId = id;
			this.lastKnownParent = lastKnownParent;
			this.type = type;
		}

		private ADNotificationChangeType changeType;

		private object context;

		private ADObjectId objectId;

		private ADObjectId lastKnownParent;

		private Type type;
	}
}
