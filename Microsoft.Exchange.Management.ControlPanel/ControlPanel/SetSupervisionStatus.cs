using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Supervision;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetSupervisionStatus : SetObjectProperties
	{
		public SetSupervisionStatus()
		{
			this.OnDeserializing(default(StreamingContext));
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			this.MyClosedCampusOutboundPolicyConfiguration = new SetClosedCampusOutboundPolicyConfiguration();
		}

		public SetClosedCampusOutboundPolicyConfiguration MyClosedCampusOutboundPolicyConfiguration { get; private set; }

		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-SupervisionPolicy";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Organization";
			}
		}

		[DataMember]
		public bool ClosedCampusPolicyEnabled
		{
			get
			{
				return (bool)(base[SupervisionPolicySchema.ClosedCampusInboundPolicyEnabled] ?? false);
			}
			set
			{
				base[SupervisionPolicySchema.ClosedCampusInboundPolicyEnabled] = value;
				this.MyClosedCampusOutboundPolicyConfiguration.ClosedCampusOutboundPolicyEnabled = value;
			}
		}

		[DataMember]
		public Identity[] ClosedCampusInboundGroupExceptions
		{
			get
			{
				return Identity.FromIdParameters(((RecipientIdParameter[])base[SupervisionPolicySchema.ClosedCampusInboundGroupExceptions]).ToStringArray());
			}
			set
			{
				base[SupervisionPolicySchema.ClosedCampusInboundGroupExceptions] = ((IEnumerable<Identity>)value).ToIdParameters();
				this.MyClosedCampusOutboundPolicyConfiguration.ClosedCampusOutboundGroupExceptions = value;
			}
		}

		[DataMember]
		public string[] ClosedCampusInboundDomainExceptions
		{
			get
			{
				return ((SmtpDomain[])base[SupervisionPolicySchema.ClosedCampusInboundDomainExceptions]).ToStringArray();
			}
			set
			{
				SmtpDomain[] array = new SmtpDomain[value.Length];
				for (int i = 0; i < value.Length; i++)
				{
					try
					{
						array[i] = SmtpDomain.Parse(value[i]);
					}
					catch (FormatException ex)
					{
						throw new FaultException(ex.Message);
					}
				}
				base[SupervisionPolicySchema.ClosedCampusInboundDomainExceptions] = array;
				this.MyClosedCampusOutboundPolicyConfiguration.ClosedCampusOutboundDomainExceptions = value;
			}
		}

		[DataMember]
		public string BadWordsList
		{
			get
			{
				return (string)base[SupervisionPolicySchema.BadWordsList];
			}
			set
			{
				base[SupervisionPolicySchema.BadWordsList] = value;
			}
		}

		[DataMember]
		public bool BadWordsPolicyEnabled
		{
			get
			{
				return (bool)(base[SupervisionPolicySchema.BadWordsPolicyEnabled] ?? false);
			}
			set
			{
				base[SupervisionPolicySchema.BadWordsPolicyEnabled] = value;
			}
		}

		[DataMember]
		public bool AntiBullyingPolicyEnabled
		{
			get
			{
				return (bool)(base[SupervisionPolicySchema.AntiBullyingPolicyEnabled] ?? false);
			}
			set
			{
				base[SupervisionPolicySchema.AntiBullyingPolicyEnabled] = value;
			}
		}
	}
}
