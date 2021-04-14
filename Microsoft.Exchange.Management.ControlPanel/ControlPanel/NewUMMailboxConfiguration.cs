using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NewUMMailboxConfiguration : UMMailboxRow
	{
		public NewUMMailboxConfiguration(UMMailbox umMailbox) : base(umMailbox)
		{
		}

		internal UMMailboxPolicy Policy
		{
			get
			{
				return this.policy;
			}
			set
			{
				if (value != null)
				{
					this.policy = value;
					this.dialPlan = this.policy.GetDialPlan();
				}
			}
		}

		[DataMember]
		public string Extension
		{
			get
			{
				if (base.UMMailbox.Extensions.Count <= 0)
				{
					return string.Empty;
				}
				return base.UMMailbox.Extensions[0];
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public int MinPinLength
		{
			get
			{
				return this.GetValue<int>(() => this.Policy.MinPINLength, 6);
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
		public string SipResourceIdentifier
		{
			get
			{
				return this.GetValue<string>(delegate
				{
					if (!this.IsSipDialPlan)
					{
						return string.Empty;
					}
					return base.UMMailbox.SIPResourceIdentifier;
				}, string.Empty);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string E164ResourceIdentifier
		{
			get
			{
				return this.GetValue<string>(delegate
				{
					if (!this.IsE164DialPlan)
					{
						return string.Empty;
					}
					return base.UMMailbox.SIPResourceIdentifier;
				}, string.Empty);
			}
			private set
			{
				throw new NotSupportedException();
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
		public bool PinExpired
		{
			get
			{
				return true;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string ManualPin
		{
			get
			{
				return string.Empty;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string AutoPin
		{
			get
			{
				return "true";
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private TResult GetValue<TResult>(Func<TResult> d1, TResult defaultValue)
		{
			if (this.policy == null | this.dialPlan == null)
			{
				return defaultValue;
			}
			return d1();
		}

		private UMDialPlan dialPlan;

		private UMMailboxPolicy policy;
	}
}
