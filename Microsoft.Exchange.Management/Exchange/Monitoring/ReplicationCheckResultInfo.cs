using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	internal class ReplicationCheckResultInfo
	{
		public ReplicationCheckResultInfo(ReplicationCheck check)
		{
			this.m_check = check;
			this.m_WarningMessages = new List<MessageInfo>();
			this.m_FailedMessages = new List<MessageInfo>();
			this.m_PassedMessages = new List<MessageInfo>();
		}

		public bool HasTransientError
		{
			get
			{
				return this.m_hasTransientError;
			}
			set
			{
				this.m_hasTransientError = value;
			}
		}

		public bool HasRun
		{
			get
			{
				return this.m_hasRun;
			}
			set
			{
				this.m_hasRun = value;
			}
		}

		public bool HasPassed
		{
			get
			{
				return this.m_hasPassed;
			}
		}

		public bool HasFailures
		{
			get
			{
				return this.NumberOfFailures > 0;
			}
		}

		public bool HasWarnings
		{
			get
			{
				return this.NumberOfWarnings > 0;
			}
		}

		public bool HasPasses
		{
			get
			{
				return this.NumberOfPasses > 0;
			}
		}

		public int NumberOfFailures
		{
			get
			{
				return this.m_FailedMessages.Count;
			}
		}

		public int NumberOfWarnings
		{
			get
			{
				return this.m_WarningMessages.Count;
			}
		}

		public int NumberOfPasses
		{
			get
			{
				return this.m_PassedMessages.Count;
			}
		}

		public int TotalNumberOfRecords
		{
			get
			{
				return this.NumberOfFailures + this.NumberOfWarnings + this.NumberOfPasses;
			}
		}

		public void AddFailure(string identity, string message, bool isTransitioningState)
		{
			this.AddFailure(identity, message, false, null, isTransitioningState);
		}

		public void AddFailure(string identity, string message, bool isException, uint? dbFailureEventId, bool isTransitioningState)
		{
			this.m_FailedMessages.Add(new MessageInfo(this.m_check.Title, identity, message, isException, dbFailureEventId, isTransitioningState));
		}

		public void AddWarning(string identity, string message, bool isTransitioningState)
		{
			this.m_WarningMessages.Add(new MessageInfo(this.m_check.Title, identity, message, false, null, isTransitioningState));
		}

		public void AddSuccess(string identity, bool isTransitioningState)
		{
			this.m_PassedMessages.Add(new MessageInfo(this.m_check.Title, identity, string.Empty, false, null, isTransitioningState));
		}

		public void SetPassed()
		{
			this.SetPassed(null, false);
		}

		public void SetPassed(string identity, bool isTransitioningState)
		{
			if (!this.m_hasPassed)
			{
				if (!string.IsNullOrEmpty(identity) && this.NumberOfPasses == 0)
				{
					this.AddSuccess(identity, isTransitioningState);
					ExTraceGlobals.HealthChecksTracer.TraceDebug<string>((long)this.GetHashCode(), "Check {0} is recording a pass entry for overall check.", this.m_check.Title);
				}
				this.m_hasPassed = true;
			}
		}

		private ReplicationCheckResultEnum GetCheckResultEnum()
		{
			if (!this.m_check.HasRun)
			{
				return ReplicationCheckResultEnum.Undefined;
			}
			if (this.m_check.HasPassed)
			{
				return ReplicationCheckResultEnum.Passed;
			}
			if (this.HasFailures)
			{
				return ReplicationCheckResultEnum.Failed;
			}
			if (this.HasWarnings)
			{
				return ReplicationCheckResultEnum.Warning;
			}
			return ReplicationCheckResultEnum.Undefined;
		}

		public string BuildErrorMessageForOutcome()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.HasFailures)
			{
				if (this.HasWarnings || this.NumberOfFailures > 1)
				{
					stringBuilder.AppendLine(Strings.ReplicationCheckFailuresLabel);
				}
				foreach (MessageInfo messageInfo in this.m_FailedMessages)
				{
					string text = messageInfo.IsException ? Strings.ReplicationCheckGeneralFail(this.m_check.Title, messageInfo.Message) : messageInfo.Message;
					if (this.HasWarnings || this.NumberOfFailures > 1)
					{
						stringBuilder.AppendFormat("\t{0}{1}", text, Environment.NewLine);
					}
					else
					{
						stringBuilder.AppendLine(text);
					}
				}
			}
			if (this.HasWarnings)
			{
				if (this.HasFailures || this.NumberOfWarnings > 1)
				{
					stringBuilder.AppendLine(Strings.ReplicationCheckWarningsLabel);
				}
				foreach (MessageInfo messageInfo2 in this.m_WarningMessages)
				{
					if (this.HasFailures || this.NumberOfWarnings > 1)
					{
						stringBuilder.AppendFormat("\t{0}{1}", messageInfo2.Message, Environment.NewLine);
					}
					else
					{
						stringBuilder.AppendLine(messageInfo2.Message);
					}
				}
			}
			string text2 = stringBuilder.ToString();
			ExTraceGlobals.HealthChecksTracer.TraceDebug<string, string>((long)this.GetHashCode(), "BuildErrorMessageForOutcome(): Message after running check '{0}' is:{1}", this.m_check.Title, (!string.IsNullOrEmpty(text2)) ? text2.Replace(Environment.NewLine, "; ") : "<blank>");
			return text2;
		}

		public ReplicationCheckOutcome GetCheckOutcome()
		{
			ReplicationCheckResultEnum checkResultEnum = this.GetCheckResultEnum();
			ReplicationCheckOutcome replicationCheckOutcome = new ReplicationCheckOutcome(this.m_check);
			if (checkResultEnum == ReplicationCheckResultEnum.Passed)
			{
				replicationCheckOutcome.Update(new ReplicationCheckResult(checkResultEnum), null);
				return replicationCheckOutcome;
			}
			string newErrorMessage = this.BuildErrorMessageForOutcome();
			replicationCheckOutcome.Update(new ReplicationCheckResult(checkResultEnum), newErrorMessage);
			return replicationCheckOutcome;
		}

		public List<ReplicationCheckOutputObject> GetCheckOutputObjects()
		{
			int totalNumberOfRecords = this.TotalNumberOfRecords;
			List<ReplicationCheckOutputObject> list = new List<ReplicationCheckOutputObject>(totalNumberOfRecords);
			foreach (MessageInfo messageInfo in this.m_FailedMessages)
			{
				ReplicationCheckOutputObject replicationCheckOutputObject = new ReplicationCheckOutputObject(this.m_check);
				replicationCheckOutputObject.Update(messageInfo.InstanceIdentity, new ReplicationCheckResult(ReplicationCheckResultEnum.Failed), messageInfo.Message, messageInfo.DbFailureEventId);
				list.Add(replicationCheckOutputObject);
			}
			foreach (MessageInfo messageInfo2 in this.m_WarningMessages)
			{
				ReplicationCheckOutputObject replicationCheckOutputObject = new ReplicationCheckOutputObject(this.m_check);
				replicationCheckOutputObject.Update(messageInfo2.InstanceIdentity, new ReplicationCheckResult(ReplicationCheckResultEnum.Warning), messageInfo2.Message, messageInfo2.DbFailureEventId);
				list.Add(replicationCheckOutputObject);
			}
			foreach (MessageInfo messageInfo3 in this.m_PassedMessages)
			{
				ReplicationCheckOutputObject replicationCheckOutputObject = new ReplicationCheckOutputObject(this.m_check);
				replicationCheckOutputObject.Update(messageInfo3.InstanceIdentity, new ReplicationCheckResult(ReplicationCheckResultEnum.Passed), string.Empty, null);
				list.Add(replicationCheckOutputObject);
			}
			return list;
		}

		public void LogEvents(IEventManager eventManager)
		{
			if (this.HasFailures)
			{
				eventManager.LogEvents(this.m_check.CheckId, ReplicationCheckResultEnum.Failed, this.m_FailedMessages);
			}
			if (this.HasWarnings)
			{
				eventManager.LogEvents(this.m_check.CheckId, ReplicationCheckResultEnum.Warning, this.m_WarningMessages);
			}
			if (this.HasPasses)
			{
				eventManager.LogEvents(this.m_check.CheckId, ReplicationCheckResultEnum.Passed, this.m_PassedMessages);
			}
		}

		private readonly ReplicationCheck m_check;

		private List<MessageInfo> m_WarningMessages;

		private List<MessageInfo> m_FailedMessages;

		private List<MessageInfo> m_PassedMessages;

		private bool m_hasTransientError;

		private bool m_hasRun;

		private bool m_hasPassed;
	}
}
