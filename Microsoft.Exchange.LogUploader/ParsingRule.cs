using System;
using System.Runtime.Serialization;
using Microsoft.Office.Compliance.Audit.Schema;

namespace Microsoft.Exchange.LogUploader
{
	[DataContract]
	public class ParsingRule : IExtensibleDataObject, IVerifiable
	{
		[DataMember(EmitDefaultValue = false, IsRequired = true)]
		public ParsingPredicate Predicate { get; set; }

		[DataMember(IsRequired = true)]
		public Actions ActionToPerform { get; set; }

		ExtensionDataObject IExtensibleDataObject.ExtensionData { get; set; }

		public virtual void Initialize()
		{
			if (this.Predicate != null)
			{
				this.Predicate.Initialize();
			}
		}

		public virtual void Validate()
		{
			if (this.Predicate == null)
			{
				throw new ExpectedValueException("ParsingRule", "Predicate");
			}
			if (this.Predicate != null)
			{
				this.Predicate.Validate();
			}
			if (!Enum.IsDefined(typeof(Actions), this.ActionToPerform))
			{
				throw new InvalidEnumValueException("ParsingRule", "ActionToPerform", this.ActionToPerform);
			}
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			this.Initialize();
		}
	}
}
