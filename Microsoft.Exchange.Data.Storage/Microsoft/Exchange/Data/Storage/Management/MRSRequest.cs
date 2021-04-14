using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class MRSRequest : UserConfigurationObject
	{
		public MRSRequest()
		{
			base.SetExchangeVersion(ExchangeObjectVersion.Exchange2012);
		}

		internal override UserConfigurationObjectSchema Schema
		{
			get
			{
				return MRSRequest.schema;
			}
		}

		public Guid RequestGuid
		{
			get
			{
				return (Guid)this[MRSRequestSchema.RequestGuid];
			}
			set
			{
				this[MRSRequestSchema.RequestGuid] = value;
			}
		}

		public string Name
		{
			get
			{
				return (string)this[MRSRequestSchema.Name];
			}
			set
			{
				this[MRSRequestSchema.Name] = value;
			}
		}

		public RequestStatus Status
		{
			get
			{
				return (RequestStatus)this[MRSRequestSchema.Status];
			}
			set
			{
				this[MRSRequestSchema.Status] = value;
			}
		}

		public RequestFlags Flags
		{
			get
			{
				return (RequestFlags)this[MRSRequestSchema.Flags];
			}
			set
			{
				this[MRSRequestSchema.Flags] = value;
			}
		}

		public string RemoteHostName
		{
			get
			{
				return (string)this[MRSRequestSchema.RemoteHostName];
			}
			set
			{
				this[MRSRequestSchema.RemoteHostName] = value;
			}
		}

		public string BatchName
		{
			get
			{
				return (string)this[MRSRequestSchema.BatchName];
			}
			set
			{
				this[MRSRequestSchema.BatchName] = value;
			}
		}

		public ADObjectId SourceMDB
		{
			get
			{
				return (ADObjectId)this[MRSRequestSchema.SourceMDB];
			}
			set
			{
				this[MRSRequestSchema.SourceMDB] = ADObjectIdResolutionHelper.ResolveDN(value);
			}
		}

		public ADObjectId TargetMDB
		{
			get
			{
				return (ADObjectId)this[MRSRequestSchema.TargetMDB];
			}
			set
			{
				this[MRSRequestSchema.TargetMDB] = ADObjectIdResolutionHelper.ResolveDN(value);
			}
		}

		public ADObjectId StorageMDB
		{
			get
			{
				return (ADObjectId)this[MRSRequestSchema.StorageMDB];
			}
			set
			{
				this[MRSRequestSchema.StorageMDB] = ADObjectIdResolutionHelper.ResolveDN(value);
			}
		}

		public string FilePath
		{
			get
			{
				return (string)this[MRSRequestSchema.FilePath];
			}
			set
			{
				this[MRSRequestSchema.FilePath] = value;
			}
		}

		public MRSRequestType Type
		{
			get
			{
				return (MRSRequestType)this[MRSRequestSchema.Type];
			}
			set
			{
				this[MRSRequestSchema.Type] = value;
			}
		}

		public ADObjectId TargetUserId
		{
			get
			{
				return (ADObjectId)this[MRSRequestSchema.TargetUserId];
			}
			set
			{
				this[MRSRequestSchema.TargetUserId] = value;
			}
		}

		public ADObjectId SourceUserId
		{
			get
			{
				return (ADObjectId)this[MRSRequestSchema.SourceUserId];
			}
			set
			{
				this[MRSRequestSchema.SourceUserId] = value;
			}
		}

		public OrganizationId OrganizationId { get; set; }

		public DateTime? WhenChanged
		{
			get
			{
				return (DateTime?)this[MRSRequestSchema.WhenChanged];
			}
		}

		public DateTime? WhenCreated
		{
			get
			{
				return (DateTime?)this[MRSRequestSchema.WhenCreated];
			}
		}

		public DateTime? WhenChangedUTC
		{
			get
			{
				return (DateTime?)this[MRSRequestSchema.WhenChangedUTC];
			}
		}

		public DateTime? WhenCreatedUTC
		{
			get
			{
				return (DateTime?)this[MRSRequestSchema.WhenCreatedUTC];
			}
		}

		public static T Read<T>(MailboxStoreTypeProvider session, Guid requestGuid) where T : MRSRequest, new()
		{
			ExchangePrincipal principal = ExchangePrincipal.FromADUser(session.ADUser, null);
			T result;
			using (UserConfigurationDictionaryAdapter<T> userConfigurationDictionaryAdapter = new UserConfigurationDictionaryAdapter<T>(session.MailboxSession, MRSRequest.GetName(requestGuid), new GetUserConfigurationDelegate(UserConfigurationHelper.GetMailboxConfiguration), MRSRequestSchema.PersistedProperties))
			{
				try
				{
					T t = userConfigurationDictionaryAdapter.Read(principal);
					if (t.RequestGuid != requestGuid)
					{
						throw new CannotFindRequestIndexEntryException(requestGuid);
					}
					result = t;
				}
				catch (FormatException innerException)
				{
					throw new CannotFindRequestIndexEntryException(requestGuid, innerException);
				}
			}
			return result;
		}

		public override void Delete(MailboxStoreTypeProvider session)
		{
			using (UserConfiguration mailboxConfiguration = UserConfigurationHelper.GetMailboxConfiguration(session.MailboxSession, MRSRequest.GetName(this.RequestGuid), UserConfigurationTypes.Dictionary, false))
			{
				if (mailboxConfiguration == null)
				{
					return;
				}
			}
			UserConfigurationHelper.DeleteMailboxConfiguration(session.MailboxSession, MRSRequest.GetName(this.RequestGuid));
		}

		public override IConfigurable Read(MailboxStoreTypeProvider session, ObjectId identity)
		{
			Guid requestGuid = new Guid(identity.GetBytes());
			return MRSRequest.Read<MRSRequest>(session, requestGuid);
		}

		public override void Save(MailboxStoreTypeProvider session)
		{
			using (UserConfigurationDictionaryAdapter<MRSRequest> userConfigurationDictionaryAdapter = new UserConfigurationDictionaryAdapter<MRSRequest>(session.MailboxSession, MRSRequest.GetName(this.RequestGuid), SaveMode.NoConflictResolutionForceSave, new GetUserConfigurationDelegate(UserConfigurationHelper.GetMailboxConfiguration), MRSRequestSchema.PersistedProperties))
			{
				userConfigurationDictionaryAdapter.Save(this);
			}
			base.ResetChangeTracking();
		}

		private static string GetName(Guid requestGuid)
		{
			return string.Format("{0}.{1}", "MRSRequest", requestGuid.ToString("N"));
		}

		public const string ConfigurationNamePrefix = "MRSRequest";

		protected static readonly MRSRequestSchema schema = ObjectSchema.GetInstance<MRSRequestSchema>();
	}
}
