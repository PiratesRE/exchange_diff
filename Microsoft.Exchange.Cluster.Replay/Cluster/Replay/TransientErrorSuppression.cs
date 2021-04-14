using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal abstract class TransientErrorSuppression<TKey>
	{
		protected TransientErrorSuppression()
		{
			this.InitializeTable();
		}

		public void ReportSuccess(TKey key)
		{
			TransientErrorInfo existingOrNewErrorInfo = this.GetExistingOrNewErrorInfo(key);
			existingOrNewErrorInfo.ReportSuccess();
		}

		public bool ReportSuccess(TKey key, TimeSpan suppressDuration)
		{
			TransientErrorInfo existingOrNewErrorInfo = this.GetExistingOrNewErrorInfo(key);
			return existingOrNewErrorInfo.ReportSuccess(suppressDuration);
		}

		public bool ReportFailure(TKey key, TimeSpan suppressDuration)
		{
			TransientErrorInfo existingOrNewErrorInfo = this.GetExistingOrNewErrorInfo(key);
			return existingOrNewErrorInfo.ReportFailure(suppressDuration);
		}

		protected abstract void InitializeTable();

		private TransientErrorInfo GetExistingOrNewErrorInfo(TKey key)
		{
			TransientErrorInfo transientErrorInfo = null;
			if (!this.m_errorTable.TryGetValue(key, out transientErrorInfo))
			{
				transientErrorInfo = new TransientErrorInfo();
				this.m_errorTable[key] = transientErrorInfo;
			}
			return transientErrorInfo;
		}

		protected Dictionary<TKey, TransientErrorInfo> m_errorTable;
	}
}
