using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NewUMCallAnsweringRule : UMCallAnsweringRuleParameters
	{
		public NewUMCallAnsweringRule()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
		}

		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "New-UMCallAnsweringRule";
			}
		}
	}
}
