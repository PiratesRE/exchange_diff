using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class PublicFolderInformation
	{
		private PublicFolderInformation(int majorVersion, int minorVersion, string rawValue)
		{
			this.majorVersion = majorVersion;
			this.minorVersion = minorVersion;
			this.type = PublicFolderInformation.HierarchyType.Unknown;
			this.rawValue = rawValue;
		}

		private PublicFolderInformation(int majorVersion, int minorVersion, PublicFolderInformation.HierarchyType type, Guid hierarchyMailboxGuid, string rawValue)
		{
			this.majorVersion = majorVersion;
			this.minorVersion = minorVersion;
			this.type = type;
			this.hierarchyMailboxGuid = new Guid?(hierarchyMailboxGuid);
			this.rawValue = rawValue;
		}

		public bool IsValid
		{
			get
			{
				return this.type != PublicFolderInformation.HierarchyType.Unknown;
			}
		}

		public bool CanUpdate
		{
			get
			{
				return this.majorVersion < 1 || (this.majorVersion == 1 && this.minorVersion <= 1);
			}
		}

		internal PublicFolderInformation.HierarchyType Type
		{
			get
			{
				return this.type;
			}
		}

		public Guid HierarchyMailboxGuid
		{
			get
			{
				if (this.hierarchyMailboxGuid == null)
				{
					return Guid.Empty;
				}
				return this.hierarchyMailboxGuid.Value;
			}
		}

		public bool LockedForMigration
		{
			get
			{
				return this.type == PublicFolderInformation.HierarchyType.InTransitMailboxGuid;
			}
		}

		internal int ItemSize
		{
			get
			{
				switch (this.type)
				{
				case PublicFolderInformation.HierarchyType.Unknown:
					return 9 + this.rawValue.Length;
				case PublicFolderInformation.HierarchyType.MailboxGuid:
				case PublicFolderInformation.HierarchyType.InTransitMailboxGuid:
					return 25 + this.rawValue.Length;
				}
				throw new InvalidOperationException("How did we get here?");
			}
		}

		public void SetHierarchyMailbox(Guid hierarchyMailboxGuid, PublicFolderInformation.HierarchyType hierarchyType)
		{
			if (hierarchyType != PublicFolderInformation.HierarchyType.MailboxGuid && hierarchyType != PublicFolderInformation.HierarchyType.InTransitMailboxGuid)
			{
				throw new ArgumentException(string.Format("hierarchyType must be either MailboxGuid or InTransitMailboxGuid: {0}", hierarchyType), "hierarchyType");
			}
			this.EnsureWritable();
			this.type = hierarchyType;
			this.hierarchyMailboxGuid = new Guid?(hierarchyMailboxGuid);
			this.hierarchySmtpAddress = SmtpAddress.Empty;
		}

		public string Serialize()
		{
			switch (this.type)
			{
			case PublicFolderInformation.HierarchyType.MailboxGuid:
			case PublicFolderInformation.HierarchyType.InTransitMailboxGuid:
			{
				string text = this.hierarchyMailboxGuid.ToString();
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(this.majorVersion);
				stringBuilder.Append(PublicFolderInformation.Separator);
				stringBuilder.Append(this.minorVersion);
				stringBuilder.Append(PublicFolderInformation.Separator);
				stringBuilder.Append((byte)this.type);
				stringBuilder.Append(PublicFolderInformation.Separator);
				stringBuilder.Append(text.Length);
				stringBuilder.Append(PublicFolderInformation.Separator);
				stringBuilder.Append(text);
				return stringBuilder.ToString();
			}
			default:
				return this.rawValue;
			}
		}

		public PublicFolderInformation Clone()
		{
			return new PublicFolderInformation(this.majorVersion, this.minorVersion, this.rawValue)
			{
				type = this.type,
				hierarchyMailboxGuid = this.hierarchyMailboxGuid,
				hierarchySmtpAddress = this.hierarchySmtpAddress
			};
		}

		public override string ToString()
		{
			switch (this.type)
			{
			case PublicFolderInformation.HierarchyType.MailboxGuid:
			case PublicFolderInformation.HierarchyType.InTransitMailboxGuid:
				return this.hierarchyMailboxGuid.ToString();
			default:
				return this.rawValue;
			}
		}

		private void EnsureWritable()
		{
			if (!this.CanUpdate)
			{
				throw new InvalidOperationException("This instance is not writable");
			}
		}

		public static PublicFolderInformation Deserialize(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				return new PublicFolderInformation(1, 1, string.Empty);
			}
			int num = 0;
			int num2 = input.IndexOf(PublicFolderInformation.Separator);
			if (num2 == -1 || !int.TryParse(input.Substring(0, num2), out num) || num > 1)
			{
				return new PublicFolderInformation(num, 1, input);
			}
			int num3 = 0;
			string text = input.Substring(num2 + 1);
			int num4 = text.IndexOf(PublicFolderInformation.Separator);
			if (num4 == -1 || !int.TryParse(text.Substring(0, num4), out num3))
			{
				return new PublicFolderInformation(num, 1, input);
			}
			text = text.Substring(num4 + 1);
			int num5 = text.IndexOf(PublicFolderInformation.Separator);
			byte b = 0;
			PublicFolderInformation.HierarchyType hierarchyType;
			if (num5 == -1 || !byte.TryParse(text.Substring(0, num5), out b) || !Enum.IsDefined(typeof(PublicFolderInformation.HierarchyType), b) || (hierarchyType = (PublicFolderInformation.HierarchyType)b) == PublicFolderInformation.HierarchyType.Unknown)
			{
				return new PublicFolderInformation(num, num3, input);
			}
			int num6 = 0;
			text = text.Substring(num5 + 1);
			int num7 = text.IndexOf(PublicFolderInformation.Separator);
			if (num7 == -1 || !int.TryParse(text.Substring(0, num7), out num6) || num6 <= 0 || text.Length < num7 + 1 + num6)
			{
				return new PublicFolderInformation(num, num3, input);
			}
			string text2 = text.Substring(num7 + 1, num6);
			if (string.IsNullOrWhiteSpace(text2))
			{
				return new PublicFolderInformation(num, num3, input);
			}
			switch (hierarchyType)
			{
			case PublicFolderInformation.HierarchyType.MailboxGuid:
			case PublicFolderInformation.HierarchyType.InTransitMailboxGuid:
			{
				Guid empty = Guid.Empty;
				if (GuidHelper.TryParseGuid(text2, out empty))
				{
					return new PublicFolderInformation(num, num3, hierarchyType, empty, input);
				}
				return new PublicFolderInformation(num, num3, input);
			}
			default:
				throw new InvalidOperationException("Unknown HierarchyType! How did we get here if parsing was successful?");
			}
		}

		private const int CurrentMajorVersion = 1;

		private const int CurrentMinorVersion = 1;

		internal static PublicFolderInformation InvalidPublicFolderInformation = new PublicFolderInformation(1, 1, string.Empty);

		private static char Separator = ';';

		private readonly string rawValue;

		private readonly int majorVersion;

		private readonly int minorVersion;

		private PublicFolderInformation.HierarchyType type;

		private Guid? hierarchyMailboxGuid;

		private SmtpAddress hierarchySmtpAddress;

		public enum HierarchyType : byte
		{
			Unknown = 1,
			MailboxGuid = 3,
			InTransitMailboxGuid
		}
	}
}
