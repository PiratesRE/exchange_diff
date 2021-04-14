using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.HA.FailureItem
{
	internal class Bookmarker
	{
		internal Bookmarker(Guid dbGuid)
		{
			this.m_dbGuid = dbGuid;
		}

		internal EventBookmark Read()
		{
			this.OpenKeyIfRequired();
			EventBookmark result = null;
			if (this.m_regKey != null)
			{
				byte[] array = (byte[])this.m_regKey.GetValue(Bookmarker.valueName);
				if (array != null)
				{
					MemoryStream serializationStream = new MemoryStream(array);
					BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
					result = (EventBookmark)binaryFormatter.Deserialize(serializationStream);
				}
			}
			return result;
		}

		internal void Write(EventBookmark bookmark)
		{
			this.OpenKeyIfRequired();
			MemoryStream memoryStream = new MemoryStream();
			BinaryFormatter binaryFormatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
			binaryFormatter.Serialize(memoryStream, bookmark);
			if (this.m_regKey != null)
			{
				this.m_regKey.SetValue(Bookmarker.valueName, memoryStream.GetBuffer());
			}
		}

		internal void Delete()
		{
			this.OpenKeyIfRequired();
			if (this.m_regKey != null)
			{
				this.m_regKey.DeleteValue(Bookmarker.valueName, false);
			}
		}

		internal void Close()
		{
			if (this.m_regKey != null)
			{
				this.m_regKey.Close();
				this.m_regKey = null;
			}
		}

		private void OpenKeyIfRequired()
		{
			if (this.m_regKey == null)
			{
				string subkey = string.Format(Bookmarker.keyFormatString, this.m_dbGuid);
				this.m_regKey = Registry.LocalMachine.CreateSubKey(subkey);
			}
		}

		private static string keyFormatString = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveManager\\Databases\\{0}";

		private static string valueName = "Bookmark";

		private Guid m_dbGuid;

		private RegistryKey m_regKey;
	}
}
