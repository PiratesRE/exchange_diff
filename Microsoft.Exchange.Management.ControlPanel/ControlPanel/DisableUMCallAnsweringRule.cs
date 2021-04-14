using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class DisableUMCallAnsweringRule : ChangeUMCallAnsweringRule
	{
		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "Disable-UMCallAnsweringRule";
			}
		}
	}
}
