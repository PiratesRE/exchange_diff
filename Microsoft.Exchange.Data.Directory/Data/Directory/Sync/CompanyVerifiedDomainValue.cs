using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[Serializable]
	public class CompanyVerifiedDomainValue : CompanyDomainValue
	{
		public override string ToString()
		{
			return string.Format("{0} defaultField={1} initialField={2}", base.ToString(), this.defaultField, this.initialField);
		}

		public CompanyVerifiedDomainValue()
		{
			this.defaultField = false;
			this.initialField = false;
			this.verificationMethodField = 0;
			this.requiresDnsPublishingField = false;
		}

		[DefaultValue(false)]
		[XmlAttribute]
		public bool Default
		{
			get
			{
				return this.defaultField;
			}
			set
			{
				this.defaultField = value;
			}
		}

		[DefaultValue(false)]
		[XmlAttribute]
		public bool Initial
		{
			get
			{
				return this.initialField;
			}
			set
			{
				this.initialField = value;
			}
		}

		[DefaultValue(0)]
		[XmlAttribute]
		public int VerificationMethod
		{
			get
			{
				return this.verificationMethodField;
			}
			set
			{
				this.verificationMethodField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool RequiresDnsPublishing
		{
			get
			{
				return this.requiresDnsPublishingField;
			}
			set
			{
				this.requiresDnsPublishingField = value;
			}
		}

		private bool defaultField;

		private bool initialField;

		private int verificationMethodField;

		private bool requiresDnsPublishingField;
	}
}
