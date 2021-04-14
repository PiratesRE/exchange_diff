using System;
using System.Runtime.Serialization;
using Microsoft.Office.Compliance.Audit.Schema;

namespace Microsoft.Exchange.LogUploader
{
	[DataContract]
	public class ParsingPredicate : IExtensibleDataObject, IVerifiable
	{
		[DataMember(IsRequired = true)]
		public Parsing ParsingOutcome { get; set; }

		ExtensionDataObject IExtensibleDataObject.ExtensionData { get; set; }

		public virtual void Initialize()
		{
		}

		public virtual void Validate()
		{
			if (!Enum.IsDefined(typeof(Parsing), this.ParsingOutcome))
			{
				throw new InvalidEnumValueException("ParsingPredicate", "ParsingOutcome", this.ParsingOutcome);
			}
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			this.Initialize();
		}
	}
}
