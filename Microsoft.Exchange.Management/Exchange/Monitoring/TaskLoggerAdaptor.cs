using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring
{
	internal sealed class TaskLoggerAdaptor : ChainedLogger
	{
		internal TaskLoggerAdaptor(Task instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			this.instance = instance;
		}

		protected override void InternalWriteVerbose(LocalizedString message)
		{
			this.instance.WriteVerbose(message);
		}

		protected override void InternalWriteDebug(LocalizedString message)
		{
			this.instance.WriteDebug(message);
		}

		protected override void InternalWriteWarning(LocalizedString message)
		{
			this.instance.WriteWarning(message);
		}

		protected override string InternalGetDiagnosticInfo(string prefix)
		{
			return prefix;
		}

		private readonly Task instance;
	}
}
