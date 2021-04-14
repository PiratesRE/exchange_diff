using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataProviders;

namespace Microsoft.Exchange.Entities.Calendaring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CannotDeleteSpecialCalendarGroupException : InvalidRequestException
	{
		public CannotDeleteSpecialCalendarGroupException(StoreId groupId, Guid groupClassId, string groupName) : base(CalendaringStrings.CannotDeleteWellKnownCalendarGroup(groupId, groupClassId, groupName))
		{
			this.groupId = groupId;
			this.groupClassId = groupClassId;
			this.groupName = groupName;
		}

		public CannotDeleteSpecialCalendarGroupException(StoreId groupId, Guid groupClassId, string groupName, Exception innerException) : base(CalendaringStrings.CannotDeleteWellKnownCalendarGroup(groupId, groupClassId, groupName), innerException)
		{
			this.groupId = groupId;
			this.groupClassId = groupClassId;
			this.groupName = groupName;
		}

		protected CannotDeleteSpecialCalendarGroupException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.groupId = (StoreId)info.GetValue("groupId", typeof(StoreId));
			this.groupClassId = (Guid)info.GetValue("groupClassId", typeof(Guid));
			this.groupName = (string)info.GetValue("groupName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("groupId", this.groupId);
			info.AddValue("groupClassId", this.groupClassId);
			info.AddValue("groupName", this.groupName);
		}

		public StoreId GroupId
		{
			get
			{
				return this.groupId;
			}
		}

		public Guid GroupClassId
		{
			get
			{
				return this.groupClassId;
			}
		}

		public string GroupName
		{
			get
			{
				return this.groupName;
			}
		}

		private readonly StoreId groupId;

		private readonly Guid groupClassId;

		private readonly string groupName;
	}
}
