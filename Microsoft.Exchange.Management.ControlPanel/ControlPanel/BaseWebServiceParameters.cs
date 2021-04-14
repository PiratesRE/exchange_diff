using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public sealed class BaseWebServiceParameters : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override string RbacScope
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}
