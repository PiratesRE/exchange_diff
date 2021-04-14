using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RemoveUMCallAnsweringRule : ChangeUMCallAnsweringRule
	{
		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "Remove-UMCallAnsweringRule";
			}
		}
	}
}
