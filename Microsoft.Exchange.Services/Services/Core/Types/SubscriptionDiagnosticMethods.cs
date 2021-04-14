using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class SubscriptionDiagnosticMethods
	{
		internal static XmlNode ClearSubscriptions(XmlNode param)
		{
			foreach (SubscriptionBase subscriptionBase in Subscriptions.Singleton.SubscriptionsList)
			{
				Subscriptions.Singleton.Delete(subscriptionBase.SubscriptionId);
			}
			return null;
		}

		[DiagnosticStrideJustification(DenialOfServiceJustification = "Enumerating subscriptions causes little load, and having a valid subscription id which you do not own gains you nothing over using a random id.", ElevationOfPrivligesJustification = "Each subscription keeps track of its owner; an attacker who does not own a subscription may not affect it.", InformationDisclosureJustification = "A subscription id contains the date/time created, the server the subscription is hosted on, and a random GUID.  None of this is privliged\r\n                information that could grant an attacker any advantage.", RepudiationJustification = "Common logging and AuthZ mechanisms apply here that apply to all EWS web methods.", SpoofingJustification = "Each subscription keeps track of its owner; an attacker may not spoof the owner using only a subscription id.", TamperingJustification = "Each subscription keeps track of its owner; an attacker who does not own a subscription may not affect it.  \r\n                Furthermore, this method effects no side-effect on the process.")]
		internal static XmlNode GetActiveSubscriptionIds(XmlNode param)
		{
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			XmlNode xmlNode = safeXmlDocument.CreateElement("Subscriptions", "http://schemas.microsoft.com/exchange/services/2006/types");
			foreach (SubscriptionBase subscriptionBase in Subscriptions.Singleton.SubscriptionsList)
			{
				if (!subscriptionBase.IsExpired)
				{
					XmlNode xmlNode2 = safeXmlDocument.CreateElement("Subscription", "http://schemas.microsoft.com/exchange/services/2006/types");
					XmlNode xmlNode3 = safeXmlDocument.CreateElement("SubscriptionId", "http://schemas.microsoft.com/exchange/services/2006/types");
					xmlNode3.InnerText = subscriptionBase.SubscriptionId;
					xmlNode2.AppendChild(xmlNode3);
					XmlNode xmlNode4 = safeXmlDocument.CreateElement("CreatorSmtpAddress", "http://schemas.microsoft.com/exchange/services/2006/types");
					xmlNode4.InnerText = subscriptionBase.CreatorSmtpAddress;
					xmlNode2.AppendChild(xmlNode4);
					xmlNode.AppendChild(xmlNode2);
				}
			}
			return xmlNode;
		}

		internal static XmlNode GetHangingSubscriptionConnections(XmlNode param)
		{
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			XmlNode xmlNode = safeXmlDocument.CreateElement("Connections", "http://schemas.microsoft.com/exchange/services/2006/types");
			foreach (StreamingConnection streamingConnection in StreamingConnection.OpenConnections)
			{
				if (!streamingConnection.IsDisposed)
				{
					List<StreamingSubscription> subscriptions = streamingConnection.Subscriptions;
					string creatorSmtpAddress = streamingConnection.CreatorSmtpAddress;
					if (subscriptions != null && subscriptions.Count != 0 && !string.IsNullOrEmpty(creatorSmtpAddress))
					{
						XmlNode xmlNode2 = safeXmlDocument.CreateElement("Connection", "http://schemas.microsoft.com/exchange/services/2006/types");
						XmlNode xmlNode3 = safeXmlDocument.CreateElement("CreatorSmtpAddress", "http://schemas.microsoft.com/exchange/services/2006/types");
						xmlNode3.InnerText = streamingConnection.CreatorSmtpAddress;
						xmlNode2.AppendChild(xmlNode3);
						XmlNode xmlNode4 = safeXmlDocument.CreateElement("Subscriptions", "http://schemas.microsoft.com/exchange/services/2006/types");
						xmlNode2.AppendChild(xmlNode4);
						foreach (StreamingSubscription streamingSubscription in subscriptions)
						{
							XmlNode xmlNode5 = safeXmlDocument.CreateElement("SubscriptionId", "http://schemas.microsoft.com/exchange/services/2006/types");
							xmlNode5.InnerText = streamingSubscription.SubscriptionId;
							xmlNode4.AppendChild(xmlNode5);
						}
						xmlNode.AppendChild(xmlNode2);
					}
				}
			}
			return xmlNode;
		}

		internal static XmlNode SetStreamingSubscriptionTimeToLiveDefault(XmlNode param)
		{
			int timeToLiveDefault = StreamingSubscription.TimeToLiveDefault;
			bool flag = param != null && int.TryParse(param.InnerText, out timeToLiveDefault) && timeToLiveDefault > 0;
			if (flag)
			{
				StreamingSubscription.TimeToLiveDefault = timeToLiveDefault;
			}
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			XmlNode xmlNode = safeXmlDocument.CreateElement("Success", "http://schemas.microsoft.com/exchange/services/2006/types");
			xmlNode.InnerText = flag.ToString();
			return xmlNode;
		}

		internal static XmlNode SetStreamingSubscriptionNewEventQueueSize(XmlNode param)
		{
			int num = 500;
			bool flag = param != null && int.TryParse(param.InnerText, out num) && num >= 10 && num <= 200;
			if (flag)
			{
				StreamingSubscription.NewEventQueueSize = num;
			}
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			XmlNode xmlNode = safeXmlDocument.CreateElement("Success", "http://schemas.microsoft.com/exchange/services/2006/types");
			xmlNode.InnerText = flag.ToString();
			return xmlNode;
		}

		internal static XmlNode SetStreamingConnectionHeartbeatDefault(XmlNode param)
		{
			int periodicConnectionCheckInterval = StreamingConnection.PeriodicConnectionCheckInterval;
			bool flag = param != null && int.TryParse(param.InnerText, out periodicConnectionCheckInterval) && periodicConnectionCheckInterval > 0;
			if (flag)
			{
				StreamingConnection.PeriodicConnectionCheckInterval = periodicConnectionCheckInterval * 1000;
			}
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			XmlNode xmlNode = safeXmlDocument.CreateElement("Success", "http://schemas.microsoft.com/exchange/services/2006/types");
			xmlNode.InnerText = flag.ToString();
			return xmlNode;
		}

		internal static XmlNode GetStreamingSubscriptionExpirationTime(XmlNode param)
		{
			string innerText = param.InnerText;
			StreamingSubscription streamingSubscription = Subscriptions.Singleton.Get(innerText) as StreamingSubscription;
			if (streamingSubscription == null)
			{
				throw new SubscriptionNotFoundException();
			}
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			XmlNode xmlNode = safeXmlDocument.CreateElement("ExpirationTime", "http://schemas.microsoft.com/exchange/services/2006/types");
			xmlNode.InnerText = streamingSubscription.ExpirationDateTime.UtcTicks.ToString();
			return xmlNode;
		}

		private const string ConnectionXmlElement = "Connection";

		private const string ConnectionsXmlElement = "Connections";

		private const string CreatorSmtpAddressXmlElement = "CreatorSmtpAddress";

		private const string ExpirationTime = "ExpirationTime";

		private const string NewDefaultValueXmlElement = "NewDefaultValue";

		private const string SubscriptionXmlElement = "Subscription";

		private const string SubscriptionIdXmlElement = "SubscriptionId";

		private const string SubscriptionsXmlElement = "Subscriptions";

		private const string SuccessXmlElement = "Success";
	}
}
