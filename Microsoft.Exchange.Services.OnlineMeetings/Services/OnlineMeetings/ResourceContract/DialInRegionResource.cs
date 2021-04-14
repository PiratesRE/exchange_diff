using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "dialInRegion")]
	internal class DialInRegionResource : Resource
	{
		public DialInRegionResource(string selfUri) : base(selfUri)
		{
		}

		[DataMember(Name = "name", EmitDefaultValue = false)]
		public string Name
		{
			get
			{
				return base.GetValue<string>("name");
			}
			set
			{
				base.SetValue<string>("name", value);
			}
		}

		[DataMember(Name = "languages", EmitDefaultValue = false)]
		public string[] Languages
		{
			get
			{
				return base.GetValue<string[]>("languages");
			}
			set
			{
				base.SetValue<string[]>("languages", value);
			}
		}

		[DataMember(Name = "number", EmitDefaultValue = false)]
		public string Number
		{
			get
			{
				return base.GetValue<string>("number");
			}
			set
			{
				base.SetValue<string>("number", value);
			}
		}

		public const string Token = "dialInRegion";

		private const string NamePropertyName = "name";

		private const string LanguagesPropertyName = "languages";

		private const string NumberPropertyName = "number";
	}
}
