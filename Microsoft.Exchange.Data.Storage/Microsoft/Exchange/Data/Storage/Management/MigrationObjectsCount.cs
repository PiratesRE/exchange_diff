using System;
using System.Globalization;
using System.IO;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MigrationObjectsCount
	{
		internal MigrationObjectsCount(int? mailboxes)
		{
			this.Mailboxes = mailboxes;
		}

		internal MigrationObjectsCount(int? mailboxes, int? groups, int? contacts, bool publicFodlers)
		{
			this.Mailboxes = mailboxes;
			this.Groups = groups;
			this.Contacts = contacts;
			this.PublicFolders = publicFodlers;
		}

		public int? Mailboxes { get; internal set; }

		public int? Groups { get; internal set; }

		public int? Contacts { get; internal set; }

		public bool PublicFolders { get; internal set; }

		public static MigrationObjectsCount operator +(MigrationObjectsCount value1, MigrationObjectsCount value2)
		{
			return new MigrationObjectsCount(MigrationObjectsCount.Add(value1.Mailboxes, value2.Mailboxes), MigrationObjectsCount.Add(value1.Groups, value2.Groups), MigrationObjectsCount.Add(value1.Contacts, value2.Contacts), value1.PublicFolders || value2.PublicFolders);
		}

		public static MigrationObjectsCount operator -(MigrationObjectsCount value1, MigrationObjectsCount value2)
		{
			return new MigrationObjectsCount(MigrationObjectsCount.Subtract(value1.Mailboxes, value2.Mailboxes), MigrationObjectsCount.Subtract(value1.Groups, value2.Groups), MigrationObjectsCount.Subtract(value1.Contacts, value2.Contacts), value1.PublicFolders || value2.PublicFolders);
		}

		public override string ToString()
		{
			if (this.GetTotal() == 0)
			{
				return ServerStrings.MigrationObjectsCountStringNone;
			}
			string text = ServerStrings.MigrationObjectsCountStringMailboxes(((this.Mailboxes != null) ? this.Mailboxes.Value : 0).ToString(CultureInfo.InvariantCulture));
			if (this.Groups != null && this.Groups.Value > 0)
			{
				text += ServerStrings.MigrationObjectsCountStringGroups(this.Groups.Value.ToString(CultureInfo.InvariantCulture));
			}
			if (this.Contacts != null && this.Contacts.Value > 0)
			{
				text += ServerStrings.MigrationObjectsCountStringContacts(this.Contacts.Value.ToString(CultureInfo.InvariantCulture));
			}
			if (this.PublicFolders)
			{
				text += ServerStrings.MigrationObjectsCountStringPFs;
			}
			return text;
		}

		public override bool Equals(object obj)
		{
			MigrationObjectsCount migrationObjectsCount = obj as MigrationObjectsCount;
			return migrationObjectsCount != null && (this.Mailboxes == migrationObjectsCount.Mailboxes && this.Groups == migrationObjectsCount.Groups && this.Contacts == migrationObjectsCount.Contacts) && (!this.PublicFolders ^ !migrationObjectsCount.PublicFolders);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		internal static MigrationObjectsCount Max(MigrationObjectsCount obj1, MigrationObjectsCount obj2)
		{
			return new MigrationObjectsCount(MigrationObjectsCount.Max(obj1.Mailboxes, obj2.Mailboxes), MigrationObjectsCount.Max(obj1.Groups, obj2.Groups), MigrationObjectsCount.Max(obj1.Contacts, obj2.Contacts), obj1.PublicFolders || obj2.PublicFolders);
		}

		internal static MigrationObjectsCount FromValue(string value)
		{
			string[] array = value.Split(new string[]
			{
				":"
			}, StringSplitOptions.None);
			if (array.Length != 4)
			{
				throw new InvalidDataException("Invalid value : " + value, null);
			}
			return new MigrationObjectsCount(MigrationObjectsCount.FromString(array[0]), MigrationObjectsCount.FromString(array[1]), MigrationObjectsCount.FromString(array[2]), bool.Parse(array[3]));
		}

		internal string ToValue()
		{
			string[] value = new string[]
			{
				MigrationObjectsCount.ToValue(this.Mailboxes),
				MigrationObjectsCount.ToValue(this.Groups),
				MigrationObjectsCount.ToValue(this.Contacts),
				this.PublicFolders.ToString()
			};
			return string.Join(":", value);
		}

		internal int GetTotal()
		{
			int num = 0;
			if (this.Mailboxes != null && this.Mailboxes.Value > 0)
			{
				num += this.Mailboxes.Value;
			}
			if (this.Groups != null && this.Groups.Value > 0)
			{
				num += this.Groups.Value;
			}
			if (this.Contacts != null && this.Contacts.Value > 0)
			{
				num += this.Contacts.Value;
			}
			if (this.PublicFolders)
			{
				num++;
			}
			return num;
		}

		private static int? Max(int? int1, int? int2)
		{
			if (int1 != null || int2 != null)
			{
				return new int?(Math.Max((int1 != null) ? int1.Value : int.MinValue, (int2 != null) ? int2.Value : int.MinValue));
			}
			return null;
		}

		private static int? Add(int? int1, int? int2)
		{
			if (int1 == null && int2 == null)
			{
				return null;
			}
			return new int?(((int1 != null) ? int1.Value : 0) + ((int2 != null) ? int2.Value : 0));
		}

		private static int? Subtract(int? int1, int? int2)
		{
			if (int1 == null && int2 == null)
			{
				return null;
			}
			return new int?(((int1 != null) ? int1.Value : 0) - ((int2 != null) ? int2.Value : 0));
		}

		private static int? FromString(string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				return new int?(int.Parse(value));
			}
			return null;
		}

		private static string ToValue(int? value)
		{
			if (value == null)
			{
				return string.Empty;
			}
			return value.Value.ToString(CultureInfo.InvariantCulture);
		}

		private const string ValueSeparation = ":";
	}
}
