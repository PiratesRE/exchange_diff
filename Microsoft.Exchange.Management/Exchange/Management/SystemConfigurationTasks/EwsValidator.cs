using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class EwsValidator : ServiceValidatorBase
	{
		public Guid PublicFolderMailboxGuid { get; set; }

		public EwsValidator(string uri, NetworkCredential credentials) : base(uri, credentials)
		{
		}

		public EwsValidator(string uri, NetworkCredential credentials, string emailAddress) : base(uri, credentials)
		{
			if (string.IsNullOrEmpty(emailAddress))
			{
				throw new ArgumentNullException("emailAddress");
			}
			this.EmailAddress = emailAddress;
		}

		public EwsValidator.RequestOperation Operation { get; set; }

		protected override string Name
		{
			get
			{
				return (this.Operation == EwsValidator.RequestOperation.GetUserAvailability) ? Strings.ServiceNameAs : Strings.ServiceNameEws;
			}
		}

		public string EmailAddress { get; set; }

		protected override void FillRequestProperties(HttpWebRequest request)
		{
			base.FillRequestProperties(request);
			request.Method = "POST";
			request.Accept = "text/xml";
			if (this.PublicFolderMailboxGuid != Guid.Empty)
			{
				request.Headers[WellKnownHeader.PublicFolderMailbox] = this.PublicFolderMailboxGuid.ToString();
			}
		}

		protected override bool FillRequestStream(Stream requestStream)
		{
			EwsValidator.OperationStrategy.GetStrategy(this.Operation).GenerateRequest(requestStream, this.EmailAddress, this.PublicFolderMailboxGuid != Guid.Empty);
			return true;
		}

		protected override Exception ValidateResponse(Stream responseStream)
		{
			return EwsValidator.OperationStrategy.GetStrategy(this.Operation).ValidateResponse(responseStream);
		}

		public enum RequestOperation
		{
			ConvertId,
			GetFolder,
			GetUserAvailability
		}

		private abstract class OperationStrategy
		{
			static OperationStrategy()
			{
				Dictionary<EwsValidator.RequestOperation, Func<EwsValidator.OperationStrategy>> dictionary = new Dictionary<EwsValidator.RequestOperation, Func<EwsValidator.OperationStrategy>>();
				dictionary.Add(EwsValidator.RequestOperation.ConvertId, () => new EwsValidator.ConvertIdStrategy());
				dictionary.Add(EwsValidator.RequestOperation.GetFolder, () => new EwsValidator.GetFolderStrategy());
				dictionary.Add(EwsValidator.RequestOperation.GetUserAvailability, () => new EwsValidator.GetUserAvailabilityStrategy());
				EwsValidator.OperationStrategy.factory = dictionary;
				EwsValidator.OperationStrategy.namespaceManager.AddNamespace("xsi", EwsValidator.OperationStrategy.xsi.NamespaceName);
				EwsValidator.OperationStrategy.namespaceManager.AddNamespace("soap", EwsValidator.OperationStrategy.soap.NamespaceName);
				EwsValidator.OperationStrategy.namespaceManager.AddNamespace("t", EwsValidator.OperationStrategy.t.NamespaceName);
				EwsValidator.OperationStrategy.namespaceManager.AddNamespace("m", EwsValidator.OperationStrategy.m.NamespaceName);
			}

			public static EwsValidator.OperationStrategy GetStrategy(EwsValidator.RequestOperation operation)
			{
				return EwsValidator.OperationStrategy.factory[operation]();
			}

			public void GenerateRequest(Stream requestStream, string emailAddress, bool isPublicFolderMailbox)
			{
				using (StreamWriter streamWriter = new StreamWriter(requestStream))
				{
					XDocument requestXml = this.GetRequestXml(emailAddress, isPublicFolderMailbox);
					streamWriter.Write(requestXml.Declaration.ToString() + "\r\n" + requestXml.ToString());
				}
			}

			public Exception ValidateResponse(Stream responseStream)
			{
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
				XElement xelement = xdocument.XPathSelectElement(this.ResponseMessageElementPath, EwsValidator.OperationStrategy.namespaceManager);
				if (xelement == null || xelement.FirstAttribute == null || xelement.FirstAttribute.Name == null || !string.Equals(xelement.FirstAttribute.Name.LocalName, "ResponseClass"))
				{
					return new ServiceValidatorException(Strings.ErrorInvalidResponseXml(xdocument.ToString()));
				}
				XElement xelement2 = xelement.XPathSelectElement(".//m:ResponseCode", EwsValidator.OperationStrategy.namespaceManager);
				if (xelement2 == null)
				{
					return new ServiceValidatorException(Strings.ErrorInvalidResponseXml(xdocument.ToString()));
				}
				string value = xelement.FirstAttribute.Value;
				if (!value.Equals("Error"))
				{
					return null;
				}
				XElement xelement3 = xelement.XPathSelectElement(".//m:MessageText", EwsValidator.OperationStrategy.namespaceManager);
				if (xelement3 == null)
				{
					return new ServiceValidatorException(Strings.ErrorInvalidResponseXml(xdocument.ToString()));
				}
				return new ServiceValidatorException(Strings.ErrorResponseContainsError(xelement2.Value, xelement3.Value));
			}

			protected abstract XDocument GetRequestXml(string emailAddress, bool isPublicFolderMailbox);

			protected abstract string ResponseMessageElementPath { get; }

			protected static readonly XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

			protected static readonly XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";

			protected static readonly XNamespace t = "http://schemas.microsoft.com/exchange/services/2006/types";

			protected static readonly XNamespace m = "http://schemas.microsoft.com/exchange/services/2006/messages";

			protected static readonly XmlNamespaceManager namespaceManager = new XmlNamespaceManager(new NameTable());

			private static Dictionary<EwsValidator.RequestOperation, Func<EwsValidator.OperationStrategy>> factory;
		}

		private class ConvertIdStrategy : EwsValidator.OperationStrategy
		{
			protected override XDocument GetRequestXml(string emailAddress, bool isPublicFolderMailbox)
			{
				return EwsValidator.ConvertIdStrategy.RequestTemplate;
			}

			protected override string ResponseMessageElementPath
			{
				get
				{
					return ".//soap:Body/m:ConvertIdResponse/m:ResponseMessages/m:ConvertIdResponseMessage";
				}
			}

			private static readonly XDocument RequestTemplate = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new object[]
			{
				new XElement(EwsValidator.OperationStrategy.soap + "Envelope", new object[]
				{
					new XAttribute(XNamespace.Xmlns + "xsi", EwsValidator.OperationStrategy.xsi.NamespaceName),
					new XAttribute(XNamespace.Xmlns + "m", EwsValidator.OperationStrategy.m.NamespaceName),
					new XAttribute(XNamespace.Xmlns + "t", EwsValidator.OperationStrategy.t.NamespaceName),
					new XAttribute(XNamespace.Xmlns + "soap", EwsValidator.OperationStrategy.soap.NamespaceName),
					new XElement(EwsValidator.OperationStrategy.soap + "Header", new XElement(EwsValidator.OperationStrategy.t + "RequestServerVersion", new XAttribute("Version", "Exchange2007_SP1"))),
					new XElement(EwsValidator.OperationStrategy.soap + "Body", new XElement(EwsValidator.OperationStrategy.m + "ConvertId", new object[]
					{
						new XAttribute("DestinationFormat", "OwaId"),
						new XElement(EwsValidator.OperationStrategy.m + "SourceIds", new XElement(EwsValidator.OperationStrategy.t + "AlternateId", new object[]
						{
							new XAttribute("Format", "EntryId"),
							new XAttribute("Id", "AAAAAJrt68tcvoJHj4EZX4tVxJkHAJkQvNaLMepFoct50Pnbm4gAAAHZbQQAAF1I2v4S5IFMi6vJDIXLpOEAAAAI6ocAAA=="),
							new XAttribute("Mailbox", "nobody@contoso.com")
						}))
					}))
				})
			});
		}

		private class GetFolderStrategy : EwsValidator.OperationStrategy
		{
			private XDocument GetRequestTemplate(bool isPublicFolderMailbox)
			{
				XElement content;
				if (isPublicFolderMailbox)
				{
					content = new XElement(EwsValidator.OperationStrategy.t + "DistinguishedFolderId", new XAttribute("Id", "publicfoldersroot"));
				}
				else
				{
					content = new XElement(EwsValidator.OperationStrategy.t + "DistinguishedFolderId", new object[]
					{
						new XElement(EwsValidator.OperationStrategy.t + "Mailbox", new XElement(EwsValidator.OperationStrategy.t + "EmailAddress", "email-address-placeholder")),
						new XAttribute("Id", "inbox")
					});
				}
				return new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new object[]
				{
					new XElement(EwsValidator.OperationStrategy.soap + "Envelope", new object[]
					{
						new XAttribute(XNamespace.Xmlns + "xsi", EwsValidator.OperationStrategy.xsi.NamespaceName),
						new XAttribute(XNamespace.Xmlns + "m", EwsValidator.OperationStrategy.m.NamespaceName),
						new XAttribute(XNamespace.Xmlns + "t", EwsValidator.OperationStrategy.t.NamespaceName),
						new XAttribute(XNamespace.Xmlns + "soap", EwsValidator.OperationStrategy.soap.NamespaceName),
						new XElement(EwsValidator.OperationStrategy.soap + "Header", new XElement(EwsValidator.OperationStrategy.t + "RequestServerVersion", new XAttribute("Version", "Exchange2007_SP1"))),
						new XElement(EwsValidator.OperationStrategy.soap + "Body", new XElement(EwsValidator.OperationStrategy.m + "GetFolder", new object[]
						{
							new XElement(EwsValidator.OperationStrategy.m + "FolderShape", new XElement(EwsValidator.OperationStrategy.t + "BaseShape", "IdOnly")),
							new XElement(EwsValidator.OperationStrategy.m + "FolderIds", content)
						}))
					})
				});
			}

			protected override XDocument GetRequestXml(string emailAddress, bool isPublicFolderMailbox)
			{
				if (string.IsNullOrEmpty(emailAddress))
				{
					throw new ArgumentNullException("emailAddress");
				}
				XDocument requestTemplate = this.GetRequestTemplate(isPublicFolderMailbox);
				if (!isPublicFolderMailbox)
				{
					XElement xelement = requestTemplate.XPathSelectElement(".//soap:Body/m:GetFolder/m:FolderIds/t:DistinguishedFolderId/t:Mailbox/t:EmailAddress", EwsValidator.OperationStrategy.namespaceManager);
					xelement.Value = emailAddress;
				}
				return requestTemplate;
			}

			protected override string ResponseMessageElementPath
			{
				get
				{
					return ".//soap:Body/m:GetFolderResponse/m:ResponseMessages/m:GetFolderResponseMessage";
				}
			}
		}

		private class GetUserAvailabilityStrategy : EwsValidator.OperationStrategy
		{
			protected override XDocument GetRequestXml(string emailAddress, bool isPublicFolderMailbox)
			{
				if (string.IsNullOrEmpty(emailAddress))
				{
					throw new ArgumentNullException("emailAddress");
				}
				XDocument xdocument = new XDocument(EwsValidator.GetUserAvailabilityStrategy.RequestTemplate);
				XElement xelement = xdocument.XPathSelectElement(".//soap:Body/m:GetUserAvailabilityRequest/m:MailboxDataArray/t:MailboxData/t:Email/t:Address", EwsValidator.OperationStrategy.namespaceManager);
				xelement.Value = emailAddress;
				DateTime dateTime = DateTime.Today.AddDays(2.0);
				DateTime dateTime2 = dateTime.AddHours(1.0);
				XElement xelement2 = xdocument.XPathSelectElement(".//soap:Body/m:GetUserAvailabilityRequest/t:FreeBusyViewOptions/t:TimeWindow/t:StartTime", EwsValidator.OperationStrategy.namespaceManager);
				xelement2.Value = dateTime.ToString("s");
				XElement xelement3 = xdocument.XPathSelectElement(".//soap:Body/m:GetUserAvailabilityRequest/t:FreeBusyViewOptions/t:TimeWindow/t:EndTime", EwsValidator.OperationStrategy.namespaceManager);
				xelement3.Value = dateTime2.ToString("s");
				return xdocument;
			}

			protected override string ResponseMessageElementPath
			{
				get
				{
					return ".//soap:Body/m:GetUserAvailabilityResponse/m:FreeBusyResponseArray/m:FreeBusyResponse/m:ResponseMessage";
				}
			}

			private static readonly XDocument RequestTemplate = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new object[]
			{
				new XElement(EwsValidator.OperationStrategy.soap + "Envelope", new object[]
				{
					new XAttribute(XNamespace.Xmlns + "xsi", EwsValidator.OperationStrategy.xsi.NamespaceName),
					new XAttribute(XNamespace.Xmlns + "m", EwsValidator.OperationStrategy.m.NamespaceName),
					new XAttribute(XNamespace.Xmlns + "t", EwsValidator.OperationStrategy.t.NamespaceName),
					new XAttribute(XNamespace.Xmlns + "soap", EwsValidator.OperationStrategy.soap.NamespaceName),
					new XElement(EwsValidator.OperationStrategy.soap + "Header", new object[]
					{
						new XElement(EwsValidator.OperationStrategy.t + "RequestServerVersion", new XAttribute("Version", "Exchange2010")),
						new XElement(EwsValidator.OperationStrategy.t + "TimeZoneContext", new XElement(EwsValidator.OperationStrategy.t + "TimeZoneDefinition", new object[]
						{
							new XAttribute("Name", "(UTC-08:00) Pacific Time (US & Canada)"),
							new XAttribute("Id", "Pacific Standard Time")
						}))
					}),
					new XElement(EwsValidator.OperationStrategy.soap + "Body", new XElement(EwsValidator.OperationStrategy.m + "GetUserAvailabilityRequest", new object[]
					{
						new XElement(EwsValidator.OperationStrategy.m + "MailboxDataArray", new XElement(EwsValidator.OperationStrategy.t + "MailboxData", new object[]
						{
							new XElement(EwsValidator.OperationStrategy.t + "Email", new XElement(EwsValidator.OperationStrategy.t + "Address", "email-address-placeholder")),
							new XElement(EwsValidator.OperationStrategy.t + "AttendeeType", "Required"),
							new XElement(EwsValidator.OperationStrategy.t + "ExcludeConflicts", "false")
						})),
						new XElement(EwsValidator.OperationStrategy.t + "FreeBusyViewOptions", new object[]
						{
							new XElement(EwsValidator.OperationStrategy.t + "TimeWindow", new object[]
							{
								new XElement(EwsValidator.OperationStrategy.t + "StartTime", "start-time-placeholder"),
								new XElement(EwsValidator.OperationStrategy.t + "EndTime", "end-time-placeholder")
							}),
							new XElement(EwsValidator.OperationStrategy.t + "MergedFreeBusyIntervalInMinutes", "30"),
							new XElement(EwsValidator.OperationStrategy.t + "RequestedView", "Detailed")
						})
					}))
				})
			});
		}
	}
}
