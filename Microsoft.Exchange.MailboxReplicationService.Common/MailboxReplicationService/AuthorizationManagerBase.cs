using System;
using System.ServiceModel;
using System.Web;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public abstract class AuthorizationManagerBase : ServiceAuthorizationManager
	{
		internal abstract AdminRoleDefinition[] ComputeAdminRoles(IRootOrganizationRecipientSession recipientSession, ITopologyConfigurationSession configSession);

		internal virtual IAuthenticationInfo Authenticate(OperationContext operationContext)
		{
			ServiceSecurityContext serviceSecurityContext = operationContext.ServiceSecurityContext;
			if (serviceSecurityContext == null || serviceSecurityContext.IsAnonymous)
			{
				return null;
			}
			return new AuthenticationInfo(serviceSecurityContext.WindowsIdentity, serviceSecurityContext.PrimaryIdentity.Name);
		}

		internal virtual bool PostAuthnCheck(OperationContext operationContext, IAuthenticationInfo authenticationInfo)
		{
			AdminRoleDefinition[] array = this.GetAdminRoles();
			if (array != null)
			{
				foreach (AdminRoleDefinition adminRoleDefinition in array)
				{
					if (authenticationInfo.Sid != null)
					{
						if (authenticationInfo.Sid == adminRoleDefinition.Sid)
						{
							MrsTracer.Authorization.Debug("AuthorizationManagerBase.PostAuthnCheck: client is in '{0}' role, MRS access is granted by Sid.", new object[]
							{
								adminRoleDefinition.RoleName
							});
							return true;
						}
					}
					else if (authenticationInfo.WindowsPrincipal.IsInRole(adminRoleDefinition.Sid))
					{
						MrsTracer.Authorization.Debug("AuthorizationManagerBase.PostAuthnCheck: client is in '{0}' role, MRS access is granted.", new object[]
						{
							adminRoleDefinition.RoleName
						});
						return true;
					}
				}
			}
			MrsTracer.Authorization.Debug("AuthorizationManagerBase.PostAuthnCheck: client is not an Admin, MRS access is denied.", new object[0]);
			return false;
		}

		protected override bool CheckAccessCore(OperationContext operationContext)
		{
			bool result;
			try
			{
				if (operationContext.ServiceSecurityContext == null)
				{
					MrsTracer.Authorization.Debug("AuthorizationManagerBase.CheckAccessCore: operationContext.ServiceSecurityContext was null", new object[0]);
					result = false;
				}
				else
				{
					IAuthenticationInfo authenticationInfo = this.Authenticate(operationContext);
					if (authenticationInfo == null)
					{
						MrsTracer.Authorization.Debug("AuthorizationManagerBase.CheckAccessCore: Client is not authenticated", new object[0]);
						result = false;
					}
					else
					{
						MrsTracer.Authorization.Debug("AuthorizationManagerBase.CheckAccessCore: user '{0}'", new object[]
						{
							authenticationInfo.PrincipalName
						});
						if (!this.PostAuthnCheck(operationContext, authenticationInfo))
						{
							MrsTracer.Authorization.Debug("AuthorizationManagerBase.CheckAccessCore: PostAuthnCheck failed for '{0}'", new object[]
							{
								authenticationInfo.PrincipalName
							});
							result = false;
						}
						else
						{
							if (HttpContext.Current != null && authenticationInfo.WindowsPrincipal != null)
							{
								HttpContext.Current.User = authenticationInfo.WindowsPrincipal;
							}
							result = base.CheckAccessCore(operationContext);
						}
					}
				}
			}
			catch (SystemException ex)
			{
				MrsTracer.Authorization.Error("SystemException in CheckAccessCore:\n{0}\n{1}", new object[]
				{
					CommonUtils.FullExceptionMessage(ex),
					ex.StackTrace
				});
				result = false;
			}
			catch (Exception ex2)
			{
				MrsTracer.Authorization.Error("Unhandled exception in CheckAccessCore:\n{0}\n{1}", new object[]
				{
					CommonUtils.FullExceptionMessage(ex2),
					ex2.StackTrace
				});
				ExWatson.SendReport(ex2);
				throw;
			}
			return result;
		}

		private AdminRoleDefinition[] GetAdminRoles()
		{
			if (this.adminRoles == null)
			{
				lock (this.adminRolesLock)
				{
					if (this.adminRoles == null)
					{
						CommonUtils.CatchKnownExceptions(delegate
						{
							IRootOrganizationRecipientSession recipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 241, "GetAdminRoles", "f:\\15.00.1497\\sources\\dev\\mrs\\src\\Common\\AuthorizationManagerBase.cs");
							ITopologyConfigurationSession configSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 246, "GetAdminRoles", "f:\\15.00.1497\\sources\\dev\\mrs\\src\\Common\\AuthorizationManagerBase.cs");
							this.adminRoles = this.ComputeAdminRoles(recipientSession, configSession);
						}, null);
					}
				}
			}
			return this.adminRoles;
		}

		private object adminRolesLock = new object();

		private AdminRoleDefinition[] adminRoles;
	}
}
