using System;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	internal sealed class SharingLevelOfDetails
	{
		public SharingLevelOfDetails()
		{
			this.currentLevel = LevelOfDetails.Unknown;
		}

		public SharingLevelOfDetails(BaseFolderType folder)
		{
			this.currentLevel = LevelOfDetails.Unknown;
			CalendarFolderType calendarFolderType = folder as CalendarFolderType;
			if (calendarFolderType == null)
			{
				ContactsFolderType contactsFolderType = folder as ContactsFolderType;
				if (contactsFolderType != null)
				{
					switch (contactsFolderType.SharingEffectiveRights)
					{
					case PermissionReadAccessType.None:
						this.currentLevel = LevelOfDetails.None;
						return;
					case PermissionReadAccessType.FullDetails:
						this.currentLevel = LevelOfDetails.Full;
						break;
					default:
						return;
					}
				}
				return;
			}
			switch (calendarFolderType.SharingEffectiveRights)
			{
			case CalendarPermissionReadAccessType.None:
				this.currentLevel = LevelOfDetails.None;
				return;
			case CalendarPermissionReadAccessType.TimeOnly:
				this.currentLevel = LevelOfDetails.Availability;
				return;
			case CalendarPermissionReadAccessType.TimeAndSubjectAndLocation:
				this.currentLevel = LevelOfDetails.Limited;
				return;
			case CalendarPermissionReadAccessType.FullDetails:
				this.currentLevel = LevelOfDetails.Full;
				return;
			default:
				return;
			}
		}

		public SharingLevelOfDetails(LevelOfDetails levelOfDetails)
		{
			this.currentLevel = levelOfDetails;
		}

		public static implicit operator LevelOfDetails(SharingLevelOfDetails levelOfDetails)
		{
			return levelOfDetails.currentLevel;
		}

		public static bool operator ==(SharingLevelOfDetails left, LevelOfDetails right)
		{
			return left.currentLevel == right;
		}

		public static bool operator !=(SharingLevelOfDetails left, LevelOfDetails right)
		{
			return left.currentLevel != right;
		}

		public static bool operator >(SharingLevelOfDetails left, SharingLevelOfDetails right)
		{
			return left.currentLevel > right.currentLevel;
		}

		public static bool operator <(SharingLevelOfDetails left, SharingLevelOfDetails right)
		{
			return left.currentLevel < right.currentLevel;
		}

		public override bool Equals(object otherObject)
		{
			SharingLevelOfDetails sharingLevelOfDetails = otherObject as SharingLevelOfDetails;
			return sharingLevelOfDetails != null && this == sharingLevelOfDetails;
		}

		public override int GetHashCode()
		{
			return this.currentLevel.GetHashCode();
		}

		public override string ToString()
		{
			return this.currentLevel.ToString();
		}

		private LevelOfDetails currentLevel;
	}
}
