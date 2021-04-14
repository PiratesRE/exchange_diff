using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class AlternateMailbox
	{
		public AlternateMailbox(Guid identity, Guid databaseGuid, AlternateMailbox.AlternateMailboxFlags flags, string name, IList<SmtpAddress> smtpAddresses, string userName)
		{
			if (!AlternateMailbox.IsValidName(name))
			{
				throw new ArgumentException(name, "name");
			}
			if (!AlternateMailbox.IsValidUserName(userName))
			{
				throw new ArgumentException(userName, "userName");
			}
			this.identity = identity;
			this.databaseGuid = databaseGuid;
			this.flags = flags;
			this.name = name;
			if (smtpAddresses != null)
			{
				this.smtpAddresses = new List<SmtpAddress>(smtpAddresses);
			}
			else
			{
				this.smtpAddresses = new List<SmtpAddress>(0);
			}
			this.userName = (userName ?? string.Empty);
		}

		private AlternateMailbox()
		{
		}

		public Guid Identity
		{
			get
			{
				return this.identity;
			}
		}

		public Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
			internal set
			{
				this.databaseGuid = value;
			}
		}

		public AlternateMailbox.AlternateMailboxFlags Type
		{
			get
			{
				if ((this.flags & AlternateMailbox.AlternateMailboxFlags.Archive) == AlternateMailbox.AlternateMailboxFlags.Archive)
				{
					return AlternateMailbox.AlternateMailboxFlags.Archive;
				}
				if ((this.flags & AlternateMailbox.AlternateMailboxFlags.Subscription) == AlternateMailbox.AlternateMailboxFlags.Subscription)
				{
					return AlternateMailbox.AlternateMailboxFlags.Subscription;
				}
				return AlternateMailbox.AlternateMailboxFlags.Unknown;
			}
		}

		public bool RetentionPolicyEnabled
		{
			get
			{
				return (this.flags & AlternateMailbox.AlternateMailboxFlags.RetentionPolicyEnabled) == AlternateMailbox.AlternateMailboxFlags.RetentionPolicyEnabled;
			}
			internal set
			{
				this.flags = (value ? (this.flags | AlternateMailbox.AlternateMailboxFlags.RetentionPolicyEnabled) : (this.flags & ~AlternateMailbox.AlternateMailboxFlags.RetentionPolicyEnabled));
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			internal set
			{
				this.name = value;
			}
		}

		public IList<SmtpAddress> SmtpAddresses
		{
			get
			{
				return new ReadOnlyCollection<SmtpAddress>(this.smtpAddresses);
			}
		}

		public string UserName
		{
			get
			{
				return this.userName;
			}
			internal set
			{
				this.userName = value;
			}
		}

		public bool Equals(AlternateMailbox value)
		{
			return object.ReferenceEquals(this, value) || (value != null && this.identity == value.identity && this.databaseGuid == value.databaseGuid && this.flags == value.flags && this.name == value.name && this.userName == value.userName);
		}

		public override bool Equals(object comparand)
		{
			AlternateMailbox alternateMailbox = comparand as AlternateMailbox;
			return alternateMailbox != null && this.Equals(alternateMailbox);
		}

		public static bool operator ==(AlternateMailbox left, AlternateMailbox right)
		{
			if (left != null)
			{
				return left.Equals(right);
			}
			return right == null;
		}

		public static bool operator !=(AlternateMailbox left, AlternateMailbox right)
		{
			return !(left == right);
		}

		public override int GetHashCode()
		{
			return this.identity.GetHashCode();
		}

		public override string ToString()
		{
			string separator = ';'.ToString();
			string[] array = new string[7];
			array[0] = this.identity.ToString();
			array[1] = "1.0";
			array[2] = this.databaseGuid.ToString();
			string[] array2 = array;
			int num = 3;
			int num2 = (int)this.flags;
			array2[num] = num2.ToString();
			array[4] = ((this.name == null) ? string.Empty : this.name.ToString());
			array[5] = AlternateMailbox.GetEmailAddressesString(this.smtpAddresses);
			array[6] = ((this.userName == null) ? string.Empty : this.userName.ToString());
			string text = string.Join(separator, array);
			if (this.unknownProperties != null)
			{
				text += this.unknownProperties;
			}
			return text;
		}

		public static AlternateMailbox Parse(string blob)
		{
			AlternateMailbox result;
			if (!AlternateMailbox.TryParse(blob, out result))
			{
				throw new ArgumentException(DataStrings.InvalidAlternateMailboxString(blob, ';'), "blob");
			}
			return result;
		}

		public static bool TryParse(string blob, out AlternateMailbox alternateMailbox)
		{
			alternateMailbox = new AlternateMailbox();
			return AlternateMailbox.TryParse(blob, ref alternateMailbox.identity, ref alternateMailbox.databaseGuid, ref alternateMailbox.flags, ref alternateMailbox.name, ref alternateMailbox.smtpAddresses, ref alternateMailbox.userName, ref alternateMailbox.unknownProperties);
		}

		private static bool TryParse(string blob, ref Guid identity, ref Guid databaseGuid, ref AlternateMailbox.AlternateMailboxFlags flags, ref string name, ref List<SmtpAddress> smtpAddresses, ref string userName, ref string unknownProperties)
		{
			if (string.IsNullOrEmpty(blob))
			{
				return false;
			}
			string[] array = blob.Split(new char[]
			{
				';'
			});
			if (array == null || array.Length < 7)
			{
				return false;
			}
			int i = 0;
			identity = new Guid(array[i++]);
			if (array[i++] != "1.0")
			{
				return false;
			}
			databaseGuid = new Guid(array[i++]);
			flags = (AlternateMailbox.AlternateMailboxFlags)int.Parse(array[i++]);
			name = array[i++];
			smtpAddresses = AlternateMailbox.ParseEmailAddressesString(array[i++]);
			userName = array[i++];
			while (i < array.Length)
			{
				unknownProperties = string.Join(';'.ToString(), new string[]
				{
					unknownProperties,
					array[i]
				});
				i++;
			}
			return true;
		}

		private static List<SmtpAddress> ParseEmailAddressesString(string blob)
		{
			string[] array = blob.Split(new char[]
			{
				','
			});
			List<SmtpAddress> list = new List<SmtpAddress>(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				list.Add(new SmtpAddress(array[i]));
			}
			return list;
		}

		private static string GetEmailAddressesString(List<SmtpAddress> smtpAddresses)
		{
			if (smtpAddresses == null || smtpAddresses.Count == 0)
			{
				return string.Empty;
			}
			string[] array = new string[smtpAddresses.Count];
			for (int i = 0; i < smtpAddresses.Count; i++)
			{
				array[i] = smtpAddresses[i].ToString();
			}
			return string.Join(','.ToString(), array);
		}

		internal static bool IsValidName(string name)
		{
			return name != null && name.IndexOf(';') < 0;
		}

		internal static bool IsValidUserName(string userName)
		{
			return userName == null || userName.IndexOf(';') < 0;
		}

		internal const char ComponentSeparator = ';';

		private const char EmailSeparator = ',';

		private const string FormatVersion = "1.0";

		private Guid identity;

		private Guid databaseGuid;

		private AlternateMailbox.AlternateMailboxFlags flags;

		private string name;

		private List<SmtpAddress> smtpAddresses;

		private string userName;

		private string unknownProperties;

		[Flags]
		public enum AlternateMailboxFlags
		{
			Unknown = 0,
			Archive = 1,
			Subscription = 2,
			RetentionPolicyEnabled = 256,
			Default = 257
		}
	}
}
