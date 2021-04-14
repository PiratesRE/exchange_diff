using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Security;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Management.Migration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration.DataAccessLayer
{
	internal abstract class MigrationEndpointBase : MigrationMessagePersistableBase, ISubscriptionSettings, IMigrationSerializable
	{
		protected MigrationEndpointBase()
		{
			this.EndpointType = MigrationType.None;
		}

		protected MigrationEndpointBase(MigrationEndpoint endpoint)
		{
			MigrationUtil.ThrowOnNullArgument(endpoint.Identity, "endpoint.Identity");
			this.InitializeFromPresentationObject(endpoint);
		}

		protected MigrationEndpointBase(MigrationType endpointType)
		{
			if (endpointType == MigrationType.None)
			{
				throw new ArgumentException("MigrationType cannot be 'None'", "endpointType");
			}
			this.EndpointType = endpointType;
		}

		public MigrationEndpointId Identity { get; private set; }

		public override long MinimumSupportedVersion
		{
			get
			{
				return 4L;
			}
		}

		public override long MaximumSupportedVersion
		{
			get
			{
				return 5L;
			}
		}

		public override long CurrentSupportedVersion
		{
			get
			{
				return this.MaximumSupportedVersion;
			}
		}

		public MigrationType EndpointType { get; private set; }

		public override PropertyDefinition[] InitializationPropertyDefinitions
		{
			get
			{
				return MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
				{
					new StorePropertyDefinition[]
					{
						MigrationEndpointMessageSchema.MigrationEndpointName,
						MigrationEndpointMessageSchema.MigrationEndpointGuid,
						MigrationEndpointMessageSchema.MigrationEndpointType
					},
					base.InitializationPropertyDefinitions
				});
			}
		}

		public override PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				PropertyDefinition[] array = new StorePropertyDefinition[]
				{
					MigrationEndpointMessageSchema.LastModifiedTime,
					MigrationEndpointMessageSchema.RemoteHostName
				};
				return MigrationHelper.AggregateProperties(new IList<PropertyDefinition>[]
				{
					base.PropertyDefinitions,
					BasicMigrationSlotProvider.PropertyDefinition,
					array
				});
			}
		}

		public Guid Guid
		{
			get
			{
				if (this.Identity == null)
				{
					return Guid.Empty;
				}
				return this.Identity.Guid;
			}
		}

		public BasicMigrationSlotProvider SlotProvider { get; private set; }

		public abstract ConnectionSettingsBase ConnectionSettings { get; }

		public abstract MigrationType PreferredMigrationType { get; }

		public virtual Fqdn RemoteServer { get; set; }

		public string EncryptedPassword
		{
			get
			{
				return base.ExtendedProperties.Get<string>("EncryptedPassword");
			}
			private set
			{
				base.ExtendedProperties.Set<string>("EncryptedPassword", value);
			}
		}

		public string Username
		{
			get
			{
				return base.ExtendedProperties.Get<string>("Username");
			}
			private set
			{
				base.ExtendedProperties.Set<string>("Username", value);
			}
		}

		public string Domain
		{
			get
			{
				return base.ExtendedProperties.Get<string>("Domain");
			}
			private set
			{
				base.ExtendedProperties.Set<string>("Domain", value);
			}
		}

		public ExDateTime LastModifiedTime { get; set; }

		public virtual PSCredential Credentials
		{
			get
			{
				return MigrationEndpointBase.BuildPSCredentials(this.Domain, this.Username, this.Password);
			}
			set
			{
				if (value != null)
				{
					ICryptoAdapter cryptoAdapter = MigrationServiceFactory.Instance.GetCryptoAdapter();
					string encryptedPassword;
					Exception ex;
					if (!cryptoAdapter.TrySecureStringToEncryptedString(value.Password, out encryptedPassword, out ex))
					{
						throw new CouldNotEncryptPasswordException(value.UserName);
					}
					this.EncryptedPassword = encryptedPassword;
					if (string.IsNullOrEmpty(value.UserName) || SmtpAddress.IsValidSmtpAddress(value.UserName))
					{
						this.Username = value.UserName;
						this.Domain = null;
					}
					else
					{
						NetworkCredential networkCredential = null;
						try
						{
							networkCredential = value.GetNetworkCredential();
						}
						catch (ArgumentException innerException)
						{
							throw new CouldNotCreateCredentialsPermanentException(this.Username, innerException);
						}
						this.Domain = networkCredential.Domain;
						this.Username = networkCredential.UserName;
					}
					if (MigrationUtil.HasUnicodeCharacters(this.Username) || MigrationUtil.HasUnicodeCharacters(this.Domain) || MigrationUtil.HasUnicodeCharacters(value.Password.AsUnsecureString()))
					{
						throw new CannotSpecifyUnicodeInCredentialsException();
					}
				}
				else
				{
					this.Username = null;
					this.EncryptedPassword = null;
					this.Domain = null;
				}
			}
		}

		public SecureString Password
		{
			get
			{
				ICryptoAdapter cryptoAdapter = MigrationServiceFactory.Instance.GetCryptoAdapter();
				string encryptedPassword = this.EncryptedPassword;
				if (string.IsNullOrEmpty(encryptedPassword))
				{
					return null;
				}
				SecureString result;
				Exception innerException;
				if (!cryptoAdapter.TryEncryptedStringToSecureString(this.EncryptedPassword, out result, out innerException))
				{
					throw new CouldNotEncryptPasswordException(this.Username, innerException);
				}
				return result;
			}
		}

		public NetworkCredential NetworkCredentials
		{
			get
			{
				return CommonUtils.GetNetworkCredential(this.Credentials, null);
			}
		}

		public AuthenticationMethod AuthenticationMethod
		{
			get
			{
				return base.ExtendedProperties.Get<AuthenticationMethod>("AuthenticationMethod", this.DefaultAuthenticationMethod);
			}
			set
			{
				this.ValidateAuthenticationMethod(value);
				base.ExtendedProperties.Set<AuthenticationMethod>("AuthenticationMethod", value);
			}
		}

		internal AutodiscoverClientResponse AutodiscoverResponse { get; private set; }

		protected virtual IEnumerable<AuthenticationMethod> SupportedAuthenticationMethods
		{
			get
			{
				AuthenticationMethod[] array = new AuthenticationMethod[2];
				array[0] = AuthenticationMethod.Ntlm;
				return array;
			}
		}

		protected AuthenticationMethod DefaultAuthenticationMethod
		{
			get
			{
				return AuthenticationMethod.Basic;
			}
		}

		public static implicit operator MigrationEndpointBase(MigrationEndpoint endpoint)
		{
			return MigrationEndpointBase.CreateFrom(endpoint);
		}

		public static implicit operator MigrationEndpoint(MigrationEndpointBase endpoint)
		{
			return endpoint.ToMigrationEndpoint();
		}

		public static PSCredential BuildPSCredentials(string domain, string username, SecureString password)
		{
			if (password == null)
			{
				return null;
			}
			string userName;
			if (string.IsNullOrEmpty(username) || SmtpAddress.IsValidSmtpAddress(username))
			{
				userName = username;
			}
			else if (domain == null)
			{
				userName = username;
			}
			else
			{
				userName = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", new object[]
				{
					domain,
					username
				});
			}
			return new PSCredential(userName, password);
		}

		public virtual void InitializeFromAutoDiscover(SmtpAddress emailAddress, PSCredential credentials)
		{
			MigrationUtil.ThrowOnNullArgument(emailAddress, "emailAddress");
			MigrationUtil.ThrowOnNullArgument(credentials, "credentials");
			IMigrationAutodiscoverClient autodiscoverClient = MigrationServiceFactory.Instance.GetAutodiscoverClient();
			MigrationUtil.AssertOrThrow(autodiscoverClient != null, "MigrationServiceFactory should never return a null AutodClient", new object[0]);
			string emailAddress2 = emailAddress.ToString();
			string encryptedPassword;
			Exception ex;
			if (!MigrationServiceFactory.Instance.GetCryptoAdapter().TrySecureStringToEncryptedString(credentials.Password, out encryptedPassword, out ex))
			{
				throw new CouldNotEncryptPasswordException(credentials.UserName);
			}
			this.Credentials = credentials;
			NetworkCredential networkCredentials = this.NetworkCredentials;
			AutodiscoverClientResponse userSettings = autodiscoverClient.GetUserSettings(networkCredentials.UserName, encryptedPassword, networkCredentials.Domain, emailAddress2);
			switch (userSettings.Status)
			{
			case AutodiscoverClientStatus.NoError:
				this.ApplyAutodiscoverSettings(userSettings);
				this.AutodiscoverResponse = userSettings;
				return;
			case AutodiscoverClientStatus.ConfigurationError:
			{
				AutoDiscoverFailedConfigurationErrorException ex2 = new AutoDiscoverFailedConfigurationErrorException(userSettings.ErrorMessage);
				ex2.Data["AutoDiscoverResponseMessage"] = userSettings.ErrorMessage;
				ex2.Data["AutoDiscoverResponseErrorDetail"] = userSettings.ErrorDetail;
				throw ex2;
			}
			case AutodiscoverClientStatus.InternalError:
			{
				AutoDiscoverFailedInternalErrorException ex3 = new AutoDiscoverFailedInternalErrorException(userSettings.ErrorMessage);
				ex3.Data["AutoDiscoverResponseMessage"] = userSettings.ErrorMessage;
				ex3.Data["AutoDiscoverResponseErrorDetail"] = userSettings.ErrorDetail;
				throw ex3;
			}
			default:
				return;
			}
		}

		public override void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			message[StoreObjectSchema.ItemClass] = "IPM.MS-Exchange.MigrationEndpoint";
			message[MigrationEndpointMessageSchema.MigrationEndpointType] = this.EndpointType;
			message[MigrationEndpointMessageSchema.MigrationEndpointName] = this.Identity.Id;
			message[MigrationEndpointMessageSchema.MigrationEndpointGuid] = this.Guid;
			if (this.RemoteServer != null)
			{
				message[MigrationEndpointMessageSchema.RemoteHostName] = this.RemoteServer.ToString();
			}
			MigrationHelperBase.SetExDateTimeProperty(message, MigrationEndpointMessageSchema.LastModifiedTime, (this.LastModifiedTime == ExDateTime.MinValue) ? null : new ExDateTime?(this.LastModifiedTime));
			this.SlotProvider.WriteToMessageItem(message);
			base.WriteToMessageItem(message, loaded);
		}

		public MigrationEndpoint ToMigrationEndpoint()
		{
			MigrationEndpoint migrationEndpoint = new MigrationEndpoint();
			migrationEndpoint.Identity = this.Identity;
			migrationEndpoint.MaxConcurrentMigrations = this.SlotProvider.MaximumConcurrentMigrations;
			migrationEndpoint.MaxConcurrentIncrementalSyncs = this.SlotProvider.MaximumConcurrentIncrementalSyncs;
			migrationEndpoint.EndpointType = this.EndpointType;
			migrationEndpoint.LastModifiedTime = (DateTime)this.LastModifiedTime;
			migrationEndpoint.ActiveMigrationCount = new int?(this.SlotProvider.ActiveMigrationCount);
			if (this.EndpointType != MigrationType.IMAP)
			{
				migrationEndpoint.ActiveIncrementalSyncCount = new int?(this.SlotProvider.ActiveIncrementalSyncCount);
			}
			this.ApplyAdditionalProperties(migrationEndpoint);
			return migrationEndpoint;
		}

		public override bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			this.SlotProvider = BasicMigrationSlotProvider.FromMessageItem(this.Guid, message);
			this.RemoteServer = MigrationHelper.GetFqdnProperty(message, MigrationEndpointMessageSchema.RemoteHostName, true);
			this.LastModifiedTime = MigrationHelper.GetExDateTimePropertyOrDefault(message, MigrationEndpointMessageSchema.LastModifiedTime, ExDateTime.MinValue);
			return base.ReadFromMessageItem(message);
		}

		public virtual void VerifyConnectivity()
		{
			throw new NotImplementedException(string.Format("Verify connectivity not implemented for endpoints of type {0}", this.EndpointType));
		}

		public virtual NspiMigrationDataReader GetNspiDataReader(MigrationJob job = null)
		{
			throw new NspiNotSupportedForEndpointTypeException(this.EndpointType);
		}

		public override XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument)
		{
			XElement xelement = new XElement("MigrationEndpoint");
			xelement.Add(new XElement("Type", this.EndpointType));
			xelement.Add(new XElement("Name", this.Identity.Id));
			xelement.Add(new XElement("GUID", this.Guid));
			xelement.Add(new XElement("LastModifiedTime", this.LastModifiedTime));
			xelement.Add(new XElement("RemoteServer", this.RemoteServer));
			if (this.SlotProvider != null)
			{
				xelement.Add(this.SlotProvider.GetDiagnosticInfo(dataProvider, argument));
			}
			this.AddDiagnosticInfoToElement(dataProvider, xelement, argument);
			return base.GetDiagnosticInfo(dataProvider, argument, xelement);
		}

		internal static void Create(IMigrationDataProvider migrationDataProvider, MigrationEndpointBase dataObject)
		{
			if (dataObject.Identity == null)
			{
				throw new ArgumentException("Data object must have an identity assigned before being persisted.", "dataObject");
			}
			using (IMigrationDataProvider providerForFolder = migrationDataProvider.GetProviderForFolder(MigrationFolderName.Settings))
			{
				IEnumerable<MigrationEndpointBase> source = MigrationEndpointBase.Get(dataObject.Identity, migrationDataProvider, false);
				if (source.Any<MigrationEndpointBase>())
				{
					throw new MigrationEndpointDuplicatedException(dataObject.Identity.Id);
				}
				MigrationEndpointBase.VerifyConcurrencyLimits(migrationDataProvider, dataObject);
				dataObject.Identity = new MigrationEndpointId(dataObject.Identity.Id, Guid.NewGuid());
				dataObject.CreateInStore(providerForFolder, null);
			}
			MigrationEndpointLog.LogStatusEvent(dataObject.ToMigrationEndpoint(), MigrationEndpointLog.EndpointState.Created);
		}

		internal static MigrationEndpointBase CreateFrom(MigrationEndpoint presentationObject)
		{
			if (presentationObject == null)
			{
				return null;
			}
			MigrationType endpointType = presentationObject.EndpointType;
			if (endpointType <= MigrationType.ExchangeOutlookAnywhere)
			{
				if (endpointType == MigrationType.IMAP)
				{
					return new ImapEndpoint(presentationObject);
				}
				if (endpointType == MigrationType.ExchangeOutlookAnywhere)
				{
					return new ExchangeOutlookAnywhereEndpoint(presentationObject);
				}
			}
			else
			{
				if (endpointType == MigrationType.ExchangeRemoteMove)
				{
					return new ExchangeRemoteMoveEndpoint(presentationObject);
				}
				if (endpointType == MigrationType.PSTImport)
				{
					return new PSTImportEndpoint(presentationObject);
				}
				if (endpointType == MigrationType.PublicFolder)
				{
					return new PublicFolderEndpoint(presentationObject);
				}
			}
			throw new ArgumentException("Endpoint doesn't have a supported type.", "presentationObject");
		}

		internal static IEnumerable<MigrationEndpointBase> Get(QueryFilter filter, IMigrationDataProvider dataProvider, bool ignoreNotFoundErrors)
		{
			MigrationUtil.ThrowOnNullArgument(filter, "filter");
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			return MigrationEndpointBase.Get(dataProvider, ignoreNotFoundErrors, (IMigrationDataProvider endpointDataProvider) => endpointDataProvider.FindMessageIds(filter, null, new SortBy[]
			{
				new SortBy(MigrationEndpointMessageSchema.MigrationEndpointType, SortOrder.Ascending),
				new SortBy(MigrationEndpointMessageSchema.MigrationEndpointName, SortOrder.Ascending)
			}, (IDictionary<PropertyDefinition, object> row) => MigrationRowSelectorResult.AcceptRow, null));
		}

		internal static IEnumerable<MigrationEndpointBase> Get(MigrationEqualityFilter primaryFilter, IMigrationDataProvider dataProvider, bool ignoreNotFoundErrors)
		{
			MigrationUtil.ThrowOnNullArgument(primaryFilter, "primaryFilter");
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			return MigrationEndpointBase.Get(dataProvider, ignoreNotFoundErrors, (IMigrationDataProvider endpointDataProvider) => endpointDataProvider.FindMessageIds(primaryFilter, null, null, (IDictionary<PropertyDefinition, object> row) => MigrationRowSelectorResult.AcceptRow, null));
		}

		internal static IEnumerable<MigrationEndpointBase> Get(MigrationEndpointId migrationEndpointId, IMigrationDataProvider dataProvider, bool ignoreNotFoundErrors)
		{
			QueryFilter filter = (migrationEndpointId ?? MigrationEndpointId.Any).GetFilter();
			return MigrationEndpointBase.Get(filter, dataProvider, ignoreNotFoundErrors);
		}

		internal static MigrationEndpointBase Get(Guid endpointId, IMigrationDataProvider dataProvider)
		{
			List<MigrationEndpointBase> source = MigrationEndpointBase.Get(new MigrationEndpointId(string.Empty, endpointId), dataProvider, false).ToList<MigrationEndpointBase>();
			return source.FirstOrDefault<MigrationEndpointBase>();
		}

		internal static IEnumerable<MigrationEndpointBase> Get(MigrationType endpointType, IMigrationDataProvider dataProvider)
		{
			return MigrationEndpointBase.Get(MigrationEndpointDataProvider.GetFilterFromEndpointType(endpointType), dataProvider, true);
		}

		internal static MigrationType GetMigrationType(MigrationEndpoint sourceEndpoint, MigrationEndpoint targetEndpoint)
		{
			MigrationEndpointBase migrationEndpointBase = MigrationEndpointBase.CreateFrom(sourceEndpoint);
			MigrationEndpointBase migrationEndpointBase2 = MigrationEndpointBase.CreateFrom(targetEndpoint);
			if (migrationEndpointBase2 == null)
			{
				if (migrationEndpointBase != null)
				{
					return migrationEndpointBase.PreferredMigrationType;
				}
				return MigrationType.ExchangeLocalMove;
			}
			else
			{
				if (migrationEndpointBase == null)
				{
					return migrationEndpointBase2.PreferredMigrationType;
				}
				return migrationEndpointBase.ComputePreferredProtocol(migrationEndpointBase2);
			}
		}

		internal static void Delete(MigrationEndpointId endpointId, IMigrationDataProvider dataProvider)
		{
			using (IMigrationDataProvider providerForFolder = dataProvider.GetProviderForFolder(MigrationFolderName.Settings))
			{
				string[] array = (from job in MigrationJob.GetByEndpoint(dataProvider, endpointId)
				select job.JobName).ToArray<string>();
				if (array.Length > 0)
				{
					string batches = string.Join(", ", array);
					throw new CannotRemoveEndpointWithAssociatedBatchesException(endpointId.ToString(), batches);
				}
				try
				{
					MigrationEndpoint migrationEndpoint = MigrationEndpointBase.Get(endpointId.Guid, dataProvider).ToMigrationEndpoint();
					migrationEndpoint.LastModifiedTime = (DateTime)ExDateTime.UtcNow;
					MigrationEndpointLog.LogStatusEvent(migrationEndpoint, MigrationEndpointLog.EndpointState.Deleted);
				}
				catch (ObjectNotFoundException)
				{
				}
				PropertyDefinition[] properties = new StorePropertyDefinition[]
				{
					MigrationEndpointMessageSchema.MigrationEndpointType
				};
				IEnumerable<StoreObjectId> enumerable = providerForFolder.FindMessageIds(endpointId.GetFilter(), properties, new SortBy[]
				{
					new SortBy(MigrationEndpointMessageSchema.MigrationEndpointName, SortOrder.Ascending)
				}, (IDictionary<PropertyDefinition, object> row) => MigrationRowSelectorResult.AcceptRow, null);
				foreach (StoreObjectId messageId in enumerable)
				{
					providerForFolder.RemoveMessage(messageId);
				}
			}
		}

		internal static void UpdateEndpoint(MigrationEndpoint presentationObject, IMigrationDataProvider dataProvider)
		{
			using (IMigrationDataProvider providerForFolder = dataProvider.GetProviderForFolder(MigrationFolderName.Settings))
			{
				MigrationEndpointBase migrationEndpointBase = MigrationEndpointBase.Get(presentationObject.Identity, dataProvider, false).FirstOrDefault<MigrationEndpointBase>();
				if (migrationEndpointBase == null)
				{
					throw new ArgumentException("Endpoint to update must be persisted already.");
				}
				migrationEndpointBase.InitializeFromPresentationObject(presentationObject);
				MigrationEndpointBase.VerifyConcurrencyLimits(dataProvider, migrationEndpointBase);
				using (IMigrationMessageItem migrationMessageItem = migrationEndpointBase.FindMessageItem(providerForFolder, migrationEndpointBase.PropertyDefinitions))
				{
					migrationMessageItem.OpenAsReadWrite();
					migrationEndpointBase.WriteToMessageItem(migrationMessageItem, true);
					migrationMessageItem.Save(SaveMode.NoConflictResolution);
				}
				MigrationEndpointLog.LogStatusEvent(presentationObject, MigrationEndpointLog.EndpointState.Updated);
			}
		}

		internal virtual MigrationType ComputePreferredProtocol(MigrationEndpointBase targetEndpointData)
		{
			throw new NotImplementedException();
		}

		protected virtual void AddDiagnosticInfoToElement(IMigrationDataProvider dataProvider, XElement parent, MigrationDiagnosticArgument argument)
		{
		}

		protected virtual void ApplyAutodiscoverSettings(AutodiscoverClientResponse response)
		{
			throw new NotImplementedException();
		}

		protected virtual void ValidateAuthenticationMethod(AuthenticationMethod authenticationMethod)
		{
			if (!this.SupportedAuthenticationMethods.Contains(authenticationMethod))
			{
				string validValues = string.Join<AuthenticationMethod>(",", this.SupportedAuthenticationMethods);
				throw new AuthenticationMethodNotSupportedException(authenticationMethod.ToString(), this.PreferredMigrationType.ToString(), validValues);
			}
		}

		protected virtual void ApplyAdditionalProperties(MigrationEndpoint presentationObject)
		{
			presentationObject.RemoteServer = this.RemoteServer;
			presentationObject.Credentials = this.Credentials;
		}

		protected override bool InitializeFromMessageItem(IMigrationStoreObject message)
		{
			if (!base.InitializeFromMessageItem(message))
			{
				return false;
			}
			Guid guidProperty = MigrationHelper.GetGuidProperty(message, MigrationEndpointMessageSchema.MigrationEndpointGuid, true);
			this.Identity = new MigrationEndpointId((string)message[MigrationEndpointMessageSchema.MigrationEndpointName], guidProperty);
			MigrationType valueOrDefault = message.GetValueOrDefault<MigrationType>(MigrationEndpointMessageSchema.MigrationEndpointType, MigrationType.None);
			if (valueOrDefault != this.EndpointType)
			{
				throw new UnexpectedMigrationTypeException(valueOrDefault.ToString(), this.EndpointType.ToString());
			}
			return true;
		}

		protected virtual void InitializeFromPresentationObject(MigrationEndpoint endpoint)
		{
			this.SlotProvider = BasicMigrationSlotProvider.Get(this.Guid, endpoint.MaxConcurrentMigrations, endpoint.MaxConcurrentIncrementalSyncs);
			this.EndpointType = endpoint.EndpointType;
			this.Identity = endpoint.Identity;
			this.RemoteServer = endpoint.RemoteServer;
			this.Credentials = endpoint.Credentials;
			this.AuthenticationMethod = (endpoint.Authentication ?? this.DefaultAuthenticationMethod);
			this.LastModifiedTime = (ExDateTime)endpoint.LastModifiedTime;
		}

		private static IEnumerable<MigrationEndpointBase> Get(IMigrationDataProvider dataProvider, bool ignoreNotFoundErrors, Func<IMigrationDataProvider, IEnumerable<StoreObjectId>> messageIdFinder)
		{
			MigrationUtil.ThrowOnNullArgument(messageIdFinder, "messageIdFinder");
			MigrationUtil.ThrowOnNullArgument(dataProvider, "dataProvider");
			using (IMigrationDataProvider endpointDataProvider = dataProvider.GetProviderForFolder(MigrationFolderName.Settings))
			{
				IEnumerable<StoreObjectId> messageIds = messageIdFinder(endpointDataProvider);
				PropertyDefinition[] propertiesToLoad = new StorePropertyDefinition[]
				{
					MigrationEndpointMessageSchema.MigrationEndpointType,
					MigrationBatchMessageSchema.MigrationVersion
				};
				foreach (StoreObjectId endpointMessageId in messageIds)
				{
					MigrationEndpointBase endpoint = null;
					try
					{
						using (IMigrationMessageItem migrationMessageItem = endpointDataProvider.FindMessage(endpointMessageId, propertiesToLoad))
						{
							endpoint = MigrationEndpointBase.CreateEndpointFromMessage(endpointDataProvider, migrationMessageItem);
						}
					}
					catch (ObjectNotFoundException exception)
					{
						if (!ignoreNotFoundErrors)
						{
							throw;
						}
						MigrationLogger.Log(MigrationEventType.Error, exception, "Encountered an object not found exception when loading a MigrationEndpoint from a message - likely due to a race condition.", new object[0]);
					}
					if (endpoint != null)
					{
						yield return endpoint;
					}
				}
			}
			yield break;
		}

		private static MigrationEndpointBase CreateEndpointFromMessage(IMigrationDataProvider endpointDataProvider, IMigrationMessageItem message)
		{
			MigrationType valueOrDefault = message.GetValueOrDefault<MigrationType>(MigrationEndpointMessageSchema.MigrationEndpointType, MigrationType.None);
			MigrationType migrationType = valueOrDefault;
			MigrationEndpointBase migrationEndpointBase;
			if (migrationType <= MigrationType.ExchangeOutlookAnywhere)
			{
				if (migrationType == MigrationType.IMAP)
				{
					migrationEndpointBase = new ImapEndpoint();
					goto IL_6F;
				}
				if (migrationType == MigrationType.ExchangeOutlookAnywhere)
				{
					migrationEndpointBase = new ExchangeOutlookAnywhereEndpoint();
					goto IL_6F;
				}
			}
			else
			{
				if (migrationType == MigrationType.ExchangeRemoteMove)
				{
					migrationEndpointBase = new ExchangeRemoteMoveEndpoint();
					goto IL_6F;
				}
				if (migrationType == MigrationType.PSTImport)
				{
					migrationEndpointBase = new PSTImportEndpoint();
					goto IL_6F;
				}
				if (migrationType == MigrationType.PublicFolder)
				{
					migrationEndpointBase = new PublicFolderEndpoint();
					goto IL_6F;
				}
			}
			throw new MigrationDataCorruptionException("Invalid endpoint type: " + valueOrDefault);
			IL_6F:
			if (!migrationEndpointBase.TryLoad(endpointDataProvider, message.Id))
			{
				throw new CouldNotLoadMigrationPersistedItemTransientException(message.Id.ToHexEntryId());
			}
			return migrationEndpointBase;
		}

		private static void VerifyConcurrencyLimits(IMigrationDataProvider dataProvider, MigrationEndpointBase incomingEndpoint)
		{
			MigrationSession migrationSession = MigrationSession.Get(dataProvider, false);
			Unlimited<int> maxConcurrentMigrations = migrationSession.MaxConcurrentMigrations;
			if (maxConcurrentMigrations.IsUnlimited)
			{
				return;
			}
			Unlimited<int> value = incomingEndpoint.SlotProvider.MaximumConcurrentMigrations;
			if (value.IsUnlimited)
			{
				throw new MaximumConcurrentMigrationLimitExceededException(value.ToString(), maxConcurrentMigrations.ToString(), incomingEndpoint.EndpointType.ToString());
			}
			foreach (MigrationEndpointBase migrationEndpointBase in MigrationEndpointBase.Get(incomingEndpoint.EndpointType, dataProvider))
			{
				if (!migrationEndpointBase.Identity.Equals(incomingEndpoint.Identity))
				{
					if (migrationEndpointBase.SlotProvider.MaximumConcurrentMigrations.IsUnlimited)
					{
						value = Unlimited<int>.UnlimitedValue;
						break;
					}
					value += migrationEndpointBase.SlotProvider.MaximumConcurrentMigrations;
				}
			}
			if (value > maxConcurrentMigrations)
			{
				throw new MaximumConcurrentMigrationLimitExceededException(value.ToString(), maxConcurrentMigrations.ToString(), incomingEndpoint.EndpointType.ToString());
			}
		}

		private const long MinimumEndpointVersion = 4L;

		private const long MigrationTypeEndpointVersion = 5L;

		private const long MaximumEndpointVersion = 5L;
	}
}
