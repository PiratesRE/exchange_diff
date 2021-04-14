using System;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Replay.Monitoring;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public sealed class TransitionInfo
	{
		private TransitionInfo()
		{
		}

		public TransitionState CurrentState { get; private set; }

		public DateTime? LastActiveTransition { get; private set; }

		public DateTime? LastInactiveTransition { get; private set; }

		public DateTime? LastTransitionTime
		{
			get
			{
				if (this.CurrentState == TransitionState.Active)
				{
					return this.LastActiveTransition;
				}
				if (this.CurrentState == TransitionState.Inactive)
				{
					return this.LastInactiveTransition;
				}
				return null;
			}
		}

		public override string ToString()
		{
			return this.m_toString;
		}

		internal bool IsSuccess
		{
			get
			{
				return this.CurrentState == TransitionState.Active;
			}
		}

		internal static TransitionInfo ConstructFromRemoteSerializable(TransientErrorInfoPersisted errorInfo)
		{
			TransitionInfo transitionInfo = new TransitionInfo();
			if (errorInfo != null)
			{
				transitionInfo.CurrentState = TransitionInfo.ConvertTransitionStateFromSerializable(errorInfo.CurrentErrorState);
				transitionInfo.LastActiveTransition = DateTimeHelper.ParseIntoNullableLocalDateTimeIfPossible(errorInfo.LastSuccessTransitionUtc);
				transitionInfo.LastInactiveTransition = DateTimeHelper.ParseIntoNullableLocalDateTimeIfPossible(errorInfo.LastFailureTransitionUtc);
			}
			transitionInfo.m_toString = transitionInfo.GetToString();
			return transitionInfo;
		}

		private static TransitionState ConvertTransitionStateFromSerializable(ErrorTypePersisted errorType)
		{
			switch (errorType)
			{
			case ErrorTypePersisted.Unknown:
				return TransitionState.Unknown;
			case ErrorTypePersisted.Success:
				return TransitionState.Active;
			case ErrorTypePersisted.Failure:
				return TransitionState.Inactive;
			default:
				DiagCore.RetailAssert(false, "Missing case for enumeration: {0}", new object[]
				{
					errorType
				});
				return TransitionState.Unknown;
			}
		}

		private static string GetTransitionStateString(TransitionState state)
		{
			string text = LocalizedDescriptionAttribute.FromEnum(typeof(TransitionState), state);
			if (!string.IsNullOrWhiteSpace(text))
			{
				return text;
			}
			return state.ToString();
		}

		private string GetToString()
		{
			return string.Format("{3}: {0}; {4}: {1}; {5}: {2}", new object[]
			{
				this.CurrentState,
				this.LastActiveTransition,
				this.LastInactiveTransition,
				Strings.TransitionInfoLabelCurrentState,
				Strings.TransitionInfoLabelLastSuccessTransition,
				Strings.TransitionInfoLabelLastFailureTransition
			});
		}

		private string m_toString;
	}
}
