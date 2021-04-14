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
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Permission;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.RecipientPermission
{
	[Cmdlet("Get", "RecipientPermission", DefaultParameterSetName = "Identity")]
	public sealed class GetRecipientPermission : GetRecipientObjectTask<RecipientIdParameter, ReducedRecipient>
	{
		[Parameter]
		public SecurityPrincipalIdParameter Trustee
		{
			get
			{
				return (SecurityPrincipalIdParameter)base.Fields["Trustee"];
			}
			set
			{
				base.Fields["Trustee"] = value;
			}
		}

		[Parameter]
		public MultiValuedProperty<RecipientAccessRight> AccessRights
		{
			get
			{
				return (MultiValuedProperty<RecipientAccessRight>)base.Fields["AccessRights"];
			}
			set
			{
				base.Fields["AccessRights"] = value;
			}
		}

		private new PSCredential Credential
		{
			get
			{
				return base.Credential;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				return RecipientIdParameter.GetRecipientTypeFilter(RecipientIdParameter.AllowedRecipientTypes);
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (this.Trustee != null)
			{
				this.trusteeSid = SecurityPrincipalIdParameter.GetUserSid(base.TenantGlobalCatalogSession, this.Trustee, new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.GetReducedRecipientSession((IRecipientSession)base.CreateSession(), 100, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientPermission\\GetRecipientPermission.cs");
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
				IEnumerable<ReducedRecipient> dataObjects = base.GetDataObjects<ReducedRecipient>(this.Identity, base.DataSession, this.RootId, base.OptionalIdentityData, out localizedString);
				EnumerableWrapper<ReducedRecipient> wrapper = EnumerableWrapper<ReducedRecipient>.GetWrapper(dataObjects);
				if (!base.HasErrors && !wrapper.HasElements())
				{
					base.WriteError(new ManagementObjectNotFoundException(localizedString ?? base.GetErrorMessageObjectNotFound(this.Identity.ToString(), typeof(ReducedRecipient).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), (ErrorCategory)1003, null);
				}
				this.WriteResult<ReducedRecipient>(dataObjects);
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
			IDirectorySession directorySession = (IDirectorySession)base.DataSession;
			if (TaskHelper.ShouldUnderscopeDataSessionToOrganization(directorySession, (ADObject)dataObject))
			{
				directorySession = TaskHelper.UnderscopeSessionToOrganization(directorySession, ((ADObject)dataObject).OrganizationId, true);
			}
			ActiveDirectorySecurity activeDirectorySecurity = PermissionTaskHelper.ReadAdSecurityDescriptor((ADRawEntry)dataObject, directorySession, new Task.TaskErrorLoggingDelegate(base.WriteError));
			AuthorizationRuleCollection accessRules = activeDirectorySecurity.GetAccessRules(true, true, typeof(SecurityIdentifier));
			foreach (object obj in accessRules)
			{
				ActiveDirectoryAccessRule activeDirectoryAccessRule = (ActiveDirectoryAccessRule)obj;
				if (this.Trustee == null || this.trusteeSid == activeDirectoryAccessRule.IdentityReference)
				{
					RecipientAccessRight? recipientAccessRight = this.FilterByRecipientAccessRights(activeDirectoryAccessRule, this.AccessRights);
					if (recipientAccessRight != null)
					{
						string text = string.Empty;
						if (Globals.IsDatacenter && base.TenantGlobalCatalogSession != null)
						{
							try
							{
								SecurityIdentifier sId = (SecurityIdentifier)activeDirectoryAccessRule.IdentityReference;
								ADRecipient adrecipient = base.TenantGlobalCatalogSession.FindBySid(sId);
								if (adrecipient != null)
								{
									text = ((!string.IsNullOrEmpty(adrecipient.DisplayName)) ? adrecipient.DisplayName : adrecipient.Name);
								}
							}
							catch
							{
							}
						}
						if (string.IsNullOrEmpty(text))
						{
							text = RecipientPermissionTaskHelper.GetFriendlyNameOfSecurityIdentifier((SecurityIdentifier)activeDirectoryAccessRule.IdentityReference, base.TenantGlobalCatalogSession, new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
						}
						RecipientPermission dataObject2 = new RecipientPermission(activeDirectoryAccessRule, ((ADRawEntry)dataObject).Id, text, recipientAccessRight.Value);
						base.WriteResult(dataObject2);
					}
				}
			}
			TaskLogger.LogExit();
		}

		private RecipientAccessRight? FilterByRecipientAccessRights(ActiveDirectoryAccessRule ace, MultiValuedProperty<RecipientAccessRight> accessRights)
		{
			RecipientAccessRight? recipientAccessRight = RecipientPermissionHelper.GetRecipientAccessRight(ace);
			if (recipientAccessRight == null)
			{
				return null;
			}
			if (accessRights == null)
			{
				return recipientAccessRight;
			}
			if (accessRights.Contains(recipientAccessRight.Value))
			{
				return recipientAccessRight;
			}
			return null;
		}

		private SecurityIdentifier trusteeSid;
	}
}
