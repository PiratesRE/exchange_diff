using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Management.Automation;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "MailboxPermission", DefaultParameterSetName = "AccessRights")]
	public sealed class GetMailboxPermission : GetRecipientObjectTask<MailboxIdParameter, ADUser>
	{
		[Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
		public override MailboxIdParameter Identity
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AccessRights")]
		public SecurityPrincipalIdParameter User
		{
			get
			{
				return (SecurityPrincipalIdParameter)base.Fields["User"];
			}
			set
			{
				base.Fields["User"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Owner")]
		public SwitchParameter Owner
		{
			get
			{
				return (SwitchParameter)(base.Fields["Owner"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Owner"] = value;
			}
		}

		private IADSecurityPrincipal SecurityPrincipal
		{
			get
			{
				return this.securityPrincipal;
			}
		}

		private bool HasObjectMatchingIdentity
		{
			get
			{
				return this.hasObjectMatchingIdentity;
			}
			set
			{
				this.hasObjectMatchingIdentity = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (this.User != null)
			{
				this.securityPrincipal = SecurityPrincipalIdParameter.GetSecurityPrincipal(base.TenantGlobalCatalogSession, this.User, new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			}
			TaskLogger.LogExit();
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			this.HasObjectMatchingIdentity = false;
			base.InternalStateReset();
			TaskLogger.LogExit();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is DatabaseNotFoundException;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity
			});
			if (this.Identity != null)
			{
				LocalizedString? localizedString;
				IEnumerable<ADUser> dataObjects = base.GetDataObjects(this.Identity, base.OptionalIdentityData, out localizedString);
				this.WriteResult<ADUser>(dataObjects);
				if (!base.HasErrors && !this.HasObjectMatchingIdentity)
				{
					base.WriteError(new ManagementObjectNotFoundException(localizedString ?? base.GetErrorMessageObjectNotFound(this.Identity.ToString(), typeof(MailboxIdParameter).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), ErrorCategory.InvalidData, null);
				}
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
			this.HasObjectMatchingIdentity = true;
			ADUser aduser = (ADUser)dataObject;
			if (aduser.Database == null || aduser.ExchangeGuid == Guid.Empty)
			{
				base.Validate(aduser);
			}
			else
			{
				ActiveDirectorySecurity activeDirectorySecurity = PermissionTaskHelper.ReadMailboxSecurityDescriptor((ADUser)dataObject, PermissionTaskHelper.GetReadOnlySession(base.DomainController), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.ErrorLoggerDelegate(base.WriteError));
				if (!this.Owner.IsPresent)
				{
					AuthorizationRuleCollection accessRules = activeDirectorySecurity.GetAccessRules(true, true, typeof(SecurityIdentifier));
					int num = 0;
					while (accessRules.Count > num)
					{
						ActiveDirectoryAccessRule activeDirectoryAccessRule = (ActiveDirectoryAccessRule)accessRules[num];
						if (this.SecurityPrincipal == null || this.SecurityPrincipal.Sid == activeDirectoryAccessRule.IdentityReference || this.SecurityPrincipal.SidHistory.Contains(activeDirectoryAccessRule.IdentityReference as SecurityIdentifier))
						{
							MailboxAcePresentationObject mailboxAcePresentationObject = new MailboxAcePresentationObject(activeDirectoryAccessRule, ((ADRawEntry)dataObject).Id);
							if (Globals.IsDatacenter && base.TenantGlobalCatalogSession != null)
							{
								SecurityIdentifier securityIdentifier = (SecurityIdentifier)activeDirectoryAccessRule.IdentityReference;
								ADRecipient adrecipient = null;
								try
								{
									adrecipient = base.TenantGlobalCatalogSession.FindBySid(securityIdentifier);
								}
								catch
								{
								}
								if (adrecipient != null)
								{
									string friendlyName = (!string.IsNullOrEmpty(adrecipient.DisplayName)) ? adrecipient.DisplayName : adrecipient.Name;
									mailboxAcePresentationObject.User = new SecurityPrincipalIdParameter(securityIdentifier, friendlyName);
								}
							}
							mailboxAcePresentationObject.ResetChangeTracking(true);
							base.WriteResult(mailboxAcePresentationObject);
						}
						num++;
					}
				}
				else
				{
					IdentityReference owner = activeDirectorySecurity.GetOwner(typeof(NTAccount));
					OwnerPresentationObject dataObject2 = new OwnerPresentationObject(((ADUser)dataObject).Id, owner.ToString());
					base.WriteResult(dataObject2);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalEndProcessing()
		{
			base.InternalEndProcessing();
			if (this.storeSession != null)
			{
				this.storeSession.Dispose();
				this.storeSession = null;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (this.storeSession != null)
			{
				this.storeSession.Dispose();
				this.storeSession = null;
			}
			base.Dispose(disposing);
		}

		private bool hasObjectMatchingIdentity;

		private IADSecurityPrincipal securityPrincipal;

		private MapiMessageStoreSession storeSession;
	}
}
