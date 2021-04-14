using System;
using System.Globalization;
using System.IO;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExternalUser
	{
		internal ExternalUser(MemoryPropertyBag propertyBag)
		{
			this.propertyBag = propertyBag;
		}

		public ExternalUser(string externalId, SmtpAddress address) : this(externalId, address, false)
		{
		}

		public static ExternalUser CreateExternalUserForGroupMailbox(string externalUserName, string externalUserId, Guid mailboxGuid, SecurityIdentity.GroupMailboxMemberType groupMailboxMemberType)
		{
			return new ExternalUser(new MemoryPropertyBag())
			{
				Name = externalUserName,
				ExternalId = externalUserId,
				SmtpAddress = SmtpAddress.Parse(externalUserId),
				Sid = SecurityIdentity.GetGroupSecurityIdentifier(mailboxGuid, groupMailboxMemberType)
			};
		}

		public ExternalUser(string externalUserName, string externalId, SmtpAddress address, SecurityIdentifier sid) : this(externalId, address, false)
		{
			this.Name = externalUserName;
			this.Sid = sid;
		}

		public ExternalUser(string externalId, SmtpAddress address, bool isReachUser) : this(new MemoryPropertyBag())
		{
			if (address.Local.StartsWith("ExchangePublishedUser."))
			{
				throw new ArgumentException(string.Format("Cannot add external user with prefix {0}", "ExchangePublishedUser."), "address");
			}
			this.IsReachUser = isReachUser;
			this.OriginalSmtpAddress = address;
			if (this.IsReachUser)
			{
				this.ExternalId = "ReachUser_" + externalId;
				this.Name = address.ToString() + " (Published)";
				this.SmtpAddress = ExternalUser.ConvertToReachSmtpAddress(address);
				this.Sid = ExternalUser.GenerateSid(this.SmtpAddress.ToString(), true);
				return;
			}
			this.ExternalId = externalId;
			this.Name = address.ToString();
			this.SmtpAddress = address;
			this.Sid = ExternalUser.GenerateSid(this.SmtpAddress.ToString(), false);
		}

		public string Name
		{
			get
			{
				if (this.name == null)
				{
					this.name = (this.propertyBag[InternalSchema.MemberName] as string);
				}
				return this.name;
			}
			private set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException();
				}
				this.propertyBag[InternalSchema.MemberName] = value;
				this.name = value;
			}
		}

		public string ExternalId
		{
			get
			{
				if (this.externalId == null)
				{
					this.externalId = (this.propertyBag[InternalSchema.MemberExternalIdLocalDirectory] as string);
				}
				return this.externalId;
			}
			private set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException();
				}
				this.propertyBag[InternalSchema.MemberExternalIdLocalDirectory] = value;
				this.externalId = value;
			}
		}

		public SmtpAddress SmtpAddress
		{
			get
			{
				if (this.smtpAddress == null)
				{
					this.smtpAddress = new SmtpAddress?(new SmtpAddress(this.propertyBag[InternalSchema.MemberEmailLocalDirectory] as string));
				}
				return this.smtpAddress.Value;
			}
			private set
			{
				this.propertyBag[InternalSchema.MemberEmailLocalDirectory] = value.ToString();
				this.smtpAddress = new SmtpAddress?(value);
			}
		}

		public SecurityIdentifier Sid
		{
			get
			{
				if (this.sid == null)
				{
					this.sid = new SecurityIdentifier(this.propertyBag[InternalSchema.MemberSIDLocalDirectory] as byte[], 0);
				}
				return this.sid;
			}
			private set
			{
				byte[] array = new byte[value.BinaryLength];
				value.GetBinaryForm(array, 0);
				this.propertyBag[InternalSchema.MemberSIDLocalDirectory] = array;
				this.sid = value;
			}
		}

		public string LegacyDn
		{
			get
			{
				if (this.legacyDn == null)
				{
					this.legacyDn = string.Format("{0}{1}", "LocalUser:", this.Sid);
				}
				return this.legacyDn;
			}
		}

		public bool IsReachUser
		{
			get
			{
				if (this.isReachUser == null)
				{
					this.isReachUser = new bool?(this.ExternalId.StartsWith("ReachUser_", StringComparison.Ordinal));
				}
				return this.isReachUser.Value;
			}
			private set
			{
				this.isReachUser = new bool?(value);
			}
		}

		public SmtpAddress OriginalSmtpAddress
		{
			get
			{
				if (this.originalSmtpAddress == null)
				{
					if (!this.IsReachUser)
					{
						this.originalSmtpAddress = new SmtpAddress?(this.SmtpAddress);
					}
					else
					{
						this.originalSmtpAddress = new SmtpAddress?(new SmtpAddress(this.SmtpAddress.ToString().Substring("ExchangePublishedUser.".Length)));
					}
				}
				return this.originalSmtpAddress.Value;
			}
			private set
			{
				this.originalSmtpAddress = new SmtpAddress?(value);
			}
		}

		internal MemoryPropertyBag PropertyBag
		{
			get
			{
				return this.propertyBag;
			}
		}

		public static bool IsExternalUserSid(SecurityIdentifier sid)
		{
			return 0 == string.Compare("S-1-8-", 0, sid.ToString(), 0, "S-1-8-".Length, StringComparison.CurrentCultureIgnoreCase);
		}

		internal static SmtpAddress ConvertToReachSmtpAddress(SmtpAddress smtpAddress)
		{
			return new SmtpAddress("ExchangePublishedUser." + smtpAddress.ToString());
		}

		internal static bool IsValidReachSid(SecurityIdentifier sid)
		{
			if (!ExternalUser.IsExternalUserSid(sid))
			{
				return false;
			}
			string text = sid.ToString();
			int num = text.LastIndexOf('-');
			string a = text.Substring(num + 1);
			string input = text.Substring(0, num + 1);
			string b = ExternalUser.ComputeHash(input);
			return string.Equals(a, b, StringComparison.Ordinal);
		}

		private static SecurityIdentifier GenerateSid(string source, bool forReachUser)
		{
			if (forReachUser)
			{
				source = Guid.NewGuid() + source;
			}
			byte[] sha1Hash = CryptoUtil.GetSha1Hash(Encoding.Unicode.GetBytes(source));
			for (int i = 0; i < 4; i++)
			{
				byte[] array = sha1Hash;
				int num = i;
				array[num] ^= sha1Hash[i + 16];
			}
			BinaryReader binaryReader = null;
			SecurityIdentifier result;
			try
			{
				binaryReader = new BinaryReader(new MemoryStream(sha1Hash));
				StringBuilder stringBuilder = new StringBuilder("S-1-8-");
				for (int j = 0; j < 4; j++)
				{
					if (j == 3 && forReachUser)
					{
						stringBuilder.Append(ExternalUser.ComputeHash(stringBuilder.ToString()));
					}
					else
					{
						stringBuilder.Append(binaryReader.ReadUInt32().ToString(NumberFormatInfo.InvariantInfo));
					}
					if (j < 3)
					{
						stringBuilder.Append('-');
					}
				}
				result = new SecurityIdentifier(stringBuilder.ToString());
			}
			finally
			{
				if (binaryReader != null)
				{
					binaryReader.Dispose();
					binaryReader = null;
				}
			}
			return result;
		}

		private static string ComputeHash(string input)
		{
			byte[] sha1Hash = CryptoUtil.GetSha1Hash(Encoding.Unicode.GetBytes(input));
			string result;
			using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(sha1Hash)))
			{
				result = binaryReader.ReadUInt32().ToString(NumberFormatInfo.InvariantInfo);
			}
			return result;
		}

		internal const string ReachUserNamePostFix = " (Published)";

		internal const string ExchangeSidPrefix = "S-1-8-";

		internal const string ReachUserSmtpPrefix = "ExchangePublishedUser.";

		private const string ReachUserPrefix = "ReachUser_";

		private readonly MemoryPropertyBag propertyBag;

		private string externalId;

		private bool? isReachUser;

		private string legacyDn;

		private string name;

		private SmtpAddress? originalSmtpAddress;

		private SecurityIdentifier sid;

		private SmtpAddress? smtpAddress;
	}
}
