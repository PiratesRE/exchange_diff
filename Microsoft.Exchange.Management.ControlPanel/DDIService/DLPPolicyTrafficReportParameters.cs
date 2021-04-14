using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	[DataContract]
	public class DLPPolicyTrafficReportParameters : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-MailTrafficPolicyReport";
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
		public string EventType
		{
			get
			{
				return base["EventType"].StringArrayJoin(",");
			}
			set
			{
				base["EventType"] = value.ToArrayOfStrings();
			}
		}

		[DataMember]
		public string Direction
		{
			get
			{
				return base["Direction"].StringArrayJoin(",");
			}
			set
			{
				base["Direction"] = value.ToArrayOfStrings();
			}
		}

		[DataMember]
		public DateTime? StartDate
		{
			get
			{
				return (DateTime?)base["StartDate"];
			}
			set
			{
				base["StartDate"] = value;
			}
		}

		[DataMember]
		public DateTime? EndDate
		{
			get
			{
				return (DateTime?)base["EndDate"];
			}
			set
			{
				base["EndDate"] = value;
			}
		}
	}
}
