using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Cmdlet("Export", "TransportRuleCollection", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class ExportTransportRuleCollection : ExportRuleCollectionTaskBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageExportTransportRuleCollection;
			}
		}

		public ExportTransportRuleCollection()
		{
			this.supportedPredicates = TransportRulePredicate.GetAvailablePredicateMappings();
			this.supportedActions = TransportRuleAction.GetAvailableActionMappings();
			base.RuleCollectionName = Utils.RuleCollectionNameFromRole();
			this.Format = RuleCollectionFormat.RuleCollectionXML;
		}

		[Parameter(Mandatory = false)]
		public RuleCollectionFormat Format
		{
			get
			{
				return (RuleCollectionFormat)base.Fields["Format"];
			}
			set
			{
				base.Fields["Format"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			if (base.NeedSuppressingPiiData)
			{
				base.ExchangeRunspaceConfig.EnablePiiMap = true;
			}
			switch (this.Format)
			{
			case RuleCollectionFormat.RuleCollectionXML:
				this.WriteFormattedRules();
				return;
			case RuleCollectionFormat.InternalXML:
				this.WriteRawRules();
				return;
			default:
				return;
			}
		}

		private void WriteFormattedRules()
		{
			ADRuleStorageManager ruleStorageManager = base.RuleStorageManager;
			ruleStorageManager.LoadRuleCollection();
			IEnumerable<TransportRuleHandle> ruleHandles = ruleStorageManager.GetRuleHandles();
			List<Rule> list = new List<Rule>();
			foreach (TransportRuleHandle transportRuleHandle in ruleHandles)
			{
				string text = null;
				try
				{
					Rule rule = Rule.CreateFromInternalRule(this.supportedPredicates, this.supportedActions, transportRuleHandle.Rule, transportRuleHandle.AdRule.Priority, transportRuleHandle.AdRule);
					if (base.NeedSuppressingPiiData)
					{
						rule.SuppressPiiData(Utils.GetSessionPiiMap(base.ExchangeRunspaceConfig));
					}
					list.Add(rule);
				}
				catch (ArgumentException ex)
				{
					text = ex.Message;
				}
				catch (InvalidOperationException ex2)
				{
					text = ex2.Message;
				}
				catch (ParserException ex3)
				{
					text = ex3.Message;
				}
				catch (RulesValidationException ex4)
				{
					text = ex4.Message;
				}
				if (text != null)
				{
					base.WriteWarning(Strings.ErrorObjectHasValidationErrorsWithId(transportRuleHandle.AdRule.Identity.ToString()) + " " + text);
				}
			}
			this.WriteResult(new BinaryFileDataObject
			{
				FileData = PowershellTransportRuleSerializer.Serialize(list)
			});
		}

		private void WriteRawRules()
		{
			ADRuleStorageManager ruleStorageManager = base.RuleStorageManager;
			using (Stream stream = new MemoryStream())
			{
				using (StreamWriter streamWriter = new StreamWriter(stream))
				{
					ruleStorageManager.LoadRuleCollectionWithoutParsing();
					ruleStorageManager.WriteRawRulesToStream(streamWriter);
					if (base.NeedSuppressingPiiData)
					{
						stream.Seek(0L, SeekOrigin.Begin);
						StreamReader streamReader = new StreamReader(stream);
						string value = SuppressingPiiData.Redact(streamReader.ReadToEnd());
						stream.SetLength(0L);
						streamWriter.Write(value);
						streamWriter.Flush();
					}
					stream.Seek(0L, SeekOrigin.Begin);
					using (BinaryReader binaryReader = new BinaryReader(stream))
					{
						BinaryFileDataObject dataObject = new BinaryFileDataObject
						{
							FileData = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length)
						};
						this.WriteResult(dataObject);
					}
				}
			}
		}

		private const string RuleCollectionFormatParameter = "Format";

		private readonly TypeMapping[] supportedPredicates;

		private readonly TypeMapping[] supportedActions;
	}
}
