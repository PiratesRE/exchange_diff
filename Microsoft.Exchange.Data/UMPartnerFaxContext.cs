using System;

namespace Microsoft.Exchange.Data
{
	internal class UMPartnerFaxContext : UMPartnerContext
	{
		public string CallId
		{
			get
			{
				return (string)base[UMPartnerFaxContext.Schema.CallId];
			}
			set
			{
				base[UMPartnerFaxContext.Schema.CallId] = value;
			}
		}

		public string CallerId
		{
			get
			{
				return (string)base[UMPartnerFaxContext.Schema.CallerId];
			}
			set
			{
				base[UMPartnerFaxContext.Schema.CallerId] = value;
			}
		}

		public string Extension
		{
			get
			{
				return (string)base[UMPartnerFaxContext.Schema.Extension];
			}
			set
			{
				base[UMPartnerFaxContext.Schema.Extension] = value;
			}
		}

		public string PhoneContext
		{
			get
			{
				return (string)base[UMPartnerFaxContext.Schema.PhoneContext];
			}
			set
			{
				base[UMPartnerFaxContext.Schema.PhoneContext] = value;
			}
		}

		public string CallerIdDisplayName
		{
			get
			{
				return (string)base[UMPartnerFaxContext.Schema.CallerIdDisplayName];
			}
			set
			{
				base[UMPartnerFaxContext.Schema.CallerIdDisplayName] = value;
			}
		}

		protected override UMPartnerContext.UMPartnerContextSchema ContextSchema
		{
			get
			{
				return UMPartnerFaxContext.Schema;
			}
		}

		private static readonly UMPartnerFaxContext.UMPartnerFaxContextSchema Schema = new UMPartnerFaxContext.UMPartnerFaxContextSchema();

		private class UMPartnerFaxContextSchema : UMPartnerContext.UMPartnerContextSchema
		{
			public UMPartnerContext.UMPartnerContextPropertyDefinition CallId = new UMPartnerContext.UMPartnerContextPropertyDefinition("CallId", typeof(string), string.Empty);

			public UMPartnerContext.UMPartnerContextPropertyDefinition CallerId = new UMPartnerContext.UMPartnerContextPropertyDefinition("CallerId", typeof(string), string.Empty);

			public UMPartnerContext.UMPartnerContextPropertyDefinition Extension = new UMPartnerContext.UMPartnerContextPropertyDefinition("Extension", typeof(string), string.Empty);

			public UMPartnerContext.UMPartnerContextPropertyDefinition PhoneContext = new UMPartnerContext.UMPartnerContextPropertyDefinition("PhoneContext", typeof(string), string.Empty);

			public readonly UMPartnerContext.UMPartnerContextPropertyDefinition CallerIdDisplayName = new UMPartnerContext.UMPartnerContextPropertyDefinition("CallerIdDisplayName", typeof(string), string.Empty);
		}
	}
}
