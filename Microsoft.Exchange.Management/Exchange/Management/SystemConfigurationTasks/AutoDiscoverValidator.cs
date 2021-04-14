using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal sealed class AutoDiscoverValidator : ServiceValidatorBase
	{
		public AutoDiscoverValidator(string uri, NetworkCredential credentials, string emailAddress) : base(uri, credentials)
		{
			if (string.IsNullOrEmpty(emailAddress))
			{
				throw new ArgumentNullException("emailAddress");
			}
			this.EmailAddress = emailAddress;
		}

		public AutoDiscoverValidator.ProviderSchema Provider { get; set; }

		public string EmailAddress { get; private set; }

		public string EwsUrl { get; private set; }

		public string OabUrl { get; private set; }

		protected override string Name
		{
			get
			{
				return Strings.ServiceNameAutoDiscover;
			}
		}

		protected override void PreCreateRequest()
		{
			base.PreCreateRequest();
			this.OabUrl = null;
			this.EwsUrl = null;
		}

		protected override void FillRequestProperties(HttpWebRequest request)
		{
			base.FillRequestProperties(request);
			request.ContentType = "text/xml; charset=utf-8";
			request.Method = "POST";
		}

		protected override bool FillRequestStream(Stream requestStream)
		{
			AutoDiscoverValidator.ProviderStrategy.GetStrategy(this.Provider).GenerateRequest(base.Uri, this.EmailAddress, requestStream);
			return true;
		}

		protected override Exception ValidateResponse(Stream responseStream)
		{
			string ewsUrl;
			string oabUrl;
			Exception ex = AutoDiscoverValidator.ProviderStrategy.GetStrategy(this.Provider).ValidateResponse(responseStream, out ewsUrl, out oabUrl);
			this.EwsUrl = ewsUrl;
			this.OabUrl = oabUrl;
			if (ex != null)
			{
				return ex;
			}
			return ex;
		}

		public enum ProviderSchema
		{
			Outlook,
			Soap
		}

		private abstract class ProviderStrategy
		{
			public static AutoDiscoverValidator.ProviderStrategy GetStrategy(AutoDiscoverValidator.ProviderSchema schema)
			{
				return AutoDiscoverValidator.ProviderStrategy.factory[schema]();
			}

			public abstract void GenerateRequest(string url, string emailAddress, Stream requestStream);

			public abstract Exception ValidateResponse(Stream responseStream, out string ewsUrl, out string oabUrl);

			// Note: this type is marked as 'beforefieldinit'.
			static ProviderStrategy()
			{
				Dictionary<AutoDiscoverValidator.ProviderSchema, Func<AutoDiscoverValidator.ProviderStrategy>> dictionary = new Dictionary<AutoDiscoverValidator.ProviderSchema, Func<AutoDiscoverValidator.ProviderStrategy>>();
				dictionary.Add(AutoDiscoverValidator.ProviderSchema.Outlook, () => new AutoDiscoverValidator.OutlookStrategy());
				dictionary.Add(AutoDiscoverValidator.ProviderSchema.Soap, () => new AutoDiscoverValidator.SoapStrategy());
				AutoDiscoverValidator.ProviderStrategy.factory = dictionary;
			}

			private static Dictionary<AutoDiscoverValidator.ProviderSchema, Func<AutoDiscoverValidator.ProviderStrategy>> factory;
		}

		private class OutlookStrategy : AutoDiscoverValidator.ProviderStrategy
		{
			public override void GenerateRequest(string url, string emailAddress, Stream requestStream)
			{
				SafeXmlSerializer safeXmlSerializer = new SafeXmlSerializer(typeof(AutoDiscoverRequestXML));
				AutoDiscoverRequestXML o = AutoDiscoverRequestXML.NewRequest(emailAddress);
				safeXmlSerializer.Serialize(requestStream, o);
			}

			public override Exception ValidateResponse(Stream responseStream, out string ewsUrl, out string oabUrl)
			{
				ewsUrl = null;
				oabUrl = null;
				SafeXmlSerializer safeXmlSerializer = new SafeXmlSerializer(typeof(AutoDiscoverResponseXML));
				AutoDiscoverResponseXML autoDiscoverResponseXML = null;
				XDocument xdocument = null;
				try
				{
					autoDiscoverResponseXML = (AutoDiscoverResponseXML)safeXmlSerializer.Deserialize(responseStream);
					responseStream.Seek(0L, SeekOrigin.Begin);
					using (StreamReader streamReader = new StreamReader(responseStream))
					{
						xdocument = XDocument.Load(streamReader);
					}
				}
				catch (XmlException innerException)
				{
					return new ServiceValidatorException(Strings.ErrorInvalidResponseFormat, innerException);
				}
				if (autoDiscoverResponseXML.ErrorResponse != null)
				{
					if (autoDiscoverResponseXML.ErrorResponse.Error == null)
					{
						return new ServiceValidatorException(Strings.ErrorInvalidResponseXml(xdocument.ToString()));
					}
					return new ServiceValidatorException(Strings.ErrorResponseContainsError(autoDiscoverResponseXML.ErrorResponse.Error.ErrorCode, autoDiscoverResponseXML.ErrorResponse.Error.Message));
				}
				else
				{
					if (autoDiscoverResponseXML.DataResponse == null || autoDiscoverResponseXML.DataResponse.Account == null || autoDiscoverResponseXML.DataResponse.Account.Action == null)
					{
						return new ServiceValidatorException(Strings.ErrorInvalidResponseXml(xdocument.ToString()));
					}
					if (autoDiscoverResponseXML.DataResponse.Account.Action.Equals("redirectAddr", StringComparison.OrdinalIgnoreCase))
					{
						string redirectAddr = autoDiscoverResponseXML.DataResponse.Account.RedirectAddr;
						return new ServiceValidatorException(Strings.ErrorAutoDiscoverValidatorRequiresRedirection(redirectAddr));
					}
					if (autoDiscoverResponseXML.DataResponse.Account.Protocol == null)
					{
						return new ServiceValidatorException(Strings.ErrorInvalidResponseXml(xdocument.ToString()));
					}
					AutoDiscoverProtocol[] protocol = autoDiscoverResponseXML.DataResponse.Account.Protocol;
					string[] array = new string[]
					{
						"EXCH",
						"EXPR"
					};
					for (int i = 0; i < array.Length; i++)
					{
						string protocolType = array[i];
						AutoDiscoverProtocol autoDiscoverProtocol = Array.Find<AutoDiscoverProtocol>(protocol, (AutoDiscoverProtocol p) => p.Type.Equals(protocolType, StringComparison.OrdinalIgnoreCase));
						if (autoDiscoverProtocol != null)
						{
							if (!string.IsNullOrEmpty(autoDiscoverProtocol.EwsUrl))
							{
								ewsUrl = autoDiscoverProtocol.EwsUrl;
							}
							if (!string.IsNullOrEmpty(autoDiscoverProtocol.OABUrl))
							{
								oabUrl = autoDiscoverProtocol.OABUrl;
							}
						}
					}
					if (string.IsNullOrEmpty(ewsUrl))
					{
						return new ServiceValidatorException(Strings.ErrorAutoDiscoverValidatorEwsNotFound(xdocument.ToString()));
					}
					if (string.IsNullOrEmpty(oabUrl))
					{
						return new ServiceValidatorException(Strings.ErrorAutoDiscoverValidatorOabNotFound(xdocument.ToString()));
					}
					return null;
				}
				Exception result;
				return result;
			}
		}

		private class SoapStrategy : AutoDiscoverValidator.ProviderStrategy
		{
			static SoapStrategy()
			{
				AutoDiscoverValidator.SoapStrategy.namespaceManager.AddNamespace("a", AutoDiscoverValidator.SoapStrategy.a.NamespaceName);
				AutoDiscoverValidator.SoapStrategy.namespaceManager.AddNamespace("wsa", AutoDiscoverValidator.SoapStrategy.wsa.NamespaceName);
				AutoDiscoverValidator.SoapStrategy.namespaceManager.AddNamespace("xsi", AutoDiscoverValidator.SoapStrategy.xsi.NamespaceName);
				AutoDiscoverValidator.SoapStrategy.namespaceManager.AddNamespace("soap", AutoDiscoverValidator.SoapStrategy.soap.NamespaceName);
			}

			public override void GenerateRequest(string url, string emailAddress, Stream requestStream)
			{
				XDocument xdocument = new XDocument(AutoDiscoverValidator.SoapStrategy.RequestTemplate);
				XElement xelement = xdocument.XPathSelectElement(".//soap:Header/wsa:To", AutoDiscoverValidator.SoapStrategy.namespaceManager);
				xelement.Value = url;
				XElement xelement2 = xdocument.XPathSelectElement(".//soap:Body/a:GetUserSettingsRequestMessage/a:Request/a:Users/a:User/a:Mailbox", AutoDiscoverValidator.SoapStrategy.namespaceManager);
				xelement2.Value = emailAddress;
				using (StreamWriter streamWriter = new StreamWriter(requestStream))
				{
					streamWriter.Write(xdocument.Declaration.ToString() + "\r\n" + xdocument.ToString());
				}
			}

			public override Exception ValidateResponse(Stream responseStream, out string ewsUrl, out string oabUrl)
			{
				ewsUrl = null;
				oabUrl = null;
				XDocument xdocument = null;
				try
				{
					using (StreamReader streamReader = new StreamReader(responseStream))
					{
						xdocument = XDocument.Load(streamReader);
					}
				}
				catch (XmlException innerException)
				{
					return new ServiceValidatorException(Strings.ErrorInvalidResponseFormat, innerException);
				}
				Exception ex = this.CheckResponseErrors(xdocument);
				if (ex != null)
				{
					return ex;
				}
				XElement xelement = xdocument.XPathSelectElement(".//soap:Body/a:GetUserSettingsResponseMessage/a:Response/a:UserResponses/a:UserResponse/a:UserSettings/a:UserSetting[./a:Name=\"ExternalEwsUrl\"]", AutoDiscoverValidator.SoapStrategy.namespaceManager);
				if (xelement != null)
				{
					XElement xelement2 = xelement.XPathSelectElement("./a:Value", AutoDiscoverValidator.SoapStrategy.namespaceManager);
					if (xelement2 != null)
					{
						ewsUrl = xelement2.Value;
					}
				}
				if (string.IsNullOrEmpty(ewsUrl))
				{
					XElement xelement3 = xdocument.XPathSelectElement(".//soap:Body/a:GetUserSettingsResponseMessage/a:Response/a:UserResponses/a:UserResponse/a:UserSettings/a:UserSetting[./a:Name=\"InternalEwsUrl\"]", AutoDiscoverValidator.SoapStrategy.namespaceManager);
					if (xelement3 != null)
					{
						XElement xelement4 = xelement3.XPathSelectElement("./a:Value", AutoDiscoverValidator.SoapStrategy.namespaceManager);
						if (xelement4 != null)
						{
							ewsUrl = xelement4.Value;
						}
					}
				}
				if (string.IsNullOrEmpty(ewsUrl))
				{
					return new ServiceValidatorException(Strings.ErrorAutoDiscoverValidatorEwsNotFound(xdocument.ToString()));
				}
				return null;
			}

			private Exception CheckResponseErrors(XDocument responseXml)
			{
				XElement xelement = responseXml.XPathSelectElement(".//soap:Body/a:GetUserSettingsResponseMessage/a:Response/a:ErrorCode", AutoDiscoverValidator.SoapStrategy.namespaceManager);
				if (xelement == null)
				{
					return new ServiceValidatorException(Strings.ErrorInvalidResponseXml(responseXml.ToString()));
				}
				if (!string.Equals(xelement.Value, "NoError", StringComparison.OrdinalIgnoreCase))
				{
					XElement xelement2 = responseXml.XPathSelectElement(".//soap:Body/a:GetUserSettingsResponseMessage/a:Response/a:ErrorMessage", AutoDiscoverValidator.SoapStrategy.namespaceManager);
					if (xelement2 == null)
					{
						return new ServiceValidatorException(Strings.ErrorInvalidResponseXml(responseXml.ToString()));
					}
					return new ServiceValidatorException(Strings.ErrorResponseContainsError(xelement.Value, xelement2.Value));
				}
				else
				{
					XElement xelement3 = responseXml.XPathSelectElement(".//soap:Body/a:GetUserSettingsResponseMessage/a:Response/a:UserResponses/a:UserResponse/a:ErrorCode", AutoDiscoverValidator.SoapStrategy.namespaceManager);
					if (xelement3 == null)
					{
						return new ServiceValidatorException(Strings.ErrorInvalidResponseXml(responseXml.ToString()));
					}
					if (string.Equals(xelement3.Value, "NoError", StringComparison.OrdinalIgnoreCase))
					{
						return null;
					}
					XElement xelement4 = responseXml.XPathSelectElement(".//soap:Body/GetUserSettingsResponseMessage/a:Response/a:UserResponses/a:UserResponse/a:ErrorMessage", AutoDiscoverValidator.SoapStrategy.namespaceManager);
					if (xelement4 == null)
					{
						return new ServiceValidatorException(Strings.ErrorInvalidResponseXml(responseXml.ToString()));
					}
					return new ServiceValidatorException(Strings.ErrorResponseContainsError(xelement3.Value, xelement4.Value));
				}
			}

			private static readonly XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

			private static readonly XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";

			private static readonly XNamespace wsa = "http://www.w3.org/2005/08/addressing";

			private static readonly XNamespace a = "http://schemas.microsoft.com/exchange/2010/Autodiscover";

			private static readonly XmlNamespaceManager namespaceManager = new XmlNamespaceManager(new NameTable());

			private static readonly XDocument RequestTemplate = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new object[]
			{
				new XElement(AutoDiscoverValidator.SoapStrategy.soap + "Envelope", new object[]
				{
					new XAttribute(XNamespace.Xmlns + "a", AutoDiscoverValidator.SoapStrategy.a.NamespaceName),
					new XAttribute(XNamespace.Xmlns + "wsa", AutoDiscoverValidator.SoapStrategy.wsa.NamespaceName),
					new XAttribute(XNamespace.Xmlns + "xsi", AutoDiscoverValidator.SoapStrategy.xsi.NamespaceName),
					new XAttribute(XNamespace.Xmlns + "soap", AutoDiscoverValidator.SoapStrategy.soap.NamespaceName),
					new XElement(AutoDiscoverValidator.SoapStrategy.soap + "Header", new object[]
					{
						new XElement(AutoDiscoverValidator.SoapStrategy.a + "RequestedServerVersion", "Exchange2010"),
						new XElement(AutoDiscoverValidator.SoapStrategy.wsa + "Action", "http://schemas.microsoft.com/exchange/2010/Autodiscover/Autodiscover/GetUserSettings"),
						new XElement(AutoDiscoverValidator.SoapStrategy.wsa + "To", "url-placeholder")
					}),
					new XElement(AutoDiscoverValidator.SoapStrategy.soap + "Body", new XElement(AutoDiscoverValidator.SoapStrategy.a + "GetUserSettingsRequestMessage", new object[]
					{
						new XAttribute(XNamespace.Xmlns + "a", AutoDiscoverValidator.SoapStrategy.a.NamespaceName),
						new XElement(AutoDiscoverValidator.SoapStrategy.a + "Request", new object[]
						{
							new XElement(AutoDiscoverValidator.SoapStrategy.a + "Users", new XElement(AutoDiscoverValidator.SoapStrategy.a + "User", new XElement(AutoDiscoverValidator.SoapStrategy.a + "Mailbox", "email-address-placeholder"))),
							new XElement(AutoDiscoverValidator.SoapStrategy.a + "RequestedSettings", new object[]
							{
								new XElement(AutoDiscoverValidator.SoapStrategy.a + "Setting", "ExternalEwsUrl"),
								new XElement(AutoDiscoverValidator.SoapStrategy.a + "Setting", "InternalEwsUrl")
							})
						})
					}))
				})
			});
		}
	}
}
