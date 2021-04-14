using System;
using System.Management.Automation;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.FfoQuarantine
{
	[Cmdlet("Get", "QuarantineMessageHeader")]
	[OutputType(new Type[]
	{
		typeof(QuarantineMessageHeader)
	})]
	public sealed class GetQuarantineMessageHeader : Task
	{
		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
		public string Identity { get; set; }

		[Parameter(ValueFromPipelineByPropertyName = true)]
		public OrganizationIdParameter Organization { get; set; }

		protected override void InternalProcessRecord()
		{
			SystemProbe.Trace(GetQuarantineMessageHeader.ComponentName, SystemProbe.Status.Pass, "Entering InternalProcessRecord", new object[0]);
			try
			{
				Assembly assembly = Assembly.Load("Microsoft.Exchange.Hygiene.ManagementHelper");
				Type type = assembly.GetType("Microsoft.Exchange.Hygiene.ManagementHelper.FfoQuarantine.GetQuarantineMessageHeaderHelper");
				MethodInfo method = type.GetMethod("InternalProcessRecordHelper", BindingFlags.Static | BindingFlags.Public, null, new Type[]
				{
					typeof(GetQuarantineMessageHeader)
				}, null);
				method.Invoke(null, new object[]
				{
					this
				});
			}
			catch (TargetInvocationException ex)
			{
				SystemProbe.Trace(GetQuarantineMessageHeader.ComponentName, SystemProbe.Status.Fail, "TargetInvocationException in InternalProcessRecord: {0}", new object[]
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
				SystemProbe.Trace(GetQuarantineMessageHeader.ComponentName, SystemProbe.Status.Fail, "Unhandled Exception in InternalProcessRecord: {0}", new object[]
				{
					ex2.ToString()
				});
				throw;
			}
			SystemProbe.Trace(GetQuarantineMessageHeader.ComponentName, SystemProbe.Status.Pass, "Exiting InternalProcessRecord", new object[0]);
		}

		private static readonly string ComponentName = "GetQuarantineMessageHeader";
	}
}
