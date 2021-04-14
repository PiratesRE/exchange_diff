using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM;
using Microsoft.Exchange.UM.Prompts.Provisioning;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Remove", "UMDialPlan", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveUMDialPlan : RemoveSystemConfigurationObjectTask<UMDialPlanIdParameter, UMDialPlan>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.Confirm(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!base.HasErrors)
			{
				UMDialPlan dataObject = base.DataObject;
				LocalizedException ex = null;
				if (dataObject.UMServers != null && dataObject.UMServers.Count > 0)
				{
					using (IEnumerator<ADObjectId> enumerator = dataObject.UMServers.GetEnumerator())
					{
						enumerator.MoveNext();
						ex = new DialPlanAssociatedWithServerException(enumerator.Current.ToString());
					}
				}
				if (base.DataObject.CheckForAssociatedUsers())
				{
					ex = new DialPlanAssociatedWithUserException();
				}
				else if (base.DataObject.CheckForAssociatedPolicies())
				{
					ex = new DialPlanAssociatedWithPoliciesException();
				}
				if (dataObject.UMAutoAttendants != null && dataObject.UMAutoAttendants.Count > 0)
				{
					using (IEnumerator<ADObjectId> enumerator2 = dataObject.UMAutoAttendants.GetEnumerator())
					{
						enumerator2.MoveNext();
						ex = new DialPlanAssociatedWithAutoAttendantException(enumerator2.Current.ToString());
					}
				}
				if (dataObject.UMIPGateway != null && dataObject.UMIPGateway.Count > 0)
				{
					using (IEnumerator<ADObjectId> enumerator3 = dataObject.UMIPGateway.GetEnumerator())
					{
						enumerator3.MoveNext();
						ex = new DialPlanAssociatedWithIPGatewayException(enumerator3.Current.Name);
					}
				}
				if (ex != null)
				{
					base.WriteError(ex, ErrorCategory.InvalidOperation, null);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.DeleteContentFromPublishingPoint();
			base.InternalProcessRecord();
			if (!base.HasErrors)
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_DialPlanRemoved, null, new object[]
				{
					base.DataObject.Name
				});
			}
			TaskLogger.LogExit();
		}

		private void DeleteContentFromPublishingPoint()
		{
			try
			{
				if (this.IsRunningInProcWithPerseus())
				{
					InterServerMailboxAccessor.TestXSOHook = true;
				}
				using (IPublishingSession publishingSession = PublishingPoint.GetPublishingSession(Environment.UserName, base.DataObject))
				{
					publishingSession.Delete();
				}
			}
			catch (PublishingException ex)
			{
				base.WriteWarning(ex.Message);
			}
		}

		private bool IsRunningInProcWithPerseus()
		{
			bool result = false;
			if (this.IsRunningInTestEnvironment())
			{
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					if (currentProcess.ProcessName.StartsWith("Perseus", StringComparison.OrdinalIgnoreCase))
					{
						result = true;
					}
				}
			}
			return result;
		}

		private bool IsRunningInTestEnvironment()
		{
			string environmentVariable = Environment.GetEnvironmentVariable("USERDNSDOMAIN");
			return !string.IsNullOrEmpty(environmentVariable) && environmentVariable.EndsWith(".EXTEST.MICROSOFT.COM", StringComparison.OrdinalIgnoreCase);
		}
	}
}
