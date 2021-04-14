using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.MeetingSuggestions
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class GroupAttendeeConflictData : AttendeeConflictData
	{
		[XmlElement]
		public int NumberOfMembers
		{
			get
			{
				return this.numberOfMembers;
			}
			set
			{
				this.numberOfMembers = value;
			}
		}

		[XmlElement]
		public int NumberOfMembersAvailable
		{
			get
			{
				return this.numberOfMembersAvailable;
			}
			set
			{
				this.numberOfMembersAvailable = value;
			}
		}

		[XmlElement]
		public int NumberOfMembersWithConflict
		{
			get
			{
				return this.numberOfMembersWithConflict;
			}
			set
			{
				this.numberOfMembersWithConflict = value;
			}
		}

		[XmlElement]
		public int NumberOfMembersWithNoData
		{
			get
			{
				return this.numberOfMembersWithNoData;
			}
			set
			{
				this.numberOfMembersWithNoData = value;
			}
		}

		internal static GroupAttendeeConflictData Create(int numberOfMembers, int numberOfMembersWithConflict, int numberOfMembersAvailable, int numberOfMembersWithNoData)
		{
			return new GroupAttendeeConflictData
			{
				numberOfMembers = numberOfMembers,
				numberOfMembersWithConflict = numberOfMembersWithConflict,
				numberOfMembersAvailable = numberOfMembersAvailable,
				numberOfMembersWithNoData = numberOfMembersWithNoData
			};
		}

		private int numberOfMembers;

		private int numberOfMembersWithConflict;

		private int numberOfMembersAvailable;

		private int numberOfMembersWithNoData;
	}
}
