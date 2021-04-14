using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FailureInfo
	{
		public LocalizedString ErrorMessage
		{
			get
			{
				return this.m_errorMessage;
			}
		}

		public ExtendedErrorInfo ExtendedErrorInfo
		{
			get
			{
				return this.m_extendedErrorInfo;
			}
		}

		public FailureInfo()
		{
			this.m_brokenFlags = FailureInfo.FailureFlags.Unknown;
			this.m_errorEventId = null;
			this.m_extendedErrorInfo = null;
		}

		public FailureInfo(FailureInfo other)
		{
			this.m_brokenFlags = other.m_brokenFlags;
			this.m_errorEventId = other.m_errorEventId;
			if (!other.m_errorMessage.IsEmpty)
			{
				this.m_errorMessage = new LocalizedString(other.m_errorMessage);
			}
			if (other.m_extendedErrorInfo != null)
			{
				this.m_extendedErrorInfo = new ExtendedErrorInfo(other.m_extendedErrorInfo.FailureException);
			}
		}

		public bool IsFailed
		{
			get
			{
				bool result;
				lock (this)
				{
					result = (this.m_brokenFlags == FailureInfo.FailureFlags.Failed);
				}
				return result;
			}
		}

		public bool IsDisconnected
		{
			get
			{
				bool result;
				lock (this)
				{
					result = (this.m_brokenFlags == FailureInfo.FailureFlags.Disconnected);
				}
				return result;
			}
		}

		public uint ErrorEventId
		{
			get
			{
				uint? errorEventId = this.m_errorEventId;
				if (errorEventId == null)
				{
					return 0U;
				}
				return errorEventId.GetValueOrDefault();
			}
		}

		public void SetBroken(uint? eventId, LocalizedString errorMessage, ExtendedErrorInfo extendedErrorInfo)
		{
			this.SetState(FailureInfo.FailureFlags.Failed, eventId, errorMessage, extendedErrorInfo);
		}

		public void SetBroken(ExEventLog.EventTuple eventTuple, LocalizedString errorMessage, ExtendedErrorInfo extendedErrorInfo)
		{
			this.SetState(FailureInfo.FailureFlags.Failed, new uint?(DiagCore.GetEventViewerEventId(eventTuple)), errorMessage, extendedErrorInfo);
		}

		public void SetDisconnected(ExEventLog.EventTuple eventTuple, LocalizedString errorMessage)
		{
			this.SetState(FailureInfo.FailureFlags.Disconnected, new uint?(DiagCore.GetEventViewerEventId(eventTuple)), errorMessage, null);
		}

		public void Reset()
		{
			lock (this)
			{
				this.m_brokenFlags = FailureInfo.FailureFlags.NoFailure;
				this.m_errorEventId = null;
				this.m_errorMessage = LocalizedString.Empty;
				this.m_extendedErrorInfo = null;
			}
		}

		public void PersistFailure(ReplayState replayState)
		{
			if (this.IsFailed)
			{
				replayState.ConfigBroken = true;
				replayState.ConfigBrokenMessage = this.ErrorMessage;
				replayState.ConfigBrokenEventId = (long)((ulong)this.ErrorEventId);
				replayState.ConfigBrokenExtendedErrorInfo = this.ExtendedErrorInfo;
			}
		}

		private void SetState(FailureInfo.FailureFlags failureState, uint? errorEventId, LocalizedString errorMessage, ExtendedErrorInfo extendedErrorInfo)
		{
			lock (this)
			{
				this.m_brokenFlags = failureState;
				this.m_errorEventId = errorEventId;
				this.m_errorMessage = errorMessage;
				this.m_extendedErrorInfo = extendedErrorInfo;
			}
		}

		private FailureInfo.FailureFlags m_brokenFlags;

		private uint? m_errorEventId;

		private LocalizedString m_errorMessage = LocalizedString.Empty;

		private ExtendedErrorInfo m_extendedErrorInfo;

		internal enum FailureFlags
		{
			Unknown,
			Failed,
			Disconnected,
			NoFailure
		}
	}
}
