using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Transport.Sync.Common;

namespace Microsoft.Exchange.Management.Aggregation
{
	[Cmdlet("Get", "SendAddress", DefaultParameterSetName = "Identity")]
	public sealed class GetSendAddress : GetTenantADObjectWithIdentityTaskBase<SendAddressIdParameter, SendAddress>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity", ValueFromPipeline = true)]
		[Parameter(Mandatory = false, ParameterSetName = "LookUpId", ValueFromPipeline = true)]
		public MailboxIdParameter Mailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Mailbox"];
			}
			set
			{
				base.Fields["Mailbox"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "LookUpId")]
		public string AddressId
		{
			get
			{
				return (string)base.Fields["AddressId"];
			}
			set
			{
				base.Fields["AddressId"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 73, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Mobility\\Aggregation\\GetSendAddress.cs");
			if (this.Identity != null && this.Identity.MailboxIdParameter != null)
			{
				this.Mailbox = this.Identity.MailboxIdParameter;
			}
			if (this.Mailbox == null)
			{
				this.WriteDebugInfoAndError(new MailboxParameterNotSpecifiedException(), ErrorCategory.InvalidData, this.Mailbox);
			}
			ADUser aduser = (ADUser)base.GetDataObject<ADUser>(this.Mailbox, tenantOrRootOrgRecipientSession, null, new LocalizedString?(Strings.ErrorUserNotFound(this.Mailbox.ToString())), new LocalizedString?(Strings.ErrorUserNotUnique(this.Mailbox.ToString())));
			ADSessionSettings adSettings = ADSessionSettings.RescopeToOrganization(base.SessionSettings, aduser.OrganizationId, true);
			try
			{
				this.userPrincipal = ExchangePrincipal.FromADUser(adSettings, aduser, RemotingOptions.AllowCrossSite);
			}
			catch (ObjectNotFoundException exception)
			{
				this.WriteDebugInfoAndError(exception, ErrorCategory.InvalidArgument, this.Mailbox);
			}
			SendAddressDataProvider result = null;
			try
			{
				result = new SendAddressDataProvider(this.userPrincipal, this.Mailbox.ToString());
			}
			catch (MailboxFailureException exception2)
			{
				this.WriteDebugInfoAndError(exception2, ErrorCategory.InvalidArgument, this.Mailbox);
			}
			return result;
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity
			});
			base.InternalStateReset();
			if (this.Identity == null)
			{
				if (this.AddressId != null)
				{
					SendAddressIdentity sendAddressIdentity = new SendAddressIdentity(this.Mailbox.ToString(), this.AddressId);
					this.Identity = new SendAddressIdParameter(sendAddressIdentity);
				}
				else
				{
					this.Identity = new SendAddressIdParameter();
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity
			});
			try
			{
				if (this.Identity.IsUniqueIdentity)
				{
					this.WriteResult(base.GetDataObject(this.Identity));
				}
				else
				{
					IEnumerable<SendAddress> dataObjects = null;
					LocalizedString? localizedString = null;
					try
					{
						dataObjects = base.GetDataObjects(this.Identity, base.OptionalIdentityData, out localizedString);
					}
					catch (LocalizedException exception)
					{
						this.WriteDebugInfoAndError(exception, ErrorCategory.InvalidOperation, this.Mailbox);
					}
					this.WriteResult<SendAddress>(dataObjects);
					if (!base.HasErrors && base.WriteObjectCount == 0U && localizedString != null)
					{
						this.WriteDebugInfoAndError(new ManagementObjectNotFoundException(localizedString.Value), ErrorCategory.InvalidData, null);
					}
				}
			}
			finally
			{
				this.WriteDebugInfo();
			}
			TaskLogger.LogExit();
		}

		private void WriteDebugInfoAndError(Exception exception, ErrorCategory category, object target)
		{
			this.WriteDebugInfo();
			base.WriteError(exception, category, target);
		}

		private void WriteDebugInfo()
		{
			if (base.IsDebugOn)
			{
				base.WriteDebug(CommonLoggingHelper.SyncLogSession.GetBlackBoxText());
			}
			CommonLoggingHelper.SyncLogSession.ClearBlackBox();
		}

		private const string LookUpIdParameterSet = "LookUpId";

		private ExchangePrincipal userPrincipal;
	}
}
