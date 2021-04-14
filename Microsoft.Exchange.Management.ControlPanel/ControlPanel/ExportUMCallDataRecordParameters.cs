using System;
using System.IO;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class ExportUMCallDataRecordParameters : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Export-UMCallDataRecord";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Organization";
			}
		}

		public ExDateTime Date
		{
			get
			{
				return (ExDateTime)base["Date"];
			}
			set
			{
				base["Date"] = value;
			}
		}

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

		public Stream ClientStream
		{
			get
			{
				return (Stream)base["ClientStream"];
			}
			set
			{
				base["ClientStream"] = value;
			}
		}

		public const string RbacParameters = "?Date&ClientStream";
	}
}
