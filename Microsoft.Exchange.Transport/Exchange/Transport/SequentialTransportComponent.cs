using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport
{
	internal sealed class SequentialTransportComponent : CompositeTransportComponent
	{
		public SequentialTransportComponent(string description) : base(description)
		{
		}

		public override void Load()
		{
			List<ITransportComponent> list = new List<ITransportComponent>();
			using (IEnumerator<ITransportComponent> enumerator = base.TransportComponents.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ITransportComponent child = enumerator.Current;
					try
					{
						ExTraceGlobals.GeneralTracer.TraceDebug<string>(0L, "Loading component {0}.", CompositeTransportComponent.GetComponentName(child));
						base.BeginTiming(CompositeTransportComponent.Operation.Load, child);
						ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
						{
							child.Load();
						}, 1);
						if (!adoperationResult.Succeeded)
						{
							throw new TransportComponentLoadFailedException(Strings.ReadingADConfigFailed, adoperationResult.Exception);
						}
						base.EndTiming(CompositeTransportComponent.Operation.Load, child);
						CompositeTransportComponent.RegisterForDiagnostics(child);
						ExTraceGlobals.GeneralTracer.TraceDebug<string>(0L, "Loaded component {0}.", CompositeTransportComponent.GetComponentName(child));
						list.Add(child);
					}
					catch (TransportComponentLoadFailedException ex)
					{
						ExTraceGlobals.GeneralTracer.TraceDebug<string, string>(0L, "Failed loading component {0}. {1}", CompositeTransportComponent.GetComponentName(child), ex.Message);
						for (int i = list.Count - 1; i >= 0; i--)
						{
							ITransportComponent transportComponent = list[i];
							CompositeTransportComponent.UnRegisterForDiagnostics(transportComponent);
							transportComponent.Unload();
						}
						string message = string.Format(CultureInfo.InvariantCulture, "Loading of component '{0}' failed.", new object[]
						{
							CompositeTransportComponent.GetComponentName(child)
						});
						throw new TransportComponentLoadFailedException(message, ex);
					}
				}
			}
		}
	}
}
