using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[CollectionDataContract(Name = "AttendanceAnnouncementsStatuses")]
	internal class AttendanceAnnouncementsStatuses : Resource
	{
		public AttendanceAnnouncementsStatuses(string selfUri) : base(selfUri)
		{
		}
	}
}
