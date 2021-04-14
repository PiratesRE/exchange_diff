using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "MailboxServer", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMailboxServer : SetTopologySystemConfigurationObjectTask<MailboxServerIdParameter, MailboxServer, Server>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetMailboxServer(this.Identity.ToString());
			}
		}

		[Parameter]
		public MultiValuedProperty<ServerIdParameter> SubmissionServerOverrideList
		{
			get
			{
				return (MultiValuedProperty<ServerIdParameter>)base.Fields["SubmissionServerOverrideList"];
			}
			set
			{
				base.Fields["SubmissionServerOverrideList"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (base.HasErrors)
			{
				return;
			}
			MailboxServer mailboxServer = (MailboxServer)this.GetDynamicParameters();
			if (base.Fields.IsModified("SubmissionServerOverrideList"))
			{
				mailboxServer.SubmissionServerOverrideList = base.ResolveIdParameterCollection<ServerIdParameter, Server, ADObjectId>(this.SubmissionServerOverrideList, this.ConfigurationSession, null, null, (ExchangeErrorCategory)0, new Func<IIdentityParameter, LocalizedString>(Strings.ErrorServerNotFound), new Func<IIdentityParameter, LocalizedString>(Strings.ErrorServerNotUnique), null, null);
			}
			TaskLogger.LogExit();
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			base.StampChangesOn(dataObject);
			Server server = (Server)dataObject;
			MailboxServer instance = this.Instance;
			if (!server.IsProvisionedServer)
			{
				Exception ex = null;
				if (string.IsNullOrEmpty(server.Fqdn))
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorInvalidObjectMissingCriticalProperty(typeof(Server).Name, server.Identity.ToString(), ServerSchema.Fqdn.Name)), ErrorCategory.ReadError, server.Identity);
				}
				if (instance.IsModified(MailboxServerSchema.Locale))
				{
					GetMailboxServer.GetConfigurationFromRegistry(server.Fqdn, out this.oldLocale, out ex);
					if (ex != null)
					{
						base.WriteError(new InvalidOperationException(Strings.ErrorAccessingRegistryRaisesException(server.Fqdn, ex.Message)), ErrorCategory.ReadError, server.Identity);
					}
				}
				else
				{
					this.oldLocale = new CultureInfo[0];
				}
			}
			else if (instance.IsModified(MailboxServerSchema.Locale))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorInvalidOperationOnProvisionedMailboxServer), ErrorCategory.InvalidArgument, server.Identity);
			}
			if (!instance.IsModified(MailboxServerSchema.Locale))
			{
				this.isLocalChanged = false;
				server.Locale = this.oldLocale;
			}
			else
			{
				server.Locale = instance.Locale;
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			Server server = (Server)base.PrepareDataObject();
			if (!base.Fields.IsModified("SubmissionServerOverrideList") && server.IsModified(MailboxServerSchema.SubmissionServerOverrideLIst))
			{
				foreach (ADObjectId adObjectId in server.SubmissionServerOverrideList.Added)
				{
					ServerIdParameter serverIdParameter = new ServerIdParameter(adObjectId);
					Server server2 = (Server)base.GetDataObject<Server>(serverIdParameter, this.ConfigurationSession, null, new LocalizedString?(Strings.ErrorServerNotFound(serverIdParameter.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(serverIdParameter.ToString())));
				}
			}
			TaskLogger.LogExit();
			return server;
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (this.Instance.IsModified(ADObjectSchema.Name))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorServerNameModified), ErrorCategory.InvalidOperation, this.Identity);
			}
			base.InternalValidate();
			if (this.isLocalChanged && this.DataObject.Locale.Count == 0)
			{
				base.WriteError(new ArgumentNullException("Locale"), ErrorCategory.InvalidArgument, this.Identity);
			}
			TaskLogger.LogExit();
		}

		protected sealed override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject
			});
			MailboxServer mailboxServer = new MailboxServer(this.DataObject);
			string fqdn = this.DataObject.Fqdn;
			Exception ex = null;
			bool flag = false;
			if (this.isLocalChanged && !this.DataObject.IsProvisionedServer)
			{
				GetMailboxServer.SetConfigurationFromRegistry(fqdn, mailboxServer.Locale, out ex);
				if (ex != null)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorFaildToWriteRegistry(fqdn, ex.Message)), ErrorCategory.WriteError, this.Identity);
				}
				flag = true;
			}
			bool flag2 = false;
			try
			{
				base.InternalProcessRecord();
				flag2 = true;
			}
			finally
			{
				if (!flag2 && flag)
				{
					GetMailboxServer.SetConfigurationFromRegistry(fqdn, this.oldLocale, out ex);
					if (ex != null)
					{
						this.WriteError(new InvalidOperationException(Strings.ErrorFaildToWriteRegistry(fqdn, ex.Message)), ErrorCategory.WriteError, this.Identity, false);
					}
				}
				TaskLogger.LogExit();
			}
		}

		private bool isLocalChanged = true;

		private CultureInfo[] oldLocale;
	}
}
