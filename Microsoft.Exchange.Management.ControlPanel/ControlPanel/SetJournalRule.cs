using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetJournalRule : JournalRuleObjectProperties
	{
		public sealed override string AssociatedCmdlet
		{
			get
			{
				return "Set-JournalRule";
			}
		}
	}
}
