using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks.MailboxSearch;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("New", "MailboxSearch", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class NewMailboxSearch : NewTenantADTaskBase<MailboxDiscoverySearch>
	{
		[Parameter(Mandatory = true, Position = 0)]
		public string Name
		{
			get
			{
				return this.objectToSave.Name;
			}
			set
			{
				this.objectToSave.Name = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public RecipientIdParameter[] SourceMailboxes
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["SourceMailboxes"];
			}
			set
			{
				base.Fields["SourceMailboxes"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxIdParameter TargetMailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["TargetMailbox"];
			}
			set
			{
				base.Fields["TargetMailbox"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string SearchQuery
		{
			get
			{
				return this.objectToSave.Query;
			}
			set
			{
				this.objectToSave.Query = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CultureInfo Language
		{
			get
			{
				return (CultureInfo)(base.Fields["Language"] ?? CultureInfo.CurrentCulture);
			}
			set
			{
				base.Fields["Language"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public PublicFolderIdParameter[] PublicFolderSources
		{
			get
			{
				return (PublicFolderIdParameter[])base.Fields["PublicFolderSources"];
			}
			set
			{
				base.Fields["PublicFolderSources"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllPublicFolderSources
		{
			get
			{
				return (bool)(base.Fields["AllPublicFolderSources"] ?? false);
			}
			set
			{
				base.Fields["AllPublicFolderSources"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllSourceMailboxes
		{
			get
			{
				return (bool)(base.Fields["AllSourceMailboxes"] ?? false);
			}
			set
			{
				base.Fields["AllSourceMailboxes"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] Senders
		{
			get
			{
				return (string[])base.Fields["Senders"];
			}
			set
			{
				base.Fields["Senders"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] Recipients
		{
			get
			{
				return (string[])base.Fields["Recipients"];
			}
			set
			{
				base.Fields["Recipients"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExDateTime? StartDate
		{
			get
			{
				return this.objectToSave.StartDate;
			}
			set
			{
				this.objectToSave.StartDate = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExDateTime? EndDate
		{
			get
			{
				return this.objectToSave.EndDate;
			}
			set
			{
				this.objectToSave.EndDate = value;
			}
		}

		[Parameter(Mandatory = false)]
		public KindKeyword[] MessageTypes
		{
			get
			{
				return (KindKeyword[])base.Fields["MessageTypes"];
			}
			set
			{
				base.Fields["MessageTypes"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LoggingLevel LogLevel
		{
			get
			{
				object obj;
				if ((obj = base.Fields["LogLevel"]) == null)
				{
					obj = ((!this.EstimateOnly) ? LoggingLevel.Basic : LoggingLevel.Suppress);
				}
				return (LoggingLevel)obj;
			}
			set
			{
				base.Fields["LogLevel"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] StatusMailRecipients
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["StatusMailRecipients"];
			}
			set
			{
				base.Fields["StatusMailRecipients"] = value;
			}
		}

		internal RecipientIdParameter[] ManagedBy
		{
			get
			{
				return (RecipientIdParameter[])base.Fields["ManagedBy"];
			}
			set
			{
				base.Fields["ManagedBy"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeUnsearchableItems
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeUnsearchableItems"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IncludeUnsearchableItems"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter EstimateOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["EstimateOnly"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["EstimateOnly"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ExcludeDuplicateMessages
		{
			get
			{
				return (bool)(base.Fields["ExcludeDuplicateMessages"] ?? (!this.EstimateOnly));
			}
			set
			{
				base.Fields["ExcludeDuplicateMessages"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeKeywordStatistics
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeKeywordStatistics"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IncludeKeywordStatistics"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool InPlaceHoldEnabled
		{
			get
			{
				return (bool)(base.Fields["InPlaceHoldEnabled"] ?? false);
			}
			set
			{
				base.Fields["InPlaceHoldEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<EnhancedTimeSpan> ItemHoldPeriod
		{
			get
			{
				return (Unlimited<EnhancedTimeSpan>)(base.Fields["ItemHoldPeriod"] ?? Unlimited<EnhancedTimeSpan>.UnlimitedValue);
			}
			set
			{
				base.Fields["ItemHoldPeriod"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Description
		{
			get
			{
				return (string)base.Fields["Description"];
			}
			set
			{
				base.Fields["Description"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string InPlaceHoldIdentity
		{
			get
			{
				return (string)base.Fields["InPlaceHoldIdentity"];
			}
			set
			{
				base.Fields["InPlaceHoldIdentity"] = value;
			}
		}

		private ADObjectId ADObjectIdFromRecipientIdParameter(RecipientIdParameter recipientId, object param)
		{
			ADRecipient adrecipient = base.GetDataObject<ADRecipient>(recipientId, this.recipientSession, null, new LocalizedString?(Strings.ExceptionUserObjectNotFound(recipientId.ToString())), new LocalizedString?(Strings.ExceptionUserObjectAmbiguous)) as ADRecipient;
			if (param != null)
			{
				RecipientType[] array = param as RecipientType[];
				foreach (RecipientType recipientType in array)
				{
					if (adrecipient.RecipientType == recipientType || RemoteMailbox.IsRemoteMailbox(adrecipient.RecipientTypeDetails))
					{
						return adrecipient.Id;
					}
				}
				base.WriteError(new MailboxSearchTaskException(Strings.ErrorInvalidRecipientType(adrecipient.ToString(), adrecipient.RecipientType.ToString())), ErrorCategory.InvalidArgument, null);
			}
			return adrecipient.Id;
		}

		private void PreSaveValidate(MailboxDiscoverySearch savedObject)
		{
			if (this.ExcludeDuplicateMessages && this.EstimateOnly)
			{
				base.WriteError(new MailboxSearchTaskException(Strings.ExcludeDuplicateMessagesParameterConflict), ErrorCategory.InvalidArgument, null);
			}
			if (savedObject.StatisticsOnly && savedObject.LogLevel != LoggingLevel.Suppress)
			{
				base.WriteError(new MailboxSearchTaskException(Strings.EstimateOnlyLogLevelParameterConflict(savedObject.LogLevel.ToString())), ErrorCategory.InvalidArgument, "LogLevel");
			}
			if (!savedObject.StatisticsOnly && savedObject.IncludeKeywordStatistics)
			{
				base.WriteError(new MailboxSearchTaskException(Strings.IncludeKeywordStatisticsParameterConflict), ErrorCategory.InvalidArgument, null);
			}
			if (base.ScopeSet != null)
			{
				foreach (string legacyExchangeDN in savedObject.Sources)
				{
					ADRecipient adrecipient = this.recipientSession.FindByLegacyExchangeDN(legacyExchangeDN);
					if (adrecipient == null)
					{
						base.WriteError(new ObjectNotFoundException(Strings.ExceptionSourceMailboxNotFound(savedObject.Target, savedObject.Name)), ErrorCategory.InvalidOperation, null);
					}
					Utils.VerifyIsInHoldScopes(savedObject.InPlaceHoldEnabled, base.ExchangeRunspaceConfig, adrecipient, "New-MailboxSearch", new Task.TaskErrorLoggingDelegate(base.WriteError));
					Utils.VerifyIsInScopes(adrecipient, base.ScopeSet, new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
			}
			if (this.TargetMailbox != null)
			{
				try
				{
					this.recipientSession.SessionSettings.IncludeInactiveMailbox = false;
					ADRecipient adrecipient2 = this.recipientSession.FindByLegacyExchangeDN(savedObject.Target);
					if (adrecipient2 == null)
					{
						base.WriteError(new ObjectNotFoundException(Strings.ExceptionTargetMailboxNotFound(savedObject.Target, savedObject.Name)), ErrorCategory.InvalidOperation, null);
					}
					Utils.VerifyMailboxVersion(adrecipient2, new Task.TaskErrorLoggingDelegate(base.WriteError));
					if (base.ScopeSet != null)
					{
						Utils.VerifyIsInScopes(adrecipient2, base.ScopeSet, new Task.TaskErrorLoggingDelegate(base.WriteError));
					}
				}
				finally
				{
					if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
					{
						this.recipientSession.SessionSettings.IncludeInactiveMailbox = true;
					}
				}
			}
			if (!string.IsNullOrEmpty(this.InPlaceHoldIdentity))
			{
				Guid guid;
				if (Guid.TryParse(this.InPlaceHoldIdentity, out guid))
				{
					MailboxDiscoverySearch mailboxDiscoverySearch = ((DiscoverySearchDataProvider)base.DataSession).FindByInPlaceHoldIdentity(guid.ToString("N"));
					if (mailboxDiscoverySearch != null)
					{
						base.WriteError(new MailboxSearchTaskException(Strings.MailboxSearchInPlaceHoldIdentityExists(this.InPlaceHoldIdentity)), ErrorCategory.InvalidArgument, mailboxDiscoverySearch);
					}
				}
				else
				{
					base.WriteError(new MailboxSearchTaskException(Strings.MailboxSearchInPlaceHoldFormatError(this.InPlaceHoldIdentity)), ErrorCategory.InvalidArgument, savedObject);
				}
			}
			if (Utils.SameNameExists(savedObject.Name, (DiscoverySearchDataProvider)base.DataSession, Utils.GetMailboxDataProvider(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, new Task.TaskErrorLoggingDelegate(base.WriteError))))
			{
				base.WriteError(new MailboxSearchTaskException(Strings.MailboxSearchObjectExist(savedObject.Name)), ErrorCategory.InvalidArgument, savedObject);
			}
			Exception ex = Utils.ValidateSourceAndTargetMailboxes((DiscoverySearchDataProvider)base.DataSession, savedObject);
			if (ex != null)
			{
				base.WriteError(ex, ErrorCategory.InvalidArgument, null);
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmNewMailboxSearchTask(this.Name);
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession recipientSession = Utils.CreateRecipientSession(base.DomainController, base.SessionSettings);
			this.recipientSession = recipientSession;
			return new DiscoverySearchDataProvider(base.CurrentOrganizationId);
		}

		protected override IConfigurable PrepareDataObject()
		{
			if (this.SourceMailboxes != null)
			{
				this.sourceMailboxIdParameters.AddRange(this.SourceMailboxes);
			}
			return base.PrepareDataObject();
		}

		protected override void InternalProcessRecord()
		{
		}

		protected override void InternalEndProcessing()
		{
			TaskLogger.LogEnter();
			this.PrepareDataObjectToSave();
			if (this.objectToSave != null)
			{
				try
				{
					this.PreSaveValidate(this.objectToSave);
					if (base.HasErrors)
					{
						return;
					}
					if (this.objectToSave.Identity != null)
					{
						base.WriteVerbose(TaskVerboseStringHelper.GetSaveObjectVerboseString(this.objectToSave, base.DataSession, typeof(MailboxDiscoverySearch)));
					}
					((DiscoverySearchDataProvider)base.DataSession).CreateOrUpdate<MailboxDiscoverySearch>(this.objectToSave);
					SearchEventLogger.Instance.LogDiscoveryAndHoldSavedEvent(this.objectToSave);
					this.DataObject = this.objectToSave;
					if (this.InPlaceHoldEnabled && this.sourceMailboxIdParameters != null && this.sourceMailboxIdParameters.Count > 0)
					{
						LocalizedString localizedString = this.objectToSave.SynchronizeHoldSettings((DiscoverySearchDataProvider)base.DataSession, this.recipientSession, true, delegate(int percentage)
						{
							base.WriteProgress(Strings.NewMailboxSearchActivity, Strings.ApplyingHoldSettings(this.objectToSave.Name), percentage);
						});
						if (localizedString != LocalizedString.Empty)
						{
							base.WriteError(new MailboxSearchTaskException(localizedString), ErrorCategory.InvalidOperation, this);
						}
						this.WriteWarning(Strings.WarningDiscoveryHoldDelay(COWSettings.COWCacheLifeTime.TotalMinutes));
					}
					if (!base.HasErrors)
					{
						this.WriteResult();
					}
				}
				catch (ObjectNotFoundException exception)
				{
					base.WriteError(exception, ErrorCategory.ObjectNotFound, null);
				}
				catch (DataSourceTransientException exception2)
				{
					base.WriteError(exception2, ErrorCategory.WriteError, null);
				}
				catch (ArgumentException exception3)
				{
					base.WriteError(exception3, ErrorCategory.WriteError, null);
				}
				catch (StorageTransientException innerException)
				{
					base.WriteError(new TaskException(Strings.ErrorMailboxSearchStorageTransient, innerException), ErrorCategory.WriteError, null);
				}
				catch (StoragePermanentException innerException2)
				{
					base.WriteError(new TaskException(Strings.ErrorMailboxSearchStoragePermanent, innerException2), ErrorCategory.WriteError, null);
				}
				finally
				{
					if (this.objectToSave.Identity != null)
					{
						base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(base.DataSession));
					}
				}
			}
			base.InternalEndProcessing();
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable result)
		{
			ADSessionSettingsFactory.RunWithInactiveMailboxVisibilityEnablerForDatacenter(delegate
			{
				MailboxDiscoverySearch discoverySearch = result as MailboxDiscoverySearch;
				this.<>n__FabricatedMethod5(new MailboxSearchObject(discoverySearch, ((DiscoverySearchDataProvider)this.DataSession).OrganizationId));
			});
		}

		private void PrepareDataObjectToSave()
		{
			if (base.ExchangeRunspaceConfig == null)
			{
				base.ThrowTerminatingError(new MailboxSearchTaskException(Strings.UnableToDetermineExecutingUser), ErrorCategory.InvalidOperation, null);
			}
			if (!(base.DataSession is DiscoverySearchDataProvider))
			{
				base.ThrowTerminatingError(new MailboxSearchTaskException(Strings.UnableToDetermineCreatingUser), ErrorCategory.InvalidOperation, null);
			}
			string[] paramNames = new string[]
			{
				"AllSourceMailboxes",
				"AllPublicFolderSources",
				"PublicFolderSources"
			};
			ScopeSet scopeSet = base.ScopeSet;
			bool flag = false;
			if (scopeSet == null)
			{
				scopeSet = ScopeSet.GetOrgWideDefaultScopeSet(base.CurrentOrganizationId);
			}
			if (scopeSet != null)
			{
				ADRawEntry executingUser = base.ExchangeRunspaceConfig.ExecutingUser;
				if (executingUser != null)
				{
					flag = base.ExchangeRunspaceConfig.IsCmdletAllowedInScope("New-MailboxSearch", paramNames, executingUser, ScopeLocation.RecipientWrite);
				}
				else
				{
					flag = base.ExchangeRunspaceConfig.IsCmdletAllowedInScope("New-MailboxSearch", paramNames, scopeSet);
				}
			}
			ADUser aduser = null;
			if (base.Fields["TargetMailbox"] != null)
			{
				aduser = (ADUser)base.GetDataObject<ADUser>(this.TargetMailbox, this.recipientSession, null, new LocalizedString?(Strings.ExceptionUserObjectNotFound(this.TargetMailbox.ToString())), new LocalizedString?(Strings.ExceptionUserObjectAmbiguous));
			}
			if (aduser != null)
			{
				if (aduser.RecipientType != RecipientType.UserMailbox)
				{
					base.ThrowTerminatingError(new MailboxSearchTaskException(Strings.ErrorInvalidRecipientType(aduser.ToString(), aduser.RecipientType.ToString())), ErrorCategory.InvalidArgument, "TargetMailbox");
				}
				bool flag2 = Utils.VerifyMailboxVersionIsSP1OrGreater(aduser);
				if ((this.EstimateOnly || this.ExcludeDuplicateMessages) && !flag2)
				{
					base.ThrowTerminatingError(new MailboxSearchTaskException(Strings.ErrorMailboxVersionTooOld(aduser.Id.ToString())), ErrorCategory.InvalidArgument, "TargetMailbox");
				}
				this.objectToSave.Target = aduser.LegacyExchangeDN;
			}
			if (flag)
			{
				this.objectToSave.AllSourceMailboxes = this.AllSourceMailboxes;
				if (this.AllSourceMailboxes)
				{
					if (this.sourceMailboxIdParameters != null && this.sourceMailboxIdParameters.Count > 0)
					{
						this.WriteWarning(Strings.AllSourceMailboxesParameterOverride("AllSourceMailboxes", "SourceMailboxes"));
					}
					this.sourceMailboxIdParameters = null;
				}
			}
			this.objectToSave.Sources = Utils.ConvertSourceMailboxesCollection(this.sourceMailboxIdParameters, this.InPlaceHoldEnabled, (RecipientIdParameter recipientId) => base.GetDataObject<ADRecipient>(recipientId, this.recipientSession, null, new LocalizedString?(Strings.ExceptionUserObjectNotFound(recipientId.ToString())), new LocalizedString?(Strings.ExceptionUserObjectAmbiguous)) as ADRecipient, this.recipientSession, new Task.TaskErrorLoggingDelegate(base.WriteError), new Action<LocalizedString>(this.WriteWarning), (LocalizedString locString) => this.Force || base.ShouldContinue(locString));
			if (this.objectToSave.Target != null && this.objectToSave.Sources != null && this.objectToSave.Sources.Contains(this.objectToSave.Target))
			{
				this.WriteWarning(Strings.TargetMailboxInSourceIsSkipped(this.DataObject.Target));
				this.objectToSave.Sources.Remove(this.objectToSave.Target);
				if (this.objectToSave.Sources.Count == 0)
				{
					base.WriteError(new MailboxSearchTaskException(Strings.TheOnlySourceMailboxIsTheTargetMailbox), ErrorCategory.InvalidArgument, null);
				}
			}
			if (flag)
			{
				this.objectToSave.AllPublicFolderSources = this.AllPublicFolderSources;
				if (this.AllPublicFolderSources)
				{
					if (this.PublicFolderSources != null)
					{
						this.WriteWarning(Strings.AllSourceMailboxesParameterOverride("AllPublicFolderSources", "PublicFolderSources"));
					}
					this.PublicFolderSources = null;
				}
				string[] array = null;
				if (this.PublicFolderSources != null && this.PublicFolderSources.Length != 0)
				{
					array = new string[this.PublicFolderSources.Length];
					string action = "Get-PublicFolder";
					try
					{
						using (PublicFolderDataProvider publicFolderDataProvider = new PublicFolderDataProvider(this.ConfigurationSession, action, Guid.Empty))
						{
							for (int i = 0; i < this.PublicFolderSources.Length; i++)
							{
								PublicFolder publicFolder = null;
								array[i] = this.PublicFolderSources[i].ToString();
								try
								{
									publicFolder = (PublicFolder)publicFolderDataProvider.Read<PublicFolder>(this.PublicFolderSources[i].PublicFolderId);
								}
								catch (FormatException exception)
								{
									base.WriteError(exception, ErrorCategory.WriteError, null);
								}
								if (publicFolder == null)
								{
									base.WriteError(new MailboxSearchTaskException(Strings.PublicFolderSourcesFolderDoesnotExist(array[i])), ErrorCategory.InvalidArgument, null);
								}
							}
						}
					}
					catch (AccessDeniedException exception2)
					{
						base.WriteError(exception2, ErrorCategory.PermissionDenied, this.Name);
					}
				}
				this.objectToSave.PublicFolderSources = Utils.ConvertCollectionToMultiValedProperty<string, string>(array, (string value, object param) => value, null, new MultiValuedProperty<string>(), new Task.TaskErrorLoggingDelegate(base.WriteError), "PublicFolderSources");
				this.objectToSave.Version = SearchObjectVersion.SecondVersion;
			}
			this.objectToSave.Senders = Utils.ConvertCollectionToMultiValedProperty<string, string>(this.Senders, (string value, object param) => value, null, null, new Task.TaskErrorLoggingDelegate(base.WriteError), SearchObjectSchema.Senders.Name);
			this.objectToSave.Recipients = Utils.ConvertCollectionToMultiValedProperty<string, string>(this.Recipients, (string value, object param) => value, null, null, new Task.TaskErrorLoggingDelegate(base.WriteError), SearchObjectSchema.Recipients.Name);
			this.objectToSave.MessageTypes = Utils.ConvertCollectionToMultiValedProperty<KindKeyword, KindKeyword>(this.MessageTypes, (KindKeyword kind, object param) => kind, null, null, new Task.TaskErrorLoggingDelegate(base.WriteError), SearchObjectSchema.MessageTypes.Name);
			this.objectToSave.CreatedBy = Utils.GetExecutingUserDisplayName(((DiscoverySearchDataProvider)base.DataSession).DisplayName, base.ExchangeRunspaceConfig);
			this.objectToSave.LogLevel = this.LogLevel;
			this.objectToSave.Language = this.Language.Name;
			this.objectToSave.IncludeUnsearchableItems = this.IncludeUnsearchableItems;
			this.objectToSave.StatisticsOnly = this.EstimateOnly;
			this.objectToSave.ExcludeDuplicateMessages = this.ExcludeDuplicateMessages;
			this.objectToSave.IncludeKeywordStatistics = this.IncludeKeywordStatistics;
			this.objectToSave.InPlaceHoldEnabled = this.InPlaceHoldEnabled;
			this.objectToSave.ItemHoldPeriod = this.ItemHoldPeriod;
			this.objectToSave.ManagedByOrganization = base.CurrentOrganizationId.ToString();
			this.objectToSave.Description = this.Description;
			this.objectToSave.StatusMailRecipients = Utils.ConvertCollectionToMultiValedProperty<RecipientIdParameter, ADObjectId>(this.StatusMailRecipients, new Utils.IdentityToRawIdDelegate<RecipientIdParameter, ADObjectId>(this.ADObjectIdFromRecipientIdParameter), null, null, new Task.TaskErrorLoggingDelegate(base.WriteError), SearchObjectSchema.StatusMailRecipients.Name);
			this.objectToSave.ManagedBy = Utils.ConvertCollectionToMultiValedProperty<RecipientIdParameter, ADObjectId>(this.ManagedBy, new Utils.IdentityToRawIdDelegate<RecipientIdParameter, ADObjectId>(this.ADObjectIdFromRecipientIdParameter), null, null, new Task.TaskErrorLoggingDelegate(base.WriteError), SearchObjectSchema.ManagedBy.Name);
			this.objectToSave.InPlaceHoldIdentity = this.InPlaceHoldIdentity;
			if (flag && (this.objectToSave.Sources == null || this.objectToSave.Sources.Count == 0) && (this.objectToSave.PublicFolderSources == null || this.objectToSave.PublicFolderSources.Count == 0) && !this.objectToSave.AllSourceMailboxes && !this.objectToSave.AllPublicFolderSources)
			{
				base.WriteError(new MailboxSearchTaskException(Strings.NoSourceMailboxesAndNoPublicFolderSourcesSet), ErrorCategory.InvalidArgument, null);
			}
			bool flag3 = this.InPlaceHoldEnabled && (this.objectToSave.Sources == null || this.objectToSave.Sources.Count == 0);
			bool flag4 = false;
			if (flag)
			{
				flag3 = (this.InPlaceHoldEnabled && this.objectToSave.AllSourceMailboxes);
				flag4 = (this.InPlaceHoldEnabled && (this.objectToSave.AllPublicFolderSources || (this.objectToSave.PublicFolderSources != null && this.objectToSave.PublicFolderSources.Count != 0)));
			}
			if (flag3)
			{
				base.WriteError(new MailboxSearchTaskException(Strings.InPlaceHoldNotAllowedForAllSourceMailboxes), ErrorCategory.InvalidArgument, null);
			}
			if (flag4)
			{
				base.WriteError(new MailboxSearchTaskException(Strings.InPlaceHoldNotAllowedForPublicFolders), ErrorCategory.InvalidArgument, null);
			}
		}

		private const string ParameterSourceMailboxes = "SourceMailboxes";

		private const string ParameterTargetMailbox = "TargetMailbox";

		private const string ParameterSearchQuery = "SearchQuery";

		private const string ParameterPublicFolderSources = "PublicFolderSources";

		private const string ParameterAllPublicFolderSources = "AllPublicFolderSources";

		private const string ParameterAllSourceMailboxes = "AllSourceMailboxes";

		private const string ParameterSenders = "Senders";

		private const string ParameterRecipients = "Recipients";

		private const string ParameterLanguage = "Language";

		private const string ParameterMessageTypes = "MessageTypes";

		private const string ParameterStatusMailRecipients = "StatusMailRecipients";

		private const string ParameterManagedBy = "ManagedBy";

		private const string ParameterLogLevel = "LogLevel";

		private const string ParameterIncludeUnsearchableItems = "IncludeUnsearchableItems";

		private const string ParameterIncludeRemoteAccounts = "IncludeRemoteAccounts";

		private const string ParameterForce = "Force";

		private const string ParameterEstimateOnly = "EstimateOnly";

		private const string ParameterExcludeDuplicateMessages = "ExcludeDuplicateMessages";

		private const string ParameterInPlaceHoldEnabled = "InPlaceHoldEnabled";

		private const string ParameterIncludeKeywordStatistics = "IncludeKeywordStatistics";

		private const string ParameterItemHoldPeriod = "ItemHoldPeriod";

		private const string ParameterDescription = "Description";

		private const string ParameterInPlaceHoldIdentity = "InPlaceHoldIdentity";

		private MailboxDiscoverySearch objectToSave = new MailboxDiscoverySearch();

		private List<RecipientIdParameter> sourceMailboxIdParameters = new List<RecipientIdParameter>();

		private IRecipientSession recipientSession;
	}
}
