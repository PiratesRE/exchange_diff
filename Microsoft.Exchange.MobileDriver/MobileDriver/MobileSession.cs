using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.VersionedXml;
using Microsoft.Exchange.TextMessaging.MobileDriver.Resources;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class MobileSession : IMobileActionProvider
	{
		public MobileSession(IMobileServiceSelector selector)
		{
			if (selector == null)
			{
				throw new ArgumentNullException("selector");
			}
			this.SendMode = MobileSessionSendMode.SynchronousSend;
			this.Services = new ReadOnlyCollection<IMobileService>(new IMobileService[]
			{
				MobileServiceCreator.Create(selector)
			});
		}

		public MobileSession(ExchangePrincipal principal, IList<DeliveryPoint> deliveryPoints)
		{
			if (deliveryPoints == null)
			{
				throw new ArgumentNullException("deliveryPoints");
			}
			if (deliveryPoints.Count == 0)
			{
				throw new ArgumentException("deliveryPoints");
			}
			this.SendMode = MobileSessionSendMode.SynchronousSend;
			this.Principal = principal;
			List<IMobileService> list = new List<IMobileService>();
			foreach (DeliveryPoint dp in DeliveryPoint.GetPersonToPersonPreferences(deliveryPoints))
			{
				list.Add(MobileServiceCreator.Create(principal, dp));
			}
			this.Services = list.AsReadOnly();
		}

		public MobileSession()
		{
			this.SendMode = MobileSessionSendMode.AsynchronousSend;
			MobileSession.InitializeBackEnd();
		}

		public IList<IMobileServiceManager> ServiceManagers
		{
			get
			{
				if (MobileSessionSendMode.SynchronousSend != this.SendMode)
				{
					throw new MobileDriverStateException(Strings.ErrorInvalidState("SendMode", this.SendMode.ToString()));
				}
				List<IMobileServiceManager> list = new List<IMobileServiceManager>();
				foreach (IMobileService mobileService in this.Services)
				{
					list.Add(mobileService.Manager);
				}
				return list.AsReadOnly();
			}
		}

		private ExchangePrincipal Principal { get; set; }

		private IList<IMobileService> Services { get; set; }

		private MobileSessionSendMode SendMode { get; set; }

		public void Send(Message message, MobileRecipient sender, ICollection<MobileRecipient> recipients, int maxSegmentsPerRecipient)
		{
			if (MobileSessionSendMode.SynchronousSend != this.SendMode)
			{
				throw new MobileDriverStateException(Strings.ErrorInvalidState("SendMode", this.SendMode.ToString()));
			}
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			if (sender == null)
			{
				throw new ArgumentNullException("sender");
			}
			if (recipients == null)
			{
				throw new ArgumentNullException("recipients");
			}
			List<MobileRecipient> list = new List<MobileRecipient>(recipients);
			if (list.Count == 0)
			{
				throw new ArgumentOutOfRangeException("recipients");
			}
			IMobileService mobileService = null;
			foreach (IMobileService mobileService2 in this.Services)
			{
				if (-1 != mobileService2.Manager.Selector.PersonToPersonMessagingPriority)
				{
					if (mobileService == null)
					{
						mobileService = mobileService2;
					}
					else if (mobileService2.Manager.Selector.PersonToPersonMessagingPriority < mobileService.Manager.Selector.PersonToPersonMessagingPriority)
					{
						mobileService = mobileService2;
					}
				}
			}
			if (mobileService == null)
			{
				throw new ArgumentNullException("recipients");
			}
			new TextMessageDeliverer(new TextMessageDeliveryContext
			{
				MobileService = this.Services[0],
				Message = new MessageItem(message, sender, recipients, maxSegmentsPerRecipient)
			}).Deliver();
		}

		internal void Send(TransportAgentWrapper agentWrapper, QueueDataAvailableEventHandler<TextMessageDeliveryContext> cleanerEventHandler)
		{
			if (this.SendMode != MobileSessionSendMode.AsynchronousSend)
			{
				throw new MobileDriverStateException(Strings.ErrorInvalidState("SendMode", this.SendMode.ToString()));
			}
			TextMessageDeliveryContext textMessageDeliveryContext = new TextMessageDeliveryContext();
			textMessageDeliveryContext.AgentWrapper = agentWrapper;
			textMessageDeliveryContext.CleanerEventHandler = cleanerEventHandler;
			MobileSession.deliveringPipeline.Deliver(textMessageDeliveryContext);
		}

		private static void InitializeBackEnd()
		{
			if (MobileSession.backEndInitialized)
			{
				return;
			}
			lock (typeof(MobileSession))
			{
				if (!MobileSession.backEndInitialized)
				{
					MobileSession.deliveringPipeline = new TextMessageDeliveringPipeline();
					MobileSession.deliveringPipeline.Start();
					MobileSession.backEndInitialized = true;
				}
			}
		}

		private static bool backEndInitialized;

		private static TextMessageDeliveringPipeline deliveringPipeline;
	}
}
