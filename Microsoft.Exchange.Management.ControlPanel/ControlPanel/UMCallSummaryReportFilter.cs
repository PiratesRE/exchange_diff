using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UMCallSummaryReportFilter : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-UMCallSummaryReport";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Organization";
			}
		}

		[DataMember]
		public string GroupBy
		{
			get
			{
				return (string)base["GroupBy"];
			}
			set
			{
				base["GroupBy"] = value;
			}
		}

		[DataMember]
		public string UMDialPlan
		{
			get
			{
				return (string)base["UMDialPlan"];
			}
			set
			{
				base["UMDialPlan"] = value;
			}
		}

		[DataMember]
		public string UMIPGateway
		{
			get
			{
				return (string)base["UMIPGateway"];
			}
			set
			{
				base["UMIPGateway"] = value;
			}
		}

		public const string RbacParameters = "?GroupBy";
	}
}
