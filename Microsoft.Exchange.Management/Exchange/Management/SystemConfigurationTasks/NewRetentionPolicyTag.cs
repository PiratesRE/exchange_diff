using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Approval;
using Microsoft.Exchange.InfoWorker.Common.ELC;
using Microsoft.Exchange.InfoWorker.Common.Search;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "RetentionPolicyTag", DefaultParameterSetName = "RetentionPolicy", SupportsShouldProcess = true)]
	public sealed class NewRetentionPolicyTag : NewMultitenancySystemConfigurationObjectTask<RetentionPolicyTag>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewRetentionTag(base.Name.ToString());
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				if (!this.IgnoreDehydratedFlag)
				{
					return SharedTenantConfigurationMode.Dehydrateable;
				}
				return SharedTenantConfigurationMode.NotShared;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> LocalizedRetentionPolicyTagName
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["LocalizedRetentionPolicyTagName"];
			}
			set
			{
				base.Fields["LocalizedRetentionPolicyTagName"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public override SwitchParameter IgnoreDehydratedFlag { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter IsDefaultAutoGroupPolicyTag
		{
			get
			{
				return (SwitchParameter)(base.Fields["IsDefaultAutoGroupPolicyTag"] ?? false);
			}
			set
			{
				base.Fields["IsDefaultAutoGroupPolicyTag"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IsDefaultModeratedRecipientsPolicyTag
		{
			get
			{
				return (SwitchParameter)(base.Fields["IsDefaultModeratedRecipientsPolicyTag"] ?? false);
			}
			set
			{
				base.Fields["IsDefaultModeratedRecipientsPolicyTag"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Comment
		{
			get
			{
				return (string)base.Fields["Comment"];
			}
			set
			{
				base.Fields["Comment"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RetentionPolicy")]
		public Guid RetentionId
		{
			get
			{
				return (Guid)base.Fields["RetentionId"];
			}
			set
			{
				base.Fields["RetentionId"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> LocalizedComment
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["LocalizedComment"];
			}
			set
			{
				base.Fields["LocalizedComment"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MustDisplayCommentEnabled
		{
			get
			{
				return (bool)base.Fields["MustDisplayCommentEnabled"];
			}
			set
			{
				base.Fields["MustDisplayCommentEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ElcFolderType Type
		{
			get
			{
				return (ElcFolderType)base.Fields["Type"];
			}
			set
			{
				base.Fields["Type"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "UpgradeManagedFolder")]
		public ELCFolderIdParameter ManagedFolderToUpgrade
		{
			get
			{
				return (ELCFolderIdParameter)base.Fields["ManagedFolderToUpgrade"];
			}
			set
			{
				base.Fields["ManagedFolderToUpgrade"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SystemTag
		{
			get
			{
				return (bool)(base.Fields["SystemTag"] ?? false);
			}
			set
			{
				base.Fields["SystemTag"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RetentionPolicy")]
		public string MessageClass
		{
			get
			{
				return this.contentSettingsObject.MessageClass;
			}
			set
			{
				this.contentSettingsObject.MessageClass = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RetentionPolicy")]
		public bool RetentionEnabled
		{
			get
			{
				return (bool)(base.Fields["RetentionEnabled"] ?? true);
			}
			set
			{
				base.Fields["RetentionEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RetentionPolicy")]
		public RetentionActionType RetentionAction
		{
			get
			{
				return this.contentSettingsObject.RetentionAction;
			}
			set
			{
				this.contentSettingsObject.RetentionAction = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RetentionPolicy")]
		public EnhancedTimeSpan? AgeLimitForRetention
		{
			get
			{
				return this.contentSettingsObject.AgeLimitForRetention;
			}
			set
			{
				this.contentSettingsObject.AgeLimitForRetention = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RetentionPolicy")]
		public bool JournalingEnabled
		{
			get
			{
				return this.contentSettingsObject.JournalingEnabled;
			}
			set
			{
				this.contentSettingsObject.JournalingEnabled = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RetentionPolicy")]
		public RecipientIdParameter AddressForJournaling
		{
			get
			{
				return (RecipientIdParameter)base.Fields["AddressForJournaling"];
			}
			set
			{
				base.Fields["AddressForJournaling"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RetentionPolicy")]
		public JournalingFormat MessageFormatForJournaling
		{
			get
			{
				return this.contentSettingsObject.MessageFormatForJournaling;
			}
			set
			{
				this.contentSettingsObject.MessageFormatForJournaling = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "RetentionPolicy")]
		public string LabelForJournaling
		{
			get
			{
				return this.contentSettingsObject.LabelForJournaling;
			}
			set
			{
				this.contentSettingsObject.LabelForJournaling = value;
			}
		}

		public NewRetentionPolicyTag()
		{
			this.contentSettingsObject = new ElcContentSettings();
			this.contentSettingsObject.StampPersistableDefaultValues();
			this.contentSettingsObject.ResetChangeTracking();
			this.contentSettingsObject.MessageClass = ElcMessageClass.AllMailboxContent;
			this.contentSettingsObject.RetentionAction = RetentionActionType.DeleteAndAllowRecovery;
		}

		protected override IConfigurable PrepareDataObject()
		{
			if (!this.IgnoreDehydratedFlag && SharedConfiguration.IsDehydratedConfiguration(base.CurrentOrganizationId))
			{
				base.WriteError(new ArgumentException(Strings.ErrorWriteOpOnDehydratedTenant), ErrorCategory.InvalidArgument, this.DataObject.Identity);
			}
			RetentionPolicyTag retentionPolicyTag = (RetentionPolicyTag)base.PrepareDataObject();
			IConfigurationSession session = base.DataSession as IConfigurationSession;
			retentionPolicyTag.SetId(session, base.Name);
			if (base.ParameterSetName == "UpgradeManagedFolder" && this.ElcFolderToUpgrade != null)
			{
				retentionPolicyTag.LegacyManagedFolder = this.ElcFolderToUpgrade.Id;
				retentionPolicyTag.LocalizedRetentionPolicyTagName = this.ElcFolderToUpgrade.LocalizedFolderName;
				retentionPolicyTag.Comment = this.ElcFolderToUpgrade.Comment;
				retentionPolicyTag.LocalizedComment = this.ElcFolderToUpgrade.LocalizedComment;
				retentionPolicyTag.MustDisplayCommentEnabled = this.ElcFolderToUpgrade.MustDisplayCommentEnabled;
				retentionPolicyTag.Type = this.ElcFolderToUpgrade.FolderType;
			}
			if (base.Fields.Contains("LocalizedRetentionPolicyTagName"))
			{
				retentionPolicyTag.LocalizedRetentionPolicyTagName = this.LocalizedRetentionPolicyTagName;
			}
			if (base.Fields.Contains("Comment"))
			{
				retentionPolicyTag.Comment = this.Comment;
			}
			if (base.Fields.Contains("LocalizedComment"))
			{
				retentionPolicyTag.LocalizedComment = this.LocalizedComment;
			}
			if (base.Fields.Contains("MustDisplayCommentEnabled"))
			{
				retentionPolicyTag.MustDisplayCommentEnabled = this.MustDisplayCommentEnabled;
			}
			if (base.Fields.Contains("Type"))
			{
				retentionPolicyTag.Type = this.Type;
			}
			if (base.Fields.Contains("RetentionId"))
			{
				retentionPolicyTag.RetentionId = this.RetentionId;
			}
			if (retentionPolicyTag.Type == ElcFolderType.ManagedCustomFolder)
			{
				retentionPolicyTag.Type = ElcFolderType.Personal;
			}
			retentionPolicyTag.SystemTag = this.SystemTag;
			if (NewRetentionPolicyTag.MessageClassNameMaps.ContainsKey(this.contentSettingsObject.MessageClass))
			{
				this.contentSettingsObject.MessageClass = NewRetentionPolicyTag.MessageClassNameMaps[this.contentSettingsObject.MessageClass];
			}
			if (!NewRetentionPolicyTag.MessageClassNameMaps.Values.Contains(this.contentSettingsObject.MessageClass, StringComparer.OrdinalIgnoreCase))
			{
				base.WriteError(new RetentionPolicyTagTaskException(Strings.MessageClassIsNotValid(this.contentSettingsObject.MessageClass)), ErrorCategory.InvalidArgument, null);
			}
			string text = base.Name;
			if (text.Length > 60)
			{
				text = text.Substring(0, 60);
			}
			text += "_cs";
			this.contentSettingsObject.SetId(retentionPolicyTag.Id.GetChildId(text));
			if (base.ParameterSetName != "UpgradeManagedFolder")
			{
				this.contentSettingsObject.RetentionEnabled = this.RetentionEnabled;
			}
			if (this.JournalingEnabled && this.AddressForJournaling != null)
			{
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, base.OrganizationId.ToADSessionSettings(), 481, "PrepareDataObject", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Elc\\NewRetentionPolicyTag.cs");
				ADRecipient adrecipient = (ADRecipient)base.GetDataObject<ADRecipient>(this.AddressForJournaling, tenantOrRootOrgRecipientSession, null, new LocalizedString?(Strings.ErrorMailboxAddressNotFound(this.AddressForJournaling.ToString())), new LocalizedString?(Strings.ErrorMailboxAddressNotUnique(this.AddressForJournaling.ToString())));
				if (adrecipient.EmailAddresses == null || adrecipient.EmailAddresses.FindPrimary(ProxyAddressPrefix.Smtp) == null)
				{
					base.WriteError(new ArgumentException(Strings.SmtpAddressMissingForAutocopy(this.AddressForJournaling.ToString()), "AddressForJournaling"), ErrorCategory.InvalidData, this);
				}
				this.contentSettingsObject.AddressForJournaling = adrecipient.Id;
			}
			if (this.DataObject.Type != ElcFolderType.All && this.DataObject.Type != ElcFolderType.Personal && this.DataObject.Type != ElcFolderType.RecoverableItems && this.RetentionAction == RetentionActionType.MoveToArchive)
			{
				base.WriteError(new RetentionPolicyTagTaskException(Strings.ErrorMoveToArchiveAppliedToSystemFolder), ErrorCategory.InvalidArgument, null);
			}
			return retentionPolicyTag;
		}

		protected override void WriteResult()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject.Identity
			});
			this.nonAtomicTagObject = this.DataObject;
			base.WriteVerbose(TaskVerboseStringHelper.GetReadObjectVerboseString(this.DataObject.Identity, base.DataSession, typeof(RetentionPolicyTag)));
			RetentionPolicyTag retentionPolicyTag = null;
			try
			{
				IConfigurationSession configurationSession = base.DataSession as IConfigurationSession;
				configurationSession.SessionSettings.IsSharedConfigChecked = true;
				retentionPolicyTag = configurationSession.Read<RetentionPolicyTag>(this.DataObject.Id);
			}
			finally
			{
				base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(base.DataSession));
			}
			if (retentionPolicyTag == null)
			{
				base.WriteError(new ManagementObjectNotFoundException(base.GetErrorMessageObjectNotFound(this.ResolveIdentityString(this.DataObject.Identity), typeof(RetentionPolicyTag).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), (ErrorCategory)1003, this.DataObject.Identity);
			}
			IConfigurationSession session = base.DataSession as IConfigurationSession;
			this.MakeContentSettingUniqueAndSave(session, this.DataObject, this.contentSettingsObject, this.contentSettingsObject.Name);
			this.nonAtomicTagObject = null;
			PresentationRetentionPolicyTag result = new PresentationRetentionPolicyTag(retentionPolicyTag, this.contentSettingsObject);
			this.WriteResult(result);
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (this.IsDefaultAutoGroupPolicyTag && this.IsDefaultModeratedRecipientsPolicyTag)
			{
				base.WriteError(new ArgumentException(Strings.ErrorMultipleDefaultRetentionPolicyTag), ErrorCategory.InvalidArgument, this.DataObject.Identity);
			}
			if (this.IsDefaultAutoGroupPolicyTag)
			{
				this.DataObject.IsDefaultAutoGroupPolicyTag = true;
				this.existingDefaultPolicyTags = ApprovalUtils.GetDefaultRetentionPolicyTag((IConfigurationSession)base.DataSession, ApprovalApplicationId.AutoGroup, int.MaxValue);
			}
			else if (this.IsDefaultModeratedRecipientsPolicyTag)
			{
				this.DataObject.IsDefaultModeratedRecipientsPolicyTag = true;
				this.existingDefaultPolicyTags = ApprovalUtils.GetDefaultRetentionPolicyTag((IConfigurationSession)base.DataSession, ApprovalApplicationId.ModeratedRecipient, int.MaxValue);
			}
			if (base.ParameterSetName == "UpgradeManagedFolder" && this.ElcFolderToUpgrade == null)
			{
				return;
			}
			if (Datacenter.IsMicrosoftHostedOnly(false))
			{
				List<RetentionPolicyTag> allRetentionTags = AdTagReader.GetAllRetentionTags(this.ConfigurationSession, base.OrganizationId);
				if (allRetentionTags.Count >= 500)
				{
					base.WriteError(new RetentionPolicyTagTaskException(Strings.ErrorTenantRetentionTagLimitReached(500)), ErrorCategory.InvalidOperation, this.DataObject);
				}
			}
			if (this.DataObject.Type == ElcFolderType.RecoverableItems && !this.contentSettingsObject.RetentionAction.Equals(RetentionActionType.MoveToArchive))
			{
				base.WriteError(new ArgumentException(Strings.ErrorDumpsterTagWrongRetentionAction), ErrorCategory.InvalidArgument, this);
			}
			if (this.DataObject.Type != ElcFolderType.All && !this.contentSettingsObject.MessageClass.Equals(ElcMessageClass.AllMailboxContent))
			{
				base.WriteError(new RetentionPolicyTagTaskException(Strings.ErrorOnlyDefaultTagAllowCustomizedMessageClass), ErrorCategory.InvalidOperation, this.DataObject);
			}
			string tagName;
			if (this.DataObject.RetentionId != Guid.Empty && !(base.DataSession as IConfigurationSession).CheckForRetentionTagWithConflictingRetentionId(this.DataObject.RetentionId, out tagName))
			{
				base.WriteError(new RetentionPolicyTagTaskException(Strings.ErrorRetentionIdConflictsWithRetentionTag(this.DataObject.RetentionId.ToString(), tagName)), ErrorCategory.InvalidOperation, this.DataObject);
			}
			ValidationError[] array = this.contentSettingsObject.Validate();
			if (array.Length > 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					this.WriteError(new DataValidationException(array[i]), (ErrorCategory)1003, this.contentSettingsObject.Identity, array.Length - 1 == i);
				}
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.DataObject != null && SharedConfiguration.IsSharedConfiguration(this.DataObject.OrganizationId) && !base.ShouldContinue(Strings.ConfirmSharedConfiguration(this.DataObject.OrganizationId.OrganizationalUnit.Name)))
			{
				TaskLogger.LogExit();
				return;
			}
			if (this.existingDefaultPolicyTags != null && this.existingDefaultPolicyTags.Count > 0)
			{
				RetentionPolicyTagUtility.ClearDefaultPolicyTag(base.DataSession as IConfigurationSession, this.existingDefaultPolicyTags, this.DataObject.IsDefaultAutoGroupPolicyTag ? ApprovalApplicationId.AutoGroup : ApprovalApplicationId.ModeratedRecipient);
			}
			try
			{
				if (base.ParameterSetName == "RetentionPolicy")
				{
					base.InternalProcessRecord();
				}
				else if (base.ParameterSetName == "UpgradeManagedFolder")
				{
					this.ElcFolderToUpgrade = (base.GetDataObject<ELCFolder>(this.ManagedFolderToUpgrade, base.DataSession, null, new LocalizedString?(Strings.ErrorElcFolderNotFound(this.ManagedFolderToUpgrade.ToString())), new LocalizedString?(Strings.ErrorAmbiguousElcFolderId(this.ManagedFolderToUpgrade.ToString()))) as ELCFolder);
					ElcContentSettings[] array = this.ElcFolderToUpgrade.GetELCContentSettings().ToArray<ElcContentSettings>();
					bool flag = this.DataObject.Type == ElcFolderType.All;
					if (array.Length > 0)
					{
						this.upgradedContentSettings = this.GetUpgradedConentSettings(array, flag).ToArray<ElcContentSettings>();
						if (this.upgradedContentSettings.Length < 1)
						{
							base.WriteError(new RetentionPolicyTagTaskException(Strings.ErrorCannotUpgradeManagedFolder(this.ElcFolderToUpgrade.Name)), ErrorCategory.InvalidOperation, null);
						}
					}
					else
					{
						this.upgradedContentSettings = new ElcContentSettings[]
						{
							new ElcContentSettings
							{
								RetentionEnabled = false,
								MessageClass = ElcMessageClass.AllMailboxContent
							}
						};
					}
					if (flag)
					{
						Array.Sort<ElcContentSettings>(this.upgradedContentSettings, delegate(ElcContentSettings x, ElcContentSettings y)
						{
							if (string.Compare(x.MessageClass, y.MessageClass, true) == 0)
							{
								return 0;
							}
							if (x.MessageClass == ElcMessageClass.AllMailboxContent)
							{
								return -1;
							}
							if (y.MessageClass == ElcMessageClass.AllMailboxContent)
							{
								return 1;
							}
							return x.MessageClass.Split(new char[]
							{
								'.'
							}).Length - y.MessageClass.Split(new char[]
							{
								'.'
							}).Length;
						});
					}
					RetentionPolicyTag dataObject = this.DataObject;
					IConfigurationSession session = base.DataSession as IConfigurationSession;
					for (int i = 0; i < this.upgradedContentSettings.Length; i++)
					{
						this.contentSettingsObject = this.upgradedContentSettings[i];
						this.InternalValidate();
						base.InternalProcessRecord();
						this.DataObject = new RetentionPolicyTag();
						this.DataObject.CopyChangesFrom(dataObject);
						base.Name = this.GetUniqueName<RetentionPolicyTag>(session, dataObject.Id.Parent, dataObject.Name, i + 1);
					}
				}
			}
			catch (DataSourceTransientException exception)
			{
				base.WriteError(exception, (ErrorCategory)1002, null);
			}
			finally
			{
				if (this.nonAtomicTagObject != null)
				{
					base.DataSession.Delete(this.nonAtomicTagObject);
				}
			}
		}

		private ElcContentSettings[] GetUpgradedConentSettings(ElcContentSettings[] folderContentSettings, bool defaultTag)
		{
			List<ElcContentSettings> upgradedSettings = new List<ElcContentSettings>();
			ElcContentSettings[] array = (from x in folderContentSettings
			where x.RetentionEnabled || x.JournalingEnabled
			select x).ToArray<ElcContentSettings>();
			folderContentSettings.Except(array).ForEach(delegate(ElcContentSettings x)
			{
				this.WriteWarning(Strings.CouldNotUpgradeDisabledContentSettings(x.Name));
			});
			RetentionActionType[] allowedRetentionActions = new RetentionActionType[]
			{
				RetentionActionType.PermanentlyDelete,
				RetentionActionType.DeleteAndAllowRecovery,
				RetentionActionType.MoveToDeletedItems
			};
			if (!defaultTag)
			{
				ElcContentSettings elcContentSettings = new ElcContentSettings();
				elcContentSettings.JournalingEnabled = false;
				elcContentSettings.RetentionEnabled = false;
				elcContentSettings.RetentionAction = RetentionActionType.PermanentlyDelete;
				ElcContentSettings[] array2 = (from x in array
				where x.RetentionEnabled
				select x).ToArray<ElcContentSettings>();
				ElcContentSettings[] array3 = (from x in array2
				where allowedRetentionActions.Any((RetentionActionType y) => x.RetentionAction == y)
				select x).ToArray<ElcContentSettings>();
				array2.Except(array3).ForEach(delegate(ElcContentSettings x)
				{
					this.WriteWarning(Strings.CouldNotUpgradeSpecificContentSetting(x.Name));
				});
				ElcContentSettings[] array4 = (from x in array3
				where x.TriggerForRetention == RetentionDateType.WhenDelivered
				select x).ToArray<ElcContentSettings>();
				array3.Except(array4).ForEach(delegate(ElcContentSettings x)
				{
					this.WriteWarning(Strings.CouldNotUpgradeRetentionTrigger(x.TriggerForRetention.ToString(), x.Name, RetentionDateType.WhenDelivered.ToString()));
				});
				array4.Aggregate(elcContentSettings, delegate(ElcContentSettings a, ElcContentSettings x)
				{
					if (a.AgeLimitForRetention == null)
					{
						a.AgeLimitForRetention = x.AgeLimitForRetention;
					}
					else if (x.AgeLimitForRetention != null && a.AgeLimitForRetention < x.AgeLimitForRetention)
					{
						a.AgeLimitForRetention = x.AgeLimitForRetention;
					}
					if (Array.FindIndex<RetentionActionType>(allowedRetentionActions, (RetentionActionType y) => y == a.RetentionAction) < Array.FindIndex<RetentionActionType>(allowedRetentionActions, (RetentionActionType y) => y == x.RetentionAction))
					{
						a.RetentionAction = x.RetentionAction;
					}
					a.RetentionEnabled = true;
					return a;
				});
				if (elcContentSettings.RetentionEnabled)
				{
					elcContentSettings.TriggerForRetention = RetentionDateType.WhenDelivered;
					elcContentSettings.MessageClass = ElcMessageClass.AllMailboxContent;
					(from x in array4
					where string.Compare(x.MessageClass, ElcMessageClass.AllMailboxContent, StringComparison.OrdinalIgnoreCase) != 0
					select x).ForEach(delegate(ElcContentSettings x)
					{
						this.WriteWarning(Strings.ChangedMessageClass(x.Name, x.MessageClass));
					});
				}
				ElcContentSettings[] array5 = (from x in array
				where x.JournalingEnabled
				select x).ToArray<ElcContentSettings>();
				if (array5.Length > 0)
				{
					elcContentSettings.JournalingEnabled = true;
					elcContentSettings.MessageFormatForJournaling = array5[0].MessageFormatForJournaling;
					elcContentSettings.AddressForJournaling = array5[0].AddressForJournaling;
					elcContentSettings.LabelForJournaling = array5[0].LabelForJournaling;
					array5.Skip(1).ForEach(delegate(ElcContentSettings x)
					{
						this.WriteWarning(Strings.CouldNotUpgradeJournaling(x.Name));
					});
				}
				if (elcContentSettings.RetentionEnabled || elcContentSettings.JournalingEnabled)
				{
					upgradedSettings.Add(elcContentSettings);
				}
			}
			else
			{
				array.ForEach(delegate(ElcContentSettings x)
				{
					ElcContentSettings elcContentSettings2 = new ElcContentSettings();
					if (x.RetentionEnabled)
					{
						if (allowedRetentionActions.Any((RetentionActionType y) => x.RetentionAction == y))
						{
							if (x.TriggerForRetention != RetentionDateType.WhenDelivered)
							{
								this.WriteWarning(Strings.CouldNotUpgradeRetentionTrigger(x.TriggerForRetention.ToString(), x.Name, RetentionDateType.WhenDelivered.ToString()));
							}
							else
							{
								elcContentSettings2.RetentionAction = x.RetentionAction;
								elcContentSettings2.AgeLimitForRetention = x.AgeLimitForRetention;
								elcContentSettings2.TriggerForRetention = RetentionDateType.WhenDelivered;
								elcContentSettings2.RetentionEnabled = x.RetentionEnabled;
								if (x.MessageClass.Equals(ElcMessageClass.AllMailboxContent, StringComparison.OrdinalIgnoreCase) || x.MessageClass.Equals(ElcMessageClass.VoiceMail, StringComparison.OrdinalIgnoreCase))
								{
									elcContentSettings2.MessageClass = x.MessageClass;
								}
								else
								{
									elcContentSettings2.MessageClass = ElcMessageClass.AllMailboxContent;
									this.WriteWarning(Strings.ChangedMessageClass(x.Name, x.MessageClass));
								}
							}
						}
						else
						{
							this.WriteWarning(Strings.CouldNotUpgradeSpecificContentSetting(x.Name));
						}
					}
					if (x.JournalingEnabled)
					{
						elcContentSettings2.MessageFormatForJournaling = x.MessageFormatForJournaling;
						elcContentSettings2.JournalingEnabled = x.JournalingEnabled;
						elcContentSettings2.AddressForJournaling = x.AddressForJournaling;
						elcContentSettings2.LabelForJournaling = x.LabelForJournaling;
						if (x.MessageClass.Equals(ElcMessageClass.AllMailboxContent, StringComparison.OrdinalIgnoreCase) || x.MessageClass.Equals(ElcMessageClass.VoiceMail, StringComparison.OrdinalIgnoreCase))
						{
							elcContentSettings2.MessageClass = x.MessageClass;
						}
						else
						{
							elcContentSettings2.MessageClass = ElcMessageClass.AllMailboxContent;
							this.WriteWarning(Strings.ChangedMessageClass(x.Name, x.MessageClass));
						}
					}
					if (elcContentSettings2.RetentionEnabled || elcContentSettings2.JournalingEnabled)
					{
						upgradedSettings.Add(elcContentSettings2);
					}
				});
			}
			return upgradedSettings.ToArray();
		}

		private static string GenerateName(RetentionPolicyTag newTag, ElcContentSettings oldContentSettings)
		{
			StringBuilder stringBuilder = new StringBuilder(63, 63);
			stringBuilder.Append(newTag.Name);
			stringBuilder.Append('-');
			stringBuilder.Append(oldContentSettings.Name);
			if (stringBuilder.Length >= 64)
			{
				return stringBuilder.ToString(0, 63);
			}
			return stringBuilder.ToString();
		}

		private string GetUniqueName<TObject>(IConfigurationSession session, ADObjectId parentId, string baseName, int hint) where TObject : ADConfigurationObject, new()
		{
			string text = string.Format("{0}-{1}", baseName, hint);
			for (int i = hint; i < hint + 10; i++)
			{
				if (session.Read<TObject>(parentId.GetChildId(text)) == null)
				{
					return text;
				}
				text = string.Format("{0}-{1}", baseName, i + 1);
			}
			return string.Format("{0}-{1}", baseName, Guid.NewGuid().ToString());
		}

		private void MakeContentSettingUniqueAndSave(IConfigurationSession session, RetentionPolicyTag newTag, ElcContentSettings newContentSettings, string baseName)
		{
			if (newContentSettings != null)
			{
				if (!newTag.OrganizationId.Equals(OrganizationId.ForestWideOrgId) && newContentSettings.OrganizationId.Equals(OrganizationId.ForestWideOrgId))
				{
					newContentSettings.OrganizationId = newTag.OrganizationId;
				}
				bool flag = false;
				int num = 1;
				while (!flag || num < 10)
				{
					try
					{
						session.Save(newContentSettings);
						flag = true;
					}
					catch (ADObjectAlreadyExistsException)
					{
						newContentSettings.SetId(newTag.Id.GetChildId(baseName + "-" + num.ToString()));
					}
					num++;
				}
				if (!flag)
				{
					base.ThrowTerminatingError(new CouldNotSaveContentSetting(baseName), ErrorCategory.InvalidData, null);
				}
			}
		}

		private const string propRetentionEnabled = "RetentionEnabled";

		private const string propAddressForJournaling = "AddressForJournaling";

		private const int TenantTagLimit = 500;

		private IList<RetentionPolicyTag> existingDefaultPolicyTags;

		internal static Dictionary<string, string> MessageClassNameMaps = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"*",
				ElcMessageClass.AllMailboxContent
			},
			{
				"AllMailboxContent",
				ElcMessageClass.AllMailboxContent
			},
			{
				"VoiceMail",
				ElcMessageClass.VoiceMail
			}
		};

		private ElcContentSettings contentSettingsObject;

		private ElcContentSettings[] upgradedContentSettings;

		private RetentionPolicyTag nonAtomicTagObject;

		private ELCFolder ElcFolderToUpgrade;
	}
}
