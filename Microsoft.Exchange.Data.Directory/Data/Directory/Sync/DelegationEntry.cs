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
	public class DelegationEntry : DirectoryLinkServicePrincipalToServicePrincipal
	{
		public override DirectoryObjectClass GetSourceClass()
		{
			return (DirectoryObjectClass)Enum.Parse(typeof(DirectoryObjectClass), base.SourceClass.ToString());
		}

		public override void SetSourceClass(DirectoryObjectClass objectClass)
		{
			throw new NotImplementedException();
		}

		public override DirectoryObjectClass GetTargetClass()
		{
			return (DirectoryObjectClass)Enum.Parse(typeof(DirectoryObjectClass), base.TargetClass.ToString());
		}

		public override void SetTargetClass(DirectoryObjectClass objectClass)
		{
			throw new NotImplementedException();
		}

		[XmlElement(Order = 0)]
		public DirectoryPropertyInt32SingleMin0Max1 DelegationEncodingVersion
		{
			get
			{
				return this.delegationEncodingVersionField;
			}
			set
			{
				this.delegationEncodingVersionField = value;
			}
		}

		[XmlElement(Order = 1)]
		public DirectoryPropertyInt32SingleMin0Max2 DelegationType
		{
			get
			{
				return this.delegationTypeField;
			}
			set
			{
				this.delegationTypeField = value;
			}
		}

		[XmlElement(Order = 2)]
		public DirectoryPropertyInt32SingleMin0Max2 UserIdentifierType
		{
			get
			{
				return this.userIdentifierTypeField;
			}
			set
			{
				this.userIdentifierTypeField = value;
			}
		}

		[XmlElement(Order = 3)]
		public DirectoryPropertyBinarySingleLength1To195 UserIdentifier
		{
			get
			{
				return this.userIdentifierField;
			}
			set
			{
				this.userIdentifierField = value;
			}
		}

		[XmlElement(Order = 4)]
		public DirectoryPropertyDateTimeSingle DelegationEndTime
		{
			get
			{
				return this.delegationEndTimeField;
			}
			set
			{
				this.delegationEndTimeField = value;
			}
		}

		[XmlElement(Order = 5)]
		public DirectoryPropertyBinarySingleLength1To8000 DelegationScope
		{
			get
			{
				return this.delegationScopeField;
			}
			set
			{
				this.delegationScopeField = value;
			}
		}

		private DirectoryPropertyInt32SingleMin0Max1 delegationEncodingVersionField;

		private DirectoryPropertyInt32SingleMin0Max2 delegationTypeField;

		private DirectoryPropertyInt32SingleMin0Max2 userIdentifierTypeField;

		private DirectoryPropertyBinarySingleLength1To195 userIdentifierField;

		private DirectoryPropertyDateTimeSingle delegationEndTimeField;

		private DirectoryPropertyBinarySingleLength1To8000 delegationScopeField;
	}
}
