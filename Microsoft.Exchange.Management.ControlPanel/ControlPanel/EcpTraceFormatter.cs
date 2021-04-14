using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class EcpTraceFormatter<T>
	{
		public EcpTraceFormatter(T o)
		{
			this.innerObject = o;
		}

		public override string ToString()
		{
			return EcpTraceHelper.GetTraceString(this.innerObject);
		}

		private T innerObject;
	}
}
