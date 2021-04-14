using System;
using System.Globalization;
using System.Management.Automation;
using System.Web;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class CreateUnifiedMailbox : AggregatedAccountCommandBase<ServiceResultNone>
	{
		public CreateUnifiedMailbox(CallContext callContext, CreateUnifiedMailboxRequest request) : base(callContext, request, ExTraceGlobals.CreateUnifiedMailboxTracer, typeof(CreateUnifiedMailboxMetadata))
		{
			this.MailboxContainerGuid = Guid.NewGuid();
			this.userPrincipalName = new Lazy<SmtpAddress>(() => SmtpAddress.Parse(string.Format("{0}@{1}", this.netId, this.organizationId_OrganizationalUnit_Name.Value())));
			this.organizationIdParameterFromOrganizationId = Hookable<Func<OrganizationIdParameter>>.Create(true, () => new OrganizationIdParameter(this.getOrganizationId.Value()));
		}

		internal SmtpAddress UserPrincipalName
		{
			get
			{
				return this.userPrincipalName.Value;
			}
		}

		protected override string TypeName
		{
			get
			{
				return "CreateUnifiedMailbox";
			}
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new CreateUnifiedMailboxResponse(base.Result.Code, base.Result.Error, this.UserPrincipalName.ToString());
		}

		internal override ServiceResult<ServiceResultNone> InternalExecute()
		{
			Mailbox mailbox = null;
			string arg = this.organizationId_OrganizationalUnit_Name.Value();
			base.ExecuteWithProtocolLogging(CreateUnifiedMailboxMetadata.NewMailboxCmdletTime, delegate
			{
				MSACallContext msacallContext = this.CallContext as MSACallContext;
				using (PSLocalTask<NewMailbox, Mailbox> pslocalTask = CmdletTaskFactory.Instance.CreateNewMailboxTask(this.getOrganizationId.Value(), SmtpAddress.Parse(msacallContext.MemberName), NetID.Parse(this.netId)))
				{
					pslocalTask.CaptureAdditionalIO = true;
					pslocalTask.Task.MailboxContainerGuid = this.MailboxContainerGuid;
					pslocalTask.Task.Name = this.netId;
					pslocalTask.Task.NetID = NetID.Parse(this.netId);
					pslocalTask.Task.Organization = this.organizationIdParameterFromOrganizationId.Value();
					pslocalTask.Task.WindowsLiveID = WindowsLiveId.Parse(this.UserPrincipalName.ToString());
					pslocalTask.Task.UseExistingLiveId = new SwitchParameter(true);
					pslocalTask.Task.ForestWideDomainControllerAffinityByExecutingUser = new SwitchParameter(true);
					pslocalTask.Task.Languages.Add(new CultureInfo("en-US"));
					if (ExEnvironment.IsSdfDomain)
					{
						pslocalTask.Task.MailboxProvisioningConstraint = new MailboxProvisioningConstraint("{DagName -eq 'NAMSR01DG050'}");
					}
					this.newMailboxExecute.Value(pslocalTask.Task);
					TaskErrorInfo taskErrorInfo = this.newMailboxError.Value(pslocalTask);
					if (taskErrorInfo != null)
					{
						if (taskErrorInfo.Exception is WindowsLiveIdAlreadyUsedException)
						{
							this.TraceError(string.Format("Unified mailbox with NetId {0} already exists.", this.netId));
							throw new UnifiedMailboxAlreadyExistsException((CoreResources.IDs)3392207806U, taskErrorInfo.Exception);
						}
						this.TraceError(string.Format("Could not create unified mailbox with UPN {0}. Exception: {1}", this.UserPrincipalName, taskErrorInfo.Exception));
						throw new CannotCreateUnifiedMailboxException(CoreResources.IDs.ErrorCannotCreateUnifiedMailbox, taskErrorInfo.Exception);
					}
					else
					{
						mailbox = pslocalTask.Result;
					}
				}
			});
			base.ExecuteWithProtocolLogging(CreateUnifiedMailboxMetadata.GlsAddUserTime, new Action(this.AddUserToGLS));
			HttpContext httpContext = HttpContext.Current;
			if (httpContext != null && httpContext.Response != null && httpContext.Response.Headers != null)
			{
				httpContext.Response.Headers[WellKnownHeader.BackEndOriginatingMailboxDatabase] = string.Format("{0}@{1}", mailbox.Database.ObjectGuid, arg);
			}
			this.TraceSuccess("Successfully created unified mailbox");
			return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
		}

		internal override void SetLogMetadataEnumProperties()
		{
			this.verifyEnvironmentTimeEnum = CreateUnifiedMailboxMetadata.VerifyEnvironmentTime;
			this.verifyUserIdentityTypeTimeEnum = CreateUnifiedMailboxMetadata.VerifyUserIdentityTypeTime;
			this.totalTimeEnum = CreateUnifiedMailboxMetadata.TotalTime;
		}

		protected override void VerifyUserIdentityType()
		{
			if (!base.CallContext.IsMSAUser)
			{
				base.VerifyOpenTenant();
				this.TraceError(string.Format("Unified mailbox with NetId {0} already exists.", this.netId));
				throw new UnifiedMailboxAlreadyExistsException((CoreResources.IDs)3392207806U);
			}
		}

		protected override void TraceError(string message)
		{
			base.InternalTraceError(message, CreateUnifiedMailbox.callContextHttpContextUserIdentityGetSafeName.Value(base.CallContext), null);
		}

		protected override void TraceSuccess(string message)
		{
			base.InternalTraceSuccess(message, base.CallContext.HttpContext.User.Identity.GetSafeName(true), null);
		}

		protected override OrganizationId GetOrganizationId()
		{
			if (base.CallContext.IsMSAUser)
			{
				try
				{
					return OpenTenantManager.Instance.ActiveOrganizationId;
				}
				catch (OpenTenantQueryException ex)
				{
					this.TraceError(string.Format("Could not create a unified mailbox because no Open Tenant was found.  Message: {0}", ex.Message));
					throw new CannotCreateUnifiedMailboxException(CoreResources.IDs.ErrorCannotCreateUnifiedMailbox);
				}
			}
			return base.GetOrganizationId();
		}

		private void AddUserToGLS()
		{
			if (!this.ADSessionSettings_IsGlsDisabled.Value())
			{
				try
				{
					GlsDirectorySession glsDirectorySession = new GlsDirectorySession();
					MSACallContext msacallContext = base.CallContext as MSACallContext;
					Guid guid = Guid.Parse(this.organizationId.Value.ToExternalDirectoryOrganizationId());
					glsDirectorySession.AddMSAUser(this.netId, msacallContext.MemberName, guid);
					this.TraceSuccess(string.Format("Successfully added user to GLS. MSAUserNetID={0}, MSAUserMemberName={1}, TenantId={2}", this.netId, msacallContext.MemberName, guid));
					base.CallContext.ProtocolLog.Set(CreateUnifiedMailboxMetadata.GlsAddUserOperationStatus, string.Format("MSAUserNetID:{0} TenantId:{1}", this.netId, guid));
					return;
				}
				catch (Exception ex)
				{
					this.TraceError(string.Format("Failed to add user to GLS. Exception: {0}", ex));
					throw new CannotCreateUnifiedMailboxException(CoreResources.IDs.ErrorCannotCreateUnifiedMailbox, ex);
				}
			}
			this.TraceSuccess("GLS is currently disabled. Not adding user to GLS.");
			this.requestDetailsLogger_Set.Value(base.CallContext.ProtocolLog, CreateUnifiedMailboxMetadata.GlsAddUserOperationStatus, "Disabled.");
		}

		private const string CreateUnifiedMailboxName = "CreateUnifiedMailbox";

		internal readonly Hookable<Func<bool>> ADSessionSettings_IsGlsDisabled = Hookable<Func<bool>>.Create(true, () => ADSessionSettings.IsGlsDisabled);

		internal readonly Guid MailboxContainerGuid;

		internal readonly Hookable<Func<OrganizationIdParameter>> organizationIdParameterFromOrganizationId;

		internal static readonly Hookable<Func<CallContext, string>> callContextHttpContextUserIdentityGetSafeName = Hookable<Func<CallContext, string>>.Create(true, (CallContext callContext) => callContext.HttpContext.User.Identity.GetSafeName(true));

		internal readonly Hookable<Action<NewMailbox>> newMailboxExecute = Hookable<Action<NewMailbox>>.Create(true, delegate(NewMailbox newMailbox)
		{
			newMailbox.Execute();
		});

		internal readonly Hookable<Func<PSLocalTask<NewMailbox, Mailbox>, TaskErrorInfo>> newMailboxError = Hookable<Func<PSLocalTask<NewMailbox, Mailbox>, TaskErrorInfo>>.Create(true, (PSLocalTask<NewMailbox, Mailbox> newMailbox) => newMailbox.Error);

		private readonly Lazy<SmtpAddress> userPrincipalName;
	}
}
