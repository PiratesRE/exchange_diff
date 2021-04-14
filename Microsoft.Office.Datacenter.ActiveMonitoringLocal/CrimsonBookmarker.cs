using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Win32;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class CrimsonBookmarker : IEventBookmarker
	{
		public CrimsonBookmarker()
		{
		}

		public CrimsonBookmarker(string baseLocation)
		{
			this.Initialize(baseLocation);
		}

		public void Initialize(string baseLocation)
		{
			this.baseLocation = baseLocation;
		}

		public EventBookmark Read(string bookmarkName)
		{
			EventBookmark result = null;
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(this.baseLocation))
			{
				if (registryKey != null)
				{
					byte[] array = (byte[])registryKey.GetValue(bookmarkName);
					if (array != null)
					{
						using (MemoryStream memoryStream = new MemoryStream(array))
						{
							BinaryFormatter binaryFormatter = new BinaryFormatter();
							result = (EventBookmark)binaryFormatter.Deserialize(memoryStream);
						}
					}
				}
			}
			return result;
		}

		public void Write(string bookmarkName, EventBookmark bookmark)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(this.baseLocation))
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					binaryFormatter.Serialize(memoryStream, bookmark);
					registryKey.SetValue(bookmarkName, memoryStream.GetBuffer());
				}
			}
		}

		public void Delete(string bookmarkName)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(this.baseLocation))
			{
				registryKey.DeleteValue(bookmarkName, false);
			}
		}

		private string baseLocation;
	}
}
