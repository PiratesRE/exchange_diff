using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "Property")]
	internal class Property : Resource
	{
		public Property(string selfUri) : base(selfUri)
		{
		}

		[DataMember(Name = "Name", EmitDefaultValue = true)]
		public string Name
		{
			get
			{
				return base.GetValue<string>("Name");
			}
			set
			{
				base.SetValue<string>("Name", value);
			}
		}

		[DataMember(Name = "Value", EmitDefaultValue = true)]
		public string Value
		{
			get
			{
				return base.GetValue<string>("Value");
			}
			set
			{
				base.SetValue<string>("Value", value);
			}
		}

		[Ignore]
		[DataMember(Name = "Values", EmitDefaultValue = false)]
		public PropertyBag Values
		{
			get
			{
				return base.GetValue<PropertyBag>("Values");
			}
			set
			{
				base.SetValue<PropertyBag>("Values", value);
			}
		}
	}
}
