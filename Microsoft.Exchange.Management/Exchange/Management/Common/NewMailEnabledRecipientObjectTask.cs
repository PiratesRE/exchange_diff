using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.Common
{
	public abstract class NewMailEnabledRecipientObjectTask<TDataObject> : NewRecipientObjectTask<TDataObject> where TDataObject : ADRecipient, new()
	{
		[Parameter(Mandatory = false)]
		public virtual MailboxIdParameter ArbitrationMailbox
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
		public virtual MultiValuedProperty<ModeratorIDParameter> ModeratedBy
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

		[Parameter]
		public virtual bool ModerationEnabled
		{
			get
			{
				TDataObject dataObject = this.DataObject;
				return dataObject.ModerationEnabled;
			}
			set
			{
				TDataObject dataObject = this.DataObject;
				dataObject.ModerationEnabled = value;
			}
		}

		[Parameter]
		public virtual TransportModerationNotificationFlags SendModerationNotifications
		{
			get
			{
				TDataObject dataObject = this.DataObject;
				return dataObject.SendModerationNotifications;
			}
			set
			{
				TDataObject dataObject = this.DataObject;
				dataObject.SendModerationNotifications = value;
			}
		}

		[Parameter]
		public SwitchParameter OverrideRecipientQuotas
		{
			get
			{
				return (SwitchParameter)(base.Fields["OverrideRecipientQuotas"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["OverrideRecipientQuotas"] = value;
			}
		}

		public virtual MultiValuedProperty<string> MailTipTranslations
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields[ADRecipientSchema.MailTipTranslations];
			}
			set
			{
				base.Fields[ADRecipientSchema.MailTipTranslations] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (base.Fields.IsModified(ADRecipientSchema.MailTipTranslations) && this.MailTipTranslations != null && this.MailTipTranslations.Count > 0)
			{
				TDataObject dataObject = this.DataObject;
				dataObject.MailTipTranslations = MailboxTaskHelper.ValidateAndSanitizeTranslations(this.MailTipTranslations, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			TaskLogger.LogExit();
		}

		protected override void PrepareRecipientObject(TDataObject recipient)
		{
			TaskLogger.LogEnter();
			base.PrepareRecipientObject(recipient);
			if (base.Fields.Contains(ADRecipientSchema.ModeratedBy))
			{
				MultiValuedProperty<ModeratorIDParameter> multiValuedProperty = (MultiValuedProperty<ModeratorIDParameter>)base.Fields[ADRecipientSchema.ModeratedBy];
				int num = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled ? 10 : 25;
				if (multiValuedProperty != null && multiValuedProperty.Count > num)
				{
					base.WriteError(new RecipientTaskException(Strings.ErrorTooManyModerators(num)), ExchangeErrorCategory.Client, null);
				}
				recipient.ModeratedBy = RecipientTaskHelper.GetModeratedByAdObjectIdFromParameterID(base.TenantGlobalCatalogSession, this.ModeratedBy, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADRecipient>), recipient, new Task.ErrorLoggerDelegate(base.WriteError));
			}
			if (base.Fields.IsModified(ADRecipientSchema.ArbitrationMailbox))
			{
				if (this.ArbitrationMailbox != null)
				{
					ADRecipient adrecipient = (ADRecipient)base.GetDataObject<ADRecipient>(this.ArbitrationMailbox, (IRecipientSession)base.DataSession, null, new LocalizedString?(Strings.ErrorMailboxNotFound(this.ArbitrationMailbox.ToString())), new LocalizedString?(Strings.ErrorMailboxNotUnique(this.ArbitrationMailbox.ToString())), ExchangeErrorCategory.Client);
					if (adrecipient.RecipientTypeDetails != RecipientTypeDetails.ArbitrationMailbox)
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorInvalidArbitrationMbxType(adrecipient.Identity.ToString())), ExchangeErrorCategory.Client, recipient.Identity);
					}
					if (MultiValuedPropertyBase.IsNullOrEmpty((ADMultiValuedProperty<ADObjectId>)adrecipient[ADUserSchema.ApprovalApplications]))
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorInvalidArbitrationMbxTypeForModerationAndAutogroup(adrecipient.Identity.ToString())), ExchangeErrorCategory.Client, recipient.Identity);
					}
					if (!recipient.OrganizationId.Equals(adrecipient.OrganizationId))
					{
						base.WriteError(new RecipientTaskException(Strings.ErrorArbitrationMbxCrossOrg(adrecipient.Identity.ToString())), ExchangeErrorCategory.Client, recipient.Identity);
					}
					recipient.ArbitrationMailbox = adrecipient.Id;
				}
				else
				{
					recipient.ArbitrationMailbox = null;
				}
			}
			TaskLogger.LogExit();
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

		[Obsolete("Use ThrowTerminatingError(Exception exception, ExchangeErrorCategory category, object target) instead.")]
		protected new void ThrowTerminatingError(Exception exception, ErrorCategory category, object target)
		{
			base.ThrowTerminatingError(exception, category, target);
		}

		[Obsolete("Use WriteError(Exception exception, ExchangeErrorCategory category, object target, bool reThrow) instead.")]
		protected new void WriteError(Exception exception, ErrorCategory category, object target, bool reThrow)
		{
			base.WriteError(exception, category, target, reThrow);
		}

		[Obsolete("Use WriteError(Exception exception, ExchangeErrorCategory category, object target) instead.")]
		internal new void WriteError(Exception exception, ErrorCategory category, object target)
		{
			base.WriteError(exception, category, target, true);
		}

		[Obsolete("Use WriteError(Exception exception, ExchangeErrorCategory category, object target, bool reThrow, string helpUrl) instead.")]
		protected new void WriteError(Exception exception, ErrorCategory category, object target, bool reThrow, string helpUrl)
		{
			base.WriteError(exception, category, target, reThrow, helpUrl);
		}
	}
}
