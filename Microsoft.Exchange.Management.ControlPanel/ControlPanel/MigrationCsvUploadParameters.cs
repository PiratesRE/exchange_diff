using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MigrationCsvUploadParameters : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override string RbacScope
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[DataMember]
		public string MigrationBatchType { get; set; }

		[DataMember]
		public string IsOffboarding { get; set; }

		[DataMember]
		public bool AllowUnknownColumnsInCsv { get; set; }
	}
}
