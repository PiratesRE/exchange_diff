using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Configuration.DelegatedAuthentication
{
	internal class DelegatedSecurityToken
	{
		public DelegatedSecurityToken(string displayName, string partnerOrgId, string[] groupsId)
		{
			if (displayName == null)
			{
				throw new ArgumentNullException("displayName");
			}
			if (string.IsNullOrEmpty(partnerOrgId))
			{
				throw new ArgumentNullException("partnerOrgId");
			}
			this.displayName = displayName;
			this.partnerOrgId = partnerOrgId;
			this.groupIds = groupsId;
			this.UTCCreationTime = DateTime.UtcNow;
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public string[] PartnerGroupIds
		{
			get
			{
				return this.groupIds;
			}
		}

		public string PartnerOrgDirectoryId
		{
			get
			{
				return this.partnerOrgId;
			}
		}

		public DateTime UTCCreationTime
		{
			get
			{
				return this.utcCreationTime;
			}
			private set
			{
				this.utcCreationTime = value;
				this.utcExpirationTime = this.utcCreationTime.Add(DelegatedSecurityToken.TokenLifetime);
			}
		}

		public DateTime UTCExpirationTime
		{
			get
			{
				return this.utcExpirationTime;
			}
		}

		internal static TimeSpan TokenLifetime
		{
			get
			{
				if (DelegatedSecurityToken.tokenLifetime != null)
				{
					return DelegatedSecurityToken.tokenLifetime.Value;
				}
				return DelegatedSecurityToken.DefaultMaximumTokenLifetime;
			}
			set
			{
				if (value < DelegatedSecurityToken.MaximumTokenLifetime)
				{
					DelegatedSecurityToken.tokenLifetime = new TimeSpan?(value);
				}
			}
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.toStringRep))
			{
				string text = this.utcCreationTime.ToString(DateTimeFormatInfo.InvariantInfo);
				string text2 = Uri.EscapeDataString(this.displayName);
				StringBuilder stringBuilder = new StringBuilder(text.Length + this.partnerOrgId.Length + text2.Length + this.groupIds.Length * 32 + this.groupIds.Length + 3);
				stringBuilder.Append(text);
				stringBuilder.Append('&');
				stringBuilder.Append(this.partnerOrgId);
				stringBuilder.Append('&');
				stringBuilder.Append(text2);
				stringBuilder.Append('&');
				int num = 0;
				while (this.groupIds != null && num < this.groupIds.Length)
				{
					stringBuilder.Append(this.groupIds[num]);
					if (num + 1 < this.groupIds.Length)
					{
						stringBuilder.Append(',');
					}
					num++;
				}
				this.toStringRep = stringBuilder.ToString();
			}
			return this.toStringRep;
		}

		public bool IsExpired()
		{
			return DateTime.UtcNow > this.UTCExpirationTime;
		}

		internal static DelegatedSecurityToken Parse(string securityToken)
		{
			string[] array = securityToken.Split(new char[]
			{
				'&'
			});
			if (array.Length < 4)
			{
				throw new ArgumentException("securityToken");
			}
			DateTime utccreationTime = DateTime.Parse(array[0], DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal);
			string text = array[1];
			string text2 = Uri.UnescapeDataString(array[2]);
			string[] groupsId = array[3].Split(new char[]
			{
				','
			});
			return new DelegatedSecurityToken(text2, text, groupsId)
			{
				UTCCreationTime = utccreationTime
			};
		}

		internal const char SecurityTokenSeparator = '&';

		internal const char GroupTokenSeparator = ',';

		internal static readonly TimeSpan DefaultMaximumTokenLifetime = new TimeSpan(6, 0, 0);

		internal static readonly TimeSpan MaximumTokenLifetime = new TimeSpan(7, 0, 0, 0);

		private static TimeSpan? tokenLifetime;

		private string displayName;

		private string partnerOrgId;

		private string[] groupIds;

		private DateTime utcCreationTime;

		private DateTime utcExpirationTime;

		private volatile string toStringRep;
	}
}
