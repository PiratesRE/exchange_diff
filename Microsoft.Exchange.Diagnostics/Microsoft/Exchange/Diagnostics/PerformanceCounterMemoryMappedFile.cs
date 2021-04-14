using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Diagnostics
{
	public class PerformanceCounterMemoryMappedFile : IDisposable
	{
		public PerformanceCounterMemoryMappedFile() : this(false)
		{
		}

		public PerformanceCounterMemoryMappedFile(bool writable)
		{
			this.firstCategoryOffset = 4;
			this.Initialize("netfxcustomperfcounters.1.0", writable);
		}

		public PerformanceCounterMemoryMappedFile(int size, bool writable)
		{
			this.firstCategoryOffset = 4;
			this.Initialize("netfxcustomperfcounters.1.0", size, writable);
		}

		public PerformanceCounterMemoryMappedFile(string categoryName) : this(categoryName, false)
		{
		}

		public PerformanceCounterMemoryMappedFile(string categoryName, bool writable)
		{
			this.firstCategoryOffset = 8;
			this.Initialize("netfxcustomperfcounters.1.0" + categoryName, writable);
		}

		public PerformanceCounterMemoryMappedFile(string categoryName, int size, bool writable)
		{
			this.firstCategoryOffset = 8;
			this.Initialize("netfxcustomperfcounters.1.0" + categoryName, size, writable);
		}

		public CategoryEntry FirstCategory
		{
			get
			{
				return this.firstCategory;
			}
		}

		public IntPtr Pointer
		{
			get
			{
				return this.perfCounterFileMapping.IntPtr;
			}
		}

		public static PerformanceCounterMemoryMappedFile GetDefaultPerformanceCounterSharedMemory()
		{
			PerformanceCounterMemoryMappedFile performanceCounterMemoryMappedFile = null;
			if (!PerformanceCounterMemoryMappedFile.perfCounterSharedMemories.TryGetValue("netfxcustomperfcounters.1.0", out performanceCounterMemoryMappedFile))
			{
				performanceCounterMemoryMappedFile = new PerformanceCounterMemoryMappedFile();
				PerformanceCounterMemoryMappedFile.perfCounterSharedMemories.Add("netfxcustomperfcounters.1.0", performanceCounterMemoryMappedFile);
			}
			return performanceCounterMemoryMappedFile;
		}

		public static PerformanceCounterMemoryMappedFile GetPerformanceCounterSharedMemory(string categoryName)
		{
			PerformanceCounterMemoryMappedFile performanceCounterMemoryMappedFile = null;
			if (!PerformanceCounterMemoryMappedFile.perfCounterSharedMemories.TryGetValue(categoryName, out performanceCounterMemoryMappedFile))
			{
				performanceCounterMemoryMappedFile = new PerformanceCounterMemoryMappedFile(categoryName);
				PerformanceCounterMemoryMappedFile.perfCounterSharedMemories.Add(categoryName, performanceCounterMemoryMappedFile);
			}
			return performanceCounterMemoryMappedFile;
		}

		public static void RefreshAll()
		{
			foreach (PerformanceCounterMemoryMappedFile performanceCounterMemoryMappedFile in PerformanceCounterMemoryMappedFile.perfCounterSharedMemories.Values)
			{
				performanceCounterMemoryMappedFile.Refresh();
			}
		}

		public static void CloseAll()
		{
			foreach (PerformanceCounterMemoryMappedFile performanceCounterMemoryMappedFile in PerformanceCounterMemoryMappedFile.perfCounterSharedMemories.Values)
			{
				performanceCounterMemoryMappedFile.Close();
			}
		}

		public void Initialize(string fileMappingName, bool writable)
		{
			fileMappingName = "Global\\" + fileMappingName.ToLowerInvariant();
			this.perfCounterFileMapping = new FileMapping(fileMappingName, writable);
			this.firstCategory = new CategoryEntry(this.perfCounterFileMapping.IntPtr, this.firstCategoryOffset);
		}

		public void Initialize(string fileMappingName, int size, bool writable)
		{
			fileMappingName = "Global\\" + fileMappingName.ToLowerInvariant();
			this.perfCounterFileMapping = new FileMapping(fileMappingName, size, writable);
			this.firstCategory = new CategoryEntry(this.perfCounterFileMapping.IntPtr, this.firstCategoryOffset);
			this.firstCategory = new CategoryEntry(this.perfCounterFileMapping.IntPtr, this.firstCategoryOffset);
		}

		public void Refresh()
		{
			this.firstCategory = new CategoryEntry(this.perfCounterFileMapping.IntPtr, this.firstCategoryOffset);
		}

		public void Dispose()
		{
			this.perfCounterFileMapping.Dispose();
		}

		public void Close()
		{
			this.Dispose();
		}

		public CategoryEntry FindCategory(string categoryName)
		{
			CategoryEntry next = this.FirstCategory;
			while (next != null && string.Compare(next.Name, categoryName, StringComparison.OrdinalIgnoreCase) != 0)
			{
				next = next.Next;
			}
			return next;
		}

		public void RemoveCategory(string categoryName, Action<CounterEntry, LifetimeEntry, InstanceEntry> logRemoveInstanceEvent)
		{
			CategoryEntry categoryEntry = this.FindCategory(categoryName);
			if (categoryEntry != null)
			{
				for (InstanceEntry instanceEntry = categoryEntry.FirstInstance; instanceEntry != null; instanceEntry = instanceEntry.Next)
				{
					CounterEntry firstCounter = instanceEntry.FirstCounter;
					if (firstCounter != null)
					{
						LifetimeEntry lifetime = firstCounter.Lifetime;
						if (lifetime != null && lifetime.Type == 1)
						{
							instanceEntry.RefCount = 0;
							logRemoveInstanceEvent(firstCounter, lifetime, instanceEntry);
						}
					}
				}
			}
		}

		public const int DefaultSharedMemorySize = 524288;

		public const int SeparateSharedMemorySize = 131072;

		public const int MinSharedMemorySize = 32768;

		public const int MaxSharedMemorySize = 33554432;

		private const string DefaultFileMappingName = "netfxcustomperfcounters.1.0";

		private const string FileMappingPrefix = "netfxcustomperfcounters.1.0";

		private static Dictionary<string, PerformanceCounterMemoryMappedFile> perfCounterSharedMemories = new Dictionary<string, PerformanceCounterMemoryMappedFile>(StringComparer.OrdinalIgnoreCase);

		private FileMapping perfCounterFileMapping;

		private CategoryEntry firstCategory;

		private int firstCategoryOffset;
	}
}
