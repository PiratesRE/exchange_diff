using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class AttachmentTypeEx
	{
		[DataMember]
		public string AttachmentType { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public string CalculatedContentType { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public string DateTimeCreated { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public string FileName { get; set; }

		[DataMember(EmitDefaultValue = false, IsRequired = false)]
		public string FileExtension { get; set; }
	}
}
