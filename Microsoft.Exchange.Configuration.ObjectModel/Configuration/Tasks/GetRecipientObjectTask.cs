using System;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.TextConverters;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class GetRecipientObjectTask<TIdentity, TDataObject> : GetTenantADObjectWithIdentityTaskBase<TIdentity, TDataObject> where TIdentity : IIdentityParameter where TDataObject : ADObject, new()
	{
		[Parameter]
		public PSCredential Credential
		{
			get
			{
				return (PSCredential)base.Fields["Credential"];
			}
			set
			{
				base.Fields["Credential"] = value;
			}
		}

		[Parameter]
		public Unlimited<uint> ResultSize
		{
			get
			{
				return base.InternalResultSize;
			}
			set
			{
				base.InternalResultSize = value;
			}
		}

		[Parameter]
		public SwitchParameter ReadFromDomainController
		{
			get
			{
				return (SwitchParameter)(base.Fields["ReadFromDomainController"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ReadFromDomainController"] = value;
			}
		}

		protected override Unlimited<uint> DefaultResultSize
		{
			get
			{
				return GetRecipientObjectTask<TIdentity, TDataObject>.defaultResultSize;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected SwitchParameter InternalIgnoreDefaultScope
		{
			get
			{
				return (SwitchParameter)(base.Fields["IgnoreDefaultScope"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IgnoreDefaultScope"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			if (this.InternalIgnoreDefaultScope && base.DomainController != null)
			{
				base.ThrowTerminatingError(new ArgumentException(Strings.ErrorIgnoreDefaultScopeAndDCSetTogether), ErrorCategory.InvalidArgument, null);
			}
			if (this.ResultSize == 0U)
			{
				base.ThrowTerminatingError(new TaskException(Strings.ErrorInvalidResultSize), ErrorCategory.InvalidArgument, null);
			}
			if (this.Credential != null)
			{
				try
				{
					base.NetCredential = this.Credential.GetNetworkCredential();
				}
				catch (PSArgumentException exception)
				{
					base.ThrowTerminatingError(exception, ErrorCategory.InvalidArgument, this.Credential);
				}
			}
			base.InternalBeginProcessing();
			TaskLogger.LogExit();
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, base.NetCredential, base.SessionSettings, 285, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\GetAdObjectTask.cs");
			if (this.InternalIgnoreDefaultScope)
			{
				tenantOrRootOrgRecipientSession.EnforceDefaultScope = false;
				tenantOrRootOrgRecipientSession.UseGlobalCatalog = (this.Identity == null);
			}
			else
			{
				bool flag;
				if (this.ReadFromDomainController && this.Identity != null)
				{
					TIdentity identity = this.Identity;
					flag = ADObjectId.IsValidDistinguishedName(identity.RawIdentity);
				}
				else
				{
					flag = false;
				}
				bool flag2 = flag;
				bool viewEntireForest = base.ServerSettings.ViewEntireForest;
				tenantOrRootOrgRecipientSession.UseGlobalCatalog = (base.DomainController == null && viewEntireForest && !flag2);
			}
			return tenantOrRootOrgRecipientSession;
		}

		protected override bool ShouldSupportPreResolveOrgIdBasedOnIdentity()
		{
			return true;
		}

		protected virtual bool ShouldSkipObject(IConfigurable dataObject)
		{
			return false;
		}

		protected virtual bool ShouldSkipPresentationObject(IConfigurable presentationObject)
		{
			return false;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity
			});
			if (this.Identity != null && this.InternalIgnoreDefaultScope)
			{
				ADObjectId adobjectId;
				if (!RecipientTaskHelper.IsValidDistinguishedName(this.Identity, out adobjectId))
				{
					base.WriteError(new ArgumentException(Strings.ErrorOnlyDNSupportedWithIgnoreDefaultScope), (ErrorCategory)1000, this.Identity);
				}
				IConfigurable dataObject = RecipientTaskHelper.ResolveDataObject<TDataObject>(base.DataSession, null, base.ServerSettings, this.Identity, this.RootId, base.OptionalIdentityData, base.DomainController, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<TDataObject>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
				this.WriteResult(dataObject);
			}
			else
			{
				base.InternalProcessRecord();
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			IDirectorySession directorySession = base.DataSession as IDirectorySession;
			IConfigurable configurable = dataObject;
			if (this.ReadFromDomainController.IsPresent && directorySession != null && directorySession.UseGlobalCatalog)
			{
				try
				{
					directorySession.UseGlobalCatalog = false;
					base.WriteVerbose(Strings.VerboseRereadADObject(dataObject.Identity.ToString(), typeof(TDataObject).Name, ((ADObjectId)dataObject.Identity).ToDNString()));
					configurable = base.DataSession.Read<TDataObject>(dataObject.Identity);
					base.WriteVerbose(TaskVerboseStringHelper.GetSourceVerboseString(base.DataSession));
				}
				finally
				{
					directorySession.UseGlobalCatalog = true;
				}
			}
			ADRecipient adrecipient = configurable as ADRecipient;
			if (adrecipient != null)
			{
				adrecipient.PopulateAcceptMessagesOnlyFromSendersOrMembers();
				adrecipient.PopulateBypassModerationFromSendersOrMembers();
				adrecipient.PopulateRejectMessagesFromSendersOrMembers();
				GetRecipientObjectTask<TIdentity, TDataObject>.SanitizeMailTips(adrecipient);
			}
			if (configurable == null)
			{
				base.WriteVerbose(Strings.VerboseFailedToReadFromDC(dataObject.Identity.ToString(), base.DataSession.Source));
			}
			else if (this.ShouldSkipObject(configurable))
			{
				base.WriteVerbose(Strings.VerboseSkipObject(configurable.Identity.ToString()));
			}
			else
			{
				IConfigurable configurable2 = this.ConvertDataObjectToPresentationObject(configurable);
				if (this.ShouldSkipPresentationObject(configurable2))
				{
					base.WriteVerbose(Strings.VerboseSkipObject(configurable.Identity.ToString()));
				}
				else
				{
					base.WriteResult(configurable2);
				}
			}
			TaskLogger.LogExit();
		}

		private static void SanitizeMailTips(ADRecipient recipient)
		{
			if (recipient.MailTipTranslations != null)
			{
				bool isReadOnly = recipient.IsReadOnly;
				if (isReadOnly)
				{
					recipient.SetIsReadOnly(false);
				}
				for (int i = 0; i < recipient.MailTipTranslations.Count; i++)
				{
					string str;
					string text;
					if (ADRecipient.TryGetMailTipParts(recipient.MailTipTranslations[i], out str, out text) && !string.IsNullOrEmpty(text))
					{
						using (StringReader stringReader = new StringReader(text))
						{
							using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
							{
								HtmlToHtml htmlToHtml = new HtmlToHtml();
								htmlToHtml.SetPreserveDisplayNoneStyle(true);
								htmlToHtml.InputEncoding = Encoding.UTF8;
								htmlToHtml.OutputEncoding = Encoding.UTF8;
								htmlToHtml.FilterHtml = true;
								htmlToHtml.Convert(stringReader, stringWriter);
								string str2 = stringWriter.ToString();
								recipient.MailTipTranslations[i] = str + ":" + str2;
							}
						}
					}
				}
				if (isReadOnly)
				{
					recipient.SetIsReadOnly(true);
				}
			}
		}

		private static readonly Unlimited<uint> defaultResultSize = new Unlimited<uint>(1000U);
	}
}
