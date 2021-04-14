using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class ResourceMetricPolicy
	{
		public ResourceMetricPolicy(ResourceMetricType type, WorkloadClassification classification) : this(type, classification, VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null))
		{
		}

		public ResourceMetricPolicy(ResourceMetricType type, WorkloadClassification classification, VariantConfigurationSnapshot config) : this(type, classification, config.WorkloadManagement.GetObject<IResourceSettings>(type, new object[0]))
		{
		}

		public ResourceMetricPolicy(ResourceMetricType metricType, WorkloadClassification classification, IResourceSettings settings)
		{
			this.MetricType = metricType;
			this.Classification = classification;
			switch (classification)
			{
			case WorkloadClassification.Discretionary:
				this.UnderloadedThreshold = settings.DiscretionaryUnderloaded;
				this.OverloadedThreshold = settings.DiscretionaryOverloaded;
				this.CriticalThreshold = settings.DiscretionaryCritical;
				break;
			case WorkloadClassification.InternalMaintenance:
				this.UnderloadedThreshold = settings.InternalMaintenanceUnderloaded;
				this.OverloadedThreshold = settings.InternalMaintenanceOverloaded;
				this.CriticalThreshold = settings.InternalMaintenanceCritical;
				break;
			case WorkloadClassification.CustomerExpectation:
				this.UnderloadedThreshold = settings.CustomerExpectationUnderloaded;
				this.OverloadedThreshold = settings.CustomerExpectationOverloaded;
				this.CriticalThreshold = settings.CustomerExpectationCritical;
				break;
			case WorkloadClassification.Urgent:
				this.UnderloadedThreshold = settings.UrgentUnderloaded;
				this.OverloadedThreshold = settings.UrgentOverloaded;
				this.CriticalThreshold = settings.UrgentCritical;
				break;
			}
			this.Validate();
		}

		public ResourceMetricType MetricType { get; private set; }

		public WorkloadClassification Classification { get; private set; }

		public int UnderloadedThreshold { get; private set; }

		public int OverloadedThreshold { get; private set; }

		public int CriticalThreshold { get; private set; }

		public ResourceLoad InterpretMetricValue(int value)
		{
			ResourceLoad unknown;
			if (value < 0)
			{
				unknown = ResourceLoad.Unknown;
			}
			else if (value > this.CriticalThreshold)
			{
				unknown = new ResourceLoad(ResourceLoad.Critical.LoadRatio, new int?(value), null);
			}
			else if (value > this.OverloadedThreshold)
			{
				unknown = new ResourceLoad((double)value / (double)this.UnderloadedThreshold, new int?(value), null);
			}
			else if (value >= this.UnderloadedThreshold)
			{
				unknown = new ResourceLoad(ResourceLoad.Full.LoadRatio, new int?(value), null);
			}
			else
			{
				unknown = new ResourceLoad((double)value / (double)this.UnderloadedThreshold, new int?(value), null);
			}
			return unknown;
		}

		public ResourceLoad MaxOverloaded
		{
			get
			{
				return new ResourceLoad(Math.Max((double)this.CriticalThreshold / (double)this.UnderloadedThreshold, 5.0), null, null);
			}
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				this.MetricType,
				this.Classification,
				this.UnderloadedThreshold,
				this.OverloadedThreshold,
				this.CriticalThreshold
			});
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ResourceMetricPolicy);
		}

		public bool Equals(ResourceMetricPolicy policy)
		{
			return policy != null && this.MetricType == policy.MetricType && this.Classification == policy.Classification && this.UnderloadedThreshold == policy.UnderloadedThreshold && this.OverloadedThreshold == policy.OverloadedThreshold && this.CriticalThreshold == policy.CriticalThreshold;
		}

		public override int GetHashCode()
		{
			return (int)(this.MetricType ^ (ResourceMetricType)this.Classification ^ (ResourceMetricType)this.UnderloadedThreshold ^ (ResourceMetricType)this.OverloadedThreshold ^ (ResourceMetricType)this.CriticalThreshold);
		}

		public XElement GetDiagnosticInfo()
		{
			XElement xelement = new XElement("ResourceMetricPolicy");
			xelement.Add(new XElement("MetricType", this.MetricType));
			xelement.Add(new XElement("Classification", this.Classification));
			xelement.Add(new XElement("UnderloadedThreshold", this.UnderloadedThreshold));
			xelement.Add(new XElement("OverloadedThreshold", this.OverloadedThreshold));
			xelement.Add(new XElement("CriticalThreshold", this.CriticalThreshold));
			return xelement;
		}

		private void Validate()
		{
			if (this.UnderloadedThreshold <= 0)
			{
				throw new InvalidResourceThresholdException(DirectoryStrings.InvalidNonPositiveResourceThreshold(this.Classification.ToString(), "Underloaded", this.UnderloadedThreshold));
			}
			if (this.OverloadedThreshold <= 0)
			{
				throw new InvalidResourceThresholdException(DirectoryStrings.InvalidNonPositiveResourceThreshold(this.Classification.ToString(), "Overloaded", this.OverloadedThreshold));
			}
			if (this.CriticalThreshold <= 0)
			{
				throw new InvalidResourceThresholdException(DirectoryStrings.InvalidNonPositiveResourceThreshold(this.Classification.ToString(), "Critical", this.CriticalThreshold));
			}
			if (this.UnderloadedThreshold > this.OverloadedThreshold)
			{
				throw new InvalidResourceThresholdException(DirectoryStrings.InvalidBiggerResourceThreshold(this.Classification.ToString(), "Underloaded", "Overloaded", this.UnderloadedThreshold, this.OverloadedThreshold));
			}
			if (this.OverloadedThreshold > this.CriticalThreshold)
			{
				throw new InvalidResourceThresholdException(DirectoryStrings.InvalidBiggerResourceThreshold(this.Classification.ToString(), "Overloaded", "Critical", this.OverloadedThreshold, this.CriticalThreshold));
			}
		}

		private const string ProcessAccessManagerComponentName = "ResourceMetricPolicy";
	}
}
