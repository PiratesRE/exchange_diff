using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	internal class NewDlpPolicyImpl : CmdletImplementation
	{
		internal NewDlpPolicyImpl(NewDlpPolicy taskObject)
		{
			this.taskObject = taskObject;
		}

		public override void Validate()
		{
			Exception exception = null;
			this.ValidateParameterSets();
			DlpPolicyTemplateMetaData dlpPolicyTemplateMetaData = null;
			if (this.taskObject.Fields.IsModified("TemplateData"))
			{
				dlpPolicyTemplateMetaData = this.LoadDlpPolicyFromCustomTemplateData();
			}
			if (this.taskObject.Fields.IsModified("Template"))
			{
				dlpPolicyTemplateMetaData = this.LoadDlpPolicyFromInstalledTemplate();
			}
			if (this.taskObject.Fields.IsModified("Name"))
			{
				this.dlpPolicy.Name = this.taskObject.Name;
			}
			if (dlpPolicyTemplateMetaData != null)
			{
				this.dlpPolicy = new DlpPolicyMetaData(dlpPolicyTemplateMetaData, this.taskObject.CommandRuntime.Host.CurrentCulture);
				if (!string.IsNullOrEmpty(this.taskObject.Name))
				{
					this.dlpPolicy.Name = this.taskObject.Name;
				}
				this.dlpPolicy.PolicyCommands = NewDlpPolicyImpl.ParameterizeCmdlets(this.dlpPolicy.Name, dlpPolicyTemplateMetaData.PolicyCommands, dlpPolicyTemplateMetaData.RuleParameters, this.taskObject.Parameters, new NewDlpPolicy.WarningWriterDelegate(this.taskObject.WriteWarning), out exception);
				this.WriteParameterErrorIfExceptionOccurred(exception, "Parameters");
				this.dlpPolicy.PolicyCommands = DlpPolicyTemplateMetaData.LocalizeCmdlets(this.dlpPolicy.PolicyCommands, dlpPolicyTemplateMetaData.LocalizedPolicyCommandResources, this.taskObject.CommandRuntime.Host.CurrentCulture).ToList<string>();
				this.dlpPolicy.PolicyCommands.ForEach(delegate(string command)
				{
					DlpPolicyTemplateMetaData.ValidateCmdletParameters(command);
				});
			}
			if (this.taskObject.Fields.IsModified("State"))
			{
				this.dlpPolicy.State = this.taskObject.State;
			}
			if (this.taskObject.Fields.IsModified("Mode"))
			{
				this.dlpPolicy.Mode = this.taskObject.Mode;
			}
			if (this.taskObject.Fields.IsModified("Description"))
			{
				this.dlpPolicy.Description = this.taskObject.Description;
			}
			this.ValidateDlpPolicyName();
		}

		private void WriteParameterErrorIfExceptionOccurred(Exception exception, string parameterName)
		{
			if (exception != null)
			{
				this.taskObject.WriteError(exception, ErrorCategory.InvalidArgument, parameterName);
			}
		}

		internal DlpPolicyTemplateMetaData LoadDlpPolicyFromCustomTemplateData()
		{
			try
			{
				return DlpUtils.LoadDlpPolicyTemplates(this.taskObject.TemplateData).FirstOrDefault<DlpPolicyTemplateMetaData>();
			}
			catch (Exception exception)
			{
				this.WriteParameterErrorIfExceptionOccurred(exception, "TemplateData");
			}
			return null;
		}

		private DlpPolicyTemplateMetaData LoadDlpPolicyFromInstalledTemplate()
		{
			DlpPolicyTemplateMetaData dlpPolicyTemplateMetaData = DlpUtils.LoadOutOfBoxDlpTemplate(this.taskObject.DomainController, this.taskObject.Template);
			if (dlpPolicyTemplateMetaData == null)
			{
				this.taskObject.WriteError(new ArgumentException(Strings.ErrorDlpPolicyTemplateNotFound(this.taskObject.Template)), ErrorCategory.InvalidArgument, "Template");
			}
			return dlpPolicyTemplateMetaData;
		}

		internal static List<string> ParameterizeCmdlets(string policyName, IEnumerable<string> cmdlets, IEnumerable<DlpTemplateRuleParameter> ruleParameters, Hashtable userSuppliedParameters, NewDlpPolicy.WarningWriterDelegate warningWriter, out Exception ex)
		{
			ex = null;
			Dictionary<string, string> parameterValues = new Dictionary<string, string>
			{
				{
					"%%DlpPolicyName%%",
					Utils.EscapeCmdletParameter(policyName)
				}
			};
			foreach (DlpTemplateRuleParameter dlpTemplateRuleParameter in ruleParameters)
			{
				bool flag = false;
				string text = dlpTemplateRuleParameter.Token.Replace("%%", string.Empty);
				if (parameterValues.ContainsKey(dlpTemplateRuleParameter.Token))
				{
					ex = new ArgumentException(Strings.ErrorDlpTemplateDuplicateParameter(text));
					return Enumerable.Empty<string>().ToList<string>();
				}
				if (userSuppliedParameters != null)
				{
					foreach (object obj in userSuppliedParameters.Keys)
					{
						string text2 = (string)obj;
						if (string.Compare(text, text2, true) == 0)
						{
							parameterValues.Add(dlpTemplateRuleParameter.Token, userSuppliedParameters[text2].ToString());
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					if (dlpTemplateRuleParameter.Required)
					{
						ex = new ArgumentException(Strings.ErrorDlpTemplateRequiresParameter(text, string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, from ruleParameter in ruleParameters
						select ruleParameter.Token.Replace("%%", ""))));
						return Enumerable.Empty<string>().ToList<string>();
					}
					parameterValues.Add(dlpTemplateRuleParameter.Token, string.Empty);
					warningWriter(Strings.DlpPolicyOptionalParameterNotSupplied(text));
				}
			}
			return (from cmdlet in cmdlets
			select parameterValues.Aggregate(cmdlet, (string current, KeyValuePair<string, string> parameter) => current.Replace(parameter.Key, parameter.Value).Trim())).ToList<string>();
		}

		internal void ValidateParameterSets()
		{
			if (!this.taskObject.Fields.IsModified("Template") && !this.taskObject.Fields.IsModified("TemplateData") && !this.taskObject.Fields.IsModified("Name"))
			{
				this.taskObject.WriteError(new ArgumentException(Strings.ErrorDlpPolicyNameOrTemplateParameterMustBeSpecified), ErrorCategory.InvalidArgument, "Name");
			}
			if (this.taskObject.Fields.IsModified("Template") && this.taskObject.Fields.IsModified("TemplateData"))
			{
				this.taskObject.WriteError(new ArgumentException(Strings.ErrorTemplateAndTemplateDataCannotBothBeDefined), ErrorCategory.InvalidArgument, "TemplateData");
			}
			if (!this.taskObject.Fields.IsModified("Template") && !this.taskObject.Fields.IsModified("TemplateData") && !this.taskObject.Fields.IsModified("Name"))
			{
				this.taskObject.WriteError(new ArgumentException(Strings.ErrorParametersThatMustBeDefinedIfNeitherTemplateNorTemplateDataAreDefined(string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, new string[]
				{
					"Name"
				}))), ErrorCategory.InvalidArgument, "Template");
			}
		}

		internal void ValidateDlpPolicyName()
		{
			if (DlpUtils.GetInstalledTenantDlpPolicies(base.DataSession, this.dlpPolicy.Name).Any<ADComplianceProgram>())
			{
				this.taskObject.WriteError(new ArgumentException(Strings.ErrorDlpPolicyAlreadyInstalled(this.dlpPolicy.Name)), ErrorCategory.InvalidArgument, "Name");
			}
		}

		public override void ProcessRecord()
		{
			try
			{
				IEnumerable<PSObject> enumerable;
				DlpUtils.AddTenantDlpPolicy(base.DataSession, this.dlpPolicy, Utils.GetOrganizationParameterValue(this.taskObject.Fields), new CmdletRunner(DlpPolicyTemplateMetaData.AllowedCommands, DlpPolicyTemplateMetaData.RequiredParams, null), out enumerable);
			}
			catch (DlpPolicyScriptExecutionException exception)
			{
				this.taskObject.WriteError(exception, ErrorCategory.InvalidArgument, null);
			}
		}

		private readonly NewDlpPolicy taskObject;

		private DlpPolicyMetaData dlpPolicy = new DlpPolicyMetaData
		{
			State = RuleState.Enabled,
			Mode = RuleMode.Audit,
			PublisherName = " ",
			Version = NewDlpPolicy.DefaultVersion,
			Description = " "
		};
	}
}
