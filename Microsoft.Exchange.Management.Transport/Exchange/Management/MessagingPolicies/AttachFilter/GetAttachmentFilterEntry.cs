using System;
using System.IO;
using System.Management.Automation;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.MessagingPolicies.AttachFilter
{
	[Cmdlet("get", "attachmentfilterentry")]
	public class GetAttachmentFilterEntry : GetSingletonSystemConfigurationObjectTask<AttachmentFilteringConfig>
	{
		[Parameter(Mandatory = false, Position = 0)]
		public string Identity
		{
			internal get
			{
				return (string)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			AttachmentFilteringConfig attachmentFilteringConfig = null;
			try
			{
				attachmentFilteringConfig = AFilterUtils.GetAFilterConfig(base.DataSession);
			}
			catch (AttachmentFilterADEntryNotFoundException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, null);
				return;
			}
			string text = (string)base.Fields["Identity"];
			try
			{
				if (string.IsNullOrEmpty(text))
				{
					using (MultiValuedProperty<string>.Enumerator enumerator = attachmentFilteringConfig.AttachmentNames.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							string attachmentName = enumerator.Current;
							this.WriteOutput(attachmentName);
						}
						goto IL_D6;
					}
				}
				foreach (string text2 in attachmentFilteringConfig.AttachmentNames)
				{
					if (string.Equals(text, text2, StringComparison.InvariantCultureIgnoreCase))
					{
						this.WriteOutput(text2);
						return;
					}
				}
				base.WriteError(new ArgumentException(Strings.AddressRewriteIdentityNotFound, "Identity"), ErrorCategory.InvalidArgument, null);
				IL_D6:;
			}
			catch (ArgumentException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidArgument, null);
			}
			catch (InvalidDataException exception3)
			{
				base.WriteError(exception3, ErrorCategory.InvalidArgument, null);
			}
		}

		private void WriteOutput(string attachmentName)
		{
			AttachmentFilterEntrySpecification attachmentFilterEntrySpecification = AttachmentFilterEntrySpecification.Parse(attachmentName);
			if (attachmentFilterEntrySpecification.Type == AttachmentType.FileName)
			{
				try
				{
					string text;
					Regex regex;
					string text2;
					AttachmentFilterEntrySpecification.ParseFileSpec(attachmentFilterEntrySpecification.Name, out text, out regex, out text2);
				}
				catch (InvalidDataException ex)
				{
					base.WriteWarning(ex.Message);
				}
			}
			this.WriteResult(attachmentFilterEntrySpecification);
		}
	}
}
