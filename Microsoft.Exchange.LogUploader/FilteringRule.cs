using System;
using System.Runtime.Serialization;
using Microsoft.Office.Compliance.Audit.Schema;

namespace Microsoft.Exchange.LogUploader
{
	[DataContract]
	public class FilteringRule : IExtensibleDataObject, IVerifiable
	{
		[DataMember(EmitDefaultValue = false, IsRequired = true)]
		public FilteringPredicate Predicate { get; set; }

		[DataMember(IsRequired = true)]
		public Actions ActionToPerform { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ThrottlingInfo Throttle { get; set; }

		ExtensionDataObject IExtensibleDataObject.ExtensionData { get; set; }

		public virtual void Initialize()
		{
			if (this.Predicate != null)
			{
				this.Predicate.Initialize();
			}
			if (this.Throttle != null)
			{
				this.Throttle.Initialize();
			}
		}

		public virtual void Validate()
		{
			if (this.Predicate == null)
			{
				throw new ExpectedValueException("FilteringRule", "Predicate");
			}
			if (this.Predicate != null)
			{
				this.Predicate.Validate();
			}
			if (!Enum.IsDefined(typeof(Actions), this.ActionToPerform))
			{
				throw new InvalidEnumValueException("FilteringRule", "ActionToPerform", this.ActionToPerform);
			}
			if (this.Throttle != null)
			{
				this.Throttle.Validate();
			}
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			this.Initialize();
		}
	}
}
