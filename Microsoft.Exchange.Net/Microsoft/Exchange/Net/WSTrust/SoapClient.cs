using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net.WSTrust
{
	internal class SoapClient
	{
		public SoapClient(Uri endpoint, WebProxy webProxy)
		{
			this.httpClient = new XmlHttpClient(endpoint, webProxy);
		}

		public XmlElement Invoke(IEnumerable<XmlElement> headers, XmlElement bodyContent)
		{
			XmlDocument requestXmlDocument = this.CreateRequestXmlDocument(headers, bodyContent);
			SoapFaultException ex = null;
			for (int i = 0; i < 3; i++)
			{
				XmlDocument xmlDocument = this.httpClient.Invoke(requestXmlDocument);
				XmlElement xmlElement = this.ExtractBodyContentFromResponse(xmlDocument);
				if (!Soap.Fault.IsMatch(xmlElement))
				{
					return xmlElement;
				}
				ex = this.GetSoapFaultException(xmlElement);
				if (!SoapClient.IsReceiverFault(ex.Code))
				{
					break;
				}
			}
			throw ex;
		}

		public IAsyncResult BeginInvoke(IEnumerable<XmlElement> headers, XmlElement bodyContent, AsyncCallback callback, object state)
		{
			XmlDocument xmlDocument = this.CreateRequestXmlDocument(headers, bodyContent);
			return this.httpClient.BeginInvoke(xmlDocument, callback, state);
		}

		public XmlElement EndInvoke(IAsyncResult asyncResult)
		{
			XmlDocument xmlDocument = this.httpClient.EndInvoke(asyncResult);
			XmlElement xmlElement = this.ExtractBodyContentFromResponse(xmlDocument);
			if (Soap.Fault.IsMatch(xmlElement))
			{
				throw this.GetSoapFaultException(xmlElement);
			}
			return xmlElement;
		}

		public void AbortInvoke(IAsyncResult asyncResult)
		{
			this.httpClient.AbortInvoke(asyncResult);
		}

		private XmlDocument CreateRequestXmlDocument(IEnumerable<XmlElement> headers, XmlElement bodyContent)
		{
			XmlDocument xmlDocument = new SafeXmlDocument();
			xmlDocument.PreserveWhitespace = true;
			List<XmlElement> list = new List<XmlElement>();
			foreach (XmlElement node in headers)
			{
				list.Add((XmlElement)xmlDocument.ImportNode(node, true));
			}
			XmlElement bodyContent2 = (XmlElement)xmlDocument.ImportNode(bodyContent, true);
			XmlElement xmlElement = this.CreateSoapEnvelope(xmlDocument, list, bodyContent2);
			xmlDocument.AppendChild(xmlElement);
			XmlNamespaceDefinition.AddPrefixes(xmlDocument, xmlElement, new XmlNamespaceDefinition[]
			{
				Soap.Namespace,
				WSAddressing.Namespace,
				WSSecurityUtility.Namespace,
				WSSecurityExtensions.Namespace,
				WSTrust.Namespace,
				WSAuthorization.Namespace,
				WSPolicy.Namespace
			});
			return xmlDocument;
		}

		private XmlElement CreateSoapEnvelope(XmlDocument xmlDocument, IEnumerable<XmlElement> headers, XmlElement bodyContent)
		{
			XmlElement xmlElement = Soap.Header.CreateElement(xmlDocument, headers);
			XmlElement xmlElement2 = Soap.Body.CreateElement(xmlDocument, new XmlElement[]
			{
				bodyContent
			});
			return Soap.Envelope.CreateElement(xmlDocument, new XmlElement[]
			{
				xmlElement,
				xmlElement2
			});
		}

		private XmlElement ExtractBodyContentFromResponse(XmlDocument xmlDocument)
		{
			XmlElement requiredChildElement = this.GetRequiredChildElement(xmlDocument.DocumentElement, Soap.Body);
			return this.GetSingleChildElement(requiredChildElement);
		}

		private SoapFaultException GetSoapFaultException(XmlElement fault)
		{
			SoapClient.Tracer.TraceError<SoapClient, string>((long)this.GetHashCode(), "{0}: soap fault received: {1}", this, fault.OuterXml);
			XmlElement requiredChildElement = this.GetRequiredChildElement(fault, Soap.Code);
			XmlElement requiredChildElement2 = this.GetRequiredChildElement(requiredChildElement, Soap.Value);
			XmlElement singleElementByName = Soap.Subcode.GetSingleElementByName(requiredChildElement);
			XmlElement xmlElement = null;
			if (singleElementByName != null)
			{
				xmlElement = this.GetRequiredChildElement(singleElementByName, Soap.Value);
			}
			return new SoapFaultException(fault, requiredChildElement2.InnerText, (xmlElement != null) ? xmlElement.InnerText : null);
		}

		protected XmlElement GetRequiredChildElement(XmlElement xmlElement, XmlElementDefinition xmlElementDefinition)
		{
			XmlElement singleElementByName = xmlElementDefinition.GetSingleElementByName(xmlElement);
			if (singleElementByName == null)
			{
				SoapClient.Tracer.TraceError<SoapClient, XmlElementDefinition, string>((long)this.GetHashCode(), "{0}: failed to find XML element: {1} in this context {1}", this, xmlElementDefinition, xmlElement.OuterXml);
				throw new SoapXmlMalformedException(xmlElement, xmlElementDefinition);
			}
			return singleElementByName;
		}

		protected XmlElement GetOptionalChildElement(XmlElement xmlElement, XmlElementDefinition xmlElementDefinition)
		{
			return xmlElementDefinition.GetSingleElementByName(xmlElement);
		}

		protected XmlElement GetSingleChildElement(XmlElement xmlElement)
		{
			if (xmlElement.ChildNodes.Count != 1)
			{
				SoapClient.Tracer.TraceError<SoapClient, string>((long)this.GetHashCode(), "{0}: found none or more than one XML element when only one was expected in this XML segment: {1}", this, xmlElement.OuterXml);
				throw new SoapXmlMalformedException(xmlElement);
			}
			return (XmlElement)xmlElement.ChildNodes[0];
		}

		private static bool IsReceiverFault(string soapFaultCode)
		{
			string x = soapFaultCode;
			string[] array = soapFaultCode.Split(new char[]
			{
				':'
			});
			if (array.Length == 2)
			{
				x = array[1];
			}
			return StringComparer.OrdinalIgnoreCase.Equals(x, "receiver");
		}

		public override string ToString()
		{
			return "Soap client over " + this.httpClient.ToString();
		}

		private const string SoapFaultReceiver = "receiver";

		private const int RetriesForReceiverFault = 3;

		private XmlHttpClient httpClient;

		private static readonly Trace Tracer = ExTraceGlobals.WSTrustTracer;
	}
}
