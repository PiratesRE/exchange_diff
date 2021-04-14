using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class EnableUMCallAnsweringRule : ChangeUMCallAnsweringRule
	{
		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "Enable-UMCallAnsweringRule";
			}
		}
	}
}
