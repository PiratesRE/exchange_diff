using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol
{
	public class FaultDefinition : ResultBase
	{
		static FaultDefinition()
		{
			FaultDefinition.description.ComplianceStructureId = 12;
			FaultDefinition.description.RegisterComplexCollectionAccessor<FaultRecord>(0, (FaultDefinition item) => item.Faults.Count, (FaultDefinition item, int index) => item.Faults.ToList<FaultRecord>()[index], delegate(FaultDefinition item, FaultRecord value, int index)
			{
				item.Faults.TryAdd(value);
			}, FaultRecord.Description);
		}

		public static ComplianceSerializationDescription<FaultDefinition> Description
		{
			get
			{
				return FaultDefinition.description;
			}
		}

		public bool IsFatalFailure { get; set; }

		public override int SerializationVersion
		{
			get
			{
				return 1;
			}
		}

		public static FaultDefinition FromErrorString(string error, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0)
		{
			FaultDefinition faultDefinition = new FaultDefinition();
			FaultRecord faultRecord = new FaultRecord();
			faultDefinition.Faults.TryAdd(faultRecord);
			faultRecord.Data["RC"] = "0";
			faultRecord.Data["EFILE"] = callerFilePath;
			faultRecord.Data["EFUNC"] = callerMember;
			faultRecord.Data["ELINE"] = callerLineNumber.ToString();
			faultRecord.Data["TEX"] = "false";
			faultRecord.Data["HEX"] = "true";
			faultRecord.Data["EX"] = error;
			faultRecord.Data["EXC"] = string.Format("{0}:{1}:{2}", callerMember, callerLineNumber, error.GetHashCode());
			return faultDefinition;
		}

		public static FaultDefinition FromException(Exception error, bool handled = true, bool transient = false, [CallerMemberName] string callerMember = null, [CallerFilePath] string callerFilePath = null, [CallerLineNumber] int callerLineNumber = 0)
		{
			FaultDefinition faultDefinition = new FaultDefinition();
			FaultRecord faultRecord = new FaultRecord();
			faultDefinition.Faults.TryAdd(faultRecord);
			faultRecord.Data["RC"] = "0";
			faultRecord.Data["EFILE"] = callerFilePath;
			faultRecord.Data["EFUNC"] = callerMember;
			faultRecord.Data["ELINE"] = callerLineNumber.ToString();
			faultRecord.Data["TEX"] = transient.ToString();
			faultRecord.Data["HEX"] = handled.ToString();
			faultRecord.Data["EX"] = string.Format("{0}", error);
			if (error != null)
			{
				faultRecord.Data["EXC"] = string.Format("{0}", error.GetType().Name);
			}
			return faultDefinition;
		}

		public void Merge(FaultDefinition source)
		{
			if (source != null)
			{
				this.MergeFaults(source);
				if (!this.IsFatalFailure)
				{
					this.IsFatalFailure = source.IsFatalFailure;
				}
			}
		}

		public WorkPayload ToPayload()
		{
			return new WorkPayload
			{
				WorkDefinitionType = WorkDefinitionType.Fault,
				WorkDefinition = this.GetSerializedResult()
			};
		}

		public override byte[] GetSerializedResult()
		{
			return ComplianceSerializer.Serialize<FaultDefinition>(FaultDefinition.Description, this);
		}

		private static ComplianceSerializationDescription<FaultDefinition> description = new ComplianceSerializationDescription<FaultDefinition>();
	}
}
