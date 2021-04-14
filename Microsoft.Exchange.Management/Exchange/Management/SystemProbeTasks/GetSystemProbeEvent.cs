using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemProbeTasks
{
	[Cmdlet("Get", "SystemProbeEvent")]
	public sealed class GetSystemProbeEvent : Task
	{
		[Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true)]
		public Guid Guid
		{
			get
			{
				return this.guid;
			}
			set
			{
				this.guid = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				Action<Type> action = delegate(Type type)
				{
					MethodInfo method = type.GetMethod("GetProbeEvents", BindingFlags.Static | BindingFlags.Public);
					List<object> list = method.Invoke(null, new object[]
					{
						this.guid
					}) as List<object>;
					list.ForEach(delegate(object e)
					{
						base.WriteObject(e);
					});
					if (list.Count == 0)
					{
						this.WriteWarning(Strings.NoSystemProbeEventFound(this.guid));
					}
				};
				SystemProbeAssemblyHelper.Invoke(this, action);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private Guid guid;
	}
}
