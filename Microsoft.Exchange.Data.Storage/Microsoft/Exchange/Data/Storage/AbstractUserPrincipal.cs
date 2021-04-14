using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AbstractUserPrincipal : IUserPrincipal, IExchangePrincipal
	{
		public virtual ADObjectId ObjectId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual string UserPrincipalName
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual string LegacyDn
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual string Alias
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual ADObjectId DefaultPublicFolderMailbox
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual SecurityIdentifier Sid
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual SecurityIdentifier MasterAccountSid
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual IEnumerable<SecurityIdentifier> SidHistory
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual IEnumerable<ADObjectId> Delegates
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual IEnumerable<CultureInfo> PreferredCultures
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual RecipientType RecipientType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual RecipientTypeDetails RecipientTypeDetails
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool? IsResource
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual ModernGroupObjectType ModernGroupType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual IEnumerable<SecurityIdentifier> PublicToGroupSids
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual string ExternalDirectoryObjectId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual IEnumerable<Guid> AggregatedMailboxGuids
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual ReleaseTrack? ReleaseTrack
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual IMailboxInfo MailboxInfo
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual IEnumerable<IMailboxInfo> AllMailboxes
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool IsCrossSiteAccessAllowed
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual bool IsMailboxInfoRequired
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual string PrincipalId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual SmtpAddress WindowsLiveId
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual NetID NetId
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}
