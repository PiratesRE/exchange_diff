using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class EndPoint
	{
		public RequestMethod Method
		{
			get
			{
				return this.method;
			}
			set
			{
				this.method = value;
			}
		}

		public bool AllowAutoRedirect
		{
			get
			{
				return this.allowAutoRedirect;
			}
			set
			{
				this.allowAutoRedirect = value;
			}
		}

		public bool GetHiddenInputValues
		{
			get
			{
				return this.getHiddenInputValues;
			}
			set
			{
				this.getHiddenInputValues = value;
			}
		}

		public string ContentType
		{
			get
			{
				return this.contentType;
			}
			set
			{
				this.contentType = value;
			}
		}

		public string FormName
		{
			get
			{
				return this.formName;
			}
			set
			{
				this.formName = value;
			}
		}

		public TimeSpan PageLoadTimeout
		{
			get
			{
				return this.pageLoadTimeout;
			}
			set
			{
				this.pageLoadTimeout = value;
			}
		}

		public string Uri
		{
			get
			{
				return this.uri;
			}
			set
			{
				this.uri = value;
			}
		}

		public PostData PostData
		{
			get
			{
				return this.postData;
			}
			set
			{
				this.postData = value;
			}
		}

		public ICollection<ExpectedResult> ExpectedResults
		{
			get
			{
				return this.expectedResults;
			}
			set
			{
				this.expectedResults = value;
			}
		}

		public ICollection<ResponseCapture> Captures
		{
			get
			{
				return this.captures;
			}
			set
			{
				this.captures = value;
			}
		}

		public string AuthenticationType
		{
			get
			{
				return this.authenticationType;
			}
			internal set
			{
				this.authenticationType = value;
			}
		}

		public string AuthenticationUser
		{
			get
			{
				return this.authenticationUser;
			}
			internal set
			{
				this.authenticationUser = value;
			}
		}

		public string AuthenticationPassword
		{
			get
			{
				return this.authenticationPassword;
			}
			internal set
			{
				this.authenticationPassword = value;
			}
		}

		public bool SslValidation
		{
			get
			{
				return this.sslValidation;
			}
			internal set
			{
				this.sslValidation = value;
			}
		}

		public Dictionary<string, string> Properties
		{
			get
			{
				return this.properties;
			}
		}

		public static ICollection<EndPoint> FromXml(XmlNode workContext, TimeSpan defaultPageLoadTimeout)
		{
			List<EndPoint> list = new List<EndPoint>();
			using (XmlNodeList xmlNodeList = workContext.SelectNodes("//Endpoint"))
			{
				foreach (object obj in xmlNodeList)
				{
					XmlElement xmlElement = (XmlElement)obj;
					EndPoint endPoint = new EndPoint();
					endPoint.AllowAutoRedirect = Utils.GetBoolean(xmlElement.GetAttribute("AllowAutoRedirect"), "AllowAutoRedirect", true);
					string attribute = xmlElement.GetAttribute("Method");
					endPoint.Method = RequestMethod.Get;
					if (!string.IsNullOrWhiteSpace(attribute))
					{
						endPoint.Method = Utils.GetEnumValue<RequestMethod>(attribute, "Method");
					}
					attribute = xmlElement.GetAttribute("AuthenticationType");
					if (!string.IsNullOrWhiteSpace(attribute))
					{
						endPoint.AuthenticationType = attribute;
					}
					attribute = xmlElement.GetAttribute("AuthenticationUser");
					if (!string.IsNullOrWhiteSpace(attribute))
					{
						endPoint.AuthenticationUser = attribute;
					}
					attribute = xmlElement.GetAttribute("AuthenticationPassword");
					if (!string.IsNullOrWhiteSpace(attribute))
					{
						endPoint.AuthenticationPassword = attribute;
					}
					endPoint.SslValidation = Utils.GetBoolean(xmlElement.GetAttribute("SslValidation"), "SslValidation", true);
					attribute = xmlElement.GetAttribute("PageLoadTimeout");
					endPoint.PageLoadTimeout = defaultPageLoadTimeout;
					if (!string.IsNullOrWhiteSpace(attribute))
					{
						endPoint.PageLoadTimeout = TimeSpan.FromMilliseconds((double)Utils.GetPositiveInteger(attribute, "PageLoadTimeout"));
					}
					foreach (object obj2 in xmlElement.SelectNodes("Properties/Property"))
					{
						XmlElement xmlElement2 = (XmlElement)obj2;
						endPoint.Properties.Add(xmlElement2.GetAttribute("Name"), xmlElement2.GetAttribute("Value"));
					}
					if (endPoint.Method == RequestMethod.Post || endPoint.Method == RequestMethod.MSLiveLogin)
					{
						XmlAttribute xmlAttribute = xmlElement.Attributes["ContentType"];
						endPoint.ContentType = ((xmlAttribute == null) ? "application/x-www-form-urlencoded" : xmlAttribute.Value);
						attribute = xmlElement.GetAttribute("FormName");
						if (!string.IsNullOrWhiteSpace(attribute))
						{
							endPoint.FormName = attribute;
						}
						endPoint.PostData = PostData.FromXml(xmlElement);
					}
					endPoint.Uri = Utils.CheckNullOrWhiteSpace(xmlElement.GetAttribute("Uri"), "Uri");
					endPoint.ExpectedResults = ExpectedResult.FromXml(xmlElement);
					endPoint.Captures = ResponseCapture.FromXml(xmlElement);
					list.Add(endPoint);
				}
			}
			if (list.Count == 0)
			{
				throw new ArgumentException("At least one EndPoint must be specified.");
			}
			return list;
		}

		private RequestMethod method;

		private bool allowAutoRedirect;

		private bool getHiddenInputValues;

		private string contentType;

		private string formName;

		private TimeSpan pageLoadTimeout;

		private string uri;

		private PostData postData;

		private string authenticationType;

		private string authenticationUser;

		private string authenticationPassword;

		private bool sslValidation;

		private Dictionary<string, string> properties = new Dictionary<string, string>();

		private ICollection<ExpectedResult> expectedResults;

		private ICollection<ResponseCapture> captures;
	}
}
