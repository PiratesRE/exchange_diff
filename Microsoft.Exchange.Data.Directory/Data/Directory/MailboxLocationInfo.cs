using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory
{
	public class MailboxLocationInfo : IMailboxLocationInfo
	{
		public MailboxLocationInfo(Guid mailboxGuid, ADObjectId databaseLocation, MailboxLocationType mailboxLocationType)
		{
			MailboxLocationInfo.ValidateMailboxInfo(mailboxGuid, databaseLocation);
			this.MailboxGuid = mailboxGuid;
			this.databaseLocation = ((databaseLocation != null) ? new ADObjectId(databaseLocation.ObjectGuid, databaseLocation.PartitionFQDN) : null);
			this.MailboxLocationType = mailboxLocationType;
			this.infoToString = null;
		}

		public MailboxLocationInfo(string mailboxLocationString)
		{
			if (string.IsNullOrEmpty(mailboxLocationString))
			{
				throw new ArgumentNullException("mailboxLocationString");
			}
			string[] array = mailboxLocationString.Split(new string[]
			{
				MailboxLocationInfo.MailboxLocationDelimiter
			}, StringSplitOptions.None);
			int minValue = int.MinValue;
			if (array.Length > 1 && (!int.TryParse(array[0], out minValue) || minValue > MailboxLocationInfo.MaxSerializableVersion || minValue < 0))
			{
				throw new ArgumentException("mailboxLocationString");
			}
			Guid guid = Guid.Empty;
			string text = null;
			for (int i = 1; i < array.Length; i++)
			{
				if (!string.IsNullOrEmpty(array[i]))
				{
					try
					{
						switch (i)
						{
						case 1:
							this.MailboxGuid = Guid.Parse(array[1]);
							break;
						case 2:
							this.MailboxLocationType = (MailboxLocationType)Enum.Parse(typeof(MailboxLocationType), array[2]);
							break;
						case 3:
							text = array[3];
							break;
						case 4:
							guid = Guid.Parse(array[4]);
							break;
						}
					}
					catch (Exception innerException)
					{
						throw new ADOperationException(DirectoryStrings.CannotParse(array[i]), innerException);
					}
				}
			}
			if (!guid.Equals(Guid.Empty) && text != null)
			{
				this.databaseLocation = new ADObjectId(guid, text);
			}
			MailboxLocationInfo.ValidateMailboxInfo(this.MailboxGuid, this.databaseLocation);
			this.infoToString = null;
		}

		private static void ValidateMailboxInfo(Guid mailboxGuid, ADObjectId databaseLocation)
		{
			if (mailboxGuid.Equals(Guid.Empty))
			{
				throw new ArgumentException("mailboxGuid");
			}
		}

		public Guid MailboxGuid { get; private set; }

		public ADObjectId DatabaseLocation
		{
			get
			{
				if (this.databaseLocation == null)
				{
					return null;
				}
				return new ADObjectId(this.databaseLocation.ObjectGuid, this.databaseLocation.PartitionFQDN);
			}
		}

		public MailboxLocationType MailboxLocationType { get; private set; }

		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.infoToString))
			{
				string[] array = new string[MailboxLocationInfo.MaxMailboxLocationIndices];
				array[0] = MailboxLocationInfo.MaxSerializableVersion.ToString();
				array[2] = this.MailboxLocationType.ToString();
				array[1] = this.MailboxGuid.ToString();
				if (this.databaseLocation != null)
				{
					array[3] = this.databaseLocation.PartitionFQDN;
					array[4] = this.databaseLocation.ObjectGuid.ToString();
				}
				this.infoToString = string.Join(MailboxLocationInfo.MailboxLocationDelimiter, array);
			}
			return this.infoToString;
		}

		public override bool Equals(object obj)
		{
			MailboxLocationInfo mailboxLocationInfo = obj as MailboxLocationInfo;
			if (mailboxLocationInfo == null)
			{
				return false;
			}
			bool flag = false;
			if (mailboxLocationInfo.DatabaseLocation == null && this.DatabaseLocation == null)
			{
				flag = true;
			}
			else if (mailboxLocationInfo.DatabaseLocation != null && this.DatabaseLocation != null)
			{
				flag = mailboxLocationInfo.DatabaseLocation.Equals(this.DatabaseLocation);
			}
			return mailboxLocationInfo.MailboxGuid.Equals(this.MailboxGuid) && flag && mailboxLocationInfo.MailboxLocationType == this.MailboxLocationType;
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		private static readonly string MailboxLocationDelimiter = ";";

		private static int MaxMailboxLocationIndices = Enum.GetValues(typeof(MailboxLocationInfo.MailboxLocationIndex)).Length;

		private static readonly int MaxSerializableVersion = 1;

		private string infoToString;

		private readonly ADObjectId databaseLocation;

		private enum MailboxLocationIndex
		{
			Version,
			MailboxGuid,
			MailboxLocationType,
			DBForestFqdn,
			DBObjectGuid
		}
	}
}
