using System;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	[XmlType(TypeName = "MPCs")]
	[Serializable]
	public sealed class MailboxProvisioningConstraints : XMLSerializableBase, IMailboxProvisioningConstraints
	{
		public MailboxProvisioningConstraints()
		{
			this.HardConstraint = new MailboxProvisioningConstraint();
			this.softConstraints = new MailboxProvisioningConstraint[0];
		}

		public MailboxProvisioningConstraints(MailboxProvisioningConstraint hardConstraint, MailboxProvisioningConstraint[] softConstraints)
		{
			this.HardConstraint = hardConstraint;
			this.softConstraints = softConstraints;
		}

		[XmlElement("Hard")]
		public MailboxProvisioningConstraint HardConstraint { get; set; }

		[XmlElement("Soft")]
		public OrderedMailboxProvisioningConstraint[] SoftConstraints
		{
			get
			{
				OrderedMailboxProvisioningConstraint[] array = new OrderedMailboxProvisioningConstraint[this.softConstraints.Length];
				for (int i = 0; i < this.softConstraints.Length; i++)
				{
					array[i] = new OrderedMailboxProvisioningConstraint(i, this.softConstraints[i].Value);
				}
				return array;
			}
			set
			{
				if (value != null)
				{
					this.softConstraints = new MailboxProvisioningConstraint[value.Length];
					for (int i = 0; i < this.softConstraints.Length; i++)
					{
						this.softConstraints[value[i].Index] = new MailboxProvisioningConstraint(value[i].Value);
					}
				}
			}
		}

		[XmlIgnore]
		IMailboxProvisioningConstraint IMailboxProvisioningConstraints.HardConstraint
		{
			get
			{
				return this.HardConstraint;
			}
		}

		[XmlIgnore]
		IMailboxProvisioningConstraint[] IMailboxProvisioningConstraints.SoftConstraints
		{
			get
			{
				return this.softConstraints.Cast<IMailboxProvisioningConstraint>().ToArray<IMailboxProvisioningConstraint>();
			}
		}

		public bool IsMatch(MailboxProvisioningAttributes attributes)
		{
			return this.HardConstraint == null || this.HardConstraint.IsMatch(attributes);
		}

		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (obj is MailboxProvisioningConstraints && this.Equals((MailboxProvisioningConstraints)obj)));
		}

		public override int GetHashCode()
		{
			return ((this.HardConstraint != null) ? this.HardConstraint.GetHashCode() : 0) * 397 ^ ((this.SoftConstraints != null) ? this.SoftConstraints.GetHashCode() : 0);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{0};", this.HardConstraint);
			foreach (MailboxProvisioningConstraint arg in this.softConstraints)
			{
				stringBuilder.AppendFormat("{0};", arg);
			}
			return stringBuilder.ToString();
		}

		private bool Equals(MailboxProvisioningConstraints other)
		{
			return object.Equals(this.HardConstraint, other.HardConstraint) && this.softConstraints.SequenceEqual(other.softConstraints);
		}

		private MailboxProvisioningConstraint[] softConstraints;
	}
}
