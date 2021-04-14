using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Management.ClassificationDefinitions;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class Fingerprint : BaseRow
	{
		public Fingerprint(Fingerprint print)
		{
			this.Value = print.ToString();
			this.Description = print.Description;
		}

		[DataMember]
		public string Value { get; set; }

		[DataMember]
		public string Description { get; set; }
	}
}
