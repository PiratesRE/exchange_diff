using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Inference.Common
{
	[DataContract(Name = "InferenceBaseModelData")]
	[Serializable]
	internal abstract class InferenceBaseModelItem
	{
		public InferenceBaseModelItem()
		{
			this.CreationTime = DateTime.UtcNow;
			this.LastModifiedTime = DateTime.UtcNow;
		}

		[DataMember]
		public Version Version { get; set; }

		[DataMember]
		public DateTime CreationTime { get; set; }

		[DataMember]
		public DateTime LastModifiedTime { get; set; }
	}
}
