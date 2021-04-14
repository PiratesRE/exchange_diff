using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.Common
{
	public abstract class SetMailEnabledRecipientObjectTask<TIdentity, TPublicObject, TDataObject> : SetRecipientObjectTask<TIdentity, TPublicObject, TDataObject> where TIdentity : IIdentityParameter, new() where TPublicObject : MailEnabledRecipient, new() where TDataObject : ADRecipient, new()
	{
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<DeliveryRecipientIdParameter> AcceptMessagesOnlyFrom
		{
			get
			{
				return (MultiValuedProperty<DeliveryRecipientIdParameter>)base.Fields[ADRecipientSchema.AcceptMessagesOnlyFrom];
			}
			set
			{
				base.Fields[ADRecipientSchema.AcceptMessagesOnlyFrom] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<DeliveryRecipientIdParameter> AcceptMessagesOnlyFromDLMembers
		{
			get
			{
				return (MultiValuedProperty<DeliveryRecipientIdParameter>)base.Fields[ADRecipientSchema.AcceptMessagesOnlyFromDLMembers];
			}
			set
			{
				base.Fields[ADRecipientSchema.AcceptMessagesOnlyFromDLMembers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<DeliveryRecipientIdParameter> AcceptMessagesOnlyFromSendersOrMembers
		{
			get
			{
				return (MultiValuedProperty<DeliveryRecipientIdParameter>)base.Fields[ADRecipientSchema.AcceptMessagesOnlyFromSendersOrMembers];
			}
			set
			{
				base.Fields[ADRecipientSchema.AcceptMessagesOnlyFromSendersOrMembers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailboxIdParameter ArbitrationMailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields[ADRecipientSchema.ArbitrationMailbox];
			}
			set
			{
				base.Fields[ADRecipientSchema.ArbitrationMailbox] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<RecipientIdParameter> GrantSendOnBehalfTo
		{
			get
			{
				return (MultiValuedProperty<RecipientIdParameter>)base.Fields[ADRecipientSchema.GrantSendOnBehalfTo];
			}
			set
			{
				base.Fields[ADRecipientSchema.GrantSendOnBehalfTo] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<ModeratorIDParameter> ModeratedBy
		{
			get
			{
				return (MultiValuedProperty<ModeratorIDParameter>)base.Fields[ADRecipientSchema.ModeratedBy];
			}
			set
			{
				base.Fields[ADRecipientSchema.ModeratedBy] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<DeliveryRecipientIdParameter> RejectMessagesFrom
		{
			get
			{
				return (MultiValuedProperty<DeliveryRecipientIdParameter>)base.Fields[ADRecipientSchema.RejectMessagesFrom];
			}
			set
			{
				base.Fields[ADRecipientSchema.RejectMessagesFrom] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<DeliveryRecipientIdParameter> RejectMessagesFromDLMembers
		{
			get
			{
				return (MultiValuedProperty<DeliveryRecipientIdParameter>)base.Fields[ADRecipientSchema.RejectMessagesFromDLMembers];
			}
			set
			{
				base.Fields[ADRecipientSchema.RejectMessagesFromDLMembers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<DeliveryRecipientIdParameter> RejectMessagesFromSendersOrMembers
		{
			get
			{
				return (MultiValuedProperty<DeliveryRecipientIdParameter>)base.Fields[ADRecipientSchema.RejectMessagesFromSendersOrMembers];
			}
			set
			{
				base.Fields[ADRecipientSchema.RejectMessagesFromSendersOrMembers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<DeliveryRecipientIdParameter> BypassModerationFromSendersOrMembers
		{
			get
			{
				return (MultiValuedProperty<DeliveryRecipientIdParameter>)base.Fields[ADRecipientSchema.BypassModerationFromSendersOrMembers];
			}
			set
			{
				base.Fields[ADRecipientSchema.BypassModerationFromSendersOrMembers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CreateDTMFMap
		{
			get
			{
				return (bool)(base.Fields["CreateDTMFMap"] ?? false);
			}
			set
			{
				base.Fields["CreateDTMFMap"] = value;
			}
		}

		protected override bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return true;
			}
		}

		protected override bool ShouldUpgradeExchangeVersion(ADObject adObject)
		{
			return base.ShouldUpgradeExchangeVersion(adObject) || adObject.IsModified(ADRecipientSchema.SendModerationNotifications) || adObject.IsModified(ADRecipientSchema.ModerationFlags) || adObject.IsModified(ADRecipientSchema.ModerationEnabled) || base.Fields.IsModified(ADRecipientSchema.ModeratedBy) || base.Fields.IsModified(ADRecipientSchema.ArbitrationMailbox);
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			TPublicObject dynamicParametersInstance = (TPublicObject)((object)this.GetDynamicParameters());
			if (dynamicParametersInstance.EmailAddresses.Changed && dynamicParametersInstance.IsChanged(ADRecipientSchema.PrimarySmtpAddress))
			{
				base.ThrowTerminatingError(new RecipientTaskException(Strings.ErrorPrimarySmtpAndEmailAddressesSpecified), ExchangeErrorCategory.Client, null);
			}
			MultiLinkSyncHelper.ValidateIncompatibleParameters(base.Fields, this.GetIncompatibleParametersDictionary(), new Task.ErrorLoggerDelegate(base.ThrowTerminatingError));
			this.ValidateMailTipsParameters(dynamicParametersInstance);
			TaskLogger.LogExit();
		}

		protected override void ResolveLocalSecondaryIdentities()
		{
			base.ResolveLocalSecondaryIdentities();
			TPublicObject tpublicObject = (TPublicObject)((object)this.GetDynamicParameters());
			this.SetMultiReferenceParameter<DeliveryRecipientIdParameter>(ADRecipientSchema.AcceptMessagesOnlyFrom, this.AcceptMessagesOnlyFrom, tpublicObject, new GetRecipientDelegate<DeliveryRecipientIdParameter>(this.GetRecipient), new ValidateRecipientDelegate(this.ValidateMessageDeliveryRestrictionIndividual));
			this.SetMultiReferenceParameter<DeliveryRecipientIdParameter>(ADRecipientSchema.AcceptMessagesOnlyFromDLMembers, this.AcceptMessagesOnlyFromDLMembers, tpublicObject, new GetRecipientDelegate<DeliveryRecipientIdParameter>(this.GetRecipient), new ValidateRecipientDelegate(this.ValidateMessageDeliveryRestrictionGroup));
			if (base.Fields.IsModified(ADRecipientSchema.AcceptMessagesOnlyFromSendersOrMembers))
			{
				tpublicObject.AcceptMessagesOnlyFrom.Clear();
				tpublicObject.AcceptMessagesOnlyFromDLMembers.Clear();
				if (this.AcceptMessagesOnlyFromSendersOrMembers != null)
				{
					MultiValuedProperty<ADObjectId> multiValuedProperty;
					MultiValuedProperty<ADObjectId> multiValuedProperty2;
					SetMailEnabledRecipientObjectTask<TIdentity, TPublicObject, TDataObject>.ResolveAndSeparateMessageDeliveryRestrictionRecipientIdentities(this.AcceptMessagesOnlyFromSendersOrMembers, "AcceptMessagesOnlyFromSendersOrMembers", base.TenantGlobalCatalogSession, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), new Task.ErrorLoggerDelegate(base.WriteError), out multiValuedProperty, out multiValuedProperty2);
					if (multiValuedProperty != null)
					{
						tpublicObject.AcceptMessagesOnlyFromDLMembers = multiValuedProperty;
					}
					if (multiValuedProperty2 != null)
					{
						tpublicObject.AcceptMessagesOnlyFrom = multiValuedProperty2;
					}
				}
			}
			this.SetMultiReferenceParameter<DeliveryRecipientIdParameter>(ADRecipientSchema.RejectMessagesFrom, this.RejectMessagesFrom, tpublicObject, new GetRecipientDelegate<DeliveryRecipientIdParameter>(this.GetRecipient), new ValidateRecipientDelegate(this.ValidateMessageDeliveryRestrictionIndividual));
			this.SetMultiReferenceParameter<DeliveryRecipientIdParameter>(ADRecipientSchema.RejectMessagesFromDLMembers, this.RejectMessagesFromDLMembers, tpublicObject, new GetRecipientDelegate<DeliveryRecipientIdParameter>(this.GetRecipient), new ValidateRecipientDelegate(this.ValidateMessageDeliveryRestrictionGroup));
			if (base.Fields.IsModified(ADRecipientSchema.RejectMessagesFromSendersOrMembers))
			{
				tpublicObject.RejectMessagesFrom.Clear();
				tpublicObject.RejectMessagesFromDLMembers.Clear();
				if (this.RejectMessagesFromSendersOrMembers != null)
				{
					MultiValuedProperty<ADObjectId> multiValuedProperty3;
					MultiValuedProperty<ADObjectId> multiValuedProperty4;
					SetMailEnabledRecipientObjectTask<TIdentity, TPublicObject, TDataObject>.ResolveAndSeparateMessageDeliveryRestrictionRecipientIdentities(this.RejectMessagesFromSendersOrMembers, "RejectMessagesFromSendersOrMembers", base.TenantGlobalCatalogSession, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), new Task.ErrorLoggerDelegate(base.WriteError), out multiValuedProperty3, out multiValuedProperty4);
					if (multiValuedProperty3 != null)
					{
						tpublicObject.RejectMessagesFromDLMembers = multiValuedProperty3;
					}
					if (multiValuedProperty4 != null)
					{
						tpublicObject.RejectMessagesFrom = multiValuedProperty4;
					}
				}
			}
			if (base.Fields.IsModified(ADRecipientSchema.BypassModerationFromSendersOrMembers))
			{
				tpublicObject.BypassModerationFrom.Clear();
				tpublicObject.BypassModerationFromDLMembers.Clear();
				if (this.BypassModerationFromSendersOrMembers != null)
				{
					MultiValuedProperty<ADObjectId> multiValuedProperty5;
					MultiValuedProperty<ADObjectId> multiValuedProperty6;
					SetMailEnabledRecipientObjectTask<TIdentity, TPublicObject, TDataObject>.ResolveAndSeparateMessageDeliveryRestrictionRecipientIdentities(this.BypassModerationFromSendersOrMembers, "BypassModerationFromSendersOrMembers", base.TenantGlobalCatalogSession, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), new Task.ErrorLoggerDelegate(base.WriteError), out multiValuedProperty5, out multiValuedProperty6);
					if (multiValuedProperty5 != null)
					{
						tpublicObject.BypassModerationFromDLMembers = multiValuedProperty5;
					}
					if (multiValuedProperty6 != null)
					{
						tpublicObject.BypassModerationFrom = multiValuedProperty6;
					}
				}
			}
			this.SetMultiReferenceParameter<RecipientIdParameter>(ADRecipientSchema.GrantSendOnBehalfTo, this.GrantSendOnBehalfTo, tpublicObject, new GetRecipientDelegate<RecipientIdParameter>(this.GetRecipient), new ValidateRecipientDelegate(this.ValidateGrantSendOnBehalfTo));
			if (tpublicObject.IsChanged(ADRecipientSchema.PrimarySmtpAddress) && tpublicObject.IsChanged(ADRecipientSchema.WindowsEmailAddress) && tpublicObject.PrimarySmtpAddress != tpublicObject.WindowsEmailAddress)
			{
				base.ThrowTerminatingError(new TaskArgumentException(Strings.ErrorPrimarySmtpAndWindowsAddressDifferent), ExchangeErrorCategory.Client, null);
			}
			if (base.Fields.Contains(ADRecipientSchema.ModeratedBy))
			{
				MultiValuedProperty<ModeratorIDParameter> multiValuedProperty7 = (MultiValuedProperty<ModeratorIDParameter>)base.Fields[ADRecipientSchema.ModeratedBy];
				if (multiValuedProperty7 != null && multiValuedProperty7.Count > 25)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorTooManyModerators(25)), ExchangeErrorCategory.Client, null);
				}
				this.SetMultiReferenceParameter<ModeratorIDParameter>(ADRecipientSchema.ModeratedBy, this.ModeratedBy, tpublicObject, new GetRecipientDelegate<ModeratorIDParameter>(this.GetRecipient), new ValidateRecipientDelegate(RecipientTaskHelper.ValidateModeratedBy));
			}
			if (this.ArbitrationMailbox != null)
			{
				this.arbitrationMbx = (ADRecipient)base.GetDataObject<ADRecipient>(this.ArbitrationMailbox, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorMailboxNotFound(this.ArbitrationMailbox.ToString())), new LocalizedString?(Strings.ErrorMailboxNotUnique(this.ArbitrationMailbox.ToString())), ExchangeErrorCategory.Client);
				if (MultiValuedPropertyBase.IsNullOrEmpty((ADMultiValuedProperty<ADObjectId>)this.arbitrationMbx[ADUserSchema.ApprovalApplications]))
				{
					base.ThrowTerminatingError(new RecipientTaskException(Strings.ErrorInvalidArbitrationMbxTypeForModerationAndAutogroup(this.arbitrationMbx.Identity.ToString())), ExchangeErrorCategory.Client, null);
				}
				if (this.arbitrationMbx.RecipientTypeDetails != RecipientTypeDetails.ArbitrationMailbox)
				{
					base.ThrowTerminatingError(new RecipientTaskException(Strings.ErrorInvalidArbitrationMbxType(this.ArbitrationMailbox.ToString())), ExchangeErrorCategory.Client, null);
				}
			}
		}

		private Dictionary<object, ArrayList> GetIncompatibleParametersDictionary()
		{
			Dictionary<object, ArrayList> dictionary = new Dictionary<object, ArrayList>();
			dictionary[ADRecipientSchema.AcceptMessagesOnlyFromSendersOrMembers] = new ArrayList
			{
				ADRecipientSchema.AcceptMessagesOnlyFrom,
				ADRecipientSchema.AcceptMessagesOnlyFromDLMembers
			};
			dictionary[ADRecipientSchema.RejectMessagesFromSendersOrMembers] = new ArrayList
			{
				ADRecipientSchema.RejectMessagesFrom,
				ADRecipientSchema.RejectMessagesFromDLMembers
			};
			return dictionary;
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADRecipient adrecipient = (ADRecipient)base.PrepareDataObject();
			if (!adrecipient.EmailAddressPolicyEnabled && adrecipient.PrimarySmtpAddress != adrecipient.OriginalPrimarySmtpAddress && adrecipient.WindowsEmailAddress == adrecipient.OriginalWindowsEmailAddress)
			{
				adrecipient.WindowsEmailAddress = adrecipient.PrimarySmtpAddress;
			}
			if (this.CreateDTMFMap)
			{
				adrecipient.PopulateDtmfMap(true);
			}
			if (base.Fields.IsModified(ADRecipientSchema.ArbitrationMailbox))
			{
				if (this.arbitrationMbx != null)
				{
					if (!adrecipient.OrganizationId.Equals(this.arbitrationMbx.OrganizationId))
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorArbitrationMbxCrossOrg(this.arbitrationMbx.Identity.ToString())), ExchangeErrorCategory.Client, adrecipient.Identity);
					}
					adrecipient.ArbitrationMailbox = this.arbitrationMbx.Id;
				}
				else
				{
					adrecipient.ArbitrationMailbox = null;
				}
			}
			MultiValuedProperty<string> source;
			if (!this.isDeletingMailTipTranslations && this.isDeletingDefaultMailTip && adrecipient.TryGetOriginalValue<MultiValuedProperty<string>>(ADRecipientSchema.MailTipTranslations, out source))
			{
				if (source.Any((string translation) => !ADRecipient.IsDefaultTranslation(translation)))
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorMailTipRemoveDefaultAndTranslationsExist), ExchangeErrorCategory.Client, null);
				}
			}
			TaskLogger.LogExit();
			return adrecipient;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			TDataObject dataObject = this.DataObject;
			if (dataObject.IsModified(ADRecipientSchema.Alias))
			{
				TDataObject dataObject2 = this.DataObject;
				if (string.IsNullOrEmpty(dataObject2.Alias))
				{
					LocalizedException exception = new RecipientTaskException(Strings.ErrorAliasEmpty);
					ExchangeErrorCategory category = ExchangeErrorCategory.Client;
					TDataObject dataObject3 = this.DataObject;
					base.WriteError(exception, category, dataObject3.Identity);
				}
			}
			TDataObject dataObject4 = this.DataObject;
			if (dataObject4.IsChanged(ADRecipientSchema.ModeratedBy))
			{
				int num = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled ? 10 : 25;
				TDataObject dataObject5 = this.DataObject;
				if (dataObject5.ModeratedBy != null)
				{
					TDataObject dataObject6 = this.DataObject;
					if (dataObject6.ModeratedBy.Count > num)
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorTooManyModerators(num)), ExchangeErrorCategory.Client, null);
					}
				}
			}
			TaskLogger.LogExit();
		}

		internal void SetReferenceParameter<TIdentityParameter>(ADPropertyDefinition propertyBagKey, TIdentityParameter parameter, ConfigurableObject dataObject, GetRecipientDelegate<TIdentityParameter> getRecipient) where TIdentityParameter : IIdentityParameter
		{
			this.SetReferenceParameter<TIdentityParameter>(propertyBagKey, propertyBagKey, parameter, propertyBagKey.Name, dataObject, getRecipient, null);
		}

		internal void SetReferenceParameter<TIdentityParameter>(ADPropertyDefinition propertyBagKey, TIdentityParameter parameter, ConfigurableObject dataObject, GetRecipientDelegate<TIdentityParameter> getRecipient, ValidateRecipientDelegate validateRecipient) where TIdentityParameter : IIdentityParameter
		{
			this.SetReferenceParameter<TIdentityParameter>(propertyBagKey, propertyBagKey, parameter, propertyBagKey.Name, dataObject, getRecipient, validateRecipient);
		}

		internal void SetReferenceParameter<TIdentityParameter>(object sourcePropertyBagKey, ADPropertyDefinition targetPropertyBagKey, TIdentityParameter parameter, string parameterName, ConfigurableObject dataObject, GetRecipientDelegate<TIdentityParameter> getRecipient, ValidateRecipientDelegate validateRecipient) where TIdentityParameter : IIdentityParameter
		{
			if (base.Fields.IsModified(sourcePropertyBagKey))
			{
				dataObject[targetPropertyBagKey] = null;
				if (parameter != null)
				{
					this.ReferenceErrorReporter.ValidateReference(parameterName, parameter.RawIdentity, delegate(Task.ErrorLoggerDelegate writeError)
					{
						ADRecipient adrecipient = getRecipient(parameter, writeError);
						if (validateRecipient != null)
						{
							validateRecipient(adrecipient, parameter.RawIdentity, writeError);
						}
						this.parameterDictionary[adrecipient] = parameter;
						this.recipientDictionary[sourcePropertyBagKey] = adrecipient;
						dataObject[targetPropertyBagKey] = adrecipient.Identity;
					});
				}
			}
		}

		internal void SetMultiReferenceParameter<TIdentityParameter>(ADPropertyDefinition propertyBagKey, MultiValuedProperty<TIdentityParameter> parameters, ConfigurableObject dataObject, GetRecipientDelegate<TIdentityParameter> getRecipient) where TIdentityParameter : class, IIdentityParameter
		{
			this.SetMultiReferenceParameter<TIdentityParameter>(propertyBagKey, propertyBagKey, parameters, propertyBagKey.Name, dataObject, getRecipient, null);
		}

		internal void SetMultiReferenceParameter<TIdentityParameter>(ADPropertyDefinition propertyBagKey, MultiValuedProperty<TIdentityParameter> parameters, ConfigurableObject dataObject, GetRecipientDelegate<TIdentityParameter> getRecipient, ValidateRecipientDelegate validateRecipient) where TIdentityParameter : class, IIdentityParameter
		{
			this.SetMultiReferenceParameter<TIdentityParameter>(propertyBagKey, propertyBagKey, parameters, propertyBagKey.Name, dataObject, getRecipient, validateRecipient);
		}

		internal void SetMultiReferenceParameter<TIdentityParameter>(object sourcePropertyBagKey, ADPropertyDefinition targetPropertyBagKey, MultiValuedProperty<TIdentityParameter> parameters, string parameterName, ConfigurableObject dataObject, GetRecipientDelegate<TIdentityParameter> getRecipient, ValidateRecipientDelegate validateRecipient) where TIdentityParameter : class, IIdentityParameter
		{
			Func<object, TIdentityParameter> func = null;
			Func<ADRecipient, ADObjectId> func2 = null;
			Func<object, TIdentityParameter> func3 = null;
			Func<ADRecipient, ADObjectId> func4 = null;
			Func<ADRecipient, ADObjectId> func5 = null;
			if (base.Fields.IsModified(sourcePropertyBagKey))
			{
				if (parameters == null)
				{
					dataObject[targetPropertyBagKey] = null;
					return;
				}
				if (parameters.IsChangesOnlyCopy)
				{
					object[] added = parameters.Added;
					object[] removed = parameters.Removed;
					Hashtable hashtable = new Hashtable();
					if (added.Length > 0)
					{
						IEnumerable<object> source = added;
						if (func == null)
						{
							func = ((object obj) => (TIdentityParameter)((object)obj));
						}
						List<ADRecipient> recipients = this.GetRecipients<TIdentityParameter>(source.Select(func), parameterName, getRecipient, validateRecipient);
						Hashtable hashtable2 = hashtable;
						object key = "Add";
						IEnumerable<ADRecipient> source2 = recipients;
						if (func2 == null)
						{
							func2 = ((ADRecipient recipient) => recipient.OriginalId);
						}
						hashtable2.Add(key, source2.Select(func2).ToArray<ADObjectId>());
						this.recipientsDictionary[sourcePropertyBagKey] = new MultiValuedProperty<ADRecipient>(recipients);
					}
					if (removed.Length > 0)
					{
						IEnumerable<object> source3 = removed;
						if (func3 == null)
						{
							func3 = ((object obj) => (TIdentityParameter)((object)obj));
						}
						List<ADRecipient> recipients2 = this.GetRecipients<TIdentityParameter>(source3.Select(func3), parameterName, getRecipient, null);
						Hashtable hashtable3 = hashtable;
						object key2 = "Remove";
						IEnumerable<ADRecipient> source4 = recipients2;
						if (func4 == null)
						{
							func4 = ((ADRecipient recipient) => recipient.OriginalId);
						}
						hashtable3.Add(key2, source4.Select(func4).ToArray<ADObjectId>());
					}
					dataObject[targetPropertyBagKey] = new MultiValuedProperty<ADObjectId>(hashtable);
					return;
				}
				List<ADRecipient> recipients3 = this.GetRecipients<TIdentityParameter>(parameters, parameterName, getRecipient, validateRecipient);
				IEnumerable<ADRecipient> source5 = recipients3;
				if (func5 == null)
				{
					func5 = ((ADRecipient recipient) => recipient.OriginalId);
				}
				dataObject[targetPropertyBagKey] = new MultiValuedProperty<ADObjectId>(source5.Select(func5).ToArray<ADObjectId>());
				this.recipientsDictionary[sourcePropertyBagKey] = new MultiValuedProperty<ADRecipient>(recipients3);
			}
		}

		private List<ADRecipient> GetRecipients<TIdentityParameter>(IEnumerable<TIdentityParameter> parameters, string parameterName, GetRecipientDelegate<TIdentityParameter> getRecipient, ValidateRecipientDelegate validateRecipient) where TIdentityParameter : class, IIdentityParameter
		{
			List<ADRecipient> list = new List<ADRecipient>();
			HashSet<ADObjectId> recipientIds = new HashSet<ADObjectId>();
			using (IEnumerator<TIdentityParameter> enumerator = parameters.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TIdentityParameter parameter = enumerator.Current;
					IReferenceErrorReporter referenceErrorReporter = this.ReferenceErrorReporter;
					string parameterName2 = parameterName;
					TIdentityParameter parameter3 = parameter;
					referenceErrorReporter.ValidateReference(parameterName2, parameter3.RawIdentity, delegate(Task.ErrorLoggerDelegate writeError)
					{
						ADRecipient adrecipient = getRecipient(parameter, writeError);
						if (recipientIds.Contains((ADObjectId)adrecipient.Identity))
						{
							writeError(new RecipientTaskException(Strings.ErrorRecipientIdParamElementsNotUnique(parameterName, adrecipient.Id.ToString())), ExchangeErrorCategory.Client, parameter);
						}
						if (validateRecipient != null)
						{
							ValidateRecipientDelegate validateRecipient2 = validateRecipient;
							ADRecipient recipient = adrecipient;
							TIdentityParameter parameter2 = parameter;
							validateRecipient2(recipient, parameter2.RawIdentity, writeError);
						}
						recipientIds.Add((ADObjectId)adrecipient.Identity);
						list.Add(adrecipient);
						this.parameterDictionary.Add(adrecipient, parameter);
					});
				}
			}
			return list;
		}

		internal virtual ADRecipient GetRecipient(RecipientIdParameter recipientIdParameter, Task.ErrorLoggerDelegate writeError)
		{
			return (ADRecipient)base.GetDataObject<ADRecipient>(recipientIdParameter, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(recipientIdParameter.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(recipientIdParameter.ToString())), ExchangeErrorCategory.Client);
		}

		internal void ValidateGrantSendOnBehalfTo(ADRecipient recipient, string recipientId, Task.ErrorLoggerDelegate writeError)
		{
			if (recipient.RecipientType != RecipientType.UserMailbox && recipient.RecipientType != RecipientType.MailUser && (recipient.RecipientType != RecipientType.MailContact || !(recipient.MasterAccountSid != null)) && recipient.RecipientType != RecipientType.MailUniversalDistributionGroup && recipient.RecipientType != RecipientType.MailUniversalSecurityGroup)
			{
				writeError(new RecipientTaskException(Strings.ErrorGrantSendOnBehalfToRecipientTypeError(recipientId)), ExchangeErrorCategory.Client, recipientId);
			}
		}

		internal void ValidateMessageDeliveryRestrictionIndividual(ADRecipient recipient, string recipientId, Task.ErrorLoggerDelegate writeError)
		{
			if (!ADRecipient.IsAllowedDeliveryRestrictionIndividual(recipient.RecipientType))
			{
				writeError(new RecipientTaskException(Strings.ErrorIndividualRecipientNeeded(recipientId)), ExchangeErrorCategory.Client, recipientId);
			}
		}

		internal void ValidateMessageDeliveryRestrictionGroup(ADRecipient recipient, string recipientId, Task.ErrorLoggerDelegate writeError)
		{
			if (!ADRecipient.IsAllowedDeliveryRestrictionGroup(recipient.RecipientType))
			{
				writeError(new RecipientTaskException(Strings.ErrorGroupRecipientNeeded(recipientId)), ExchangeErrorCategory.Client, recipientId);
			}
		}

		internal void ValidateMultiReferenceParameter(ADPropertyDefinition propertyKey, ValidateRecipientWithBaseObjectDelegate<TDataObject> validateRecipient)
		{
			this.ValidateMultiReferenceParameter(propertyKey.Name, propertyKey, validateRecipient);
		}

		internal void ValidateMultiReferenceParameter(string propertyKey, ValidateRecipientWithBaseObjectDelegate<TDataObject> validateRecipient)
		{
			this.ValidateMultiReferenceParameter(propertyKey, propertyKey, validateRecipient);
		}

		internal void ValidateMultiReferenceParameter(string parameterName, object propertyKey, ValidateRecipientWithBaseObjectDelegate<TDataObject> validateRecipient)
		{
			if (base.Fields.IsModified(propertyKey))
			{
				SyncTaskHelper.ValidateModifiedMultiReferenceParameter<TDataObject>(parameterName, propertyKey, this.DataObject, validateRecipient, this.ReferenceErrorReporter, this.recipientsDictionary, this.parameterDictionary);
			}
		}

		internal void ValidateReferenceParameter(string propertyKey, ValidateRecipientWithBaseObjectDelegate<TDataObject> validateRecipient)
		{
			this.ValidateReferenceParameter(propertyKey, propertyKey, validateRecipient);
		}

		internal void ValidateReferenceParameter(ADPropertyDefinition propertyKey, ValidateRecipientWithBaseObjectDelegate<TDataObject> validateRecipient)
		{
			this.ValidateReferenceParameter(propertyKey.Name, propertyKey, validateRecipient);
		}

		internal void ValidateReferenceParameter(string parameterName, object propertyKey, ValidateRecipientWithBaseObjectDelegate<TDataObject> validateRecipient)
		{
			if (base.Fields.IsModified(propertyKey))
			{
				ADRecipient recipient;
				if (this.recipientDictionary.TryGetValue(propertyKey, out recipient))
				{
					this.ReferenceErrorReporter.ValidateReference(parameterName, this.parameterDictionary[recipient].RawIdentity, delegate(Task.ErrorLoggerDelegate writeError)
					{
						validateRecipient(this.DataObject, recipient, writeError);
					});
				}
			}
		}

		protected void ValidateExpansionServer(Server server, bool throwExceptionOnError)
		{
			if (server.IsExchange2007OrLater && !server.IsHubTransportServer)
			{
				if (throwExceptionOnError)
				{
					base.ThrowTerminatingError(new RecipientTaskException(Strings.ErrorExpansionServerMustBeTiOrBhServer), ExchangeErrorCategory.Client, this.Identity);
					return;
				}
				base.WriteError(new RecipientTaskException(Strings.ErrorExpansionServerMustBeTiOrBhServer), ExchangeErrorCategory.Client, this.Identity);
			}
		}

		internal static MultiValuedProperty<ADRecipient> ResolveAndSeparateMessageDeliveryRestrictionRecipientIdentities(MultiValuedProperty<DeliveryRecipientIdParameter> identities, string parameterName, IRecipientSession recipientSession, DataAccessHelper.CategorizedGetDataObjectDelegate getDataObject, Task.ErrorLoggerDelegate writeError, out MultiValuedProperty<ADObjectId> resolvedGroups, out MultiValuedProperty<ADObjectId> resolvedIndividuals)
		{
			resolvedGroups = null;
			resolvedIndividuals = null;
			MultiValuedProperty<ADRecipient> multiValuedProperty = new MultiValuedProperty<ADRecipient>();
			foreach (RecipientIdParameter recipientIdParameter in identities)
			{
				ADRecipient adrecipient = (ADRecipient)getDataObject(recipientIdParameter, recipientSession, null, null, new LocalizedString?(Strings.ErrorRecipientNotFound(recipientIdParameter.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(recipientIdParameter.ToString())), ExchangeErrorCategory.Client);
				multiValuedProperty.Add(adrecipient);
				if ((resolvedGroups != null && resolvedGroups.Contains(adrecipient.Id)) || (resolvedIndividuals != null && resolvedIndividuals.Contains(adrecipient.Id)))
				{
					writeError(new RecipientTaskException(Strings.ErrorRecipientIdParamElementsNotUnique(parameterName, adrecipient.Id.ToString())), ExchangeErrorCategory.Client, recipientIdParameter);
				}
				bool flag = ADRecipient.IsAllowedDeliveryRestrictionGroup(adrecipient.RecipientType);
				bool flag2 = ADRecipient.IsAllowedDeliveryRestrictionIndividual(adrecipient.RecipientType);
				if (!flag && !flag2)
				{
					writeError(new RecipientTaskException(Strings.ErrorGroupOrIndividualRecipientNeeded(recipientIdParameter.ToString())), ExchangeErrorCategory.Client, recipientIdParameter);
				}
				if (flag)
				{
					if (resolvedGroups == null)
					{
						resolvedGroups = new MultiValuedProperty<ADObjectId>();
					}
					resolvedGroups.Add(adrecipient.Id);
				}
				else
				{
					if (resolvedIndividuals == null)
					{
						resolvedIndividuals = new MultiValuedProperty<ADObjectId>();
					}
					resolvedIndividuals.Add(adrecipient.Id);
				}
			}
			return multiValuedProperty;
		}

		private void ValidateMailTipsParameters(TPublicObject dynamicParametersInstance)
		{
			bool flag = dynamicParametersInstance.IsChanged(ADRecipientSchema.DefaultMailTip);
			bool flag2 = dynamicParametersInstance.IsChanged(ADRecipientSchema.MailTipTranslations);
			this.isDeletingMailTipTranslations = (flag2 && (dynamicParametersInstance.MailTipTranslations == null || 0 == dynamicParametersInstance.MailTipTranslations.Count));
			this.isDeletingDefaultMailTip = (flag && string.IsNullOrEmpty(dynamicParametersInstance.MailTip));
			bool flag3 = flag && !string.IsNullOrEmpty(dynamicParametersInstance.MailTip);
			if (flag2)
			{
				if (!this.isDeletingMailTipTranslations)
				{
					HashSet<string> hashSet = new HashSet<string>(dynamicParametersInstance.MailTipTranslations.Count, StringComparer.OrdinalIgnoreCase);
					dynamicParametersInstance.MailTipTranslations = MailboxTaskHelper.ValidateAndSanitizeTranslations(dynamicParametersInstance.MailTipTranslations, hashSet, flag, this.isDeletingDefaultMailTip, new Task.ErrorLoggerDelegate(base.WriteError));
					if (!flag && !hashSet.Contains("default"))
					{
						this.ThrowInvalidOperationError(Strings.ErrorMailTipSetTranslationsWithoutDefault);
					}
					if (flag3)
					{
						dynamicParametersInstance.MailTip = TextConverterHelper.SanitizeHtml(dynamicParametersInstance.MailTip);
						this.MergeDefaultMailTipIntoTranslation(dynamicParametersInstance);
						return;
					}
				}
				else if (flag3)
				{
					this.ThrowInvalidOperationError(Strings.ErrorMoreThanOneDefaultMailTipTranslationSpecified);
					return;
				}
			}
			else if (flag3)
			{
				dynamicParametersInstance.MailTip = TextConverterHelper.SanitizeHtml(dynamicParametersInstance.MailTip);
			}
		}

		private void MergeDefaultMailTipIntoTranslation(TPublicObject dynamicParametersInstance)
		{
			string defaultMailTipTranslation = SetMailEnabledRecipientObjectTask<TIdentity, TPublicObject, TDataObject>.GetDefaultMailTipTranslation(dynamicParametersInstance.MailTip);
			if (dynamicParametersInstance.MailTipTranslations == null)
			{
				dynamicParametersInstance.MailTipTranslations = new MultiValuedProperty<string>(defaultMailTipTranslation);
				return;
			}
			dynamicParametersInstance.MailTipTranslations.Add(defaultMailTipTranslation);
		}

		private static string GetDefaultMailTipTranslation(string mailTip)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
			{
				"default",
				mailTip
			});
		}

		private void ThrowInvalidOperationError(LocalizedString errorString)
		{
			base.ThrowTerminatingError(new RecipientTaskException(errorString), ExchangeErrorCategory.Client, null);
		}

		[Obsolete("Use GetDataObject<TObject>(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID, OptionalIdentityData optionalData, LocalizedString? notFoundError, LocalizedString? multipleFoundError, ExchangeErrorCategory errorCategory) instead")]
		protected new IConfigurable GetDataObject<TObject>(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID, OptionalIdentityData optionalData, LocalizedString? notFoundError, LocalizedString? multipleFoundError) where TObject : IConfigurable, new()
		{
			return base.GetDataObject<TObject>(id, session, rootID, optionalData, notFoundError, multipleFoundError);
		}

		[Obsolete("Use GetDataObject<TObject>(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID, LocalizedString? notFoundError, LocalizedString? multipleFoundError, ExchangeErrorCategory errorCategory) instead")]
		protected new IConfigurable GetDataObject<TObject>(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID, LocalizedString? notFoundError, LocalizedString? multipleFoundError) where TObject : IConfigurable, new()
		{
			return base.GetDataObject<TObject>(id, session, rootID, null, notFoundError, multipleFoundError);
		}

		[Obsolete("Use ThrowTerminatingError(LocalizedException exception, ExchangeErrorCategory category, object target) instead.")]
		protected new void ThrowTerminatingError(Exception exception, ErrorCategory category, object target)
		{
			base.ThrowTerminatingError(exception, category, target);
		}

		[Obsolete("Use WriteError(LocalizedException exception, ExchangeErrorCategory category, object target, bool reThrow) instead.")]
		protected new void WriteError(Exception exception, ErrorCategory category, object target, bool reThrow)
		{
			base.WriteError(exception, category, target, reThrow);
		}

		[Obsolete("Use WriteError(LocalizedException exception, ExchangeErrorCategory category, object target) instead.")]
		internal new void WriteError(Exception exception, ErrorCategory category, object target)
		{
			base.WriteError(exception, category, target, true);
		}

		[Obsolete("Use WriteError(Exception exception, ExchangeErrorCategory category, object target, bool reThrow, string helpUrl) instead.")]
		protected new void WriteError(Exception exception, ErrorCategory category, object target, bool reThrow, string helpUrl)
		{
			base.WriteError(exception, category, target, reThrow, helpUrl);
		}

		private const string createDTMFMap = "CreateDTMFMap";

		protected Dictionary<object, MultiValuedProperty<ADRecipient>> recipientsDictionary = new Dictionary<object, MultiValuedProperty<ADRecipient>>();

		protected Dictionary<object, ADRecipient> recipientDictionary = new Dictionary<object, ADRecipient>();

		protected Dictionary<ADRecipient, IIdentityParameter> parameterDictionary = new Dictionary<ADRecipient, IIdentityParameter>();

		private ADRecipient arbitrationMbx;

		private bool isDeletingMailTipTranslations;

		private bool isDeletingDefaultMailTip;
	}
}
