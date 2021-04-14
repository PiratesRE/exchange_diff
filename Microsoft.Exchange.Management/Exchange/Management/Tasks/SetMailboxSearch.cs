using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
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
	[Cmdlet("Set", "MailboxSearch", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class SetMailboxSearch : SetTenantADTaskBase<EwsStoreObjectIdParameter, MailboxDiscoverySearch, MailboxDiscoverySearch>
	{
		[Parameter(Mandatory = false)]
		public string SearchQuery
		{
			get
			{
				return (string)base.Fields["SearchQuery"];
			}
			set
			{
				base.Fields["SearchQuery"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] SourceMailboxes
		{
			get
			{
				return (RecipientIdParameter[])base.Fields[SearchObjectSchema.SourceMailboxes.Name];
			}
			set
			{
				base.Fields[SearchObjectSchema.SourceMailboxes.Name] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxIdParameter TargetMailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields[SearchObjectSchema.TargetMailbox.Name];
			}
			set
			{
				base.Fields[SearchObjectSchema.TargetMailbox.Name] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public PublicFolderIdParameter[] PublicFolderSources
		{
			get
			{
				return (PublicFolderIdParameter[])base.Fields[MailboxDiscoverySearchSchema.PublicFolderSources.Name];
			}
			set
			{
				base.Fields[MailboxDiscoverySearchSchema.PublicFolderSources.Name] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllPublicFolderSources
		{
			get
			{
				return (bool)(base.Fields[MailboxDiscoverySearchSchema.AllPublicFolderSources.Name] ?? false);
			}
			set
			{
				base.Fields[MailboxDiscoverySearchSchema.AllPublicFolderSources.Name] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllSourceMailboxes
		{
			get
			{
				return (bool)(base.Fields[MailboxDiscoverySearchSchema.AllSourceMailboxes.Name] ?? false);
			}
			set
			{
				base.Fields[MailboxDiscoverySearchSchema.AllSourceMailboxes.Name] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] Senders
		{
			get
			{
				return (string[])base.Fields[SearchObjectSchema.Senders.Name];
			}
			set
			{
				base.Fields[SearchObjectSchema.Senders.Name] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] Recipients
		{
			get
			{
				return (string[])base.Fields[SearchObjectSchema.Recipients.Name];
			}
			set
			{
				base.Fields[SearchObjectSchema.Recipients.Name] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExDateTime? StartDate
		{
			get
			{
				return (ExDateTime?)base.Fields[SearchObjectSchema.StartDate.Name];
			}
			set
			{
				base.Fields[SearchObjectSchema.StartDate.Name] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExDateTime? EndDate
		{
			get
			{
				return (ExDateTime?)base.Fields[SearchObjectSchema.EndDate.Name];
			}
			set
			{
				base.Fields[SearchObjectSchema.EndDate.Name] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public KindKeyword[] MessageTypes
		{
			get
			{
				return (KindKeyword[])base.Fields[SearchObjectSchema.MessageTypes.Name];
			}
			set
			{
				base.Fields[SearchObjectSchema.MessageTypes.Name] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter[] StatusMailRecipients
		{
			get
			{
				return (RecipientIdParameter[])base.Fields[SearchObjectSchema.StatusMailRecipients.Name];
			}
			set
			{
				base.Fields[SearchObjectSchema.StatusMailRecipients.Name] = value;
			}
		}

		internal RecipientIdParameter[] ManagedBy
		{
			get
			{
				return (RecipientIdParameter[])base.Fields[SearchObjectSchema.ManagedBy.Name];
			}
			set
			{
				base.Fields[SearchObjectSchema.ManagedBy.Name] = value;
			}
		}

		[Parameter]
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
		public bool EstimateOnly
		{
			get
			{
				return (bool)(base.Fields[SearchObjectSchema.EstimateOnly.Name] ?? false);
			}
			set
			{
				base.Fields[SearchObjectSchema.EstimateOnly.Name] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeKeywordStatistics
		{
			get
			{
				return (SwitchParameter)(base.Fields[SearchObjectSchema.IncludeKeywordStatistics.Name] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields[SearchObjectSchema.IncludeKeywordStatistics.Name] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int StatisticsStartIndex
		{
			get
			{
				return (int)(base.Fields[MailboxDiscoverySearchSchema.StatisticsStartIndex.Name] ?? 1);
			}
			set
			{
				base.Fields[MailboxDiscoverySearchSchema.StatisticsStartIndex.Name] = value;
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

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.SetMailboxSearchConfirmation(this.Identity.ToString());
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession recipientSession = Utils.CreateRecipientSession(base.DomainController, base.SessionSettings);
			this.recipientSession = recipientSession;
			return new DiscoverySearchDataProvider(base.CurrentOrganizationId);
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.HasErrors)
			{
				if (this.DataObject.ExcludeDuplicateMessages && this.EstimateOnly)
				{
					base.WriteError(new MailboxSearchTaskException(Strings.ExcludeDuplicateMessagesParameterConflict), ErrorCategory.InvalidArgument, null);
				}
				if (this.DataObject.StatisticsOnly && this.DataObject.LogLevel != LoggingLevel.Suppress)
				{
					base.WriteError(new MailboxSearchTaskException(Strings.EstimateOnlyLogLevelParameterConflict(this.DataObject.LogLevel.ToString())), ErrorCategory.InvalidArgument, "LogLevel");
				}
				if (base.ExchangeRunspaceConfig == null)
				{
					base.WriteError(new MailboxSearchTaskException(Strings.UnableToDetermineExecutingUser), ErrorCategory.InvalidOperation, null);
				}
				if (!this.DataObject.StatisticsOnly && this.DataObject.IncludeKeywordStatistics)
				{
					base.WriteError(new MailboxSearchTaskException(Strings.IncludeKeywordStatisticsParameterConflict), ErrorCategory.InvalidArgument, null);
				}
				if (this.DataObject.Target != null)
				{
					ADRecipient adrecipient = this.recipientSession.FindByLegacyExchangeDN(this.DataObject.Target);
					if (adrecipient != null)
					{
						this.targetUser = (ADUser)this.recipientSession.Read(adrecipient.Id);
					}
					if (this.targetUser == null)
					{
						base.WriteError(new ObjectNotFoundException(Strings.ExceptionTargetMailboxNotFound(this.DataObject.Target, this.DataObject.Name)), ErrorCategory.InvalidOperation, null);
					}
					bool flag = Utils.VerifyMailboxVersionIsSP1OrGreater(this.targetUser);
					if ((this.EstimateOnly || this.DataObject.ExcludeDuplicateMessages) && !flag)
					{
						base.WriteError(new MailboxSearchTaskException(Strings.ErrorMailboxVersionTooOld(this.targetUser.Id.ToString())), ErrorCategory.InvalidOperation, null);
					}
				}
				if (this.DataObject.IsChanged(EwsStoreObjectSchema.AlternativeId) && Utils.SameNameExists(this.DataObject.Name, (DiscoverySearchDataProvider)base.DataSession, Utils.GetMailboxDataProvider(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, new Task.TaskErrorLoggingDelegate(base.WriteError))))
				{
					base.WriteError(new MailboxSearchNameIsNotUniqueException(this.DataObject.Name), ErrorCategory.InvalidArgument, this.DataObject);
				}
				Exception ex = Utils.ValidateSourceAndTargetMailboxes((DiscoverySearchDataProvider)base.DataSession, this.DataObject);
				if (ex != null)
				{
					base.WriteError(ex, ErrorCategory.InvalidArgument, null);
				}
				if (this.DataObject.IsChanged(MailboxDiscoverySearchSchema.Query) || this.DataObject.IsChanged(MailboxDiscoverySearchSchema.Sources) || this.DataObject.IsChanged(MailboxDiscoverySearchSchema.StatisticsOnly) || this.DataObject.IsChanged(MailboxDiscoverySearchSchema.IncludeUnsearchableItems) || this.DataObject.IsChanged(MailboxDiscoverySearchSchema.Language) || this.DataObject.IsChanged(MailboxDiscoverySearchSchema.LogLevel) || this.DataObject.IsChanged(MailboxDiscoverySearchSchema.StatusMailRecipients) || this.DataObject.IsChanged(MailboxDiscoverySearchSchema.Target))
				{
					Utils.CheckSearchRunningStatus(this.DataObject, new Task.TaskErrorLoggingDelegate(base.WriteError), Strings.MailboxSearchIsInProgress(this.DataObject.Name));
				}
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable ResolveDataObject()
		{
			string text = this.Identity.ToString();
			MailboxDataProvider mailboxDataProvider = Utils.GetMailboxDataProvider(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, new Task.TaskErrorLoggingDelegate(base.WriteError));
			SearchObject searchObject;
			if (Utils.IsLegacySearchObjectIdentity(text))
			{
				searchObject = (SearchObject)base.GetDataObject<SearchObject>(new SearchObjectIdParameter(text), mailboxDataProvider, this.RootId, base.OptionalIdentityData, new LocalizedString?(Strings.ErrorManagementObjectNotFound(text)), new LocalizedString?(Strings.ErrorManagementObjectAmbiguous(text)));
			}
			else
			{
				searchObject = Utils.GetE14SearchObjectByName(text, mailboxDataProvider);
			}
			if (searchObject == null)
			{
				return base.ResolveDataObject();
			}
			if (!this.Force && !base.ShouldContinue(Strings.EditWillUpgradeSearchObject))
			{
				base.WriteError(new MailboxSearchTaskException(Strings.CannotEditLegacySearchObjectWithoutUpgrade(searchObject.Name)), ErrorCategory.InvalidArgument, text);
			}
			return Utils.UpgradeLegacySearchObject(searchObject, mailboxDataProvider, (DiscoverySearchDataProvider)base.DataSession, new Task.TaskErrorLoggingDelegate(base.WriteError), new Action<LocalizedString>(this.WriteWarning));
		}

		protected override IConfigurable PrepareDataObject()
		{
			MailboxDiscoverySearch mailboxDiscoverySearch = (MailboxDiscoverySearch)base.PrepareDataObject();
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
					flag = base.ExchangeRunspaceConfig.IsCmdletAllowedInScope("Set-MailboxSearch", paramNames, executingUser, ScopeLocation.RecipientWrite);
				}
				else
				{
					flag = base.ExchangeRunspaceConfig.IsCmdletAllowedInScope("Set-MailboxSearch", paramNames, scopeSet);
				}
			}
			if (flag && mailboxDiscoverySearch.Version == SearchObjectVersion.Original && (mailboxDiscoverySearch.Sources == null || mailboxDiscoverySearch.Sources.Count == 0) && (mailboxDiscoverySearch.PublicFolderSources == null || mailboxDiscoverySearch.PublicFolderSources.Count == 0) && !mailboxDiscoverySearch.AllSourceMailboxes && !mailboxDiscoverySearch.AllPublicFolderSources)
			{
				this.AllSourceMailboxes = true;
			}
			if (flag)
			{
				mailboxDiscoverySearch.Version = SearchObjectVersion.SecondVersion;
			}
			if (base.Fields.IsModified(SearchObjectSchema.TargetMailbox.Name))
			{
				if (this.TargetMailbox != null)
				{
					try
					{
						this.recipientSession.SessionSettings.IncludeInactiveMailbox = false;
						ADUser aduser = (ADUser)base.GetDataObject<ADUser>(this.TargetMailbox, this.recipientSession, null, new LocalizedString?(Strings.ExceptionUserObjectNotFound(this.TargetMailbox.ToString())), new LocalizedString?(Strings.ExceptionUserObjectAmbiguous));
						if (aduser.RecipientType != RecipientType.UserMailbox)
						{
							base.ThrowTerminatingError(new MailboxSearchTaskException(Strings.ErrorInvalidRecipientType(aduser.ToString(), aduser.RecipientType.ToString())), ErrorCategory.InvalidArgument, SearchObjectSchema.TargetMailbox.Name);
						}
						mailboxDiscoverySearch.Target = aduser.LegacyExchangeDN;
						if (base.ScopeSet != null)
						{
							Utils.VerifyIsInScopes(aduser, base.ScopeSet, new Task.TaskErrorLoggingDelegate(base.WriteError));
						}
						Utils.VerifyMailboxVersion(aduser, new Task.TaskErrorLoggingDelegate(base.WriteError));
						goto IL_20B;
					}
					finally
					{
						if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
						{
							this.recipientSession.SessionSettings.IncludeInactiveMailbox = true;
						}
					}
				}
				mailboxDiscoverySearch.Target = null;
			}
			IL_20B:
			if (flag)
			{
				if (base.Fields.IsModified(MailboxDiscoverySearchSchema.AllSourceMailboxes.Name))
				{
					mailboxDiscoverySearch.AllSourceMailboxes = this.AllSourceMailboxes;
				}
				if (this.AllSourceMailboxes)
				{
					if (this.SourceMailboxes != null || mailboxDiscoverySearch.Sources != null)
					{
						this.WriteWarning(Strings.AllSourceMailboxesParameterOverride("AllSourceMailboxes", "SourceMailboxes"));
					}
					this.SourceMailboxes = null;
					mailboxDiscoverySearch.Sources = null;
				}
			}
			bool flag2 = base.Fields.IsModified(SearchObjectSchema.SourceMailboxes.Name);
			if (!flag2)
			{
				if (!mailboxDiscoverySearch.IsChanged(MailboxDiscoverySearchSchema.InPlaceHoldEnabled) || !mailboxDiscoverySearch.InPlaceHoldEnabled)
				{
					goto IL_442;
				}
			}
			try
			{
				if (mailboxDiscoverySearch.InPlaceHoldEnabled)
				{
					this.recipientSession.SessionSettings.IncludeSoftDeletedObjects = true;
				}
				IEnumerable<RecipientIdParameter> enumerable;
				if (!flag2)
				{
					if (mailboxDiscoverySearch.Sources != null)
					{
						enumerable = from legacyDn in mailboxDiscoverySearch.Sources
						select new RecipientIdParameter(legacyDn);
					}
					else
					{
						enumerable = null;
					}
				}
				else
				{
					enumerable = this.SourceMailboxes;
				}
				IEnumerable<RecipientIdParameter> recipientIds = enumerable;
				MultiValuedProperty<string> multiValuedProperty = Utils.ConvertSourceMailboxesCollection(recipientIds, mailboxDiscoverySearch.InPlaceHoldEnabled, (RecipientIdParameter recipientId) => base.GetDataObject<ADRecipient>(recipientId, this.recipientSession, null, new LocalizedString?(Strings.ExceptionUserObjectNotFound(recipientId.ToString())), new LocalizedString?(Strings.ExceptionUserObjectAmbiguous)) as ADRecipient, this.recipientSession, new Task.TaskErrorLoggingDelegate(base.WriteError), new Action<LocalizedString>(this.WriteWarning), (LocalizedString locString) => this.Force || base.ShouldContinue(locString)) ?? new MultiValuedProperty<string>();
				mailboxDiscoverySearch.Sources.CopyChangesFrom(multiValuedProperty);
				if (mailboxDiscoverySearch.Sources.Count != multiValuedProperty.Count)
				{
					mailboxDiscoverySearch.Sources = multiValuedProperty;
				}
				if (base.ScopeSet != null)
				{
					foreach (string legacyExchangeDN in mailboxDiscoverySearch.Sources)
					{
						ADRecipient adrecipient = this.recipientSession.FindByLegacyExchangeDN(legacyExchangeDN);
						if (adrecipient == null)
						{
							base.WriteError(new ObjectNotFoundException(Strings.ExceptionSourceMailboxNotFound(mailboxDiscoverySearch.Target, mailboxDiscoverySearch.Name)), ErrorCategory.InvalidOperation, null);
						}
						Utils.VerifyIsInHoldScopes(mailboxDiscoverySearch.InPlaceHoldEnabled, base.ExchangeRunspaceConfig, adrecipient, "Set-MailboxSearch", new Task.TaskErrorLoggingDelegate(base.WriteError));
						Utils.VerifyIsInScopes(adrecipient, base.ScopeSet, new Task.TaskErrorLoggingDelegate(base.WriteError));
					}
				}
			}
			finally
			{
				this.recipientSession.SessionSettings.IncludeSoftDeletedObjects = false;
			}
			IL_442:
			if (flag)
			{
				if (base.Fields.IsModified(MailboxDiscoverySearchSchema.AllPublicFolderSources.Name))
				{
					mailboxDiscoverySearch.AllPublicFolderSources = this.AllPublicFolderSources;
				}
				if (this.AllPublicFolderSources)
				{
					if (this.PublicFolderSources != null)
					{
						this.WriteWarning(Strings.AllSourceMailboxesParameterOverride("AllPublicFolderSources", "PublicFolderSources"));
					}
					this.PublicFolderSources = null;
				}
				if (base.Fields.IsModified(MailboxDiscoverySearchSchema.PublicFolderSources.Name))
				{
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
							base.WriteError(exception2, ErrorCategory.PermissionDenied, mailboxDiscoverySearch.Name);
						}
					}
					mailboxDiscoverySearch.PublicFolderSources = Utils.ConvertCollectionToMultiValedProperty<string, string>(array, (string value, object param) => value, null, new MultiValuedProperty<string>(), new Task.TaskErrorLoggingDelegate(base.WriteError), MailboxDiscoverySearchSchema.PublicFolderSources.Name);
				}
			}
			if (base.Fields.IsModified(SearchObjectSchema.Senders.Name))
			{
				MultiValuedProperty<string> senders = Utils.ConvertCollectionToMultiValedProperty<string, string>(this.Senders, (string value, object param) => value, null, new MultiValuedProperty<string>(), new Task.TaskErrorLoggingDelegate(base.WriteError), SearchObjectSchema.Senders.Name);
				mailboxDiscoverySearch.Senders = senders;
			}
			if (base.Fields.IsModified(SearchObjectSchema.Recipients.Name))
			{
				MultiValuedProperty<string> recipients = Utils.ConvertCollectionToMultiValedProperty<string, string>(this.Recipients, (string value, object param) => value, null, new MultiValuedProperty<string>(), new Task.TaskErrorLoggingDelegate(base.WriteError), SearchObjectSchema.Recipients.Name);
				mailboxDiscoverySearch.Recipients = recipients;
			}
			if (base.Fields.IsModified(SearchObjectSchema.StartDate.Name))
			{
				mailboxDiscoverySearch.StartDate = this.StartDate;
			}
			if (base.Fields.IsModified(SearchObjectSchema.EndDate.Name))
			{
				mailboxDiscoverySearch.EndDate = this.EndDate;
			}
			if (base.Fields.IsModified(SearchObjectSchema.MessageTypes.Name))
			{
				MultiValuedProperty<KindKeyword> messageTypes = Utils.ConvertCollectionToMultiValedProperty<KindKeyword, KindKeyword>(this.MessageTypes, (KindKeyword kind, object param) => kind, null, new MultiValuedProperty<KindKeyword>(), new Task.TaskErrorLoggingDelegate(base.WriteError), SearchObjectSchema.MessageTypes.Name);
				mailboxDiscoverySearch.MessageTypes = messageTypes;
			}
			if (base.Fields.IsModified(SearchObjectSchema.StatusMailRecipients.Name))
			{
				MultiValuedProperty<ADObjectId> multiValuedProperty2 = Utils.ConvertCollectionToMultiValedProperty<RecipientIdParameter, ADObjectId>(this.StatusMailRecipients, new Utils.IdentityToRawIdDelegate<RecipientIdParameter, ADObjectId>(this.ADObjectIdFromRecipientIdParameter), null, new MultiValuedProperty<ADObjectId>(), new Task.TaskErrorLoggingDelegate(base.WriteError), SearchObjectSchema.StatusMailRecipients.Name);
				mailboxDiscoverySearch.StatusMailRecipients.CopyChangesFrom(multiValuedProperty2);
				if (mailboxDiscoverySearch.StatusMailRecipients.Count != multiValuedProperty2.Count)
				{
					mailboxDiscoverySearch.StatusMailRecipients = multiValuedProperty2;
				}
			}
			if (base.Fields.IsModified(SearchObjectSchema.ManagedBy.Name))
			{
				MultiValuedProperty<ADObjectId> multiValuedProperty3 = Utils.ConvertCollectionToMultiValedProperty<RecipientIdParameter, ADObjectId>(this.ManagedBy, new Utils.IdentityToRawIdDelegate<RecipientIdParameter, ADObjectId>(this.ADObjectIdFromRecipientIdParameter), null, new MultiValuedProperty<ADObjectId>(), new Task.TaskErrorLoggingDelegate(base.WriteError), SearchObjectSchema.ManagedBy.Name);
				mailboxDiscoverySearch.ManagedBy.CopyChangesFrom(multiValuedProperty3);
				if (mailboxDiscoverySearch.ManagedBy.Count != multiValuedProperty3.Count)
				{
					mailboxDiscoverySearch.ManagedBy = multiValuedProperty3;
				}
			}
			if (base.Fields.IsModified("SearchQuery"))
			{
				mailboxDiscoverySearch.Query = this.SearchQuery;
			}
			if (base.Fields.IsModified(SearchObjectSchema.EstimateOnly.Name))
			{
				mailboxDiscoverySearch.StatisticsOnly = this.EstimateOnly;
				if (this.EstimateOnly)
				{
					mailboxDiscoverySearch.LogLevel = LoggingLevel.Suppress;
				}
				else
				{
					mailboxDiscoverySearch.IncludeKeywordStatistics = false;
				}
			}
			if (base.Fields.IsModified(SearchObjectSchema.IncludeKeywordStatistics.Name))
			{
				mailboxDiscoverySearch.IncludeKeywordStatistics = this.IncludeKeywordStatistics.ToBool();
			}
			if (base.Fields.IsModified(MailboxDiscoverySearchSchema.StatisticsStartIndex.Name))
			{
				mailboxDiscoverySearch.StatisticsStartIndex = this.StatisticsStartIndex;
			}
			if (flag && (mailboxDiscoverySearch.Sources == null || mailboxDiscoverySearch.Sources.Count == 0) && (mailboxDiscoverySearch.PublicFolderSources == null || mailboxDiscoverySearch.PublicFolderSources.Count == 0) && !mailboxDiscoverySearch.AllSourceMailboxes && !mailboxDiscoverySearch.AllPublicFolderSources)
			{
				base.WriteError(new MailboxSearchTaskException(Strings.NoSourceMailboxesAndNoPublicFolderSourcesSet), ErrorCategory.InvalidArgument, null);
			}
			bool flag3 = mailboxDiscoverySearch.InPlaceHoldEnabled && (mailboxDiscoverySearch.Sources == null || mailboxDiscoverySearch.Sources.Count == 0);
			bool flag4 = false;
			if (flag)
			{
				flag3 = (mailboxDiscoverySearch.InPlaceHoldEnabled && mailboxDiscoverySearch.AllSourceMailboxes);
				flag4 = (mailboxDiscoverySearch.InPlaceHoldEnabled && (mailboxDiscoverySearch.AllPublicFolderSources || (mailboxDiscoverySearch.PublicFolderSources != null && mailboxDiscoverySearch.PublicFolderSources.Count != 0)));
			}
			if (flag3)
			{
				base.WriteError(new MailboxSearchTaskException(Strings.InPlaceHoldNotAllowedForAllSourceMailboxes), ErrorCategory.InvalidArgument, null);
			}
			if (flag4)
			{
				base.WriteError(new MailboxSearchTaskException(Strings.InPlaceHoldNotAllowedForPublicFolders), ErrorCategory.InvalidArgument, null);
			}
			return mailboxDiscoverySearch;
		}

		protected override void InternalProcessRecord()
		{
			bool flag = false;
			bool flag2 = this.IsObjectStateChanged();
			if (flag2)
			{
				if (base.ExchangeRunspaceConfig == null)
				{
					base.WriteError(new MailboxSearchTaskException(Strings.UnableToDetermineExecutingUser), ErrorCategory.InvalidOperation, null);
					return;
				}
				if (this.DataObject.IsChanged(MailboxDiscoverySearchSchema.InPlaceHoldEnabled) || (this.DataObject.InPlaceHoldEnabled && this.DataObject.IsChanged(MailboxDiscoverySearchSchema.Sources)))
				{
					flag = true;
				}
			}
			if (flag)
			{
				bool flag3 = false;
				if ((this.DataObject.IsChanged(MailboxDiscoverySearchSchema.InPlaceHoldEnabled) && !this.DataObject.InPlaceHoldEnabled) || this.DataObject.InPlaceHoldEnabled)
				{
					flag3 = this.DataObject.ShouldWarnForInactiveOnHold((DiscoverySearchDataProvider)base.DataSession, this.DataObject.InPlaceHoldEnabled ? this.recipientSession : this.GetRecipientSessionWithoutScopeSet(), this.DataObject.InPlaceHoldEnabled);
				}
				if (flag3 && !base.ShouldContinue(Strings.ContinueToRemoveHoldForInactive))
				{
					return;
				}
			}
			base.InternalProcessRecord();
			if (flag2)
			{
				SearchEventLogger.Instance.LogDiscoveryAndHoldSavedEvent(this.DataObject);
			}
			if (flag)
			{
				LocalizedString localizedString = this.DataObject.SynchronizeHoldSettings((DiscoverySearchDataProvider)base.DataSession, this.DataObject.InPlaceHoldEnabled ? this.recipientSession : this.GetRecipientSessionWithoutScopeSet(), this.DataObject.InPlaceHoldEnabled, delegate(int percentage)
				{
					base.WriteProgress(Strings.SetMailboxSearchActivity, Strings.ApplyingHoldSettings(this.DataObject.Name), percentage);
				});
				if (localizedString != LocalizedString.Empty)
				{
					base.WriteError(new MailboxSearchTaskException(localizedString), ErrorCategory.InvalidOperation, this);
				}
				this.WriteWarning(Strings.WarningDiscoveryHoldDelay(COWSettings.COWCacheLifeTime.TotalMinutes));
			}
		}

		private IRecipientSession GetRecipientSessionWithoutScopeSet()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, false);
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, sessionSettings, 989, "GetRecipientSessionWithoutScopeSet", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Search\\SetMailboxSearch.cs");
		}

		private const string ParameterSearchQuery = "SearchQuery";

		private IRecipientSession recipientSession;

		private ADUser targetUser;
	}
}
