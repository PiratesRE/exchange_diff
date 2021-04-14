using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ChangeInboxRule : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-InboxRule";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Self";
			}
		}

		public override string SuppressConfirmParameterName
		{
			get
			{
				return "AlwaysDeleteOutlookRulesBlob";
			}
		}
	}
}
