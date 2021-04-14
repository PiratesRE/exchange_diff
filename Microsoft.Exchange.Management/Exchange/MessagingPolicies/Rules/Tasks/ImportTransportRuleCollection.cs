using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Cmdlet("Import", "TransportRuleCollection", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public class ImportTransportRuleCollection : GetMultitenancySystemConfigurationObjectTask<RuleIdParameter, TransportRule>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageImportTransportRuleCollection;
			}
		}

		public ImportTransportRuleCollection()
		{
			this.ruleCollectionName = Utils.RuleCollectionNameFromRole();
		}

		[Parameter(Mandatory = true, Position = 0)]
		public byte[] FileData
		{
			get
			{
				return (byte[])base.Fields["FileData"];
			}
			set
			{
				base.Fields["FileData"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MigrationSourceType MigrationSource
		{
			get
			{
				return (MigrationSourceType)base.Fields["MigrationSource"];
			}
			set
			{
				base.Fields["MigrationSource"] = value;
			}
		}

		protected override void InternalValidate()
		{
			if (this.FileData == null)
			{
				base.WriteError(new ArgumentException(Strings.ImportFileDataIsNull), ErrorCategory.InvalidArgument, "FileData");
				return;
			}
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			if (!this.Force && !base.ShouldContinue(Strings.PromptToOverwriteRulesOnImport))
			{
				return;
			}
			Exception ex = null;
			using (MemoryStream memoryStream = new MemoryStream(this.FileData))
			{
				bool flag = false;
				if (!this.IsMigratedFopeRuleCollectionBeingImported())
				{
					ex = ImportTransportRuleCollection.TryParseVersion(memoryStream, out flag);
				}
				if (ex == null)
				{
					if (flag)
					{
						if (this.IsDatacenter())
						{
							base.WriteError(new ParseException(Strings.ImportE14TransportRuleCollectionInDCError), ErrorCategory.InvalidData, "File Data");
							return;
						}
						TransportRuleCollection rules;
						ex = ImportTransportRuleCollection.TryParseE14Format(memoryStream, out rules);
						if (ex == null)
						{
							this.ProcessE14Format(rules);
							return;
						}
					}
					else
					{
						IEnumerable<string> cmdlets;
						ex = ImportTransportRuleCollection.TryParseE15Format(memoryStream, out cmdlets);
						if (ex == null)
						{
							this.ProcessE15Format(cmdlets);
							return;
						}
					}
				}
			}
			if (ex != null)
			{
				base.WriteError(ex, ErrorCategory.InvalidData, "File Data");
			}
		}

		internal static Exception TryParseVersion(Stream contentStream, out bool isE14Format)
		{
			isE14Format = false;
			try
			{
				contentStream.Position = 0L;
				XDocument xdocument = XDocument.Load(contentStream);
				IEnumerable<XElement> source = from rule in xdocument.Elements("rules").Elements("rule")
				select rule;
				if (!source.Any<XElement>())
				{
					return null;
				}
				IEnumerable<string> enumerable = from rule in source
				let formatAttribute = rule.Attribute("format")
				where formatAttribute != null && !string.IsNullOrWhiteSpace(formatAttribute.Value)
				select formatAttribute.Value;
				if (!enumerable.Any<string>())
				{
					isE14Format = true;
					return null;
				}
				if (source.Count<XElement>() > enumerable.Count<string>())
				{
					throw new ParseException(RulesStrings.InvalidAttribute("format", "rule", enumerable.First<string>()));
				}
				foreach (string text in enumerable)
				{
					if (string.Compare(text, "cmdlet", true) != 0)
					{
						throw new ParseException(RulesStrings.InvalidAttribute("format", "rule", text));
					}
				}
				isE14Format = false;
			}
			catch (ParseException result)
			{
				return result;
			}
			catch (XmlException result2)
			{
				return result2;
			}
			return null;
		}

		internal static Exception TryParseE15Format(Stream contentStream, out IEnumerable<string> cmdlets)
		{
			try
			{
				contentStream.Position = 0L;
				cmdlets = PowershellTransportRuleSerializer.ParseStream(contentStream);
			}
			catch (ParseException result)
			{
				cmdlets = null;
				return result;
			}
			catch (ArgumentException result2)
			{
				cmdlets = null;
				return result2;
			}
			catch (XmlException result3)
			{
				cmdlets = null;
				return result3;
			}
			if (cmdlets == null || cmdlets.Any(new Func<string, bool>(string.IsNullOrEmpty)))
			{
				return new ArgumentException("File Data - Empty cmdlet block");
			}
			return null;
		}

		internal static Exception TryParseE14Format(Stream contentStream, out TransportRuleCollection rules)
		{
			try
			{
				contentStream.Position = 0L;
				rules = (TransportRuleCollection)TransportRuleParser.Instance.LoadStream(contentStream);
			}
			catch (ParserException result)
			{
				rules = null;
				return result;
			}
			return null;
		}

		private static bool IsMigratedRule(TransportRuleHandle ruleHandle)
		{
			ArgumentValidator.ThrowIfNull("ruleHandle", ruleHandle);
			return ruleHandle.Rule.Comments != null && ruleHandle.Rule.Comments.Contains("FOPEPolicyMigration");
		}

		private IEnumerable<string> ProcessCmdlets(IEnumerable<string> cmdlets)
		{
			List<string> list = new List<string>();
			foreach (string item in cmdlets)
			{
				list.Add(item);
			}
			return list;
		}

		private void ClearExistingRules(ADRuleStorageManager storedRules)
		{
			switch ((base.Fields["MigrationSource"] == null) ? MigrationSourceType.None : ((MigrationSourceType)base.Fields["MigrationSource"]))
			{
			case MigrationSourceType.Fope:
				storedRules.ClearRules(new ADRuleStorageManager.RuleFilter(ImportTransportRuleCollection.IsMigratedRule));
				return;
			case MigrationSourceType.Ehe:
				this.BackupRulesForEheMigration();
				storedRules.ClearRules(null);
				return;
			default:
				storedRules.ClearRules(null);
				return;
			}
		}

		private bool IsMigratedFopeRuleCollectionBeingImported()
		{
			return base.Fields["MigrationSource"] != null && (MigrationSourceType)base.Fields["MigrationSource"] == MigrationSourceType.Fope;
		}

		private void ProcessE15Format(IEnumerable<string> cmdlets)
		{
			Exception ex = null;
			try
			{
				IConfigDataProvider session = new MessagingPoliciesSyncLogDataSession(base.DataSession, null, null);
				ADRuleStorageManager storedRules = new ADRuleStorageManager(this.ruleCollectionName, session);
				this.ClearExistingRules(storedRules);
			}
			catch (RuleCollectionNotInAdException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, null);
				return;
			}
			string lastUsedDc = (base.DataSession as IConfigurationSession).LastUsedDc;
			cmdlets = this.ProcessCmdlets(cmdlets);
			try
			{
				string organizationParameterValue = Utils.GetOrganizationParameterValue(base.Fields);
				CmdletRunner cmdletRunner = new CmdletRunner(ImportTransportRuleCollection.AllowedCommands, null, null);
				foreach (string policyCommand in cmdlets)
				{
					string cmdlet = Utils.AddOrganizationScopeToCmdlet(policyCommand, organizationParameterValue);
					cmdletRunner.RunCmdlet(cmdlet, !this.ContinueOnFailure);
				}
			}
			catch (ArgumentException ex2)
			{
				ex = ex2;
			}
			catch (ParseException ex3)
			{
				ex = ex3;
			}
			catch (RuntimeException ex4)
			{
				ex = ex4;
			}
			catch (CmdletExecutionException ex5)
			{
				ex = ex5;
			}
			if (ex != null)
			{
				this.RecoverDeletedRules(lastUsedDc);
				base.WriteError(ex, ErrorCategory.InvalidArgument, "Error executing script from the cmdlet block: " + ex.Message);
			}
		}

		private void ProcessE14Format(TransportRuleCollection rules)
		{
			ADRuleStorageManager adruleStorageManager;
			try
			{
				IConfigDataProvider session = new MessagingPoliciesSyncLogDataSession(base.DataSession, null, null);
				adruleStorageManager = new ADRuleStorageManager(this.ruleCollectionName, session);
			}
			catch (RuleCollectionNotInAdException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, null);
				return;
			}
			Exception ex = null;
			try
			{
				if (!Utils.IsEdgeRoleInstalled())
				{
					Version v = null;
					bool flag = false;
					bool flag2 = false;
					foreach (Rule rule in rules)
					{
						TransportRule transportRule = (TransportRule)rule;
						if (transportRule.IsTooAdvancedToParse)
						{
							base.WriteError(new InvalidOperationException(Strings.CannotCreateRuleDueToVersion(transportRule.Name)), ErrorCategory.InvalidOperation, null);
							return;
						}
						Version minimumVersion = transportRule.MinimumVersion;
						if (v == null || v < minimumVersion)
						{
							v = minimumVersion;
						}
						if (!flag || !flag2)
						{
							foreach (Action action in transportRule.Actions)
							{
								if (string.Equals(action.Name, "ApplyDisclaimer") || string.Equals(action.Name, "ApplyDisclaimerWithSeparator") || string.Equals(action.Name, "ApplyDisclaimerWithSeparatorAndReadingOrder"))
								{
									flag = true;
								}
								if (string.Equals(action.Name, "LogEvent"))
								{
									flag2 = true;
								}
							}
						}
					}
					if (flag && !this.Force && !base.ShouldContinue(Strings.PromptToUpgradeRulesFormat))
					{
						return;
					}
					if (flag2 && !this.Force && !base.ShouldContinue(Strings.PromptToRemoveLogEventAction))
					{
						return;
					}
				}
				try
				{
					adruleStorageManager.ReplaceRules(rules, this.ResolveCurrentOrganization());
				}
				catch (DataValidationException exception2)
				{
					base.WriteError(exception2, ErrorCategory.InvalidArgument, null);
					return;
				}
			}
			catch (ArgumentOutOfRangeException ex2)
			{
				ex = ex2;
			}
			catch (ArgumentException ex3)
			{
				ex = ex3;
			}
			catch (PathTooLongException ex4)
			{
				ex = ex4;
			}
			catch (DirectoryNotFoundException ex5)
			{
				ex = ex5;
			}
			catch (UnauthorizedAccessException ex6)
			{
				ex = ex6;
			}
			catch (FileNotFoundException ex7)
			{
				ex = ex7;
			}
			catch (IOException ex8)
			{
				ex = ex8;
			}
			catch (NotSupportedException ex9)
			{
				ex = ex9;
			}
			if (ex != null)
			{
				base.WriteError(ex, ErrorCategory.InvalidOperation, null);
			}
		}

		private bool IsDatacenter()
		{
			OrganizationId a = this.ResolveCurrentOrganization();
			return a != OrganizationId.ForestWideOrgId;
		}

		private void BackupRulesForEheMigration()
		{
			ADRuleStorageManager adruleStorageManager = new ADRuleStorageManager(this.ruleCollectionName, base.DataSession);
			adruleStorageManager.LoadRuleCollection();
			this.transportRuleCollectionBackUp = adruleStorageManager.GetRuleCollection();
		}

		private void RecoverDeletedRules(string domainController)
		{
			if (this.transportRuleCollectionBackUp != null)
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(domainController, false, ConsistencyMode.IgnoreInvalid, base.SessionSettings, 631, "RecoverDeletedRules", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\TransportRule\\ImportTransportRuleCollection.cs");
				IConfigDataProvider session = new MessagingPoliciesSyncLogDataSession(tenantOrTopologyConfigurationSession, null, null);
				ADRuleStorageManager adruleStorageManager = new ADRuleStorageManager(this.ruleCollectionName, session);
				adruleStorageManager.ClearRules(null);
				adruleStorageManager.ReplaceRules(this.transportRuleCollectionBackUp, this.ResolveCurrentOrganization());
			}
		}

		internal const string FileDataParameterName = "FileData";

		internal const string MigrationSourceParameterName = "MigrationSource";

		internal const string E15FormatValue = "cmdlet";

		private static readonly HashSet<string> AllowedCommands = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"New-TransportRule"
		};

		protected bool ContinueOnFailure;

		private readonly string ruleCollectionName;

		private TransportRuleCollection transportRuleCollectionBackUp;
	}
}
