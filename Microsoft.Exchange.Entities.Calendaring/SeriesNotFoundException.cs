using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Entities.Calendaring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SeriesNotFoundException : ObjectNotFoundException
	{
		public SeriesNotFoundException(string seriesId) : base(CalendaringStrings.SeriesNotFound(seriesId))
		{
			this.seriesId = seriesId;
		}

		public SeriesNotFoundException(string seriesId, Exception innerException) : base(CalendaringStrings.SeriesNotFound(seriesId), innerException)
		{
			this.seriesId = seriesId;
		}

		protected SeriesNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.seriesId = (string)info.GetValue("seriesId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("seriesId", this.seriesId);
		}

		public string SeriesId
		{
			get
			{
				return this.seriesId;
			}
		}

		private readonly string seriesId;
	}
}
