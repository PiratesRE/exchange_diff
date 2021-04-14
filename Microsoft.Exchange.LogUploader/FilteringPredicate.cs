using System;
using System.Runtime.Serialization;
using Microsoft.Office.Compliance.Audit.Schema;

namespace Microsoft.Exchange.LogUploader
{
	[DataContract]
	public class FilteringPredicate : IExtensibleDataObject, IVerifiable
	{
		[DataMember(EmitDefaultValue = false)]
		public string Component { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Tenant { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string User { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Operation { get; set; }

		ExtensionDataObject IExtensibleDataObject.ExtensionData { get; set; }

		public virtual void Initialize()
		{
			this.Component = "*";
			this.Tenant = "*";
			this.User = "*";
			this.Operation = "*";
		}

		public virtual void Validate()
		{
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			this.Initialize();
		}
	}
}
