using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Supervision;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetClosedCampusOutboundPolicyConfiguration : SetObjectProperties
	{
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
		public bool ClosedCampusOutboundPolicyEnabled
		{
			get
			{
				return (bool)(base[SupervisionPolicySchema.ClosedCampusOutboundPolicyEnabled] ?? false);
			}
			set
			{
				base[SupervisionPolicySchema.ClosedCampusOutboundPolicyEnabled] = value;
			}
		}

		[DataMember]
		public Identity[] ClosedCampusOutboundGroupExceptions
		{
			get
			{
				return Identity.FromIdParameters(((RecipientIdParameter[])base[SupervisionPolicySchema.ClosedCampusOutboundGroupExceptions]).ToStringArray());
			}
			set
			{
				base[SupervisionPolicySchema.ClosedCampusOutboundGroupExceptions] = ((IEnumerable<Identity>)value).ToIdParameters();
			}
		}

		[DataMember]
		public string[] ClosedCampusOutboundDomainExceptions
		{
			get
			{
				return ((SmtpDomain[])base[SupervisionPolicySchema.ClosedCampusOutboundDomainExceptions]).ToStringArray();
			}
			set
			{
				SmtpDomain[] array = new SmtpDomain[value.Length];
				for (int i = 0; i < value.Length; i++)
				{
					array[i] = SmtpDomain.Parse(value[i]);
				}
				base[SupervisionPolicySchema.ClosedCampusOutboundDomainExceptions] = array;
			}
		}
	}
}
