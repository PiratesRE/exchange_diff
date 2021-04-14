using System;
using Microsoft.Exchange.InfoWorker.Common.MessageTracking;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class GetMessageTrackingReportParameters : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-MessageTrackingReport";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Self";
			}
		}

		public string Identity
		{
			get
			{
				return (string)base["Identity"];
			}
			set
			{
				base["Identity"] = value;
			}
		}

		public int ResultSize
		{
			get
			{
				return (int)base["ResultSize"];
			}
			set
			{
				base["ResultSize"] = value;
			}
		}

		public string Recipients
		{
			get
			{
				return base["Recipients"].StringArrayJoin(",");
			}
			set
			{
				base["Recipients"] = value.ToArrayOfStrings();
			}
		}

		public bool ByPassDelegateChecking
		{
			get
			{
				return (bool?)base["ByPassDelegateChecking"] == true;
			}
			set
			{
				if (base.CanSetParameter("ByPassDelegateChecking"))
				{
					base["ByPassDelegateChecking"] = value;
				}
			}
		}

		public MessageTrackingDetailLevel DetailLevel
		{
			get
			{
				return (MessageTrackingDetailLevel)base["DetailLevel"];
			}
			set
			{
				if (base.CanSetParameter("DetailLevel"))
				{
					base["DetailLevel"] = value;
				}
			}
		}

		public const string RbacParameters = "?ResultSize&Identity&Recipients";
	}
}
