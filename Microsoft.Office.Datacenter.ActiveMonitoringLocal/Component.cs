using System;
using Microsoft.Exchange.Common;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class Component
	{
		public Component(string name, HealthGroup healthGroup, string escalationTeam = null, string service = null, ManagedAvailabilityPriority priority = ManagedAvailabilityPriority.Low) : this(name, healthGroup, escalationTeam, service, priority, ServerComponentEnum.None)
		{
		}

		public Component(string value)
		{
			if (!string.IsNullOrWhiteSpace(value))
			{
				string[] array = value.Split(new char[]
				{
					'/'
				}, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length >= 2)
				{
					ManagedAvailabilityPriority priority = ManagedAvailabilityPriority.Low;
					HealthGroup healthGroup;
					if (Enum.TryParse<HealthGroup>(array[0], true, out healthGroup))
					{
						if (array.Length == 3 && !Enum.TryParse<ManagedAvailabilityPriority>(array[2], true, out priority))
						{
							priority = ManagedAvailabilityPriority.Low;
						}
						this.Name = array[1];
						this.HealthGroup = healthGroup;
						this.Priority = priority;
						this.ServerComponent = ServerComponentEnum.None;
						Component component = Component.FindWellKnownComponent(this.Name);
						if (component != null)
						{
							this.ServerComponent = component.ServerComponent;
							this.EscalationTeam = component.EscalationTeam;
							this.Service = component.Service;
						}
						return;
					}
				}
			}
			throw new InvalidOperationException(string.Format("Cannot create a Component with value: {0}", value));
		}

		internal Component(string name, HealthGroup healthGroup, string escalationTeam, string service, ManagedAvailabilityPriority priority, ServerComponentEnum serverComponent)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new InvalidOperationException("Cannot create a Component with a null or empty name.");
			}
			this.Name = name;
			this.HealthGroup = healthGroup;
			this.EscalationTeam = escalationTeam;
			this.Service = service;
			this.Priority = priority;
			this.ServerComponent = serverComponent;
		}

		internal string Name { get; private set; }

		internal HealthGroup HealthGroup { get; private set; }

		internal string EscalationTeam { get; private set; }

		internal string Service { get; private set; }

		internal ManagedAvailabilityPriority Priority { get; private set; }

		internal ServerComponentEnum ServerComponent { get; private set; }

		public string ShortName
		{
			get
			{
				return this.Name.Split(new char[]
				{
					'.'
				})[0];
			}
		}

		public string SubsetName
		{
			get
			{
				int num = this.Name.IndexOf('.');
				if (num == -1)
				{
					return string.Empty;
				}
				return this.Name.Substring(num + 1, this.Name.Length - num - 1);
			}
		}

		public static Component FindWellKnownComponent(string componentName)
		{
			Component result = null;
			string[] array = componentName.Split(new char[]
			{
				'/'
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length > 1)
			{
				componentName = array[1];
			}
			ExchangeComponent.WellKnownComponents.TryGetValue(componentName, out result);
			return result;
		}

		public static bool operator ==(Component a, Component b)
		{
			return (a == null && b == null) || (a != null && b != null && a.ToString().Equals(b.ToString()));
		}

		public static bool operator !=(Component a, Component b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			Component component = obj as Component;
			return component != null && this == component;
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public override string ToString()
		{
			if (!string.IsNullOrWhiteSpace(this.Name))
			{
				return string.Format("{0}/{1}/{2}", this.HealthGroup.ToString(), this.Name, this.Priority);
			}
			return string.Empty;
		}

		private const char SeparatorChar = '/';
	}
}
