using System;
using System.Management.Automation;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.FfoQuarantine
{
	[OutputType(new Type[]
	{
		typeof(bool)
	})]
	[Cmdlet("Release", "QuarantineMessage", SupportsShouldProcess = true, DefaultParameterSetName = "ReleaseToSelf")]
	public sealed class ReleaseQuarantineMessage : Task
	{
		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
		public string Identity { get; set; }

		[Parameter(Mandatory = false)]
		public OrganizationIdParameter Organization { get; set; }

		[Parameter(ParameterSetName = "OrgReleaseToUser", Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public string[] User { get; set; }

		[Parameter]
		public SwitchParameter ReportFalsePositive { get; set; }

		[Parameter(ParameterSetName = "OrgReleaseToAll", Mandatory = true)]
		public SwitchParameter ReleaseToAll { get; set; }

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageReleaseQuarantineMessage(this.Identity.ToString());
			}
		}

		protected override void InternalProcessRecord()
		{
			SystemProbe.Trace(ReleaseQuarantineMessage.ComponentName, SystemProbe.Status.Pass, "Entering InternalProcessRecord", new object[0]);
			try
			{
				Assembly assembly = Assembly.Load("Microsoft.Exchange.Hygiene.ManagementHelper");
				Type type = assembly.GetType("Microsoft.Exchange.Hygiene.ManagementHelper.FfoQuarantine.ReleaseQuarantineMessageHelper");
				MethodInfo method = type.GetMethod("InternalProcessRecordHelper", BindingFlags.Static | BindingFlags.Public, null, new Type[]
				{
					typeof(ReleaseQuarantineMessage)
				}, null);
				method.Invoke(null, new object[]
				{
					this
				});
			}
			catch (TargetInvocationException ex)
			{
				SystemProbe.Trace(ReleaseQuarantineMessage.ComponentName, SystemProbe.Status.Fail, "TargetInvocationException in InternalProcessRecord: {0}", new object[]
				{
					ex.ToString()
				});
				if (ex.InnerException != null)
				{
					throw ex.InnerException;
				}
				throw;
			}
			catch (Exception ex2)
			{
				SystemProbe.Trace(ReleaseQuarantineMessage.ComponentName, SystemProbe.Status.Fail, "Unhandled Exception in InternalProcessRecord: {0}", new object[]
				{
					ex2.ToString()
				});
				throw;
			}
			SystemProbe.Trace(ReleaseQuarantineMessage.ComponentName, SystemProbe.Status.Pass, "Exiting InternalProcessRecord", new object[0]);
		}

		private static readonly string ComponentName = "ReleaseQuarantineMessage";
	}
}
