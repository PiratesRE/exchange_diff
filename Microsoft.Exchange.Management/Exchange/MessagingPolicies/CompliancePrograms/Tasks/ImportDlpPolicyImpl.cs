using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	internal class ImportDlpPolicyImpl : CmdletImplementation
	{
		public ImportDlpPolicyImpl(ImportDlpPolicyTemplate dataObject)
		{
			this.dataObject = dataObject;
		}

		public override void Validate()
		{
			if (this.dataObject.FileData == null)
			{
				this.dataObject.WriteError(new ArgumentException(Strings.ImportDlpPolicyFileDataIsNull), ErrorCategory.InvalidArgument, "FileData");
			}
			try
			{
				this.templates = DlpUtils.LoadDlpPolicyTemplates(this.dataObject.FileData);
			}
			catch (Exception ex)
			{
				if (!this.IsKnownException(ex))
				{
					throw;
				}
				this.dataObject.WriteError(ex, ErrorCategory.InvalidOperation, null);
			}
			using (IEnumerator<ADComplianceProgram> enumerator = DlpUtils.GetOutOfBoxDlpTemplates(base.DataSession).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ADComplianceProgram dlpPolicyTemplate = enumerator.Current;
					if (this.templates.Any((DlpPolicyTemplateMetaData a) => a.Name == dlpPolicyTemplate.Name))
					{
						this.dataObject.WriteError(new ArgumentException(Strings.ErrorDlpPolicyTemplateAlreadyInstalled(dlpPolicyTemplate.Name)), ErrorCategory.InvalidArgument, "FileData");
					}
				}
			}
		}

		public override void ProcessRecord()
		{
			try
			{
				DlpUtils.SaveOutOfBoxDlpTemplates(base.DataSession, this.templates);
			}
			catch (Exception ex)
			{
				if (!this.IsKnownException(ex))
				{
					throw;
				}
				this.dataObject.WriteError(ex, ErrorCategory.InvalidOperation, null);
			}
		}

		private bool IsKnownException(Exception e)
		{
			return ImportDlpPolicyImpl.KnownExceptions.Any((Type exceptionType) => exceptionType.IsInstanceOfType(e));
		}

		private ImportDlpPolicyTemplate dataObject;

		private IEnumerable<DlpPolicyTemplateMetaData> templates;

		private static readonly Type[] KnownExceptions = new Type[]
		{
			typeof(DirectoryNotFoundException),
			typeof(IOException),
			typeof(DlpPolicyParsingException)
		};
	}
}
