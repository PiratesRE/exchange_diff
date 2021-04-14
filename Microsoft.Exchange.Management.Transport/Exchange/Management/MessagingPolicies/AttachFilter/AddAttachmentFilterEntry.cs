using System;
using System.IO;
using System.Management.Automation;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.MessagingPolicies.AttachFilter
{
	[Cmdlet("add", "attachmentfilterentry", SupportsShouldProcess = true)]
	public class AddAttachmentFilterEntry : SingletonSystemConfigurationObjectActionTask<AttachmentFilteringConfig>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageAddAttachmentfilterentry(this.Name.ToString(), this.Type.ToString());
			}
		}

		[Parameter(Mandatory = true)]
		public AttachmentType Type
		{
			get
			{
				return (AttachmentType)base.Fields["Type"];
			}
			set
			{
				base.Fields["Type"] = value;
			}
		}

		[Parameter(Mandatory = true, Position = 0)]
		public string Name
		{
			get
			{
				return (string)base.Fields["Name"];
			}
			set
			{
				base.Fields["Name"] = value;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return ((IConfigurationSession)base.DataSession).GetOrgContainerId();
			}
		}

		protected override void InternalValidate()
		{
			if (this.Type == AttachmentType.FileName)
			{
				try
				{
					string text;
					Regex regex;
					string text2;
					AttachmentFilterEntrySpecification.ParseFileSpec(this.Name, out text, out regex, out text2);
				}
				catch (InvalidDataException exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidArgument, null);
				}
			}
		}

		protected override void InternalProcessRecord()
		{
			AttachmentFilteringConfig afilterConfig = AFilterUtils.GetAFilterConfig(base.DataSession);
			string text = this.Type.ToString() + ":" + this.Name;
			string[] array = afilterConfig.AttachmentNames.ToArray();
			foreach (string text2 in array)
			{
				if (text2.Equals(text, StringComparison.InvariantCultureIgnoreCase))
				{
					base.WriteError(new ArgumentException(Strings.AttachmentFilterEntryExists, "AttachmentFilterEntry"), ErrorCategory.InvalidArgument, null);
				}
			}
			afilterConfig.AttachmentNames.Add(text);
			base.DataSession.Save(afilterConfig);
			base.WriteObject(new AttachmentFilterEntrySpecification(this.Type, this.Name));
		}
	}
}
