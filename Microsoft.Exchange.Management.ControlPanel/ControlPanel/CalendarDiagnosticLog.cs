using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class CalendarDiagnosticLog : WebServiceParameters
	{
		[DataMember]
		public string Subject
		{
			get
			{
				return (string)base["Subject"];
			}
			set
			{
				base["Subject"] = value;
			}
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-CalendarDiagnosticLog";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Self";
			}
		}
	}
}
