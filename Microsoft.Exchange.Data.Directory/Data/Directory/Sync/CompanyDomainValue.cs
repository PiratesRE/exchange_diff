using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[XmlInclude(typeof(CompanyVerifiedDomainValue))]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[Serializable]
	public class CompanyDomainValue : IComparable
	{
		public override string ToString()
		{
			return string.Format("nameField={0} liveTypeField={1} capabilitiesField={2}", this.nameField, this.liveTypeField, this.capabilitiesField);
		}

		public AcceptedDomainType AcceptedDomainType
		{
			get
			{
				if ((this.Capabilities & 4) != 4)
				{
					return AcceptedDomainType.Authoritative;
				}
				return AcceptedDomainType.InternalRelay;
			}
		}

		public int CompareTo(object other)
		{
			if (other is CompanyDomainValue)
			{
				CompanyDomainValue companyDomainValue = other as CompanyDomainValue;
				int num = string.IsNullOrEmpty(this.Name) ? 0 : this.Name.Length;
				int num2 = string.IsNullOrEmpty(companyDomainValue.Name) ? 0 : companyDomainValue.Name.Length;
				return num - num2;
			}
			throw new ArgumentException("other is not a CompanyDomainValue");
		}

		public CompanyDomainValue()
		{
			this.liveTypeField = LiveNamespaceType.None;
			this.capabilitiesField = 0;
		}

		[XmlAttribute]
		public string Name
		{
			get
			{
				return this.nameField;
			}
			set
			{
				this.nameField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(LiveNamespaceType.None)]
		public LiveNamespaceType LiveType
		{
			get
			{
				return this.liveTypeField;
			}
			set
			{
				this.liveTypeField = value;
			}
		}

		[XmlAttribute(DataType = "hexBinary")]
		public byte[] LiveNetId
		{
			get
			{
				return this.liveNetIdField;
			}
			set
			{
				this.liveNetIdField = value;
			}
		}

		[XmlAttribute]
		[DefaultValue(0)]
		public int Capabilities
		{
			get
			{
				return this.capabilitiesField;
			}
			set
			{
				this.capabilitiesField = value;
			}
		}

		[XmlAttribute]
		public string MailTargetKey
		{
			get
			{
				return this.mailTargetKeyField;
			}
			set
			{
				this.mailTargetKeyField = value;
			}
		}

		[XmlAttribute]
		public int PasswordValidityPeriodDays
		{
			get
			{
				return this.passwordValidityPeriodDaysField;
			}
			set
			{
				this.passwordValidityPeriodDaysField = value;
			}
		}

		[XmlIgnore]
		public bool PasswordValidityPeriodDaysSpecified
		{
			get
			{
				return this.passwordValidityPeriodDaysFieldSpecified;
			}
			set
			{
				this.passwordValidityPeriodDaysFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public int PasswordNotificationWindowDays
		{
			get
			{
				return this.passwordNotificationWindowDaysField;
			}
			set
			{
				this.passwordNotificationWindowDaysField = value;
			}
		}

		[XmlIgnore]
		public bool PasswordNotificationWindowDaysSpecified
		{
			get
			{
				return this.passwordNotificationWindowDaysFieldSpecified;
			}
			set
			{
				this.passwordNotificationWindowDaysFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public string VerificationCode
		{
			get
			{
				return this.verificationCodeField;
			}
			set
			{
				this.verificationCodeField = value;
			}
		}

		private const int BposInternalRelayFlagValue = 4;

		private string nameField;

		private LiveNamespaceType liveTypeField;

		private byte[] liveNetIdField;

		private int capabilitiesField;

		private string mailTargetKeyField;

		private int passwordValidityPeriodDaysField;

		private bool passwordValidityPeriodDaysFieldSpecified;

		private int passwordNotificationWindowDaysField;

		private bool passwordNotificationWindowDaysFieldSpecified;

		private string verificationCodeField;
	}
}
