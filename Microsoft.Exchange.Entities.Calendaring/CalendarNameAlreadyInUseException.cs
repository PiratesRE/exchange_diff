using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Entities.Calendaring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CalendarNameAlreadyInUseException : ObjectExistedException
	{
		public CalendarNameAlreadyInUseException(string name) : base(CalendaringStrings.CalendarNameAlreadyInUse(name))
		{
			this.name = name;
		}

		public CalendarNameAlreadyInUseException(string name, Exception innerException) : base(CalendaringStrings.CalendarNameAlreadyInUse(name), innerException)
		{
			this.name = name;
		}

		protected CalendarNameAlreadyInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private readonly string name;
	}
}
