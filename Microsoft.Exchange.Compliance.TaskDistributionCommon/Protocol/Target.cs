using System;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol
{
	public class Target
	{
		static Target()
		{
			Target.description.ComplianceStructureId = 2;
			Target.description.RegisterIntegerPropertyGetterAndSetter(0, (Target item) => EnumConverter.ConvertEnumToInteger<Target.Type>(item.TargetType), delegate(Target item, int value)
			{
				item.TargetType = EnumConverter.ConvertIntegerToEnum<Target.Type>(value);
			});
			Target.description.RegisterStringPropertyGetterAndSetter(0, (Target item) => item.Identifier, delegate(Target item, string value)
			{
				item.Identifier = value;
			});
			Target.description.RegisterStringPropertyGetterAndSetter(1, (Target item) => item.Folder, delegate(Target item, string value)
			{
				item.Folder = value;
			});
			Target.description.RegisterGuidPropertyGetterAndSetter(0, (Target item) => item.Database, delegate(Target item, Guid value)
			{
				item.Database = value;
			});
			Target.description.RegisterGuidPropertyGetterAndSetter(1, (Target item) => item.Mailbox, delegate(Target item, Guid value)
			{
				item.Mailbox = value;
			});
		}

		public static ComplianceSerializationDescription<Target> Description
		{
			get
			{
				return Target.description;
			}
		}

		public Target.Type TargetType { get; set; }

		public string Identifier { get; set; }

		public string Server { get; set; }

		public Guid Database { get; set; }

		public Guid Mailbox { get; set; }

		public string Folder { get; set; }

		public Target Clone()
		{
			return (Target)base.MemberwiseClone();
		}

		private static ComplianceSerializationDescription<Target> description = new ComplianceSerializationDescription<Target>();

		public enum Type
		{
			MailboxSmtpAddress,
			MailboxGuid,
			QueryFilter,
			InactiveMailboxes,
			TenantMaster,
			Driver = 99
		}
	}
}
