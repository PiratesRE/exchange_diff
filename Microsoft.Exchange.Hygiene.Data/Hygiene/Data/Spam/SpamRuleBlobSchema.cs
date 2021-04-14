using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	internal class SpamRuleBlobSchema
	{
		public static HygienePropertyDefinition IdProperty = new HygienePropertyDefinition("id_RuleId", typeof(Guid));

		public static HygienePropertyDefinition RuleIdProperty = new HygienePropertyDefinition("bi_RuleId", typeof(long), 0L, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition GroupIdProperty = new HygienePropertyDefinition("bi_GroupId", typeof(long), 0L, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition ScopeIdProperty = new HygienePropertyDefinition("ti_ScopeId", typeof(byte));

		public static HygienePropertyDefinition PriorityProperty = new HygienePropertyDefinition("ti_Priority", typeof(byte));

		public static HygienePropertyDefinition PublishingStateProperty = new HygienePropertyDefinition("ti_PublishingState", typeof(byte));

		public static HygienePropertyDefinition RuleDataProperty = new HygienePropertyDefinition("nvc_RuleData", typeof(string));

		public static HygienePropertyDefinition RuleMetaDataProperty = new HygienePropertyDefinition("nvc_RuleMetaData", typeof(string));

		public static HygienePropertyDefinition ProcessorDataProperty = new HygienePropertyDefinition("nvc_ProcessorData", typeof(string));

		public static HygienePropertyDefinition CreatedDatetimeProperty = new HygienePropertyDefinition("dt_CreatedDatetime", typeof(DateTime?));

		public static HygienePropertyDefinition ChangedDatetimeProperty = new HygienePropertyDefinition("dt_ChangedDatetime", typeof(DateTime?));

		public static HygienePropertyDefinition DeletedDatetimeProperty = new HygienePropertyDefinition("dt_DeletedDatetime", typeof(DateTime?));
	}
}
