using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Online.CSE.SmartAlerts;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.O365.Common
{
	public class SmartAlertsV2Client
	{
		public static void NewAlert(Guid alertGuid, string alertTypeId, string alertName, string alertDescription, DateTime raisedTime, string escalationTeam, string service, string alertSource, bool urgent, string environment, string location, string forest, string dag, string site, string region, string capacityUnit, string rack, NotificationServiceClass notificationServiceClass, string authCertSubject)
		{
			Alert alert = new Alert();
			alert.AlertId = new Guid?(alertGuid);
			alert.AlertType = alertTypeId;
			alert.AlertName = alertName;
			alert.AlertDescription = alertDescription;
			alert.AlertRaisedTime = new DateTime?(raisedTime);
			alert.Team = escalationTeam;
			alert.AlertSource = alertSource;
			alert.Service = service;
			Dictionary<string, Collection<string>> dictionary = new Dictionary<string, Collection<string>>();
			if (!string.IsNullOrEmpty(location))
			{
				dictionary.Add("Location", new Collection<string>
				{
					location
				});
			}
			if (!string.IsNullOrEmpty(forest))
			{
				dictionary.Add("Forest", new Collection<string>
				{
					forest
				});
			}
			if (!string.IsNullOrEmpty(dag))
			{
				dictionary.Add("DAG", new Collection<string>
				{
					dag
				});
			}
			if (!string.IsNullOrEmpty(site))
			{
				dictionary.Add("Site", new Collection<string>
				{
					site
				});
			}
			if (!string.IsNullOrEmpty(region))
			{
				dictionary.Add("Region", new Collection<string>
				{
					region
				});
			}
			if (!string.IsNullOrEmpty(capacityUnit))
			{
				dictionary.Add("Capacity Unit", new Collection<string>
				{
					capacityUnit
				});
			}
			if (!string.IsNullOrEmpty(rack))
			{
				dictionary.Add("Rack", new Collection<string>
				{
					rack
				});
			}
			alert.Scope = dictionary;
			if (notificationServiceClass == NotificationServiceClass.Urgent)
			{
				alert.AlertCategory = new AlertCategory?(3);
			}
			else
			{
				alert.AlertCategory = new AlertCategory?(5);
			}
			ISmartAlertsService smartAlertsService = SmartAlertsV2Client.InitializeSmartAlertsV2ClientChannel(SmartAlertsV2Client.RetrieveServiceUrl(environment), authCertSubject);
			smartAlertsService.NewAlert(alert);
		}

		private static string RetrieveServiceUrl(string environment)
		{
			string result = "https://osa.officeppe.net/SmartAlertsService.svc";
			if (!string.IsNullOrEmpty(environment))
			{
				string key;
				switch (key = environment.ToLower())
				{
				case "prod":
				case "proddc":
				case "gallatin":
				case "gallatinprv":
					return "https://osa.office.net/SmartAlertsService.svc";
				}
				result = "https://osa.officeppe.net/SmartAlertsService.svc";
			}
			return result;
		}

		private static ISmartAlertsService InitializeSmartAlertsV2ClientChannel(string serviceUrl, string authCertSubject)
		{
			new EndpointAddress(serviceUrl);
			WebChannelFactory<ISmartAlertsService> webChannelFactory = new WebChannelFactory<ISmartAlertsService>(new WebHttpBinding(WebHttpSecurityMode.Transport)
			{
				Security = 
				{
					Transport = 
					{
						ClientCredentialType = HttpClientCredentialType.Certificate
					}
				},
				MaxReceivedMessageSize = 2147483647L,
				UseDefaultWebProxy = false
			}, new Uri(serviceUrl));
			webChannelFactory.Credentials.SupportInteractive = false;
			webChannelFactory.Credentials.ClientCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindBySubjectDistinguishedName, authCertSubject);
			return webChannelFactory.CreateChannel();
		}
	}
}
