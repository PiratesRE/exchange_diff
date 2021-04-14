using System;

namespace Microsoft.Exchange.Data
{
	internal class UMPartnerMinimalTranscriptionContext : UMPartnerContext
	{
		public string TimeoutMessageId
		{
			get
			{
				return (string)base[UMPartnerMinimalTranscriptionContext.Schema.TimeoutMessageId];
			}
			set
			{
				base[UMPartnerMinimalTranscriptionContext.Schema.TimeoutMessageId] = value;
			}
		}

		protected override UMPartnerContext.UMPartnerContextSchema ContextSchema
		{
			get
			{
				return UMPartnerMinimalTranscriptionContext.Schema;
			}
		}

		private static readonly UMPartnerMinimalTranscriptionContext.UMPartnerMinimalTranscriptionContextSchema Schema = new UMPartnerMinimalTranscriptionContext.UMPartnerMinimalTranscriptionContextSchema();

		private class UMPartnerMinimalTranscriptionContextSchema : UMPartnerContext.UMPartnerContextSchema
		{
			public readonly UMPartnerContext.UMPartnerContextPropertyDefinition TimeoutMessageId = new UMPartnerContext.UMPartnerContextPropertyDefinition("TimeoutMessageId", typeof(string), string.Empty);
		}
	}
}
