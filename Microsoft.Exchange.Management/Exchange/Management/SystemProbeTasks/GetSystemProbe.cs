using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemProbeTasks
{
	[OutputType(new Type[]
	{
		typeof(Guid)
	})]
	[Cmdlet("Get", "SystemProbe")]
	public sealed class GetSystemProbe : Task
	{
		[Parameter(Mandatory = false)]
		public DateTimeOffset? StartTime
		{
			get
			{
				return this.startTime;
			}
			set
			{
				this.startTime = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DateTimeOffset? EndTime
		{
			get
			{
				return this.endTime;
			}
			set
			{
				this.endTime = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				Action<Type> action = delegate(Type type)
				{
					DateTimeOffset dateTimeOffset = this.StartTime ?? (DateTimeOffset.UtcNow - TimeSpan.FromHours(24.0));
					DateTimeOffset dateTimeOffset2 = this.EndTime ?? DateTimeOffset.UtcNow;
					MethodInfo method = type.GetMethod("GetProbes", BindingFlags.Static | BindingFlags.Public);
					List<Guid> list = method.Invoke(null, new object[]
					{
						dateTimeOffset,
						dateTimeOffset2
					}) as List<Guid>;
					list.ForEach(delegate(Guid g)
					{
						base.WriteObject(g);
					});
					if (list.Count == 0)
					{
						this.WriteWarning(Strings.NoSystemProbesFound(dateTimeOffset.DateTime, dateTimeOffset2.DateTime));
					}
				};
				SystemProbeAssemblyHelper.Invoke(this, action);
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private DateTimeOffset? startTime;

		private DateTimeOffset? endTime;
	}
}
