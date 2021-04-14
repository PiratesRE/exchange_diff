using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ServiceError
	{
		private void InitializeServiceError(string messageText, ResponseCodeType messageKey, int descriptiveLinkKey, ExchangeVersion currentExchangeVersion, ExchangeVersion effectiveVersion, bool stopsBatchProcessing)
		{
			if (!currentExchangeVersion.Supports(effectiveVersion))
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string, ExchangeVersion, ExchangeVersion>(0L, "ServiceError.InitializeServiceError: Error, '{0}', not supported by request version. Internal server error used.  Error effective version: '{1}' Request version: '{2}'.", messageKey.ToString(), effectiveVersion, currentExchangeVersion);
				this.innerError = new Dictionary<string, string>();
				this.innerError.Add(ServiceError.InnerErrorMessageTextKey, messageText);
				this.innerError.Add(ServiceError.InnerErrorResponseCodeKey, messageKey.ToString());
				this.innerError.Add(ServiceError.InnerErrorDescriptiveLinkKey, descriptiveLinkKey.ToString());
				messageKey = ServiceError.InternalServerError.ResponseCode;
				descriptiveLinkKey = 0;
				messageText = this.ConvertToString(ServiceError.InternalServerError.MessageText);
			}
			this.messageKey = messageKey;
			this.messageText = messageText;
			this.descriptiveLinkKey = descriptiveLinkKey;
			this.stopsBatchProcessing = stopsBatchProcessing;
		}

		private string ConvertToString(LocalizedString localizedString)
		{
			string result = localizedString;
			CallContext callContext = CallContext.Current;
			if (callContext != null)
			{
				result = localizedString.ToString(callContext.ServerCulture);
			}
			return result;
		}

		public ServiceError(string messageText, ResponseCodeType messageKey, int descriptiveLinkKey, ExchangeVersion effectiveVersion)
		{
			this.InitializeServiceError(messageText, messageKey, descriptiveLinkKey, ExchangeVersion.Current, effectiveVersion, false);
		}

		public ServiceError(string messageText, ResponseCodeType messageKey, int descriptiveLinkKey, ExchangeVersion effectiveVersion, bool stopsBatchProcessing)
		{
			this.InitializeServiceError(messageText, messageKey, descriptiveLinkKey, ExchangeVersion.Current, effectiveVersion, stopsBatchProcessing);
		}

		public ServiceError(Enum messageId, ResponseCodeType responseCode, int descriptiveLinkKey, ExchangeVersion exchangeVersion) : this(CoreResources.GetLocalizedString((CoreResources.IDs)messageId), responseCode, descriptiveLinkKey, exchangeVersion)
		{
		}

		public ServiceError(LocalizedException localizedException, LocalizedString localizedString, ResponseCodeType messageKey, int descriptiveLinkKey, ExchangeVersion currentExchangeVersion, ExchangeVersion effectiveVersion, bool stopBatchProcessing, PropertyPath[] propertyPaths, IDictionary<string, string> constantValues)
		{
			this.InitializeServiceError(this.ConvertToString(localizedString), messageKey, descriptiveLinkKey, currentExchangeVersion, effectiveVersion, stopBatchProcessing);
			this.propertyPaths = propertyPaths;
			this.constantValues = constantValues;
			this.LocalizedException = localizedException;
		}

		public ServiceError(Enum messageId, ResponseCodeType responseCode, int descriptiveLinkKey, ExchangeVersion exchangeVersion, IDictionary<string, string> constantValues) : this(CoreResources.GetLocalizedString((CoreResources.IDs)messageId), responseCode, descriptiveLinkKey, exchangeVersion)
		{
			this.constantValues = constantValues;
		}

		public static ServiceError CreateBatchProcessingStoppedError()
		{
			return new ServiceError(CoreResources.GetLocalizedString(CoreResources.IDs.ErrorBatchProcessingStopped), ResponseCodeType.ErrorBatchProcessingStopped, 0, ExchangeVersion.Exchange2007, true);
		}

		public string MessageText
		{
			get
			{
				return this.messageText;
			}
		}

		public ResponseCodeType MessageKey
		{
			get
			{
				return this.messageKey;
			}
		}

		internal LocalizedException LocalizedException { get; private set; }

		internal bool IsTransient { get; set; }

		internal void AddConstantValueProperty(string key, string value)
		{
			if (this.constantValues == null)
			{
				this.constantValues = new Dictionary<string, string>();
			}
			this.constantValues.Add(key, value);
		}

		public int DescriptiveLinkId
		{
			get
			{
				return this.descriptiveLinkKey;
			}
		}

		public bool StopsBatchProcessing
		{
			get
			{
				return this.stopsBatchProcessing;
			}
		}

		public override bool Equals(object obj)
		{
			ServiceError serviceError = obj as ServiceError;
			return serviceError != null && this.messageKey == serviceError.messageKey && this.messageText.Equals(serviceError.messageText);
		}

		public override int GetHashCode()
		{
			return this.messageKey.GetHashCode() ^ this.messageText.GetHashCode();
		}

		public ServiceError(LocalizedException localizedException, LocalizedString localizedString, ResponseCodeType messageKey, int descriptiveLinkKey, ExchangeVersion effectiveVersion, bool stopBatchProcessing, PropertyPath[] propertyPaths, IDictionary<string, string> constantValues, XmlNodeArray xmlNodeArray) : this(localizedException, localizedString, messageKey, descriptiveLinkKey, ExchangeVersion.Current, effectiveVersion, stopBatchProcessing, propertyPaths, constantValues)
		{
			this.xmlNodeArray = xmlNodeArray;
		}

		public ServiceError(LocalizedString localizedString, ResponseCodeType messageKey, int descriptiveLinkKey, ExchangeVersion currentExchangeVersion, ExchangeVersion effectiveVersion, bool stopsBatchProcessing, PropertyPath[] propertyPaths, IDictionary<string, string> constantValues, XmlNodeArray xmlNodeArray)
		{
			this.InitializeServiceError(this.ConvertToString(localizedString), messageKey, descriptiveLinkKey, currentExchangeVersion, effectiveVersion, stopsBatchProcessing);
			this.propertyPaths = propertyPaths;
			this.constantValues = constantValues;
			this.xmlNodeArray = xmlNodeArray;
		}

		public XmlNodeArray MessageXml
		{
			get
			{
				if ((this.propertyPaths == null || this.propertyPaths.Length == 0) && (this.constantValues == null || this.constantValues.Count == 0) && (this.innerError == null || this.innerError.Count == 0) && this.xmlNodeArray == null)
				{
					return null;
				}
				if (this.xmlNodeArray != null)
				{
					return this.xmlNodeArray;
				}
				XmlNodeArray result = new XmlNodeArray();
				SafeXmlDocument xmlDocument = new SafeXmlDocument();
				XmlElement messageXmlElement = ServiceXml.CreateElement(xmlDocument, ServiceError.MessageXmlElementName, "http://schemas.microsoft.com/exchange/services/2006/types");
				this.WriteConstantValueElements(this.innerError, result, messageXmlElement);
				this.WritePropertyPathElements(result, messageXmlElement);
				this.WriteConstantValueElements(this.constantValues, result, messageXmlElement);
				return result;
			}
		}

		private void WritePropertyPathElements(XmlNodeArray xmlNodeArray, XmlElement messageXmlElement)
		{
			if (this.propertyPaths != null)
			{
				foreach (PropertyPath propertyPath in this.propertyPaths)
				{
					if (propertyPath != null)
					{
						try
						{
							XmlElement item = propertyPath.ToXml(messageXmlElement);
							xmlNodeArray.Nodes.Add(item);
						}
						catch (InvalidExtendedPropertyException)
						{
						}
					}
				}
			}
		}

		private void WriteConstantValueElements(IDictionary<string, string> errorDetails, XmlNodeArray xmlNodeArray, XmlElement messageXmlElement)
		{
			if (errorDetails != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in errorDetails)
				{
					XmlElement xmlElement = ServiceXml.CreateTextElement(messageXmlElement, ServiceError.ConstantValueElementName, keyValuePair.Value, "http://schemas.microsoft.com/exchange/services/2006/types");
					ServiceXml.CreateAttribute(xmlElement, ServiceError.ConstantValueAttributeName, keyValuePair.Key);
					xmlNodeArray.Nodes.Add(xmlElement);
				}
			}
		}

		internal string GetAsXmlString()
		{
			XmlDocument xmlDocument;
			if (this.MessageXml != null)
			{
				xmlDocument = this.MessageXml.Nodes[0].OwnerDocument;
			}
			else
			{
				xmlDocument = new SafeXmlDocument();
			}
			XmlElement xmlElement = ServiceXml.CreateElement(xmlDocument, SchemaValidationException.DummyElementName, "http://schemas.microsoft.com/exchange/services/2006/errors");
			ServiceXml.CreateTextElement(xmlElement, ServiceError.ResponseCodeElementName, this.MessageKey.ToString());
			ServiceXml.CreateTextElement(xmlElement, ServiceError.MessageElementName, this.MessageText);
			if (this.MessageXml != null)
			{
				XmlElement xmlElement2 = ServiceXml.CreateElement(xmlElement, ServiceError.MessageXmlElementName, "http://schemas.microsoft.com/exchange/services/2006/types");
				foreach (XmlNode node in this.MessageXml.Nodes)
				{
					xmlElement2.AppendChild(xmlDocument.ImportNode(node, true));
				}
			}
			return xmlElement.InnerXml;
		}

		public const int DefaultDescriptiveLinkKey = 0;

		private static readonly string InnerErrorMessageTextKey = "InnerErrorMessageText";

		private static readonly string InnerErrorResponseCodeKey = "InnerErrorResponseCode";

		private static readonly string InnerErrorDescriptiveLinkKey = "InnerErrorDescriptiveLinkKey";

		private static readonly StaticExceptionMapping InternalServerError = new StaticExceptionMapping(typeof(LocalizedException), ResponseCodeType.ErrorInternalServerError, CoreResources.IDs.ErrorInternalServerError);

		private IDictionary<string, string> innerError;

		private PropertyPath[] propertyPaths;

		private IDictionary<string, string> constantValues;

		private string messageText;

		private int descriptiveLinkKey;

		private ResponseCodeType messageKey;

		private bool stopsBatchProcessing;

		private static readonly string ConstantValueElementName = "Value";

		private static readonly string ConstantValueAttributeName = "Name";

		private static readonly string MessageXmlElementName = "MessageXml";

		private static readonly string ResponseCodeElementName = "ResponseCode";

		private static readonly string MessageElementName = "Message";

		private XmlNodeArray xmlNodeArray;
	}
}
