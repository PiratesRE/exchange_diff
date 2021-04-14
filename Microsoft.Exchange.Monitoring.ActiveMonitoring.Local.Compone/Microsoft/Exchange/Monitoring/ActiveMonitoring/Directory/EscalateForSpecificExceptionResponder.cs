using System;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory
{
	public class EscalateForSpecificExceptionResponder : EscalateResponder
	{
		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			DirectoryUtils.InvokeBaseResponderMethodIfRequired(this, delegate(CancellationToken cancelToken)
			{
				this.<>n__FabricatedMethod1(cancelToken);
			}, base.TraceContext, cancellationToken);
		}
	}
}
