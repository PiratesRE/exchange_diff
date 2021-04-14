using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public abstract class WorkItemIdentity
	{
		protected WorkItemIdentity(Component component, string localName, string targetResource)
		{
			ArgumentValidator.ThrowIfNull("component", component);
			ArgumentValidator.ThrowIfNullOrEmpty("localName", localName);
			this.Component = component;
			this.Name = string.Format("{0}{1}", component.ShortName, localName);
			this.TargetResource = targetResource;
		}

		public Component Component { get; private set; }

		public string ServiceName
		{
			get
			{
				return this.Component.Name;
			}
		}

		public string Name { get; private set; }

		public string TargetResource { get; private set; }

		public string GetAlertMask()
		{
			if (!string.IsNullOrWhiteSpace(this.TargetResource))
			{
				return string.Format("{0}/{1}", this.Name, this.TargetResource);
			}
			return this.Name;
		}

		public string GetIdentity(bool useTargetResource = true)
		{
			return string.Format((string.IsNullOrWhiteSpace(this.TargetResource) || !useTargetResource) ? "{0}\\{1}" : "{0}\\{1}\\{2}", this.Component.Name, this.Name, this.TargetResource);
		}

		public override bool Equals(object obj)
		{
			WorkItemIdentity workItemIdentity = obj as WorkItemIdentity;
			return workItemIdentity != null && workItemIdentity.GetIdentity(true) == this.GetIdentity(true);
		}

		public override int GetHashCode()
		{
			return this.GetIdentity(true).GetHashCode();
		}

		public override string ToString()
		{
			return this.GetIdentity(true);
		}

		protected static string ToLocalName(string baseName, string standardSuffix)
		{
			return baseName + standardSuffix;
		}

		protected static string GetLocalName(Component component, string name, string standardSuffix)
		{
			if (!name.EndsWith(standardSuffix))
			{
				throw new ArgumentException(string.Format("WorkItem name {0} doesn't end with the expected suffix {1}", name, standardSuffix), "baseName");
			}
			string shortName = component.ShortName;
			if (!name.StartsWith(shortName))
			{
				throw new ArgumentException(string.Format("WorkItem name {0} doesn't start with the expected prefix {1}", name, shortName), "baseName");
			}
			return name.Substring(shortName.Length, name.Length - shortName.Length - standardSuffix.Length);
		}

		public class Typed<TDefinition> : WorkItemIdentity where TDefinition : WorkDefinition
		{
			protected Typed(Component component, string localName, string targetResource) : base(component, localName, targetResource)
			{
			}

			public virtual void ApplyTo(TDefinition definition)
			{
				definition.ServiceName = base.ServiceName;
				definition.Name = base.Name;
				definition.TargetResource = base.TargetResource;
			}
		}
	}
}
