using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Cluster.Replay.Dumpster
{
	internal class SafetyNetRegKey
	{
		public SafetyNetRegKey(string dbGuidStr, string dbName)
		{
			this.m_dbGuidStr = dbGuidStr;
			this.m_dbName = dbName;
		}

		public IEnumerable<SafetyNetRequestKey> ReadRequestKeys()
		{
			string[] valueNames = null;
			using (SafetyNetRegKeyStore safetyNetRegKeyStore = new SafetyNetRegKeyStore(this.m_dbGuidStr, this.m_dbName))
			{
				valueNames = safetyNetRegKeyStore.ReadRequestKeyNames();
			}
			foreach (string valueName in valueNames)
			{
				yield return SafetyNetRequestKey.Parse(valueName);
			}
			yield break;
		}

		public SafetyNetInfo ReadRequestInfo(SafetyNetRequestKey requestKey, SafetyNetInfo prevInfo)
		{
			SafetyNetInfo result;
			using (SafetyNetRegKeyStore safetyNetRegKeyStore = new SafetyNetRegKeyStore(this.m_dbGuidStr, this.m_dbName))
			{
				result = safetyNetRegKeyStore.ReadRequestInfo(requestKey, prevInfo);
			}
			return result;
		}

		public void WriteRequestInfo(SafetyNetInfo info)
		{
			using (SafetyNetRegKeyStore safetyNetRegKeyStore = new SafetyNetRegKeyStore(this.m_dbGuidStr, this.m_dbName))
			{
				safetyNetRegKeyStore.WriteRequestInfo(info);
			}
		}

		public void DeleteRequest(SafetyNetInfo info)
		{
			using (SafetyNetRegKeyStore safetyNetRegKeyStore = new SafetyNetRegKeyStore(this.m_dbGuidStr, this.m_dbName))
			{
				safetyNetRegKeyStore.DeleteRequest(info);
			}
		}

		private readonly string m_dbGuidStr;

		private readonly string m_dbName;
	}
}
