using System;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Diagnostics;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Routing
{
	internal class OutgoingEntry : Entry
	{
		public OutgoingEntry(ComplianceMessage message)
		{
			base.MessageId = message.MessageId;
			base.CorrelationId = message.CorrelationId;
			base.Message = message;
			base.ExpiryTime = TaskDistributionSettings.OutgoingEntryExpiryTime;
		}

		public OutgoingEntry.OutgoingEntryStatus Status
		{
			get
			{
				return this.status;
			}
			set
			{
				if (this.status < value)
				{
					base.RetryCount = 0;
					this.status = value;
					this.EvaluateState(false);
				}
			}
		}

		public override string GetKey()
		{
			return string.Format("OUTGOING:{0}:{1}", base.CorrelationId, base.MessageId);
		}

		public override void EvaluateState(bool expired)
		{
			if (this.Status != OutgoingEntry.OutgoingEntryStatus.Failed && ExceptionHandler.IsFaulted(base.Message))
			{
				this.Status = OutgoingEntry.OutgoingEntryStatus.Failed;
				return;
			}
			switch (this.Status)
			{
			case OutgoingEntry.OutgoingEntryStatus.Initialized:
				if (base.RetryCount > TaskDistributionSettings.OutgoingEntryRetriesToFailure)
				{
					ExceptionHandler.FaultMessage(base.Message, FaultDefinition.FromErrorString("Delivery Failed", "EvaluateState", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Routing\\Cache\\OutgoingEntry.cs", 135), true);
					this.Status = OutgoingEntry.OutgoingEntryStatus.Failed;
				}
				else
				{
					this.SendMessage();
				}
				break;
			case OutgoingEntry.OutgoingEntryStatus.Delivered:
				if (base.RetryCount > TaskDistributionSettings.OutgoingEntryRetriesToFailure)
				{
					ExceptionHandler.FaultMessage(base.Message, FaultDefinition.FromErrorString("Delivery message was not returned", "EvaluateState", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Routing\\Cache\\OutgoingEntry.cs", 148), true);
					this.Status = OutgoingEntry.OutgoingEntryStatus.Failed;
				}
				else if (expired)
				{
					this.SendMessage();
				}
				break;
			case OutgoingEntry.OutgoingEntryStatus.Returned:
				if (base.RetryCount > TaskDistributionSettings.OutgoingEntryRetriesToFailure)
				{
					ExceptionHandler.FaultMessage(base.Message, FaultDefinition.FromErrorString("Failed to commit results", "EvaluateState", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Routing\\Cache\\OutgoingEntry.cs", 165), true);
					this.Status = OutgoingEntry.OutgoingEntryStatus.Failed;
				}
				break;
			case OutgoingEntry.OutgoingEntryStatus.Failed:
				if (!ExceptionHandler.IsFaulted(base.Message))
				{
					ExceptionHandler.FaultMessage(base.Message, FaultDefinition.FromErrorString("Unknown failure", "EvaluateState", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Routing\\Cache\\OutgoingEntry.cs", 174), true);
				}
				this.FailMessage();
				break;
			}
			if (base.RetryCount > TaskDistributionSettings.OutgoingEntryRetriesToAbandon)
			{
				base.KeepAlive = false;
				return;
			}
			base.KeepAlive = true;
		}

		protected override void UpdateExistingEntry(Entry existing)
		{
		}

		private void SendMessage()
		{
			base.Message.ProtocolContext.DispatchData = this;
			RoutingCache.Instance.QueueDispatch(base.Message);
		}

		private void FailMessage()
		{
			if (ExceptionHandler.IsFaulted(base.Message))
			{
				ComplianceMessage complianceMessage = base.Message.Clone();
				Target messageTarget = complianceMessage.MessageTarget;
				complianceMessage.MessageTarget = complianceMessage.MessageSource;
				complianceMessage.MessageSource = messageTarget;
				complianceMessage.ComplianceMessageType = ComplianceMessageType.RecordResult;
				complianceMessage.ProtocolContext.DispatchData = this;
				complianceMessage.Payload = ComplianceSerializer.Serialize<WorkPayload>(WorkPayload.Description, base.Message.ProtocolContext.FaultDefinition.ToPayload());
				RoutingCache.Instance.QueueDispatch(complianceMessage);
			}
		}

		private OutgoingEntry.OutgoingEntryStatus status;

		public enum OutgoingEntryStatus
		{
			Initialized,
			Delivered,
			Returned,
			Failed,
			Completed
		}
	}
}
