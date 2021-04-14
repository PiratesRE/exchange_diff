using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataProviders;

namespace Microsoft.Exchange.Entities.Calendaring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CalendarGroupIsNotEmptyException : InvalidRequestException
	{
		public CalendarGroupIsNotEmptyException(StoreId groupId, Guid groupClassId, string groupName, int calendarsCount) : base(CalendaringStrings.CalendarGroupIsNotEmpty(groupId, groupClassId, groupName, calendarsCount))
		{
			this.groupId = groupId;
			this.groupClassId = groupClassId;
			this.groupName = groupName;
			this.calendarsCount = calendarsCount;
		}

		public CalendarGroupIsNotEmptyException(StoreId groupId, Guid groupClassId, string groupName, int calendarsCount, Exception innerException) : base(CalendaringStrings.CalendarGroupIsNotEmpty(groupId, groupClassId, groupName, calendarsCount), innerException)
		{
			this.groupId = groupId;
			this.groupClassId = groupClassId;
			this.groupName = groupName;
			this.calendarsCount = calendarsCount;
		}

		protected CalendarGroupIsNotEmptyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.groupId = (StoreId)info.GetValue("groupId", typeof(StoreId));
			this.groupClassId = (Guid)info.GetValue("groupClassId", typeof(Guid));
			this.groupName = (string)info.GetValue("groupName", typeof(string));
			this.calendarsCount = (int)info.GetValue("calendarsCount", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("groupId", this.groupId);
			info.AddValue("groupClassId", this.groupClassId);
			info.AddValue("groupName", this.groupName);
			info.AddValue("calendarsCount", this.calendarsCount);
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

		public int CalendarsCount
		{
			get
			{
				return this.calendarsCount;
			}
		}

		private readonly StoreId groupId;

		private readonly Guid groupClassId;

		private readonly string groupName;

		private readonly int calendarsCount;
	}
}
