using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Dkm;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Imap;

namespace Microsoft.Exchange.Transport.Sync.Migration.Rpc
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CreateIMAPSyncSubscriptionArgs : AbstractCreateSyncSubscriptionArgs
	{
		internal CreateIMAPSyncSubscriptionArgs(ADObjectId organizationalUnit, string userLegacyDN, string subscriptionName, string userDisplayName, SmtpAddress imapEmailAddress, string imapLogOnName, string logonPasswordEncrypted, Fqdn imapServer, int imapPort, IEnumerable<string> foldersToExclude, IMAPSecurityMechanism imapSecurityMechanism, IMAPAuthenticationMechanism imapAuthenticationMechanism, string userRootFolder, bool forceNew) : base(AggregationSubscriptionType.IMAP, organizationalUnit, subscriptionName, userLegacyDN, userDisplayName, imapEmailAddress, forceNew)
		{
			this.imapLogOnName = imapLogOnName;
			this.logonPasswordEncrypted = logonPasswordEncrypted;
			this.imapServer = imapServer;
			this.imapPort = imapPort;
			this.imapSecurityMechanism = imapSecurityMechanism;
			this.imapAuthenticationMechanism = imapAuthenticationMechanism;
			this.foldersToExclude = new List<string>(5);
			this.migrationUserRootFolder = userRootFolder;
			if (foldersToExclude != null)
			{
				this.foldersToExclude.AddRange(foldersToExclude);
			}
		}

		internal string ImapLogOnName
		{
			get
			{
				return this.imapLogOnName;
			}
		}

		internal string LogonPasswordEncrypted
		{
			get
			{
				return this.logonPasswordEncrypted;
			}
		}

		internal Fqdn ImapServer
		{
			get
			{
				return this.imapServer;
			}
		}

		internal int ImapPort
		{
			get
			{
				return this.imapPort;
			}
		}

		internal IMAPSecurityMechanism ImapSecurityMechanism
		{
			get
			{
				return this.imapSecurityMechanism;
			}
		}

		internal IMAPAuthenticationMechanism ImapAuthenticationMechanism
		{
			get
			{
				return this.imapAuthenticationMechanism;
			}
		}

		internal IEnumerable<string> FoldersToExclude
		{
			get
			{
				return this.foldersToExclude;
			}
		}

		internal string MigrationUserRootFolder
		{
			get
			{
				return this.migrationUserRootFolder;
			}
			set
			{
				this.migrationUserRootFolder = value;
			}
		}

		internal static CreateIMAPSyncSubscriptionArgs Unmarshal(MdbefPropertyCollection inputArgs)
		{
			string[] array = null;
			string text;
			if (MigrationRpcHelper.TryReadValue<string>(inputArgs, 2686058527U, out text) && !string.IsNullOrEmpty(text))
			{
				array = text.Split(new string[]
				{
					AggregationSubscription.FolderExclusionDelimiter
				}, StringSplitOptions.RemoveEmptyEntries);
			}
			string userRootFolder = MigrationRpcHelper.ReadValue<string>(inputArgs, 2686124063U, null);
			bool forceNew = MigrationRpcHelper.ReadValue<bool>(inputArgs, 2686255115U, false);
			return new CreateIMAPSyncSubscriptionArgs(MigrationRpcHelper.ReadADObjectId(inputArgs, 2688811266U), MigrationRpcHelper.ReadValue<string>(inputArgs, 2684485663U), MigrationRpcHelper.ReadValue<string>(inputArgs, 2685403167U), MigrationRpcHelper.ReadValue<string>(inputArgs, 2685927455U), new SmtpAddress(MigrationRpcHelper.ReadValue<string>(inputArgs, 2685599775U)), MigrationRpcHelper.ReadValue<string>(inputArgs, 2685665311U), MigrationRpcHelper.ReadValue<string>(inputArgs, 2685992991U), new Fqdn(MigrationRpcHelper.ReadValue<string>(inputArgs, 2685468703U)), MigrationRpcHelper.ReadValue<int>(inputArgs, 2685534211U), array, MigrationRpcHelper.ReadEnum<IMAPSecurityMechanism>(inputArgs, 2685796355U), MigrationRpcHelper.ReadEnum<IMAPAuthenticationMechanism>(inputArgs, 2685861891U), userRootFolder, forceNew);
		}

		internal override MdbefPropertyCollection Marshal()
		{
			MdbefPropertyCollection mdbefPropertyCollection = base.Marshal();
			mdbefPropertyCollection[2685665311U] = this.imapLogOnName;
			mdbefPropertyCollection[2685992991U] = this.logonPasswordEncrypted;
			mdbefPropertyCollection[2685468703U] = this.imapServer.ToString();
			mdbefPropertyCollection[2685534211U] = this.imapPort;
			if (this.foldersToExclude.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string value in this.foldersToExclude)
				{
					stringBuilder.Append(value);
					stringBuilder.Append(AggregationSubscription.FolderExclusionDelimiter);
				}
				mdbefPropertyCollection[2686058527U] = stringBuilder.ToString();
			}
			mdbefPropertyCollection[2685796355U] = (int)this.imapSecurityMechanism;
			mdbefPropertyCollection[2685861891U] = (int)this.imapAuthenticationMechanism;
			if (this.MigrationUserRootFolder != null)
			{
				mdbefPropertyCollection[2686124063U] = this.MigrationUserRootFolder;
			}
			return mdbefPropertyCollection;
		}

		internal override AggregationSubscription CreateInMemorySubscription()
		{
			IMAPAggregationSubscription imapaggregationSubscription = new IMAPAggregationSubscription();
			this.FillSubscription(imapaggregationSubscription);
			return imapaggregationSubscription;
		}

		internal override void FillSubscription(AggregationSubscription aggregationSubscription)
		{
			SyncUtilities.ThrowIfArgumentNull("aggregationSubscription", aggregationSubscription);
			IMAPAggregationSubscription imapaggregationSubscription = (IMAPAggregationSubscription)aggregationSubscription;
			base.FillSubscription(aggregationSubscription);
			imapaggregationSubscription.SendAsState = SendAsState.Disabled;
			imapaggregationSubscription.UserDisplayName = base.UserDisplayName;
			imapaggregationSubscription.UserEmailAddress = base.SmtpAddress;
			imapaggregationSubscription.IMAPLogOnName = this.ImapLogOnName;
			imapaggregationSubscription.LogonPasswordSecured = CreateIMAPSyncSubscriptionArgs.exchangeGroupKey.EncryptedStringToSecureString(this.LogonPasswordEncrypted);
			imapaggregationSubscription.IMAPServer = this.ImapServer;
			imapaggregationSubscription.IMAPPort = this.ImapPort;
			imapaggregationSubscription.IMAPSecurity = this.ImapSecurityMechanism;
			imapaggregationSubscription.IMAPAuthentication = this.ImapAuthenticationMechanism;
			imapaggregationSubscription.SetFoldersToExclude(this.FoldersToExclude);
			imapaggregationSubscription.ImapPathPrefix = this.MigrationUserRootFolder;
		}

		private const int DefaultFolderToExcludeCapacity = 5;

		private static readonly ExchangeGroupKey exchangeGroupKey = new ExchangeGroupKey(null, "Microsoft Exchange DKM");

		private readonly string imapLogOnName;

		private readonly string logonPasswordEncrypted;

		private readonly Fqdn imapServer;

		private readonly int imapPort;

		private readonly IMAPSecurityMechanism imapSecurityMechanism;

		private readonly IMAPAuthenticationMechanism imapAuthenticationMechanism;

		private readonly List<string> foldersToExclude;

		private string migrationUserRootFolder;
	}
}
