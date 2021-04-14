using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.ApplicationLogic.TextMessaging;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class SmtpToSmsGatewayManager : IMobileServiceManager
	{
		public SmtpToSmsGatewayManager(SmtpToSmsGatewaySelector selector)
		{
			this.Selector = selector;
		}

		IMobileServiceSelector IMobileServiceManager.Selector
		{
			get
			{
				return this.Selector;
			}
		}

		public bool CapabilityPerRecipientSupported
		{
			get
			{
				return true;
			}
		}

		public SmtpToSmsGatewaySelector Selector { get; private set; }

		public TextMessagingHostingDataServicesServiceSmtpToSmsGateway GetParameters(MobileRecipient recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			if (recipient.Region == null)
			{
				throw new ArgumentException("recipient");
			}
			TextMessagingHostingDataCache instance = TextMessagingHostingDataCache.Instance;
			TextMessagingHostingDataServicesService service = TextMessagingHostingDataCache.Instance.GetService(recipient.Region.TwoLetterISORegionName, recipient.Carrier.ToString("00000"), TextMessagingHostingDataServicesServiceType.SmtpToSmsGateway);
			if (service == null)
			{
				return null;
			}
			return service.SmtpToSmsGateway;
		}

		public SmtpToSmsGatewayCapability GetCapabilityForRecipient(MobileRecipient recipient)
		{
			TextMessagingHostingDataServicesServiceSmtpToSmsGateway parameters = this.GetParameters(recipient);
			if (parameters == null)
			{
				return null;
			}
			List<CodingSupportability> list = new List<CodingSupportability>(parameters.MessageRendering.Capacity.Length);
			TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity[] capacity = parameters.MessageRendering.Capacity;
			int i = 0;
			while (i < capacity.Length)
			{
				TextMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity = capacity[i];
				CodingScheme codingScheme = CodingScheme.Neutral;
				try
				{
					codingScheme = (CodingScheme)Enum.Parse(typeof(CodingScheme), textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity.CodingScheme.ToString());
				}
				catch (ArgumentException)
				{
					goto IL_85;
				}
				goto IL_64;
				IL_85:
				i++;
				continue;
				IL_64:
				if (0 < textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity.Value)
				{
					list.Add(new CodingSupportability(codingScheme, textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity.Value, textMessagingHostingDataServicesServiceSmtpToSmsGatewayMessageRenderingCapacity.Value));
					goto IL_85;
				}
				goto IL_85;
			}
			if (list.Count == 0)
			{
				return null;
			}
			return new SmtpToSmsGatewayCapability(PartType.Short, 1, list.ToArray(), FeatureSupportability.None, parameters);
		}

		MobileServiceCapability IMobileServiceManager.GetCapabilityForRecipient(MobileRecipient recipient)
		{
			return this.GetCapabilityForRecipient(recipient);
		}

		private const string ServiceType = "SmtpToSmsGateway";

		private const string MessageRenderingContainerBody = "Body";
	}
}
