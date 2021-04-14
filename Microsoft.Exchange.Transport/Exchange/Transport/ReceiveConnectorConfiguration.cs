using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.Transport
{
	internal class ReceiveConnectorConfiguration : IDiagnosable
	{
		protected ReceiveConnectorConfiguration(List<ReceiveConnector> connectors)
		{
			if (connectors == null)
			{
				throw new ArgumentNullException("connectors");
			}
			this.connectors = connectors;
		}

		public List<ReceiveConnector> Connectors
		{
			get
			{
				return this.connectors;
			}
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return "ReceiveConnectors";
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = new XElement("ReceiveConnectors", new XElement("count", this.connectors.Count));
			foreach (ReceiveConnector receiveConnector in this.connectors)
			{
				XElement xelement2 = new XElement("ReceiveConnector", new XElement("id", receiveConnector.propertyBag[ADObjectSchema.Id]));
				xelement.Add(xelement2);
				ProviderPropertyDefinition[] array = new ProviderPropertyDefinition[receiveConnector.propertyBag.Keys.Count];
				receiveConnector.propertyBag.Keys.CopyTo(array, 0);
				Array.Sort<ProviderPropertyDefinition>(array, (ProviderPropertyDefinition a, ProviderPropertyDefinition b) => a.Name.CompareTo(b.Name));
				foreach (ProviderPropertyDefinition providerPropertyDefinition in array)
				{
					object obj = receiveConnector.propertyBag[providerPropertyDefinition];
					XElement xelement3;
					if (obj != null && obj is MultiValuedPropertyBase)
					{
						xelement3 = new XElement(providerPropertyDefinition.Name);
						XElement xelement4 = new XElement("Values");
						xelement3.Add(xelement4);
						int num = 0;
						foreach (object content in ((IEnumerable)obj))
						{
							xelement4.Add(new XElement("value", content));
							num++;
						}
						xelement3.AddFirst(new XElement("count", num));
					}
					else
					{
						string expandedName = (providerPropertyDefinition.Name.Length > 1) ? (char.ToLower(providerPropertyDefinition.Name[0]) + providerPropertyDefinition.Name.Substring(1)) : providerPropertyDefinition.Name.ToLowerInvariant();
						string text = (obj == null) ? null : obj.ToString();
						if (string.IsNullOrEmpty(text))
						{
							text = null;
						}
						xelement3 = new XElement(expandedName, text);
					}
					xelement2.Add(xelement3);
				}
			}
			return xelement;
		}

		protected readonly List<ReceiveConnector> connectors;

		public class Builder : ConfigurationLoader<ReceiveConnectorConfiguration, ReceiveConnectorConfiguration.Builder>.SimpleBuilder<ReceiveConnector>
		{
			public TransportServerConfiguration Server
			{
				set
				{
					base.RootId = value.TransportServer.Id;
				}
			}

			protected override ReceiveConnectorConfiguration BuildCache(List<ReceiveConnector> connectors)
			{
				return new ReceiveConnectorConfiguration(connectors);
			}

			protected override ADOperationResult TryRegisterChangeNotification<TConfigObject>(Func<ADObjectId> rootIdGetter, out ADNotificationRequestCookie cookie)
			{
				return TransportADNotificationAdapter.TryRegisterNotifications(rootIdGetter, new ADNotificationCallback(base.Reload), new TransportADNotificationAdapter.TransportADNotificationRegister(TransportADNotificationAdapter.Instance.RegisterForLocalServerReceiveConnectorNotifications), 3, out cookie);
			}
		}
	}
}
