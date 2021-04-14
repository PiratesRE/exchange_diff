using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class RuleAction
	{
		protected RuleAction(RuleActionType type, uint userFlags)
		{
			EnumValidator.ThrowIfInvalid<RuleActionType>(type);
			this.ActionType = type;
			this.UserFlags = userFlags;
		}

		public override string ToString()
		{
			return string.Format("[{0}, user flags=0x{1:X}.{2}]", this.ActionType, this.UserFlags, this.InternalToString());
		}

		protected virtual string InternalToString()
		{
			return string.Empty;
		}

		public readonly RuleActionType ActionType;

		public readonly uint UserFlags;

		public abstract class MoveCopyActionBase : RuleAction
		{
			protected MoveCopyActionBase(RuleActionType type, uint userFlags, byte[] destinationStoreEntryId, StoreObjectId destinationFolderId, byte[] externalDestinationFolderId) : base(type, userFlags)
			{
				if (externalDestinationFolderId != null)
				{
					Util.ThrowOnNullArgument(destinationStoreEntryId, "destinationStoreEntryId");
				}
				if (destinationFolderId != null == (externalDestinationFolderId != null))
				{
					throw new ArgumentException("There must be either a destination folder id (as a StoreObjectId) or an external folder id (as a byte[]), but not both.");
				}
				this.DestinationStoreEntryId = destinationStoreEntryId;
				this.DestinationFolderId = destinationFolderId;
				this.ExternalDestinationFolderId = externalDestinationFolderId;
				this.FolderIsInThisStore = (destinationFolderId != null);
			}

			protected override string InternalToString()
			{
				return string.Format("DestinationStoreEntryId={0}.DestinationFolderId={1}.", this.DestinationStoreEntryId, this.DestinationFolderId);
			}

			public readonly byte[] DestinationStoreEntryId;

			public readonly StoreObjectId DestinationFolderId;

			public readonly byte[] ExternalDestinationFolderId;

			public readonly bool FolderIsInThisStore;
		}

		public sealed class MoveAction : RuleAction.MoveCopyActionBase
		{
			public MoveAction(uint userFlags, StoreObjectId destinationFolderId) : base(RuleActionType.Move, userFlags, null, destinationFolderId, null)
			{
			}

			public MoveAction(uint userFlags, byte[] destinationStoreEntryId, byte[] externalDestinationFolderId) : base(RuleActionType.Move, userFlags, destinationStoreEntryId, null, externalDestinationFolderId)
			{
			}
		}

		public sealed class CopyAction : RuleAction.MoveCopyActionBase
		{
			public CopyAction(uint userFlags, StoreObjectId destinationFolderId) : base(RuleActionType.Copy, userFlags, null, destinationFolderId, null)
			{
			}

			public CopyAction(uint userFlags, byte[] destinationStoreEntryId, byte[] externalDestinationFolderId) : base(RuleActionType.Copy, userFlags, destinationStoreEntryId, null, externalDestinationFolderId)
			{
			}
		}

		public abstract class ReplyActionBase : RuleAction
		{
			internal ReplyActionBase(RuleActionType type, uint userFlags, StoreObjectId replyTemplateMessageId, Guid replyTemplateGuid) : base(type, userFlags)
			{
				this.ReplyTemplateMessageId = replyTemplateMessageId;
				this.ReplyTemplateGuid = replyTemplateGuid;
			}

			protected override string InternalToString()
			{
				return string.Format("TemplateMessageId={0}.TemplateGuid={1}.", this.ReplyTemplateMessageId, this.ReplyTemplateGuid);
			}

			public readonly StoreObjectId ReplyTemplateMessageId;

			public readonly Guid ReplyTemplateGuid;
		}

		public sealed class ReplyAction : RuleAction.ReplyActionBase
		{
			public ReplyAction(uint userFlags, RuleAction.ReplyAction.ReplyFlags replyFlags, StoreObjectId replyTemplateMessageId, Guid replyTemplateGuid) : base(RuleActionType.Reply, userFlags, replyTemplateMessageId, replyTemplateGuid)
			{
				EnumValidator.ThrowIfInvalid<RuleAction.ReplyAction.ReplyFlags>(replyFlags);
				this.Flags = replyFlags;
			}

			protected override string InternalToString()
			{
				return string.Format("ReplyFlags={0}.{1}", this.Flags, base.InternalToString());
			}

			public readonly RuleAction.ReplyAction.ReplyFlags Flags;

			[Flags]
			public enum ReplyFlags
			{
				None = 0,
				DoNotSendToOriginator = 1,
				UseStockReplyTemplate = 2
			}
		}

		public sealed class OutOfOfficeReplyAction : RuleAction.ReplyActionBase
		{
			public OutOfOfficeReplyAction(uint userFlags, StoreObjectId replyTemplateMessageId, Guid replyTemplateGuid) : base(RuleActionType.OutOfOfficeReply, userFlags, replyTemplateMessageId, replyTemplateGuid)
			{
			}
		}

		public sealed class DeferAction : RuleAction
		{
			public DeferAction(uint userFlags, byte[] data) : base(RuleActionType.DeferAction, userFlags)
			{
				Util.ThrowOnNullArgument(data, "data");
				this.Data = data;
			}

			protected override string InternalToString()
			{
				return string.Format("Data.Length={0}", this.Data.Length);
			}

			public readonly byte[] Data;
		}

		public sealed class BounceAction : RuleAction
		{
			public BounceAction(uint userFlags, uint bounceCode) : base(RuleActionType.Bounce, userFlags)
			{
				this.BounceCode = bounceCode;
			}

			protected override string InternalToString()
			{
				return string.Format("bounceCode=0x{0:X}", this.BounceCode);
			}

			public readonly uint BounceCode;
		}

		public abstract class ForwardActionBase : RuleAction
		{
			protected ForwardActionBase(RuleActionType type, uint userFlags, RuleAction.ForwardActionBase.ActionRecipient[] recipients) : base(type, userFlags)
			{
				Util.ThrowOnNullArgument(recipients, "recipients");
				this.Recipients = recipients;
			}

			protected override string InternalToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (RuleAction.ForwardActionBase.ActionRecipient actionRecipient in this.Recipients)
				{
					stringBuilder.AppendFormat("[{0}]", actionRecipient);
				}
				return string.Format("Recipients=[{0}].", stringBuilder.ToString());
			}

			public readonly RuleAction.ForwardActionBase.ActionRecipient[] Recipients;

			public struct ActionRecipient
			{
				public ActionRecipient(IList<NativeStorePropertyDefinition> propertyDefinitions, IList<object> propertyValues)
				{
					Util.ThrowOnNullArgument(propertyDefinitions, "propertyDefinitions");
					Util.ThrowOnNullArgument(propertyValues, "propertyValues");
					if (propertyDefinitions.Count != propertyValues.Count)
					{
						throw new ArgumentException("propertyDefinitions and propertyValues should be of the same size.");
					}
					this.PropertyDefinitions = propertyDefinitions;
					this.PropertyValues = propertyValues;
				}

				public readonly IList<NativeStorePropertyDefinition> PropertyDefinitions;

				public readonly IList<object> PropertyValues;
			}
		}

		public sealed class ForwardAction : RuleAction.ForwardActionBase
		{
			public ForwardAction(uint userFlags, RuleAction.ForwardActionBase.ActionRecipient[] recipients, RuleAction.ForwardAction.ForwardFlags forwardFlags) : base(RuleActionType.Forward, userFlags, recipients)
			{
				EnumValidator.ThrowIfInvalid<RuleAction.ForwardAction.ForwardFlags>(forwardFlags);
				this.Flags = forwardFlags;
			}

			protected override string InternalToString()
			{
				return string.Format("{0}ForwardFlags={1}.", base.InternalToString(), this.Flags);
			}

			public readonly RuleAction.ForwardAction.ForwardFlags Flags;

			[Flags]
			public enum ForwardFlags
			{
				None = 0,
				PreserveSender = 1,
				DoNotChangeMessage = 2,
				ForwardAsAttachment = 4,
				SendSmsAlert = 8
			}
		}

		public sealed class DelegateAction : RuleAction.ForwardActionBase
		{
			public DelegateAction(uint userFlags, RuleAction.ForwardActionBase.ActionRecipient[] recipients) : base(RuleActionType.Delegate, userFlags, recipients)
			{
			}
		}

		public sealed class TagAction : RuleAction
		{
			public TagAction(uint userFlags, NativeStorePropertyDefinition propertyDefinition, object propertyValue) : base(RuleActionType.Tag, userFlags)
			{
				Util.ThrowOnNullArgument(propertyDefinition, "propertyDefinition");
				Util.ThrowOnNullArgument(propertyValue, "propertyValue");
				this.PropertyDefinition = propertyDefinition;
				this.PropertyValue = propertyValue;
			}

			protected override string InternalToString()
			{
				return string.Format("Property={0}.Value={1}.", this.PropertyDefinition, this.PropertyValue);
			}

			public readonly NativeStorePropertyDefinition PropertyDefinition;

			public readonly object PropertyValue;
		}

		public sealed class DeleteAction : RuleAction
		{
			public DeleteAction(uint userFlags) : base(RuleActionType.Delete, userFlags)
			{
			}
		}

		public sealed class MarkAsReadAction : RuleAction
		{
			public MarkAsReadAction(uint userFlags) : base(RuleActionType.MarkAsRead, userFlags)
			{
			}
		}
	}
}
