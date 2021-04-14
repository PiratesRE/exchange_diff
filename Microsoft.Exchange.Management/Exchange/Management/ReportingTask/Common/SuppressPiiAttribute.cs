using System;

namespace Microsoft.Exchange.Management.ReportingTask.Common
{
	[AttributeUsage(AttributeTargets.Property, Inherited = false)]
	public class SuppressPiiAttribute : Attribute
	{
		public PiiDataType PiiDataType { get; set; }
	}
}
