using System;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics.Components.WorkloadManagement;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class WorkloadPolicy
	{
		public WorkloadPolicy(WorkloadType type) : this(type, VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null))
		{
		}

		public WorkloadPolicy(WorkloadType type, VariantConfigurationSnapshot config) : this(type, config.WorkloadManagement.GetObject<IWorkloadSettings>(type, new object[0]))
		{
		}

		public WorkloadPolicy(WorkloadType type, IWorkloadSettings settings) : this(type, settings.Classification, settings.MaxConcurrency)
		{
		}

		public WorkloadPolicy(WorkloadType type, WorkloadClassification classification, int maxConcurrency)
		{
			this.Type = type;
			this.classification = classification;
			this.maxConcurrency = maxConcurrency;
		}

		public WorkloadType Type { get; private set; }

		public WorkloadClassification Classification
		{
			get
			{
				string text = null;
				ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(3368430909U, ref text);
				WorkloadType workloadType;
				WorkloadClassification result;
				int num;
				if (!string.IsNullOrEmpty(text) && this.ParseFaultInjectionState(text, out workloadType, out result, out num))
				{
					return result;
				}
				return this.classification;
			}
		}

		public int MaxConcurrency
		{
			get
			{
				string text = null;
				ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(3368430909U, ref text);
				WorkloadType workloadType;
				WorkloadClassification workloadClassification;
				int result;
				if (!string.IsNullOrEmpty(text) && this.ParseFaultInjectionState(text, out workloadType, out workloadClassification, out result))
				{
					return result;
				}
				return this.maxConcurrency;
			}
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as WorkloadPolicy);
		}

		public bool Equals(WorkloadPolicy policy)
		{
			return policy != null && this.Type == policy.Type && this.Classification == policy.Classification && this.MaxConcurrency == policy.MaxConcurrency;
		}

		public override int GetHashCode()
		{
			return (int)(this.Type ^ (WorkloadType)this.classification ^ (WorkloadType)this.maxConcurrency);
		}

		public override string ToString()
		{
			string text = this.Type + ':' + this.Classification;
			if (this.MaxConcurrency < 2147483647)
			{
				text = text + ":" + this.MaxConcurrency;
			}
			return text;
		}

		public XElement GetDiagnosticInfo()
		{
			XElement xelement = new XElement("WorkloadPolicy");
			xelement.Add(new XElement("WorkloadType", this.Type));
			xelement.Add(new XElement("Classification", this.Classification));
			if (this.MaxConcurrency < 2147483647)
			{
				xelement.Add(new XElement("MaxConcurrency", this.MaxConcurrency));
			}
			return xelement;
		}

		private bool ParseFaultInjectionState(string state, out WorkloadType parsedWorkloadType, out WorkloadClassification parsedClassification, out int parsedMaxConcurrency)
		{
			parsedWorkloadType = WorkloadType.Unknown;
			parsedClassification = WorkloadClassification.Unknown;
			parsedMaxConcurrency = -1;
			string[] array = state.Split(new char[]
			{
				'~'
			});
			if (array == null || array.Length != 3)
			{
				ExTraceGlobals.PoliciesTracer.TraceError<string>((long)this.GetHashCode(), "[WorkloadPolicy.ParseFaultInjectionState] Invalid fault injection state: '{0}'.  Using normal values instead.", state);
				return false;
			}
			WorkloadType workloadType;
			if (!Enum.TryParse<WorkloadType>(array[0], out workloadType))
			{
				ExTraceGlobals.PoliciesTracer.TraceError<string, WorkloadType>((long)this.GetHashCode(), "[WorkloadPolicy.ParseFaultInjectionState] Failed to parse WorkloadType.  Value: {0}.  Using {1} instead.", array[0], this.Type);
				return false;
			}
			if (workloadType != this.Type)
			{
				ExTraceGlobals.PoliciesTracer.TraceDebug<WorkloadType, WorkloadType>((long)this.GetHashCode(), "[WorkloadPolicy.ParseFaultInjectionState] Fault injection state refers to workload type {0} while this instance is for type {1}.  Ignoring", workloadType, this.Type);
				return false;
			}
			WorkloadClassification workloadClassification;
			if (!Enum.TryParse<WorkloadClassification>(array[1], out workloadClassification))
			{
				ExTraceGlobals.PoliciesTracer.TraceError<string, WorkloadClassification>((long)this.GetHashCode(), "[WorkloadPolicy.ParseFaultInjectionState] Failed to parse WorkloadClassification.  Value: {0}.  Using {1} instead.", array[1], this.classification);
				return false;
			}
			if (!int.TryParse(array[2], out parsedMaxConcurrency))
			{
				ExTraceGlobals.PoliciesTracer.TraceError<string, int>((long)this.GetHashCode(), "[WorkloadPolicy.ParseFaultInjectionState] Failed to parse MaxConcurrency. Value: {0}.  Using {1} instead.", array[2], this.maxConcurrency);
				return false;
			}
			parsedWorkloadType = workloadType;
			parsedClassification = workloadClassification;
			if (parsedMaxConcurrency < 0)
			{
				parsedMaxConcurrency = this.maxConcurrency;
			}
			return true;
		}

		private const string ProcessAccessManagerComponentName = "WorkloadPolicy";

		private const uint LidWorkloadClassification = 3368430909U;

		private readonly WorkloadClassification classification;

		private readonly int maxConcurrency;
	}
}
