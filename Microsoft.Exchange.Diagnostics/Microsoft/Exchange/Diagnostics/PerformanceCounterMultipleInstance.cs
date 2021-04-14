using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;

namespace Microsoft.Exchange.Diagnostics
{
	public class PerformanceCounterMultipleInstance
	{
		public PerformanceCounterMultipleInstance(string categoryName, CreateInstanceDelegate instanceCreator)
		{
			this.instanceCreator = instanceCreator;
			this.category = PerformanceCounterFactory.CreatePerformanceCounterCategory(categoryName);
		}

		public virtual PerformanceCounterInstance GetInstance(string instanceName)
		{
			return this.GetInstance(instanceName, null);
		}

		public virtual void ResetInstance(string instanceName)
		{
			lock (this)
			{
				PerformanceCounterInstance instance;
				if (!this.cachedInstances.TryGetValue(instanceName, out instance))
				{
					if (!this.InstanceExists(instanceName))
					{
						return;
					}
					instance = this.GetInstance(instanceName);
				}
				instance.Reset();
			}
		}

		public virtual void CloseInstance(string instanceName)
		{
			lock (this)
			{
				PerformanceCounterInstance performanceCounterInstance;
				if (this.cachedInstances.TryGetValue(instanceName, out performanceCounterInstance))
				{
					performanceCounterInstance.Close();
					this.cachedInstances.Remove(instanceName);
				}
			}
		}

		public bool InstanceExists(string instanceName)
		{
			bool result;
			try
			{
				lock (this)
				{
					result = this.category.InstanceExists(instanceName);
				}
			}
			catch (InvalidOperationException)
			{
				result = false;
			}
			catch (Win32Exception)
			{
				result = false;
			}
			catch (FormatException)
			{
				result = false;
			}
			return result;
		}

		public virtual string[] GetInstanceNames()
		{
			string[] instanceNames;
			try
			{
				lock (this)
				{
					instanceNames = this.category.GetInstanceNames();
				}
			}
			catch (InvalidOperationException)
			{
				instanceNames = PerformanceCounterMultipleInstance.zeroInstances;
			}
			catch (Win32Exception)
			{
				instanceNames = PerformanceCounterMultipleInstance.zeroInstances;
			}
			catch (FormatException)
			{
				instanceNames = PerformanceCounterMultipleInstance.zeroInstances;
			}
			return instanceNames;
		}

		public virtual void RemoveInstance(string instanceName)
		{
			lock (this)
			{
				this.RemoveInstanceInternal(instanceName);
			}
		}

		public virtual void RemoveAllInstances()
		{
			lock (this)
			{
				this.RemoveAllInstancesInternal();
			}
		}

		public void GetPerfCounterDiagnosticsInfo(XElement element)
		{
			foreach (string instanceName in this.GetInstanceNames())
			{
				PerformanceCounterInstance instance = this.GetInstance(instanceName);
				if (instance != null)
				{
					instance.GetPerfCounterDiagnosticsInfo(element);
				}
			}
		}

		protected PerformanceCounterInstance GetInstance(string instanceName, PerformanceCounterInstance totalInstance)
		{
			PerformanceCounterInstance result;
			lock (this)
			{
				PerformanceCounterInstance performanceCounterInstance;
				if (!this.cachedInstances.TryGetValue(instanceName, out performanceCounterInstance))
				{
					performanceCounterInstance = this.InstanceCreator(instanceName, totalInstance);
					this.cachedInstances.Add(instanceName, performanceCounterInstance);
				}
				result = performanceCounterInstance;
			}
			return result;
		}

		protected virtual void RemoveInstanceInternal(string instanceName)
		{
			PerformanceCounterInstance instance;
			if (!this.cachedInstances.TryGetValue(instanceName, out instance))
			{
				if (!this.InstanceExists(instanceName))
				{
					return;
				}
				instance = this.GetInstance(instanceName);
			}
			this.cachedInstances.Remove(instanceName);
			instance.Reset();
			instance.Remove();
			instance.Close();
		}

		protected void RemoveAllInstancesInternal()
		{
			string[] instanceNames = this.GetInstanceNames();
			foreach (string instanceName in instanceNames)
			{
				this.RemoveInstanceInternal(instanceName);
			}
		}

		protected CreateInstanceDelegate InstanceCreator
		{
			get
			{
				return this.instanceCreator;
			}
		}

		private CreateInstanceDelegate instanceCreator;

		private IPerformanceCounterCategory category;

		private static readonly string[] zeroInstances = new string[0];

		private Dictionary<string, PerformanceCounterInstance> cachedInstances = new Dictionary<string, PerformanceCounterInstance>(StringComparer.OrdinalIgnoreCase);
	}
}
