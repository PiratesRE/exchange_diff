using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol
{
	public class FaultRecord
	{
		static FaultRecord()
		{
			ComplianceSerializationDescription<FaultRecord.MutableKeyValuePair> complianceSerializationDescription = new ComplianceSerializationDescription<FaultRecord.MutableKeyValuePair>();
			complianceSerializationDescription.ComplianceStructureId = 98;
			complianceSerializationDescription.RegisterStringPropertyGetterAndSetter(0, (FaultRecord.MutableKeyValuePair item) => item.Key, delegate(FaultRecord.MutableKeyValuePair item, string value)
			{
				item.Key = value;
			});
			complianceSerializationDescription.RegisterStringPropertyGetterAndSetter(1, (FaultRecord.MutableKeyValuePair item) => item.Value, delegate(FaultRecord.MutableKeyValuePair item, string value)
			{
				item.Value = value;
			});
			FaultRecord.description = new ComplianceSerializationDescription<FaultRecord>();
			FaultRecord.description.ComplianceStructureId = 11;
			FaultRecord.description.RegisterComplexCollectionAccessor<FaultRecord.MutableKeyValuePair>(0, (FaultRecord item) => item.Data.Count, (FaultRecord item, int index) => FaultRecord.MutableKeyValuePair.From(item.Data.ToList<KeyValuePair<string, string>>()[index]), delegate(FaultRecord item, FaultRecord.MutableKeyValuePair value, int index)
			{
				item.Data[value.Key] = value.Value;
			}, complianceSerializationDescription);
		}

		public static ComplianceSerializationDescription<FaultRecord> Description
		{
			get
			{
				return FaultRecord.description;
			}
		}

		public IDictionary<string, string> Data
		{
			get
			{
				return this.data;
			}
		}

		public const string CorrelationId = "CID";

		public const string MessageId = "MID";

		public const string MessageSourceId = "MSID";

		public const string TargetId = "TID";

		public const string TargetType = "TTYPE";

		public const string TargetDatabase = "TDB";

		public const string TargetMailbox = "TMBX";

		public const string SourceTargetId = "STID";

		public const string SourceTargetType = "STTYPE";

		public const string Tenant = "TENANT";

		public const string TenantGuid = "TGUID";

		public const string Exception = "EX";

		public const string ExceptionCode = "EXC";

		public const string TransientException = "TEX";

		public const string FatalException = "FEX";

		public const string HandledException = "HEX";

		public const string RetryCount = "RC";

		public const string UserMessage = "UM";

		public const string ExecutingFunction = "EFUNC";

		public const string ExecutingFile = "EFILE";

		public const string ExecutingLine = "ELINE";

		private static ComplianceSerializationDescription<FaultRecord> description;

		private ConcurrentDictionary<string, string> data = new ConcurrentDictionary<string, string>();

		private class MutableKeyValuePair
		{
			public string Key { get; set; }

			public string Value { get; set; }

			public static FaultRecord.MutableKeyValuePair From(KeyValuePair<string, string> input)
			{
				return new FaultRecord.MutableKeyValuePair
				{
					Key = input.Key,
					Value = input.Value
				};
			}
		}
	}
}
