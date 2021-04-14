using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class JournalRuleObjectProperties : SetObjectProperties
	{
		[DataMember]
		public string Name
		{
			get
			{
				return (string)base["Name"];
			}
			set
			{
				base["Name"] = value;
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
		public string JournalEmailAddress
		{
			get
			{
				return (string)base["JournalEmailAddress"];
			}
			set
			{
				base["JournalEmailAddress"] = value;
			}
		}

		[DataMember]
		public PeopleIdentity[] Recipient
		{
			get
			{
				SmtpAddress? smtpAddress = (SmtpAddress?)base["Recipient"];
				List<ADIdParameter> list = new List<ADIdParameter>();
				if (smtpAddress != null)
				{
					list.Add(RecipientIdParameter.Parse(smtpAddress.Value.ToString()));
				}
				return PeopleIdentity.FromIdParameters(list);
			}
			set
			{
				SmtpAddress? smtpAddress = null;
				if (value != null && value.Length > 0)
				{
					smtpAddress = new SmtpAddress?(SmtpAddress.Parse(value[0].SMTPAddress));
				}
				base["Recipient"] = smtpAddress;
			}
		}

		[DataMember]
		public string Scope
		{
			get
			{
				return (string)base["Scope"];
			}
			set
			{
				base["Scope"] = value;
			}
		}

		[DataMember]
		public bool? Global
		{
			get
			{
				string text = (string)base["Scope"];
				return new bool?(!string.IsNullOrEmpty(text) && text.Equals("Global", StringComparison.InvariantCultureIgnoreCase));
			}
			set
			{
				if (value != null && value.Value)
				{
					base["Scope"] = "Global";
				}
			}
		}

		[DataMember]
		public bool? Internal
		{
			get
			{
				string text = (string)base["Scope"];
				return new bool?(!string.IsNullOrEmpty(text) && text.Equals("Internal", StringComparison.InvariantCultureIgnoreCase));
			}
			set
			{
				if (value != null && value.Value)
				{
					base["Scope"] = "Internal";
				}
			}
		}

		[DataMember]
		public bool? External
		{
			get
			{
				string text = (string)base["Scope"];
				return new bool?(!string.IsNullOrEmpty(text) && text.Equals("External", StringComparison.InvariantCultureIgnoreCase));
			}
			set
			{
				if (value != null && value.Value)
				{
					base["Scope"] = "External";
				}
			}
		}
	}
}
