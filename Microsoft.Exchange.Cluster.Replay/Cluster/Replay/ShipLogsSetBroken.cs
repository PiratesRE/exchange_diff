using System;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ShipLogsSetBroken : ISetBroken, ISetDisconnected
	{
		private ISetBroken SetBrokenInterface
		{
			get
			{
				ISetBroken result;
				lock (this)
				{
					ISetBroken setBroken = this.m_setBroken;
					if (this.m_setBrokenForAcll != null)
					{
						setBroken = this.m_setBrokenForAcll;
					}
					result = setBroken;
				}
				return result;
			}
		}

		private ISetDisconnected SetDisconnectedInterface
		{
			get
			{
				ISetDisconnected result;
				lock (this)
				{
					ISetDisconnected setDisconnected = this.m_setDisconnected;
					if (this.m_setDisconnectedForAcll != null)
					{
						setDisconnected = this.m_setDisconnectedForAcll;
					}
					result = setDisconnected;
				}
				return result;
			}
		}

		public ShipLogsSetBroken(ISetBroken setBroken, ISetDisconnected setDisconnected)
		{
			this.m_setBroken = setBroken;
			this.m_setDisconnected = setDisconnected;
		}

		internal void SetReportingCallbacksForAcll(ISetBroken setBroken, ISetDisconnected setDisconnected)
		{
			lock (this)
			{
				this.m_setBrokenForAcll = setBroken;
				this.m_setDisconnectedForAcll = setDisconnected;
			}
		}

		public void SetBroken(FailureTag failureTag, ExEventLog.EventTuple setBrokenEventTuple, params string[] setBrokenArgs)
		{
			lock (this)
			{
				this.SetBrokenInterface.SetBroken(failureTag, setBrokenEventTuple, setBrokenArgs);
			}
		}

		public void SetBroken(FailureTag failureTag, ExEventLog.EventTuple setBrokenEventTuple, Exception exception, params string[] setBrokenArgs)
		{
			lock (this)
			{
				this.SetBrokenInterface.SetBroken(failureTag, setBrokenEventTuple, exception, setBrokenArgs);
			}
		}

		public void ClearBroken()
		{
			lock (this)
			{
				this.m_setBroken.ClearBroken();
				if (this.m_setBrokenForAcll != null)
				{
					this.m_setBrokenForAcll.ClearBroken();
				}
			}
		}

		public void RestartInstanceSoon(bool fPrepareToStop)
		{
			lock (this)
			{
				this.m_setBroken.RestartInstanceSoon(fPrepareToStop);
				if (this.m_setBrokenForAcll != null)
				{
					this.m_setBrokenForAcll.RestartInstanceSoon(fPrepareToStop);
				}
			}
		}

		public void RestartInstanceSoonAdminVisible()
		{
			lock (this)
			{
				this.m_setBroken.RestartInstanceSoonAdminVisible();
				if (this.m_setBrokenForAcll != null)
				{
					this.m_setBrokenForAcll.RestartInstanceSoonAdminVisible();
				}
			}
		}

		public void RestartInstanceNow(ReplayConfigChangeHints restartReason)
		{
			lock (this)
			{
				this.m_setBroken.RestartInstanceNow(restartReason);
				if (this.m_setBrokenForAcll != null)
				{
					this.m_setBrokenForAcll.RestartInstanceNow(restartReason);
				}
			}
		}

		public bool IsBroken
		{
			get
			{
				bool result;
				lock (this)
				{
					bool flag2 = this.m_setBroken.IsBroken;
					if (this.m_setBrokenForAcll != null)
					{
						flag2 = (flag2 || this.m_setBrokenForAcll.IsBroken);
					}
					result = flag2;
				}
				return result;
			}
		}

		public LocalizedString ErrorMessage
		{
			get
			{
				LocalizedString result;
				lock (this)
				{
					LocalizedString errorMessage = this.m_setBroken.ErrorMessage;
					if (this.m_setBrokenForAcll != null && !this.m_setBrokenForAcll.ErrorMessage.IsEmpty)
					{
						errorMessage = this.m_setBrokenForAcll.ErrorMessage;
					}
					result = errorMessage;
				}
				return result;
			}
		}

		public bool IsDisconnected
		{
			get
			{
				bool flag = this.m_setDisconnected.IsDisconnected;
				if (this.m_setDisconnectedForAcll != null)
				{
					flag = (flag || this.m_setDisconnectedForAcll.IsDisconnected);
				}
				return flag;
			}
		}

		public void SetDisconnected(FailureTag failureTag, ExEventLog.EventTuple setDisconnectedEventTuple, params string[] setDisconnectedArgs)
		{
			this.SetDisconnectedInterface.SetDisconnected(failureTag, setDisconnectedEventTuple, setDisconnectedArgs);
		}

		public void ClearDisconnected()
		{
			lock (this)
			{
				this.m_setDisconnected.ClearDisconnected();
				if (this.m_setDisconnectedForAcll != null)
				{
					this.m_setDisconnectedForAcll.ClearDisconnected();
				}
			}
		}

		private ISetBroken m_setBroken;

		private ISetDisconnected m_setDisconnected;

		private ISetBroken m_setBrokenForAcll;

		private ISetDisconnected m_setDisconnectedForAcll;
	}
}
