using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Approval.Applications.Resources;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Approval;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Approval.Applications
{
	internal class ApprovalApplication
	{
		private static ApprovalApplication CreateApprovalApplication(ApprovalApplicationId applicationId)
		{
			switch (applicationId)
			{
			case ApprovalApplicationId.AutoGroup:
				return new AutoGroupApplication();
			case ApprovalApplicationId.ModeratedRecipient:
				return new ModeratedDLApplication();
			default:
				return null;
			}
		}

		internal static ApprovalApplication[] CreateApprovalApplications()
		{
			ApprovalApplication[] array = new ApprovalApplication[2];
			for (ApprovalApplicationId approvalApplicationId = ApprovalApplicationId.AutoGroup; approvalApplicationId < ApprovalApplicationId.Count; approvalApplicationId++)
			{
				array[(int)approvalApplicationId] = ApprovalApplication.CreateApprovalApplication(approvalApplicationId);
			}
			return array;
		}

		internal static Collection<PSObject> ExecuteCommandsInRunspace(SmtpAddress user, PSCommand command, CultureInfo executingCulture, out string errorMessage, out string warningMessage)
		{
			Collection<PSObject> result = null;
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			errorMessage = string.Empty;
			warningMessage = string.Empty;
			IRecipientSession recipientSession = ApprovalProcessor.CreateRecipientSessionFromSmtpAddress(user);
			ADUser aduser = recipientSession.FindByProxyAddress(ProxyAddress.Parse((string)user)) as ADUser;
			if (aduser == null)
			{
				errorMessage = Strings.ErrorUserNotFound((string)user);
				return null;
			}
			GenericIdentity identity = new GenericIdentity(aduser.Sid.ToString());
			InitialSessionState initialSessionState = null;
			try
			{
				initialSessionState = new ExchangeRunspaceConfiguration(identity).CreateInitialSessionState();
				initialSessionState.LanguageMode = PSLanguageMode.NoLanguage;
			}
			catch (CmdletAccessDeniedException)
			{
				errorMessage = Strings.ErrorNoRBACRoleAssignment((string)user);
				return null;
			}
			CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
			CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
			try
			{
				if (executingCulture != null)
				{
					Thread.CurrentThread.CurrentCulture = executingCulture;
					Thread.CurrentThread.CurrentUICulture = executingCulture;
				}
				using (RunspaceProxy runspaceProxy = new RunspaceProxy(new RunspaceMediator(new ForestScopeRunspaceFactory(new BasicInitialSessionStateFactory(initialSessionState), new BasicPSHostFactory(typeof(RunspaceHost), true)), new EmptyRunspaceCache())))
				{
					try
					{
						PowerShellProxy powerShellProxy = new PowerShellProxy(runspaceProxy, command);
						result = powerShellProxy.Invoke<PSObject>();
						if (powerShellProxy.Errors != null)
						{
							foreach (ErrorRecord errorRecord in powerShellProxy.Errors)
							{
								stringBuilder.Append(errorRecord.ToString());
							}
						}
						if (powerShellProxy.Warnings != null)
						{
							foreach (WarningRecord warningRecord in powerShellProxy.Warnings)
							{
								stringBuilder2.Append(warningRecord.ToString());
							}
						}
					}
					catch (CmdletInvocationException)
					{
						stringBuilder.Append(Strings.ErrorTaskInvocationFailed((string)user).ToString(executingCulture));
					}
					catch (ParameterBindingException)
					{
						stringBuilder.Append(Strings.ErrorTaskInvocationFailed((string)user).ToString(executingCulture));
					}
					catch (CommandNotFoundException)
					{
						stringBuilder.Append(Strings.ErrorTaskInvocationFailed((string)user).ToString(executingCulture));
					}
					catch (RuntimeException)
					{
						stringBuilder.Append(Strings.ErrorTaskInvocationFailed((string)user).ToString(executingCulture));
					}
					finally
					{
						errorMessage = stringBuilder.ToString();
						warningMessage = stringBuilder2.ToString();
					}
				}
			}
			finally
			{
				if (executingCulture != null)
				{
					Thread.CurrentThread.CurrentCulture = currentCulture;
					Thread.CurrentThread.CurrentUICulture = currentUICulture;
				}
			}
			return result;
		}

		internal ApprovalApplication()
		{
		}

		internal virtual bool OnApprove(MessageItem message)
		{
			return true;
		}

		internal virtual bool OnReject(MessageItem message)
		{
			return true;
		}

		internal virtual void OnAllDecisionMakersNdred(MessageItem message)
		{
		}

		internal virtual void OnAllDecisionMakersOof(MessageItem message)
		{
		}

		internal virtual bool OnExpire(MessageItem message, out bool sendUpdate)
		{
			sendUpdate = true;
			return true;
		}

		internal virtual void OnStart()
		{
		}

		internal virtual void OnStop()
		{
		}
	}
}
