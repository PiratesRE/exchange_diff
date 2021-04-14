using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetUMCallAnsweringRule : UMCallAnsweringRuleParameters
	{
		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "Set-UMCallAnsweringRule";
			}
		}
	}
}
