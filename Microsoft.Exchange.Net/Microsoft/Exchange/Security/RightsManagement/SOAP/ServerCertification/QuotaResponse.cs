using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Security.RightsManagement.SOAP.ServerCertification
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://microsoft.com/DRM/CertificationService")]
	[Serializable]
	public class QuotaResponse
	{
		public bool Verified
		{
			get
			{
				return this.verifiedField;
			}
			set
			{
				this.verifiedField = value;
			}
		}

		public int CurrentConsumption
		{
			get
			{
				return this.currentConsumptionField;
			}
			set
			{
				this.currentConsumptionField = value;
			}
		}

		public int Maximum
		{
			get
			{
				return this.maximumField;
			}
			set
			{
				this.maximumField = value;
			}
		}

		private bool verifiedField;

		private int currentConsumptionField;

		private int maximumField;
	}
}
