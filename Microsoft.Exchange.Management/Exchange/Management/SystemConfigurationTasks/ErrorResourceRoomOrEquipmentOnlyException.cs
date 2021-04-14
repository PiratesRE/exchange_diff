using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorResourceRoomOrEquipmentOnlyException : LocalizedException
	{
		public ErrorResourceRoomOrEquipmentOnlyException(string room, string equipment, string fullString, string partialType) : base(Strings.ErrorResourceRoomOrEquipmentOnly(room, equipment, fullString, partialType))
		{
			this.room = room;
			this.equipment = equipment;
			this.fullString = fullString;
			this.partialType = partialType;
		}

		public ErrorResourceRoomOrEquipmentOnlyException(string room, string equipment, string fullString, string partialType, Exception innerException) : base(Strings.ErrorResourceRoomOrEquipmentOnly(room, equipment, fullString, partialType), innerException)
		{
			this.room = room;
			this.equipment = equipment;
			this.fullString = fullString;
			this.partialType = partialType;
		}

		protected ErrorResourceRoomOrEquipmentOnlyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.room = (string)info.GetValue("room", typeof(string));
			this.equipment = (string)info.GetValue("equipment", typeof(string));
			this.fullString = (string)info.GetValue("fullString", typeof(string));
			this.partialType = (string)info.GetValue("partialType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("room", this.room);
			info.AddValue("equipment", this.equipment);
			info.AddValue("fullString", this.fullString);
			info.AddValue("partialType", this.partialType);
		}

		public string Room
		{
			get
			{
				return this.room;
			}
		}

		public string Equipment
		{
			get
			{
				return this.equipment;
			}
		}

		public string FullString
		{
			get
			{
				return this.fullString;
			}
		}

		public string PartialType
		{
			get
			{
				return this.partialType;
			}
		}

		private readonly string room;

		private readonly string equipment;

		private readonly string fullString;

		private readonly string partialType;
	}
}
