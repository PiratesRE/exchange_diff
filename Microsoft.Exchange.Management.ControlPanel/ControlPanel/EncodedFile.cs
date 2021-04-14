using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class EncodedFile
	{
		[DataMember]
		public string FileContent { get; set; }
	}
}
