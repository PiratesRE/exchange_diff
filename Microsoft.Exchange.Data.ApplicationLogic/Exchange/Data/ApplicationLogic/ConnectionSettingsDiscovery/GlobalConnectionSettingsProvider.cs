using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Connections.Imap;
using Microsoft.Exchange.Connections.Pop;
using Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery.Connections;
using Microsoft.Exchange.Data.ApplicationLogic.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.ConnectionSettingsDiscovery
{
	internal class GlobalConnectionSettingsProvider : IConnectionSettingsReadProvider, IConnectionSettingsWriteProvider
	{
		public GlobalConnectionSettingsProvider(ILogAdapter logAdapter)
		{
			if (logAdapter == null)
			{
				throw new ArgumentNullException("logAdapter", "The logAdapter argument cannot be null.");
			}
			this.log = logAdapter;
		}

		public string SourceId
		{
			get
			{
				return "Exchange.GlobalConnectionSettingsProvider";
			}
		}

		public IEnumerable<ConnectionSettings> GetConnectionSettingsMatchingEmail(SmtpAddress emailAddress)
		{
			if (!emailAddress.IsValidAddress)
			{
				throw new ArgumentException("The emailAddress argument must have a valid value.", "emailAddress");
			}
			List<ConnectionSettings> list = null;
			XDocument responseXml;
			if (this.TryGetConnectionSettingsXml(emailAddress, out responseXml))
			{
				try
				{
					list = this.ProcessResponseXml(emailAddress, responseXml);
					this.log.LogOperationResult(ConnectionSettingsDiscoveryMetadata.EssConnectionSettingsFound, emailAddress.Domain, list.Count > 0);
					return list;
				}
				catch (XmlException ex)
				{
					this.log.LogException(ex, "The response received from the Email Settings Service (for {0}) was malformed. Assuming no settings found.", new object[]
					{
						emailAddress
					});
					this.log.LogOperationException(ConnectionSettingsDiscoveryMetadata.EssException, ex);
					return list;
				}
			}
			list = new List<ConnectionSettings>();
			this.log.Trace("Email Settings Service did not find settings for: {0}.", new object[]
			{
				emailAddress.Domain
			});
			this.log.LogOperationResult(ConnectionSettingsDiscoveryMetadata.EssConnectionSettingsFound, emailAddress.Domain, false);
			return list;
		}

		public bool SetConnectionSettingsMatchingEmail(SmtpAddress emailAddress, ConnectionSettings connectionSettings)
		{
			if (!emailAddress.IsValidAddress)
			{
				throw new ArgumentException("The emailAddress argument must have a valid value.", "emailAddress");
			}
			return connectionSettings.IncomingConnectionSettings.LogonResult != OperationStatusCode.Success && false;
		}

		private bool TryGetConnectionSettingsXml(SmtpAddress emailAddress, out XDocument connectionSettingsXml)
		{
			XDocument responseData = null;
			this.log.ExecuteMonitoredOperation(ConnectionSettingsDiscoveryMetadata.RequestDataFromEss, delegate
			{
				try
				{
					responseData = GlobalConnectionSettingsProvider.requestConnectionSetting.Value(emailAddress);
				}
				catch (NotSupportedException ex)
				{
					responseData = null;
					this.log.LogException(ex, "Failed to communicate with Email Settings Service (for {0}). Assuming no settings were found.", new object[]
					{
						emailAddress
					});
					this.log.LogOperationException(ConnectionSettingsDiscoveryMetadata.EssException, ex);
				}
				catch (ProtocolViolationException ex2)
				{
					responseData = null;
					this.log.LogException(ex2, "Failed to communicate with Email Settings Service (for {0}). There is no response stream, assuming no settings were found.", new object[]
					{
						emailAddress
					});
					this.log.LogOperationException(ConnectionSettingsDiscoveryMetadata.EssException, ex2);
				}
				catch (ObjectDisposedException ex3)
				{
					responseData = null;
					this.log.LogException(ex3, "Failed to communicate with Email Settings Service (for {0}). The current response instance has been previously disposed, assuming no settings were found.", new object[]
					{
						emailAddress
					});
					this.log.LogOperationException(ConnectionSettingsDiscoveryMetadata.EssException, ex3);
				}
				catch (WebException ex4)
				{
					responseData = null;
					this.log.LogException(ex4, "Failed to communicate with Email Settings Service (for {0}). Assuming no settings were found.", new object[]
					{
						emailAddress
					});
					this.log.LogOperationException(ConnectionSettingsDiscoveryMetadata.EssException, ex4);
				}
				catch (XmlException ex5)
				{
					responseData = null;
					this.log.LogException(ex5, "Failed to create an XmlDocument from the response for {0}", new object[]
					{
						emailAddress
					});
					this.log.LogOperationException(ConnectionSettingsDiscoveryMetadata.EssException, ex5);
				}
			});
			connectionSettingsXml = responseData;
			return connectionSettingsXml != null;
		}

		protected static HttpWebRequest BuildRequest(SmtpAddress emailAddress)
		{
			string requestUriString = string.Format(CultureInfo.InvariantCulture, "https://{0}/configure_email?ver={1:00}&email={2}&langid=0x{3:X}&device={4}", new object[]
			{
				"ess.windowsphone.net",
				7,
				(string)emailAddress,
				CultureInfo.InvariantCulture.LCID,
				"exchange"
			});
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
			httpWebRequest.AllowAutoRedirect = true;
			httpWebRequest.KeepAlive = false;
			return httpWebRequest;
		}

		protected static XDocument GetEssResponseForRequest(HttpWebRequest request)
		{
			XDocument result;
			using (WebResponse response = request.GetResponse())
			{
				using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
				{
					if (streamReader.EndOfStream)
					{
						result = null;
					}
					else
					{
						result = XDocument.Load(streamReader);
					}
				}
			}
			return result;
		}

		private List<ConnectionSettings> ProcessResponseXml(SmtpAddress emailAddress, XDocument responseXml)
		{
			if (responseXml == null)
			{
				throw new ArgumentNullException("responseXml", "The responseXml argument cannot be null.");
			}
			List<ConnectionSettings> result = new List<ConnectionSettings>();
			this.log.ExecuteMonitoredOperation(ConnectionSettingsDiscoveryMetadata.ProcessEssResponse, delegate
			{
				XElement xelement = responseXml.Element("isp-collection");
				if (xelement == null)
				{
					throw new XmlException("Could not find the root node: <isp-collection>. The response from the Email Settings Service was malformed");
				}
				foreach (XElement settings in xelement.Elements("isp"))
				{
					ConnectionSettings connectionSettings = this.ProcessSettingsXml(settings);
					this.log.Trace("Email Settings Service found settings for {0}: {1}", new object[]
					{
						emailAddress.Domain,
						connectionSettings.ToString()
					});
					result.Add(connectionSettings);
				}
			});
			return result;
		}

		private ConnectionSettings ProcessSettingsXml(XElement settings)
		{
			if (settings == null)
			{
				throw new ArgumentNullException("settings", "The settings argument cannot be null.");
			}
			ConnectionSettingsType attributeValue = GlobalConnectionSettingsProvider.GetAttributeValue<ConnectionSettingsType>(settings, "transport", new Func<string, string, ConnectionSettingsType>(GlobalConnectionSettingsProvider.TransportAttributeToSettingsType));
			ConnectionSettingsType connectionSettingsType = attributeValue;
			if (connectionSettingsType == ConnectionSettingsType.Imap)
			{
				return new ConnectionSettings(this, this.BuildImapConnectionSettings(settings), this.BuildSmtpConnectionSettings(settings));
			}
			if (connectionSettingsType != ConnectionSettingsType.Pop)
			{
				throw new XmlException(string.Format(CultureInfo.InvariantCulture, "Invalid response received from ESS service: The attribute [transport] has an unexpected value: {0}.", new object[]
				{
					attributeValue
				}));
			}
			return new ConnectionSettings(this, this.BuildPopConnectionSettings(settings), this.BuildSmtpConnectionSettings(settings));
		}

		private ImapConnectionSettings BuildImapConnectionSettings(XElement settings)
		{
			Fqdn attributeValue = GlobalConnectionSettingsProvider.GetAttributeValue<Fqdn>(settings, "in-serv", new Func<string, string, Fqdn>(GlobalConnectionSettingsProvider.ServerAttributeToFqdn));
			int attributeValue2 = GlobalConnectionSettingsProvider.GetAttributeValue<int>(settings, "in-port", new Func<string, string, int>(GlobalConnectionSettingsProvider.ServerAttributeToInteger));
			ImapSecurityMechanism attributeValue3 = GlobalConnectionSettingsProvider.GetAttributeValue<ImapSecurityMechanism>(settings, "in-ssl", delegate(string attrName, string val)
			{
				if (!val.Equals("1", StringComparison.Ordinal))
				{
					return ImapSecurityMechanism.None;
				}
				return ImapSecurityMechanism.Ssl;
			});
			return new ImapConnectionSettings(attributeValue, attributeValue2, ImapAuthenticationMechanism.Basic, attributeValue3);
		}

		private PopConnectionSettings BuildPopConnectionSettings(XElement settings)
		{
			Fqdn attributeValue = GlobalConnectionSettingsProvider.GetAttributeValue<Fqdn>(settings, "in-serv", new Func<string, string, Fqdn>(GlobalConnectionSettingsProvider.ServerAttributeToFqdn));
			int attributeValue2 = GlobalConnectionSettingsProvider.GetAttributeValue<int>(settings, "in-port", new Func<string, string, int>(GlobalConnectionSettingsProvider.ServerAttributeToInteger));
			Pop3SecurityMechanism attributeValue3 = GlobalConnectionSettingsProvider.GetAttributeValue<Pop3SecurityMechanism>(settings, "in-ssl", delegate(string attrName, string val)
			{
				if (!val.Equals("1", StringComparison.Ordinal))
				{
					return Pop3SecurityMechanism.None;
				}
				return Pop3SecurityMechanism.Ssl;
			});
			return new PopConnectionSettings(attributeValue, attributeValue2, Pop3AuthenticationMechanism.Basic, attributeValue3);
		}

		private SmtpConnectionSettings BuildSmtpConnectionSettings(XElement settings)
		{
			Fqdn attributeValue = GlobalConnectionSettingsProvider.GetAttributeValue<Fqdn>(settings, "out-serv", new Func<string, string, Fqdn>(GlobalConnectionSettingsProvider.ServerAttributeToFqdn));
			int attributeValue2 = GlobalConnectionSettingsProvider.GetAttributeValue<int>(settings, "out-port", new Func<string, string, int>(GlobalConnectionSettingsProvider.ServerAttributeToInteger));
			return new SmtpConnectionSettings(attributeValue, attributeValue2);
		}

		private static T GetAttributeValue<T>(XElement element, string attributeName, Func<string, string, T> convertStringValue)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element", "The element argument cannot be null.");
			}
			if (string.IsNullOrWhiteSpace(attributeName))
			{
				throw new ArgumentException("The attributeName argument cannot be null or empty string.", "attributeName");
			}
			if (convertStringValue == null)
			{
				throw new ArgumentNullException("convertStringValue", "The convertStringValue argument cannot be null.");
			}
			XAttribute xattribute = element.Attribute(attributeName);
			if (xattribute == null)
			{
				throw new XmlException(string.Format(CultureInfo.InvariantCulture, "Invalid response received from ESS service: The attribute [{0}] is missing from [{1}] node.", new object[]
				{
					attributeName,
					element.Name
				}));
			}
			return convertStringValue(attributeName, xattribute.Value);
		}

		private static ConnectionSettingsType TransportAttributeToSettingsType(string attributeName, string attributeValue)
		{
			if (attributeValue != null)
			{
				if (attributeValue == "0")
				{
					return ConnectionSettingsType.Pop;
				}
				if (attributeValue == "1")
				{
					return ConnectionSettingsType.Imap;
				}
			}
			throw new XmlException(string.Format(CultureInfo.InvariantCulture, "Invalid response received from ESS service: The attribute [{0}] has an unhandled value: {1}.", new object[]
			{
				attributeName,
				attributeValue
			}));
		}

		private static Fqdn ServerAttributeToFqdn(string attributeName, string attributeValue)
		{
			Fqdn result;
			if (!Fqdn.TryParse(attributeValue, out result))
			{
				throw new XmlException(string.Format(CultureInfo.InvariantCulture, "Invalid response received from ESS service: The attribute [{0}] has an unhandled value: {1}.", new object[]
				{
					attributeName,
					attributeValue
				}));
			}
			return result;
		}

		private static int ServerAttributeToInteger(string attributeName, string attributeValue)
		{
			int result;
			if (!int.TryParse(attributeValue, out result))
			{
				throw new XmlException(string.Format(CultureInfo.InvariantCulture, "Invalid response received from ESS service: The attribute [{0}] has an unhandled value: {1}.", new object[]
				{
					attributeName,
					attributeValue
				}));
			}
			return result;
		}

		private const int EssServiceVersion = 7;

		private const string DeviceType = "exchange";

		private const string EssServerName = "ess.windowsphone.net";

		private const string EssRequestUrlTemplate = "https://{0}/configure_email?ver={1:00}&email={2}&langid=0x{3:X}&device={4}";

		private const string IncomingServerNameAttribute = "in-serv";

		private const string IncomingPortAttribute = "in-port";

		private const string IncomingSecurityAttribute = "in-ssl";

		private const string OutgoingServerNameAttribute = "out-serv";

		private const string OutgoingPortAttribute = "out-port";

		private readonly ILogAdapter log;

		internal static readonly Hookable<Func<SmtpAddress, XDocument>> requestConnectionSetting = Hookable<Func<SmtpAddress, XDocument>>.Create(true, delegate(SmtpAddress emailAddress)
		{
			HttpWebRequest request = GlobalConnectionSettingsProvider.BuildRequest(emailAddress);
			return GlobalConnectionSettingsProvider.GetEssResponseForRequest(request);
		});
	}
}
