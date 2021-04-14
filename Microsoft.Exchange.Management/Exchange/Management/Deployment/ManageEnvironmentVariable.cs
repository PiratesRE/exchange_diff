using System;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public abstract class ManageEnvironmentVariable : Task
	{
		public ManageEnvironmentVariable()
		{
			this.Target = EnvironmentVariableTarget.Process;
		}

		[Parameter(Mandatory = true)]
		public string Name
		{
			get
			{
				return (string)base.Fields["Name"];
			}
			set
			{
				base.Fields["Name"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnvironmentVariableTarget Target
		{
			get
			{
				return (EnvironmentVariableTarget)base.Fields["Target"];
			}
			set
			{
				base.Fields["Target"] = value;
			}
		}

		protected void SetVariable(string name, string value, EnvironmentVariableTarget target)
		{
			try
			{
				Environment.SetEnvironmentVariable(name, value, target);
			}
			catch (ArgumentNullException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, null);
			}
			catch (ArgumentException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidArgument, null);
			}
			catch (NotSupportedException exception3)
			{
				base.WriteError(exception3, ErrorCategory.InvalidOperation, null);
			}
			catch (SecurityException exception4)
			{
				base.WriteError(exception4, ErrorCategory.SecurityError, null);
			}
		}
	}
}
