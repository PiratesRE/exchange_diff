using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ConversionLimitsTracker
	{
		internal ConversionLimitsTracker(ConversionLimits limits)
		{
			this.limits = limits;
			this.depth = 0;
			this.bodyCounted = false;
			this.partCount = 0;
			this.recipientCount = 0;
		}

		internal ConversionLimitsTracker.State SaveState()
		{
			ConversionLimitsTracker.State result;
			result.Depth = this.depth;
			result.RecipientCount = this.recipientCount;
			result.PartCount = this.partCount;
			result.BodyCounted = this.bodyCounted;
			return result;
		}

		internal void RestoreState(ConversionLimitsTracker.State state)
		{
			while (this.depth > state.Depth)
			{
				this.EndEmbeddedMessage();
			}
			this.partCount = state.PartCount;
			this.recipientCount = state.RecipientCount;
			this.bodyCounted = state.BodyCounted;
		}

		internal void StartEmbeddedMessage()
		{
			if (this.enforceLimits && this.depth >= this.limits.MaxEmbeddedMessageDepth)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcGenericTracer, "ConversionLimitsTracker::StartEmbeddedMessage: maximum depth exceeded.");
				throw new ConversionFailedException(ConversionFailureReason.ExceedsLimit, ServerStrings.ConversionMaxEmbeddedDepthExceeded(this.limits.MaxEmbeddedMessageDepth), null);
			}
			this.depth++;
		}

		internal void EndEmbeddedMessage()
		{
			this.depth--;
		}

		internal void CountMessageBody()
		{
			if (this.depth == 0 && !this.bodyCounted)
			{
				this.CountMessagePart();
				this.bodyCounted = true;
			}
		}

		internal void CountMessageAttachment()
		{
			this.CountMessagePart();
		}

		private void CountMessagePart()
		{
			if (this.enforceLimits && this.partCount >= this.limits.MaxBodyPartsTotal)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcGenericTracer, "ConversionLimitsTracker::CountBodyPart: maximum body part count exceeded.");
				throw new ConversionFailedException(ConversionFailureReason.ExceedsLimit, ServerStrings.ConversionMaxBodyPartsExceeded(this.limits.MaxBodyPartsTotal), null);
			}
			this.partCount++;
		}

		internal int PartCount
		{
			get
			{
				return this.partCount;
			}
		}

		internal void CountRecipient()
		{
			if (this.enforceLimits && this.recipientCount >= this.limits.MaxMimeRecipients)
			{
				StorageGlobals.ContextTraceError(ExTraceGlobals.CcGenericTracer, "ConversionLimitsTracker::CountRecipient: maximum recipient count exceeded.");
				throw new ConversionFailedException(ConversionFailureReason.ExceedsLimit, ServerStrings.ConversionMaxRecipientExceeded(this.limits.MaxMimeRecipients), null);
			}
			this.recipientCount++;
		}

		internal void RollbackRecipients(int count)
		{
			this.recipientCount -= count;
		}

		internal void SuppressLimitChecks()
		{
			if (this.limits.ExemptPFReplicationMessages)
			{
				this.enforceLimits = false;
			}
		}

		internal bool EnforceLimits
		{
			get
			{
				return this.enforceLimits;
			}
		}

		private int depth;

		private bool bodyCounted;

		private int partCount;

		private int recipientCount;

		private bool enforceLimits = true;

		private ConversionLimits limits;

		internal struct State
		{
			internal int Depth;

			internal int PartCount;

			internal int RecipientCount;

			internal bool BodyCounted;
		}
	}
}
