using System;
using System.Management.Automation;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.FfoQuarantine
{
	[Cmdlet("Get", "QuarantineMessage", DefaultParameterSetName = "Summary")]
	[OutputType(new Type[]
	{
		typeof(QuarantineMessage)
	})]
	public sealed class GetQuarantineMessage : Task
	{
		[Parameter(ParameterSetName = "Details", Mandatory = true, ValueFromPipelineByPropertyName = true)]
		[ValidateNotNullOrEmpty]
		public string Identity { get; set; }

		[Parameter]
		public OrganizationIdParameter Organization { get; set; }

		[Parameter(ParameterSetName = "Summary")]
		public DateTime? StartReceivedDate { get; set; }

		[Parameter(ParameterSetName = "Summary")]
		public DateTime? EndReceivedDate { get; set; }

		[Parameter(ParameterSetName = "Summary")]
		public string[] Domain { get; set; }

		[Parameter(ParameterSetName = "Summary")]
		public QuarantineMessageDirectionEnum? Direction { get; set; }

		[Parameter(ParameterSetName = "Summary")]
		[ValidateLength(1, 320)]
		public string MessageId { get; set; }

		[Parameter]
		[ValidateLength(1, 320)]
		public string[] SenderAddress { get; set; }

		[Parameter(ParameterSetName = "Summary")]
		[ValidateLength(1, 320)]
		public string[] RecipientAddress { get; set; }

		[ValidateLength(1, 320)]
		[Parameter(ParameterSetName = "Summary")]
		public string Subject { get; set; }

		[Parameter(ParameterSetName = "Summary")]
		public QuarantineMessageTypeEnum? Type { get; set; }

		[Parameter(ParameterSetName = "Summary")]
		[ValidateNotNullOrEmpty]
		public bool? Reported { get; set; }

		[Parameter(ParameterSetName = "Summary")]
		public DateTime? StartExpiresDate { get; set; }

		[Parameter(ParameterSetName = "Summary")]
		public DateTime? EndExpiresDate { get; set; }

		[Parameter(ParameterSetName = "Summary")]
		public int? Page { get; set; }

		[Parameter(ParameterSetName = "Summary")]
		public int? PageSize { get; set; }

		internal new OrganizationId ExecutingUserOrganizationId
		{
			get
			{
				return base.ExecutingUserOrganizationId;
			}
		}

		internal new OrganizationId CurrentOrganizationId
		{
			get
			{
				return base.CurrentOrganizationId;
			}
		}

		protected sealed override void InternalProcessRecord()
		{
			SystemProbe.Trace(GetQuarantineMessage.ComponentName, SystemProbe.Status.Pass, "Entering InternalProcessRecord", new object[0]);
			try
			{
				Assembly assembly = Assembly.Load("Microsoft.Exchange.Hygiene.ManagementHelper");
				Type type = assembly.GetType("Microsoft.Exchange.Hygiene.ManagementHelper.FfoQuarantine.GetQuarantineMessageHelper");
				MethodInfo method = type.GetMethod("InternalProcessRecordHelper", BindingFlags.Static | BindingFlags.Public, null, new Type[]
				{
					typeof(GetQuarantineMessage)
				}, null);
				method.Invoke(null, new object[]
				{
					this
				});
			}
			catch (TargetInvocationException ex)
			{
				SystemProbe.Trace(GetQuarantineMessage.ComponentName, SystemProbe.Status.Fail, "TargetInvocationException in InternalProcessRecord: {0}", new object[]
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
				SystemProbe.Trace(GetQuarantineMessage.ComponentName, SystemProbe.Status.Fail, "Unhandled Exception in InternalProcessRecord: {0}", new object[]
				{
					ex2.ToString()
				});
				throw;
			}
			SystemProbe.Trace(GetQuarantineMessage.ComponentName, SystemProbe.Status.Pass, "Exiting InternalProcessRecord", new object[0]);
		}

		private static readonly string ComponentName = "GetQuarantineMessage";
	}
}
