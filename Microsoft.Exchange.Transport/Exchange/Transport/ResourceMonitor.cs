using System;
using System.Xml.Linq;

namespace Microsoft.Exchange.Transport
{
	internal abstract class ResourceMonitor
	{
		protected ResourceMonitor(string displayName, ResourceManagerConfiguration.ResourceMonitorConfiguration configuration)
		{
			if (displayName == null)
			{
				throw new ArgumentNullException("displayName");
			}
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			this.displayName = displayName;
			this.Configuration = configuration;
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			protected set
			{
				this.displayName = value;
			}
		}

		public int CurrentPressure
		{
			get
			{
				return this.currentPressure;
			}
		}

		public virtual int CurrentPressureRaw
		{
			get
			{
				return this.currentPressure;
			}
		}

		public int HighPressureLimit
		{
			get
			{
				return this.highPercentageResourceUsedLimit;
			}
			protected set
			{
				this.highPercentageResourceUsedLimit = value;
			}
		}

		public int MediumPressureLimit
		{
			get
			{
				return this.mediumPercentageResourceUsedLimit;
			}
			protected set
			{
				this.mediumPercentageResourceUsedLimit = value;
			}
		}

		public int LowPressureLimit
		{
			get
			{
				return this.lowPercentageResourceUsedLimit;
			}
			protected set
			{
				this.lowPercentageResourceUsedLimit = value;
			}
		}

		public virtual ResourceUses ResourceUses
		{
			get
			{
				ResourceUses result = ResourceUses.Normal;
				if (this.currentPressure >= this.HighPressureLimit || (this.currentPressure > this.MediumPressureLimit && this.previousResourceUses == ResourceUses.High))
				{
					result = ResourceUses.High;
				}
				else if (this.currentPressure >= this.MediumPressureLimit || (this.currentPressure > this.LowPressureLimit && this.previousResourceUses > ResourceUses.Normal))
				{
					result = ResourceUses.Medium;
				}
				return result;
			}
		}

		public virtual ResourceUses CurrentResourceUsesRaw
		{
			get
			{
				return this.ResourceUses;
			}
		}

		public ResourceUses PreviousResourceUses
		{
			get
			{
				return this.previousResourceUses;
			}
		}

		public virtual void UpdateReading()
		{
			ResourceUses resourceUses = this.ResourceUses;
			int num;
			if (this.GetCurrentReading(out num))
			{
				this.previousResourceUses = resourceUses;
				this.currentPressure = num;
			}
		}

		public virtual void UpdateConfig()
		{
			this.HighPressureLimit = this.Configuration.HighThreshold;
			this.MediumPressureLimit = this.Configuration.MediumThreshold;
			this.LowPressureLimit = this.Configuration.NormalThreshold;
		}

		public virtual void DoCleanup()
		{
		}

		public virtual XElement GetDiagnosticInfo(bool verbose)
		{
			XElement xelement = new XElement("Configuration");
			this.Configuration.AddDiagnosticInfo(xelement);
			return new XElement("ResourceMonitor", new object[]
			{
				new XElement("type", base.GetType().Name),
				new XElement("displayName", this.displayName),
				new XElement("currentPressure", this.currentPressure),
				new XElement("resourceUses", this.ResourceUses),
				new XElement("lowPressureLimit", this.LowPressureLimit),
				new XElement("mediumPressureLimit", this.MediumPressureLimit),
				new XElement("highPressureLimit", this.HighPressureLimit),
				xelement
			});
		}

		public virtual string ToString(ResourceUses resourceUses, int currentPressure)
		{
			return Strings.ResourceUses(this.displayName, currentPressure, ResourceManager.MapToLocalizedString(resourceUses), this.LowPressureLimit, this.MediumPressureLimit, this.HighPressureLimit);
		}

		public override string ToString()
		{
			return this.ToString(this.ResourceUses, this.currentPressure);
		}

		protected abstract bool GetCurrentReading(out int currentReading);

		protected readonly ResourceManagerConfiguration.ResourceMonitorConfiguration Configuration;

		private int highPercentageResourceUsedLimit = 90;

		private int mediumPercentageResourceUsedLimit = 80;

		private int lowPercentageResourceUsedLimit = 70;

		private int currentPressure;

		private ResourceUses previousResourceUses;

		private string displayName;
	}
}
