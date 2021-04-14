using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class JsonMessageHeaderProcessor : MessageHeaderProcessor
	{
		internal JsonMessageHeaderProcessor()
		{
			this.RequestVersion = null;
			this.MailboxCulture = null;
			this.TimeZoneContext = null;
			this.DateTimePrecision = null;
			this.IsBackgroundLoad = false;
			this.ManagementRoleType = null;
		}

		private static DateTimePrecision ReadDateTimePrecisionHeader(XmlDictionaryReader reader)
		{
			return JsonMessageHeaderProcessor.ReadDateTimePrecisionHeader(reader.ReadString());
		}

		protected static DateTimePrecision ReadDateTimePrecisionHeader(string value)
		{
			DateTimePrecision result;
			try
			{
				result = EnumUtilities.Parse<DateTimePrecision>(value);
			}
			catch (ArgumentException)
			{
				throw FaultExceptionUtilities.CreateFault(new ServiceInvalidOperationException((CoreResources.IDs)3468080577U), FaultParty.Sender);
			}
			return result;
		}

		protected static ExchangeVersion ReadRequestVersionHeader(XmlDictionaryReader reader)
		{
			return JsonMessageHeaderProcessor.ReadRequestVersionHeader(reader.ReadString());
		}

		protected static ExchangeVersion ReadRequestVersionHeader(string version)
		{
			if (!string.IsNullOrEmpty(version))
			{
				try
				{
					return ExchangeVersion.MapRequestVersionToServerVersion(version);
				}
				catch (InvalidServerVersionException ex)
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceError<string>(0L, "[JsonMessageHeaderProcessor::ReadRequestVersionHeader] Invalid request version value. Message: {0}", ex.Message);
				}
			}
			return null;
		}

		private static TimeZoneContextType ReadTimeZoneContextHeader(XmlDictionaryReader reader)
		{
			try
			{
				XmlReader reader2 = reader.ReadSubtree();
				TimeZoneContextType timeZoneContextType = new TimeZoneContextType();
				timeZoneContextType.TimeZoneDefinition = new TimeZoneDefinitionType();
				XElement xelement = XElement.Load(reader2, LoadOptions.None);
				XElement xelement2 = xelement.Element("TimeZoneDefinition");
				if (xelement2 != null)
				{
					foreach (XElement xelement3 in xelement2.Elements())
					{
						string localName;
						if ((localName = xelement3.Name.LocalName) != null)
						{
							if (!(localName == "Id"))
							{
								if (!(localName == "Name"))
								{
									if (!(localName == "Periods") && !(localName == "TransitionsGroups") && !(localName == "Transitions"))
									{
									}
								}
								else
								{
									timeZoneContextType.TimeZoneDefinition.Name = xelement3.Value;
								}
							}
							else
							{
								timeZoneContextType.TimeZoneDefinition.Id = xelement3.Value;
							}
						}
					}
				}
				return timeZoneContextType;
			}
			catch (XmlException ex)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<string>(0L, "[JsonMessageHeaderProcessor::ReadTimeZoneContextHeader] Caught XmlException exception while parsing timezone header.  Message: {0}", ex.Message);
			}
			return null;
		}

		protected static ManagementRoleType ReadManagementRoleHeader(XmlDictionaryReader reader)
		{
			try
			{
				XmlReader reader2 = reader.ReadSubtree();
				XElement root = XElement.Load(reader2, LoadOptions.None);
				ManagementRoleType managementRoleType = new ManagementRoleType
				{
					UserRoles = JsonMessageHeaderProcessor.GetRolesUnder(root, "UserRoles"),
					ApplicationRoles = JsonMessageHeaderProcessor.GetRolesUnder(root, "ApplicationRoles")
				};
				managementRoleType.ValidateAndConvert();
				return managementRoleType;
			}
			catch (XmlException ex)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<string>(0L, "[JsonMessageHeaderProcessor::ReadManagementRoleHeader] Caught XmlException exception while parsing timezone header.  Message: {0}", ex.Message);
			}
			return null;
		}

		private static string[] GetRolesUnder(XElement root, string name)
		{
			XElement xelement = root.Element(name);
			if (xelement == null)
			{
				return null;
			}
			return (from property in xelement.Elements()
			select property.Value).ToArray<string>();
		}

		private static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(MethodInfo methodInfo)
		{
			return methodInfo.GetCustomAttributes(typeof(TAttribute), false).OfType<TAttribute>();
		}

		protected void ProcessRequestVersion(Message request)
		{
			if (this.RequestVersion == null || !this.RequestVersion.Supports(ExchangeVersion.Exchange2012))
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidServerVersionException(CoreResources.IDs.MessageInvalidServerVersionForJsonRequest), FaultParty.Sender);
			}
			request.Properties["WS_ServerVersionKey"] = this.RequestVersion;
			ExchangeVersion.Current = this.RequestVersion;
		}

		internal static HashSet<string> BuildNoHeaderProcessingMap(Type serviceType)
		{
			return JsonMessageHeaderProcessor.BuildServiceMethodMap<JsonRequestFormatAttribute>(serviceType, (JsonRequestFormatAttribute attr) => attr.Format != JsonRequestFormat.HeaderBodyFormat);
		}

		internal static HashSet<string> BuildNoHeaderQueryProcessingMap(Type serviceType)
		{
			return JsonMessageHeaderProcessor.BuildServiceMethodMap<JsonRequestFormatAttribute>(serviceType, (JsonRequestFormatAttribute attr) => attr.Format != JsonRequestFormat.QueryString);
		}

		internal static HashSet<string> BuildNoHeaderHttpHeaderProcessingMap(Type serviceType)
		{
			return JsonMessageHeaderProcessor.BuildServiceMethodMap<JsonRequestFormatAttribute>(serviceType, (JsonRequestFormatAttribute attr) => attr.Format != JsonRequestFormat.HttpHeaders);
		}

		internal static HashSet<string> BuildServiceMethodMap<TAttr>(Type serviceType, Func<TAttr, bool> predicateFunc)
		{
			HashSet<string> hashSet = new HashSet<string>();
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public;
			foreach (Type type in serviceType.GetInterfaces())
			{
				foreach (MethodInfo methodInfo in type.GetMethods(bindingAttr))
				{
					if (JsonMessageHeaderProcessor.GetCustomAttributes<OperationContractAttribute>(methodInfo).Count<OperationContractAttribute>() != 0)
					{
						foreach (TAttr arg in JsonMessageHeaderProcessor.GetCustomAttributes<TAttr>(methodInfo))
						{
							if (predicateFunc(arg))
							{
								string text = methodInfo.Name.ToLowerInvariant();
								if (text.StartsWith("begin", StringComparison.Ordinal))
								{
									text = text.Substring("begin".Length);
								}
								hashSet.Add(text);
								break;
							}
						}
					}
				}
			}
			return hashSet;
		}

		internal TimeZoneContextType TimeZoneContext { get; set; }

		internal DateTimePrecision? DateTimePrecision { get; set; }

		internal string MailboxCulture { get; set; }

		internal ExchangeVersion RequestVersion { get; set; }

		internal bool IsBackgroundLoad { get; set; }

		internal bool IsServiceUnavailableOnTransientError { get; set; }

		internal ManagementRoleType ManagementRoleType { get; set; }

		internal virtual void ProcessMessageHeaders(Message request)
		{
			try
			{
				XmlDictionaryReader readerAtBodyContents = request.GetReaderAtBodyContents();
				bool flag = false;
				while (readerAtBodyContents.Read())
				{
					if (readerAtBodyContents.IsStartElement("Header"))
					{
						flag = true;
					}
					else
					{
						if (readerAtBodyContents.IsStartElement("Body"))
						{
							break;
						}
						string name;
						if (flag && (name = readerAtBodyContents.Name) != null)
						{
							if (<PrivateImplementationDetails>{66AAB65C-53B7-4036-8215-CAF8E2ECAEBD}.$$method0x6005591-1 == null)
							{
								<PrivateImplementationDetails>{66AAB65C-53B7-4036-8215-CAF8E2ECAEBD}.$$method0x6005591-1 = new Dictionary<string, int>(8)
								{
									{
										"RequestServerVersion",
										0
									},
									{
										"MailboxCulture",
										1
									},
									{
										"TimeZoneContext",
										2
									},
									{
										"DateTimePrecision",
										3
									},
									{
										"BackgroundLoad",
										4
									},
									{
										"ExchangeImpersonation",
										5
									},
									{
										"SerializedSecurityContext",
										6
									},
									{
										"ManagementRole",
										7
									}
								};
							}
							int num;
							if (<PrivateImplementationDetails>{66AAB65C-53B7-4036-8215-CAF8E2ECAEBD}.$$method0x6005591-1.TryGetValue(name, out num))
							{
								switch (num)
								{
								case 0:
									this.RequestVersion = JsonMessageHeaderProcessor.ReadRequestVersionHeader(readerAtBodyContents);
									break;
								case 1:
									this.MailboxCulture = readerAtBodyContents.ReadString();
									break;
								case 2:
									this.TimeZoneContext = JsonMessageHeaderProcessor.ReadTimeZoneContextHeader(readerAtBodyContents);
									break;
								case 3:
									this.DateTimePrecision = new DateTimePrecision?(JsonMessageHeaderProcessor.ReadDateTimePrecisionHeader(readerAtBodyContents));
									break;
								case 4:
									this.IsBackgroundLoad = bool.Parse(readerAtBodyContents.ReadString());
									break;
								case 7:
									this.ManagementRoleType = JsonMessageHeaderProcessor.ReadManagementRoleHeader(readerAtBodyContents);
									break;
								}
							}
						}
					}
				}
			}
			catch (XmlException ex)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<string>((long)this.GetHashCode(), "[JsonMessageHeaderProcessor::ProcessHeaders] Caught XmlException exception while parsing message headers. Message: {0}", ex.Message);
			}
			this.ProcessRequestVersion(request);
		}

		internal virtual void ProcessMessageHeadersFromQueryString(Message request)
		{
			bool flag = false;
			HttpRequestMessageProperty httpRequestMessageProperty = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
			if (httpRequestMessageProperty != null && !string.IsNullOrEmpty(httpRequestMessageProperty.QueryString))
			{
				NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(httpRequestMessageProperty.QueryString);
				foreach (object obj in nameValueCollection.Keys)
				{
					string text = (string)obj;
					string a;
					if ((a = text) != null)
					{
						if (!(a == "ManagementRole"))
						{
							if (a == "RequestServerVersion")
							{
								this.RequestVersion = JsonMessageHeaderProcessor.ReadRequestVersionHeader(nameValueCollection.Get(text));
							}
						}
						else
						{
							this.QueryStringXmlDictionaryReaderAction(nameValueCollection.Get(text), delegate(XmlDictionaryReader reader)
							{
								this.ManagementRoleType = JsonMessageHeaderProcessor.ReadManagementRoleHeader(reader);
							});
							flag = true;
						}
					}
				}
			}
			if (flag)
			{
				this.ProcessRequestVersion(request);
			}
		}

		internal virtual void ProcessHttpHeaders(Message request, ExchangeVersion defaultVersion)
		{
			object obj;
			if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out obj))
			{
				HttpRequestMessageProperty httpRequestMessageProperty = obj as HttpRequestMessageProperty;
				if (httpRequestMessageProperty == null)
				{
					return;
				}
				string text = httpRequestMessageProperty.Headers["X-MailboxCulture"];
				if (!string.IsNullOrEmpty(text))
				{
					this.MailboxCulture = text;
				}
				string text2 = httpRequestMessageProperty.Headers["X-TimeZoneContext"];
				if (!string.IsNullOrEmpty(text2))
				{
					this.TimeZoneContext = new TimeZoneContextType
					{
						TimeZoneDefinition = new TimeZoneDefinitionType
						{
							Id = text2
						}
					};
				}
				string value = httpRequestMessageProperty.Headers["X-DateTimePrecision"];
				if (!string.IsNullOrEmpty(value))
				{
					this.DateTimePrecision = new DateTimePrecision?(JsonMessageHeaderProcessor.ReadDateTimePrecisionHeader(value));
				}
				string value2 = httpRequestMessageProperty.Headers["X-BackgroundLoad"];
				if (!string.IsNullOrEmpty(value2))
				{
					this.IsBackgroundLoad = bool.Parse(value2);
				}
				ExchangeVersion ewsVersionFromHttpHeaders = this.GetEwsVersionFromHttpHeaders(request, httpRequestMessageProperty);
				if (ewsVersionFromHttpHeaders != null)
				{
					this.RequestVersion = ewsVersionFromHttpHeaders;
				}
				else
				{
					string text3 = httpRequestMessageProperty.Headers["X-RequestServerVersion"];
					this.RequestVersion = (string.IsNullOrEmpty(text3) ? defaultVersion : JsonMessageHeaderProcessor.ReadRequestVersionHeader(text3));
				}
				this.ProcessRequestVersion(request);
				string value3 = httpRequestMessageProperty.Headers["X-OWA-ServiceUnavailableOnTransientError"];
				if (!string.IsNullOrEmpty(value3))
				{
					this.IsServiceUnavailableOnTransientError = bool.Parse(value3);
				}
			}
		}

		internal virtual void ProcessEwsVersionFromHttpHeaders(Message request)
		{
			object obj;
			if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out obj))
			{
				HttpRequestMessageProperty httpRequestMessageProperty = obj as HttpRequestMessageProperty;
				if (httpRequestMessageProperty == null)
				{
					return;
				}
				ExchangeVersion ewsVersionFromHttpHeaders = this.GetEwsVersionFromHttpHeaders(request, httpRequestMessageProperty);
				if (ewsVersionFromHttpHeaders != null)
				{
					this.RequestVersion = ewsVersionFromHttpHeaders;
					this.ProcessRequestVersion(request);
				}
			}
		}

		private ExchangeVersion GetEwsVersionFromHttpHeaders(Message request, HttpRequestMessageProperty httpRequest)
		{
			ExchangeVersion result = null;
			string headerValue = httpRequest.Headers["X-EWS-TargetVersion"];
			ExchangeVersionHeader exchangeVersionHeader = new ExchangeVersionHeader(headerValue);
			if (!exchangeVersionHeader.IsMissing)
			{
				ExchangeVersionType version = exchangeVersionHeader.CheckAndGetRequestedVersion();
				result = new ExchangeVersion(version);
			}
			return result;
		}

		protected void QueryStringXmlDictionaryReaderAction(string value, Action<XmlDictionaryReader> action)
		{
			if (!string.IsNullOrEmpty(value))
			{
				try
				{
					using (XmlDictionaryReader xmlDictionaryReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.Unicode.GetBytes(value), new XmlDictionaryReaderQuotas()))
					{
						if (xmlDictionaryReader.Read())
						{
							action(xmlDictionaryReader);
						}
					}
				}
				catch (XmlException ex)
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceError<string>((long)this.GetHashCode(), "[JsonMessageHeaderProcessor::ProcessMessageHeadersFromQueryString] Caught XmlException exception while parsing message headers. Message: {0}", ex.Message);
				}
			}
		}

		internal override void ProcessMailboxCultureHeader(Message request)
		{
			CultureInfo serverCulture;
			CultureInfo clientCulture;
			MessageHeaderProcessor.GetCulture(this.MailboxCulture, out serverCulture, out clientCulture);
			EWSSettings.ClientCulture = clientCulture;
			EWSSettings.ServerCulture = serverCulture;
		}

		internal override void ProcessTimeZoneContextHeader(Message request)
		{
			if (this.TimeZoneContext != null && this.TimeZoneContext.TimeZoneDefinition != null && !string.IsNullOrEmpty(this.TimeZoneContext.TimeZoneDefinition.Id))
			{
				EWSSettings.RequestTimeZone = this.TimeZoneContext.TimeZoneDefinition.ExTimeZone;
			}
		}

		internal override void ProcessDateTimePrecisionHeader(Message request)
		{
			if (this.DateTimePrecision != null)
			{
				EWSSettings.DateTimePrecision = this.DateTimePrecision.Value;
			}
		}

		internal override bool ProcessBackgroundLoadHeader(Message request)
		{
			return this.IsBackgroundLoad;
		}

		internal override bool ProcessServiceUnavailableOnTransientErrorHeader(Message request)
		{
			return this.IsServiceUnavailableOnTransientError;
		}

		internal override ManagementRoleType ProcessManagementRoleHeader(Message request)
		{
			return this.ManagementRoleType;
		}

		internal override ProxyRequestType? ProcessRequestTypeHeader(Message request)
		{
			if (WebOperationContext.Current == null || WebOperationContext.Current.IncomingRequest == null || WebOperationContext.Current.IncomingRequest.Headers == null)
			{
				return null;
			}
			return base.ParseProxyRequestType(WebOperationContext.Current.IncomingRequest.Headers["RequestType"]);
		}

		internal override AuthZClientInfo ProcessProxyHeaders(Message incomingMessage, AuthZClientInfo callerClientInfo)
		{
			return null;
		}

		internal override AuthZClientInfo ProcessImpersonationHeaders(Message request, AuthZClientInfo proxyClientInfo, AuthZClientInfo impersonatingClientInfo)
		{
			return null;
		}

		internal override AuthZClientInfo ProcessSerializedSecurityContextHeaders(Message request)
		{
			return null;
		}

		internal override AuthZClientInfo ProcessOpenAsAdminOrSystemServiceHeader(Message request, AuthZClientInfo impersonatingClientInfo, out SpecialLogonType? specialLogonType, out int? budgetType)
		{
			specialLogonType = null;
			budgetType = null;
			return null;
		}

		private const string AsyncMethodPrefix = "begin";

		public const string MailboxCultureHeaderName = "X-MailboxCulture";

		public const string TimeZoneContextHeaderName = "X-TimeZoneContext";

		public const string DateTimePrecisionHeaderName = "X-DateTimePrecision";

		public const string BackgroundLoadHeaderName = "X-BackgroundLoad";

		public const string RequestServerVersionHeaderName = "X-RequestServerVersion";

		public const string ServiceUnavailableOnTransientErrorHeaderName = "X-OWA-ServiceUnavailableOnTransientError";
	}
}
