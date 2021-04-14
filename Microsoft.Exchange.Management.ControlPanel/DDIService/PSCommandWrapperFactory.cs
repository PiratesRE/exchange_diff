using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Management.DDIService
{
	public class PSCommandWrapperFactory : IPSCommandWrapperFactory
	{
		private PSCommandWrapperFactory()
		{
		}

		public static IPSCommandWrapperFactory Instance { get; internal set; } = new PSCommandWrapperFactory();

		public IPSCommandWrapper CreatePSCommand()
		{
			return new PSCommandWrapper(new PSCommand());
		}
	}
}
