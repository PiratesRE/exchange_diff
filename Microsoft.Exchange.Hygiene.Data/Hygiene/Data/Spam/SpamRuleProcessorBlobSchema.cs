using System;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class SpamRuleProcessorBlobSchema
	{
		public static HygienePropertyDefinition IdProperty = new HygienePropertyDefinition("id_ProcessorId", typeof(Guid));

		public static HygienePropertyDefinition ProcessorIdProperty = new HygienePropertyDefinition("nvc_ProcessorId", typeof(string));

		public static HygienePropertyDefinition DataProperty = new HygienePropertyDefinition("nvc_Data", typeof(string));

		public static HygienePropertyDefinition CreatedDatetimeProperty = new HygienePropertyDefinition("dt_CreatedDatetime", typeof(DateTime?));

		public static HygienePropertyDefinition ChangedDatetimeProperty = new HygienePropertyDefinition("dt_ChangedDatetime", typeof(DateTime?));

		public static HygienePropertyDefinition DeletedDatetimeProperty = new HygienePropertyDefinition("dt_DeletedDatetime", typeof(DateTime?));
	}
}
