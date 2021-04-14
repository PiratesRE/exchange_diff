using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetUMMailboxConfiguration : UMBasePinSetConfiguration
	{
		public SetUMMailboxConfiguration(UMMailbox umMailbox) : base(umMailbox)
		{
			this.dialPlan = base.UMMailbox.GetDialPlan();
			this.primaryExtension = UMMailbox.GetPrimaryExtension(base.UMMailbox.EmailAddresses, ProxyAddressPrefix.UM);
			this.secondaryExtensions = new List<UMExtension>();
			foreach (ProxyAddress proxyAddress in base.UMMailbox.EmailAddresses)
			{
				string extension;
				string phoneContext;
				string dpName;
				if (!proxyAddress.IsPrimaryAddress && proxyAddress.Prefix == ProxyAddressPrefix.UM && UMMailbox.ExtractExtensionInformation(proxyAddress, out extension, out phoneContext, out dpName))
				{
					this.secondaryExtensions.Add(new UMExtension(extension, phoneContext, dpName));
				}
			}
		}

		[DataMember]
		public bool IsE164DialPlan
		{
			get
			{
				return this.GetValue<bool>(() => this.dialPlan.URIType == UMUriType.E164, false);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DialPlanName
		{
			get
			{
				return this.GetValue<string>(() => this.dialPlan.ToIdentity().DisplayName, string.Empty);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DialPlanId
		{
			get
			{
				return this.GetValue<string>(() => this.dialPlan.ToIdentity().RawIdentity.ToString(), string.Empty);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsSipDialPlan
		{
			get
			{
				return this.GetValue<bool>(() => this.dialPlan.URIType == UMUriType.SipName, false);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public int ExtensionLength
		{
			get
			{
				return this.GetValue<int>(() => this.dialPlan.NumberOfDigitsInExtension, 20);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public Identity UMMailboxPolicy
		{
			get
			{
				return base.UMMailbox.UMMailboxPolicy.ToIdentity();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string OperatorNumber
		{
			get
			{
				return base.UMMailbox.OperatorNumber;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string PrimaryExtension
		{
			get
			{
				return this.primaryExtension;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IEnumerable<UMExtension> SecondaryExtensions
		{
			get
			{
				return this.secondaryExtensions;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string WhenChanged
		{
			get
			{
				return base.UMMailbox.WhenChanged.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private TResult GetValue<TResult>(Func<TResult> d1, TResult defaultValue)
		{
			if (base.UMMailboxPin == null || this.dialPlan == null)
			{
				return defaultValue;
			}
			return d1();
		}

		private List<UMExtension> secondaryExtensions;

		private string primaryExtension;

		private UMDialPlan dialPlan;
	}
}
