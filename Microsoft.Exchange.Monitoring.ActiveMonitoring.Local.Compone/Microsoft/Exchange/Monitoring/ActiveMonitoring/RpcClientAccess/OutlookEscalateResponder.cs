using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.RpcClientAccess
{
	internal class OutlookEscalateResponder<TInterpretedResult> : EscalateResponder where TInterpretedResult : InterpretedResult, new()
	{
		public static ResponderDefinition Configure(ResponderDefinition definition)
		{
			definition.SetType(typeof(OutlookEscalateResponder<TInterpretedResult>));
			return definition;
		}

		internal override void BeforeContentGeneration(ResponseMessageReader propertyReader)
		{
			base.BeforeContentGeneration(propertyReader);
			propertyReader.AddObjectResolver<TInterpretedResult>("MapiProbe", delegate
			{
				TInterpretedResult result = Activator.CreateInstance<TInterpretedResult>();
				result.RawResult = (ProbeResult)propertyReader.EnsureNotNull((ResponseMessageReader pr) => pr.GetObject("Probe"));
				return result;
			});
		}
	}
}
