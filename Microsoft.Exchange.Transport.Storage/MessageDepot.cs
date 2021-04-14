using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.MessageDepot
{
	[PartCreationPolicy(CreationPolicy.Shared)]
	[Export(typeof(IMessageDepot))]
	internal sealed class MessageDepot : IMessageDepot, IMessageDepotQueueViewer
	{
		public MessageDepot() : this(null, null)
		{
		}

		public MessageDepot(Func<DateTime> timeProvider, TimeSpan? delayNotificationTimeout)
		{
			this.timeProvider = timeProvider;
			this.CreateMessageTransitionMap();
			if (delayNotificationTimeout == null)
			{
				this.delayNotificationTimeout = MessageDepot.DelayNotificationTimeout;
			}
			else
			{
				this.delayNotificationTimeout = delayNotificationTimeout.Value;
			}
			Type typeFromHandle = typeof(MessageDepotItemStage);
			foreach (object obj in Enum.GetValues(typeFromHandle))
			{
				int num = (int)obj;
				this.messageAddedHandlers[num] = delegate(MessageEventArgs param0)
				{
				};
				this.messageActivatedHandlers[num] = delegate(MessageActivatedEventArgs param0)
				{
				};
				this.messageDeactivatedHandlers[num] = delegate(MessageDeactivatedEventArgs param0)
				{
				};
				this.messageRemovedHandlers[num] = delegate(MessageRemovedEventArgs param0)
				{
				};
				this.messageExpiredHandlers[num] = delegate(MessageEventArgs param0)
				{
				};
				this.messageDelayedHandlers[num] = delegate(MessageEventArgs param0)
				{
				};
				string name = Enum.GetName(typeFromHandle, num);
				MessageDepotPerfCountersInstance instance = MessageDepotPerfCounters.GetInstance(name);
				this.messageCounterWrappers[num] = new MessageDepot.CounterWrapper[MessageDepot.StateCount];
				this.messageCounterWrappers[num][0] = new MessageDepot.CounterWrapper(instance.ReadyMessages);
				this.messageCounterWrappers[num][1] = new MessageDepot.CounterWrapper(instance.DeferredMessages);
				this.messageCounterWrappers[num][5] = new MessageDepot.CounterWrapper(instance.ExpiringMessages);
				this.messageCounterWrappers[num][2] = new MessageDepot.CounterWrapper(instance.PoisonMessages);
				this.messageCounterWrappers[num][4] = new MessageDepot.CounterWrapper(instance.ProcessingMessages);
				this.messageCounterWrappers[num][3] = new MessageDepot.CounterWrapper(instance.SuspendedMessages);
			}
		}

		public void SubscribeToAddEvent(MessageDepotItemStage targetStage, MessageEventHandler eventHandler)
		{
			MessageEventHandler[] array;
			(array = this.messageAddedHandlers)[(int)targetStage] = (MessageEventHandler)Delegate.Combine(array[(int)targetStage], eventHandler);
		}

		public void UnsubscribeFromAddEvent(MessageDepotItemStage targetStage, MessageEventHandler eventHandler)
		{
			MessageEventHandler[] array;
			(array = this.messageAddedHandlers)[(int)targetStage] = (MessageEventHandler)Delegate.Remove(array[(int)targetStage], eventHandler);
		}

		public void SubscribeToActivatedEvent(MessageDepotItemStage targetStage, MessageActivatedEventHandler eventHandler)
		{
			MessageActivatedEventHandler[] array;
			(array = this.messageActivatedHandlers)[(int)targetStage] = (MessageActivatedEventHandler)Delegate.Combine(array[(int)targetStage], eventHandler);
		}

		public void UnsubscribeFromActivatedEvent(MessageDepotItemStage targetStage, MessageActivatedEventHandler eventHandler)
		{
			MessageActivatedEventHandler[] array;
			(array = this.messageActivatedHandlers)[(int)targetStage] = (MessageActivatedEventHandler)Delegate.Remove(array[(int)targetStage], eventHandler);
		}

		public void SubscribeToDeactivatedEvent(MessageDepotItemStage targetStage, MessageDeactivatedEventHandler eventHandler)
		{
			MessageDeactivatedEventHandler[] array;
			(array = this.messageDeactivatedHandlers)[(int)targetStage] = (MessageDeactivatedEventHandler)Delegate.Combine(array[(int)targetStage], eventHandler);
		}

		public void UnsubscribeFromDeactivatedEvent(MessageDepotItemStage targetStage, MessageDeactivatedEventHandler eventHandler)
		{
			MessageDeactivatedEventHandler[] array;
			(array = this.messageDeactivatedHandlers)[(int)targetStage] = (MessageDeactivatedEventHandler)Delegate.Remove(array[(int)targetStage], eventHandler);
		}

		public void SubscribeToRemovedEvent(MessageDepotItemStage targetStage, MessageRemovedEventHandler eventHandler)
		{
			MessageRemovedEventHandler[] array;
			(array = this.messageRemovedHandlers)[(int)targetStage] = (MessageRemovedEventHandler)Delegate.Combine(array[(int)targetStage], eventHandler);
		}

		public void UnsubscribeFromRemovedEvent(MessageDepotItemStage targetStage, MessageRemovedEventHandler eventHandler)
		{
			MessageRemovedEventHandler[] array;
			(array = this.messageRemovedHandlers)[(int)targetStage] = (MessageRemovedEventHandler)Delegate.Remove(array[(int)targetStage], eventHandler);
		}

		public void SubscribeToExpiredEvent(MessageDepotItemStage targetStage, MessageEventHandler eventHandler)
		{
			MessageEventHandler[] array;
			(array = this.messageExpiredHandlers)[(int)targetStage] = (MessageEventHandler)Delegate.Combine(array[(int)targetStage], eventHandler);
		}

		public void UnsubscribeFromExpiredEvent(MessageDepotItemStage targetStage, MessageEventHandler eventHandler)
		{
			MessageEventHandler[] array;
			(array = this.messageExpiredHandlers)[(int)targetStage] = (MessageEventHandler)Delegate.Remove(array[(int)targetStage], eventHandler);
		}

		public void SubscribeToDelayedEvent(MessageDepotItemStage targetStage, MessageEventHandler eventHandler)
		{
			MessageEventHandler[] array;
			(array = this.messageDelayedHandlers)[(int)targetStage] = (MessageEventHandler)Delegate.Combine(array[(int)targetStage], eventHandler);
		}

		public void UnsubscribeFromDelayedEvent(MessageDepotItemStage targetStage, MessageEventHandler eventHandler)
		{
			MessageEventHandler[] array;
			(array = this.messageDelayedHandlers)[(int)targetStage] = (MessageEventHandler)Delegate.Remove(array[(int)targetStage], eventHandler);
		}

		public void Add(IMessageDepotItem item)
		{
			this.ValidateAddArguments(item);
			MessageDepot.MessageDepotItemWrapper messageDepotItemWrapper = new MessageDepot.MessageDepotItemWrapper(item, MessageDepotItemState.Ready);
			lock (messageDepotItemWrapper)
			{
				this.SetNewMessageState(messageDepotItemWrapper);
				if (!this.allMessages.TryAdd(item.Id, messageDepotItemWrapper))
				{
					throw new DuplicateItemException(item.Id, messageDepotItemWrapper.State, Strings.DuplicateItemFound(item.Id), null);
				}
			}
			this.AddToNearLists(messageDepotItemWrapper);
			this.IncrementMessageCount(messageDepotItemWrapper);
			this.RaiseEventsAfterAddItem(messageDepotItemWrapper);
		}

		public void DeferMessage(TransportMessageId messageId, TimeSpan deferTimeSpan, AcquireToken acquireToken)
		{
			ArgumentValidator.ThrowIfNull("acquireToken", acquireToken);
			if (deferTimeSpan < TimeSpan.Zero)
			{
				throw new ArgumentException("Defer time span cannot be negative");
			}
			MessageDepot.MessageDepotItemWrapper item = this.GetItem(messageId);
			if (acquireToken != item.AcquireToken)
			{
				throw new MessageDepotPermanentException(Strings.AcquireTokenMatchFail(item.Item.Id), null);
			}
			lock (item)
			{
				this.ChangeMessageState(item, MessageDepotItemState.Deferred);
				item.Item.DeferUntil = this.GetCurrentTime().Add(deferTimeSpan);
				this.messageDeactivatedHandlers[(int)item.Item.Stage](new MessageDeactivatedEventArgs(item, MessageDeactivationReason.Deferred));
				this.AddToNearLists(item);
			}
		}

		public AcquireResult Acquire(TransportMessageId messageId)
		{
			Exception ex;
			AcquireResult result = this.Acquire(messageId, out ex);
			if (ex != null)
			{
				throw ex;
			}
			return result;
		}

		public bool TryAcquire(TransportMessageId messageId, out AcquireResult result)
		{
			Exception ex;
			result = this.Acquire(messageId, out ex);
			return ex == null;
		}

		public void Release(TransportMessageId messageId, AcquireToken acquireToken)
		{
			ArgumentValidator.ThrowIfNull("acquireToken", acquireToken);
			MessageDepot.MessageDepotItemWrapper item = this.GetItem(messageId);
			if (acquireToken != item.AcquireToken)
			{
				throw new MessageDepotPermanentException(Strings.AcquireTokenMatchFail(item.Item.Id), null);
			}
			lock (item)
			{
				if (item.State != MessageDepotItemState.Processing)
				{
					throw new MessageDepotPermanentException(Strings.InvalidMessageStateTransition(messageId, item.State, MessageDepotItemState.Processing), null);
				}
				this.RemoveItemFromMessageDepot(item, true);
				MessageRemovedEventArgs args = new MessageRemovedEventArgs(item, MessageRemovalReason.Deleted, false);
				this.messageRemovedHandlers[(int)item.Item.Stage](args);
			}
		}

		public IMessageDepotItemWrapper Get(TransportMessageId messageId)
		{
			return this.GetItem(messageId);
		}

		public bool TryGet(TransportMessageId messageId, out IMessageDepotItemWrapper item)
		{
			ArgumentValidator.ThrowIfNull("messageId", messageId);
			MessageDepot.MessageDepotItemWrapper messageDepotItemWrapper;
			if (this.allMessages.TryGetValue(messageId, out messageDepotItemWrapper))
			{
				item = messageDepotItemWrapper;
				return true;
			}
			item = null;
			return false;
		}

		public void Remove(TransportMessageId messageId, bool withNdr)
		{
			MessageDepot.MessageDepotItemWrapper item = this.GetItem(messageId);
			lock (item)
			{
				this.RemoveItemFromMessageDepot(item, false);
				this.messageRemovedHandlers[(int)item.Item.Stage](new MessageRemovedEventArgs(item, MessageRemovalReason.Deleted, withNdr));
			}
		}

		public void Suspend(TransportMessageId messageId)
		{
			MessageDepot.MessageDepotItemWrapper item = this.GetItem(messageId);
			lock (item)
			{
				this.ChangeMessageState(item, MessageDepotItemState.Suspended);
				this.messageDeactivatedHandlers[(int)item.Item.Stage](new MessageDeactivatedEventArgs(item, MessageDeactivationReason.Suspended));
			}
		}

		public void Resume(TransportMessageId messageId)
		{
			MessageDepot.MessageDepotItemWrapper item = this.GetItem(messageId);
			lock (item)
			{
				if (item.State != MessageDepotItemState.Suspended)
				{
					throw new MessageDepotPermanentException(Strings.InvalidMessageStateTransition(messageId, item.State, MessageDepotItemState.Suspended), null);
				}
				this.ChangeMessageState(item, MessageDepotItemState.Ready);
				this.messageActivatedHandlers[(int)item.Item.Stage](new MessageActivatedEventArgs(item, MessageActivationReason.Resumed));
			}
		}

		public void DehydrateAll()
		{
			Parallel.ForEach<MessageDepot.MessageDepotItemWrapper>(from itemWrapper in this.allMessages
			select itemWrapper.Value, delegate(MessageDepot.MessageDepotItemWrapper itemWrapper)
			{
				itemWrapper.Item.Dehydrate();
			});
		}

		public void VisitMailItems(Func<IMessageDepotItemWrapper, bool> visitor)
		{
			foreach (MessageDepot.MessageDepotItemWrapper arg in from item in this.allMessages
			select item.Value)
			{
				if (!visitor(arg))
				{
					break;
				}
			}
		}

		public long GetCount(MessageDepotItemStage stage, MessageDepotItemState state)
		{
			return this.messageCounterWrappers[(int)stage][(int)state].Count;
		}

		public void TimedUpdate()
		{
			this.UpdateNearLists();
			this.MoveDeferToReady();
			this.DelayMessages();
			this.ExpireMessages();
		}

		private void ExpireMessages()
		{
			DateTime currentTime = this.GetCurrentTime();
			foreach (MessageDepot.MessageDepotItemWrapper messageDepotItemWrapper in from item in this.nearExpiryMessages
			select item.Value)
			{
				lock (messageDepotItemWrapper)
				{
					if (messageDepotItemWrapper.State != MessageDepotItemState.Expiring && messageDepotItemWrapper.Item.ExpirationTime <= currentTime)
					{
						Exception ex;
						this.ChangeMessageState(messageDepotItemWrapper, MessageDepotItemState.Expiring, out ex);
						this.messageExpiredHandlers[(int)messageDepotItemWrapper.Item.Stage](new MessageEventArgs(messageDepotItemWrapper));
					}
				}
			}
		}

		private void DelayMessages()
		{
			DateTime currentTime = this.GetCurrentTime();
			foreach (MessageDepot.MessageDepotItemWrapper messageDepotItemWrapper in from item in this.nearDelayMessages
			select item.Value)
			{
				lock (messageDepotItemWrapper)
				{
					if ((messageDepotItemWrapper.State == MessageDepotItemState.Ready || messageDepotItemWrapper.State == MessageDepotItemState.Deferred) && !messageDepotItemWrapper.Item.IsDelayDsnGenerated && messageDepotItemWrapper.Item.ArrivalTime.Add(this.delayNotificationTimeout) <= currentTime)
					{
						this.messageDelayedHandlers[(int)messageDepotItemWrapper.Item.Stage](new MessageEventArgs(messageDepotItemWrapper));
					}
				}
			}
		}

		private void MoveDeferToReady()
		{
			DateTime currentTime = this.GetCurrentTime();
			foreach (MessageDepot.MessageDepotItemWrapper messageDepotItemWrapper in from item in this.nearDeferralOverMessages
			select item.Value)
			{
				lock (messageDepotItemWrapper)
				{
					if (messageDepotItemWrapper.State == MessageDepotItemState.Deferred && messageDepotItemWrapper.Item.DeferUntil <= currentTime)
					{
						Exception ex;
						this.ChangeMessageState(messageDepotItemWrapper, MessageDepotItemState.Ready, out ex);
						if (ex == null)
						{
							this.messageActivatedHandlers[(int)messageDepotItemWrapper.Item.Stage](new MessageActivatedEventArgs(messageDepotItemWrapper, MessageActivationReason.DeferralOver));
						}
					}
				}
			}
		}

		private MessageDepot.MessageDepotItemWrapper GetItem(TransportMessageId messageId)
		{
			Exception ex;
			MessageDepot.MessageDepotItemWrapper item = this.GetItem(messageId, out ex);
			if (ex != null)
			{
				throw ex;
			}
			return item;
		}

		private MessageDepot.MessageDepotItemWrapper GetItem(TransportMessageId messageId, out Exception exception)
		{
			exception = null;
			ArgumentValidator.ThrowIfNull("messageId", messageId);
			MessageDepot.MessageDepotItemWrapper messageDepotItemWrapper;
			if (!this.allMessages.TryGetValue(messageId, out messageDepotItemWrapper) || messageDepotItemWrapper == null)
			{
				exception = new ItemNotFoundException(messageId, Strings.ItemNotFound(messageId), null);
			}
			return messageDepotItemWrapper;
		}

		private void RaiseEventsAfterAddItem(MessageDepot.MessageDepotItemWrapper itemWrapper)
		{
			switch (itemWrapper.State)
			{
			case MessageDepotItemState.Ready:
				this.messageAddedHandlers[(int)itemWrapper.Item.Stage](new MessageEventArgs(itemWrapper));
				if (itemWrapper.Item.ExpirationTime > this.GetCurrentTime())
				{
					this.messageActivatedHandlers[(int)itemWrapper.Item.Stage](new MessageActivatedEventArgs(itemWrapper, MessageActivationReason.New));
					return;
				}
				break;
			case MessageDepotItemState.Deferred:
			case MessageDepotItemState.Poisoned:
			case MessageDepotItemState.Suspended:
			{
				MessageDeactivationReason reason = MessageDeactivationReason.Deferred;
				if (itemWrapper.State == MessageDepotItemState.Poisoned)
				{
					reason = MessageDeactivationReason.Poison;
				}
				else if (itemWrapper.State == MessageDepotItemState.Suspended)
				{
					reason = MessageDeactivationReason.Suspended;
				}
				this.messageAddedHandlers[(int)itemWrapper.Item.Stage](new MessageEventArgs(itemWrapper));
				this.messageDeactivatedHandlers[(int)itemWrapper.Item.Stage](new MessageDeactivatedEventArgs(itemWrapper, reason));
				break;
			}
			default:
				return;
			}
		}

		private DateTime GetCurrentTime()
		{
			if (this.timeProvider == null)
			{
				return DateTime.UtcNow;
			}
			return this.timeProvider();
		}

		private AcquireResult Acquire(TransportMessageId messageId, out Exception exception)
		{
			MessageDepot.MessageDepotItemWrapper item = this.GetItem(messageId, out exception);
			if (exception != null)
			{
				return null;
			}
			lock (item)
			{
				this.ChangeMessageState(item, MessageDepotItemState.Processing, out exception);
				if (exception != null)
				{
					return null;
				}
				item.AcquireToken = new AcquireToken();
			}
			return new AcquireResult(item, item.AcquireToken);
		}

		private void RemoveItemFromMessageDepot(MessageDepot.MessageDepotItemWrapper itemWrapper, bool forceRemove)
		{
			if (!forceRemove && !this.IsStateTransitionAllowed(this.statesAllowedForRemoveApi, itemWrapper.State))
			{
				throw new MessageDepotPermanentException(Strings.InvalidMessageStateForRemove(itemWrapper.Item.Id, itemWrapper.State), null);
			}
			MessageDepot.MessageDepotItemWrapper messageDepotItemWrapper;
			if (!this.allMessages.TryRemove(itemWrapper.Item.Id, out messageDepotItemWrapper))
			{
				throw new MessageDepotPermanentException(Strings.FailedToRemove(itemWrapper.Item.Id), null);
			}
			this.nearDeferralOverMessages.TryRemove(itemWrapper.Item.Id, out messageDepotItemWrapper);
			this.nearExpiryMessages.TryRemove(itemWrapper.Item.Id, out messageDepotItemWrapper);
			this.nearDelayMessages.TryRemove(itemWrapper.Item.Id, out messageDepotItemWrapper);
			this.DecrementMessageCount(itemWrapper);
		}

		private void AddToNearLists(MessageDepot.MessageDepotItemWrapper itemWrapper)
		{
			DateTime t = this.GetCurrentTime().Add(MessageDepot.DefaultNearTimeSpan);
			if (itemWrapper.Item.ExpirationTime < t)
			{
				this.nearExpiryMessages.TryAdd(itemWrapper.Item.Id, itemWrapper);
			}
			if (itemWrapper.Item.DeferUntil > DateTime.MinValue && itemWrapper.Item.DeferUntil < t)
			{
				this.nearDeferralOverMessages.TryAdd(itemWrapper.Item.Id, itemWrapper);
			}
			if (itemWrapper.Item.ArrivalTime.Add(this.delayNotificationTimeout) < t)
			{
				this.nearDelayMessages.TryAdd(itemWrapper.Item.Id, itemWrapper);
			}
		}

		private void UpdateNearLists()
		{
			if (this.lastNearListRefreshTime > DateTime.MinValue && this.lastNearListRefreshTime < this.GetCurrentTime().Add(MessageDepot.DefaultNearTimeSpan))
			{
				return;
			}
			this.nearDeferralOverMessages.Clear();
			this.nearDelayMessages.Clear();
			this.nearExpiryMessages.Clear();
			foreach (MessageDepot.MessageDepotItemWrapper itemWrapper in from item in this.allMessages
			select item.Value)
			{
				this.AddToNearLists(itemWrapper);
			}
			this.lastNearListRefreshTime = this.GetCurrentTime();
		}

		private void IncrementMessageCount(MessageDepot.MessageDepotItemWrapper itemWrapper)
		{
			int stage = (int)itemWrapper.Item.Stage;
			int state = (int)itemWrapper.State;
			this.messageCounterWrappers[stage][state].Increment();
		}

		private void DecrementMessageCount(MessageDepot.MessageDepotItemWrapper itemWrapper)
		{
			int stage = (int)itemWrapper.Item.Stage;
			int state = (int)itemWrapper.State;
			this.messageCounterWrappers[stage][state].Decrement();
		}

		private void SetNewMessageState(MessageDepot.MessageDepotItemWrapper itemWrapper)
		{
			if (itemWrapper.Item.IsPoison)
			{
				itemWrapper.SetState(MessageDepotItemState.Poisoned);
				return;
			}
			if (itemWrapper.Item.IsSuspended)
			{
				itemWrapper.SetState(MessageDepotItemState.Suspended);
				return;
			}
			if (itemWrapper.Item.DeferUntil > this.GetCurrentTime())
			{
				itemWrapper.SetState(MessageDepotItemState.Deferred);
				return;
			}
			itemWrapper.SetState(MessageDepotItemState.Ready);
		}

		private void ValidateAddArguments(IMessageDepotItem item)
		{
			ArgumentValidator.ThrowIfNull("item", item);
			if (item.Id == null)
			{
				throw new ArgumentException("Message Id cannot be null");
			}
			if (item.ArrivalTime > this.GetCurrentTime())
			{
				throw new ArgumentException("Message arrival time cannot be in future");
			}
		}

		private void CreateMessageTransitionMap()
		{
			this.messageTransitionMap = new BitVector32[MessageDepot.StateCount];
			this.messageTransitionMap[0] = this.GetAllowedStateMap(new MessageDepotItemState[]
			{
				MessageDepotItemState.Suspended,
				MessageDepotItemState.Suspended,
				MessageDepotItemState.Processing,
				MessageDepotItemState.Expiring
			});
			this.messageTransitionMap[1] = this.GetAllowedStateMap(new MessageDepotItemState[]
			{
				MessageDepotItemState.Ready,
				MessageDepotItemState.Suspended,
				MessageDepotItemState.Expiring
			});
			BitVector32[] array = this.messageTransitionMap;
			int num = 2;
			MessageDepotItemState[] allowedStates = new MessageDepotItemState[1];
			array[num] = this.GetAllowedStateMap(allowedStates);
			BitVector32[] array2 = this.messageTransitionMap;
			int num2 = 3;
			MessageDepotItemState[] allowedStates2 = new MessageDepotItemState[1];
			array2[num2] = this.GetAllowedStateMap(allowedStates2);
			this.messageTransitionMap[4] = this.GetAllowedStateMap(new MessageDepotItemState[]
			{
				MessageDepotItemState.Deferred
			});
			this.messageTransitionMap[5] = this.GetAllowedStateMap(new MessageDepotItemState[]
			{
				MessageDepotItemState.Processing
			});
			this.statesAllowedForRemoveApi = this.GetAllowedStateMap(new MessageDepotItemState[]
			{
				MessageDepotItemState.Ready,
				MessageDepotItemState.Deferred,
				MessageDepotItemState.Poisoned,
				MessageDepotItemState.Suspended
			});
		}

		private void ChangeMessageState(MessageDepot.MessageDepotItemWrapper itemWrapper, MessageDepotItemState nextState)
		{
			Exception ex;
			this.ChangeMessageState(itemWrapper, nextState, out ex);
			if (ex != null)
			{
				throw ex;
			}
		}

		private void ChangeMessageState(MessageDepot.MessageDepotItemWrapper itemWrapper, MessageDepotItemState nextState, out Exception exception)
		{
			exception = null;
			if (!this.IsStateTransitionAllowed(this.messageTransitionMap[(int)itemWrapper.State], nextState))
			{
				exception = new MessageDepotPermanentException(Strings.InvalidMessageStateTransition(itemWrapper.Item.Id, itemWrapper.State, nextState), null);
				return;
			}
			this.DecrementMessageCount(itemWrapper);
			itemWrapper.SetState(nextState);
			this.IncrementMessageCount(itemWrapper);
		}

		private bool IsStateTransitionAllowed(BitVector32 bitVector, MessageDepotItemState nextState)
		{
			return bitVector[1 << (int)nextState];
		}

		private BitVector32 GetAllowedStateMap(params MessageDepotItemState[] allowedStates)
		{
			BitVector32 result = new BitVector32(0);
			foreach (MessageDepotItemState messageDepotItemState in allowedStates)
			{
				result[1 << (int)messageDepotItemState] = true;
			}
			return result;
		}

		private static readonly TimeSpan DefaultNearTimeSpan = TimeSpan.FromHours(1.0);

		private static readonly TimeSpan DelayNotificationTimeout = TimeSpan.FromHours(4.0);

		private static readonly int StageCount = Enum.GetValues(typeof(MessageDepotItemStage)).Length;

		private static readonly int StateCount = Enum.GetValues(typeof(MessageDepotItemState)).Length;

		private readonly MessageEventHandler[] messageAddedHandlers = new MessageEventHandler[MessageDepot.StageCount];

		private readonly MessageActivatedEventHandler[] messageActivatedHandlers = new MessageActivatedEventHandler[MessageDepot.StageCount];

		private readonly MessageDeactivatedEventHandler[] messageDeactivatedHandlers = new MessageDeactivatedEventHandler[MessageDepot.StageCount];

		private readonly MessageRemovedEventHandler[] messageRemovedHandlers = new MessageRemovedEventHandler[MessageDepot.StageCount];

		private readonly MessageEventHandler[] messageExpiredHandlers = new MessageEventHandler[MessageDepot.StageCount];

		private readonly MessageEventHandler[] messageDelayedHandlers = new MessageEventHandler[MessageDepot.StageCount];

		private readonly ConcurrentDictionary<TransportMessageId, MessageDepot.MessageDepotItemWrapper> allMessages = new ConcurrentDictionary<TransportMessageId, MessageDepot.MessageDepotItemWrapper>();

		private readonly ConcurrentDictionary<TransportMessageId, MessageDepot.MessageDepotItemWrapper> nearDeferralOverMessages = new ConcurrentDictionary<TransportMessageId, MessageDepot.MessageDepotItemWrapper>();

		private readonly ConcurrentDictionary<TransportMessageId, MessageDepot.MessageDepotItemWrapper> nearDelayMessages = new ConcurrentDictionary<TransportMessageId, MessageDepot.MessageDepotItemWrapper>();

		private readonly ConcurrentDictionary<TransportMessageId, MessageDepot.MessageDepotItemWrapper> nearExpiryMessages = new ConcurrentDictionary<TransportMessageId, MessageDepot.MessageDepotItemWrapper>();

		private readonly Func<DateTime> timeProvider;

		private readonly MessageDepot.CounterWrapper[][] messageCounterWrappers = new MessageDepot.CounterWrapper[MessageDepot.StageCount][];

		private readonly TimeSpan delayNotificationTimeout;

		private BitVector32[] messageTransitionMap;

		private BitVector32 statesAllowedForRemoveApi;

		private DateTime lastNearListRefreshTime = DateTime.MinValue;

		private class CounterWrapper
		{
			public CounterWrapper(ExPerformanceCounter performanceCounter)
			{
				ArgumentValidator.ThrowIfNull("performanceCounter", performanceCounter);
				this.performanceCounter = performanceCounter;
			}

			public long Count
			{
				get
				{
					return this.messageCount;
				}
			}

			public void Increment()
			{
				long rawValue = Interlocked.Increment(ref this.messageCount);
				this.performanceCounter.RawValue = rawValue;
			}

			public void Decrement()
			{
				long rawValue = Interlocked.Decrement(ref this.messageCount);
				this.performanceCounter.RawValue = rawValue;
			}

			private readonly ExPerformanceCounter performanceCounter;

			private long messageCount;
		}

		private class MessageDepotItemWrapper : IMessageDepotItemWrapper
		{
			public MessageDepotItemWrapper(IMessageDepotItem item, MessageDepotItemState state)
			{
				ArgumentValidator.ThrowIfNull("item", item);
				this.item = item;
				this.state = state;
			}

			public AcquireToken AcquireToken
			{
				get
				{
					return this.acquireToken;
				}
				set
				{
					this.acquireToken = value;
				}
			}

			public MessageDepotItemState State
			{
				get
				{
					return this.state;
				}
			}

			public IMessageDepotItem Item
			{
				get
				{
					return this.item;
				}
			}

			public void SetState(MessageDepotItemState newState)
			{
				this.state = newState;
			}

			private readonly IMessageDepotItem item;

			private MessageDepotItemState state;

			private AcquireToken acquireToken;
		}
	}
}
