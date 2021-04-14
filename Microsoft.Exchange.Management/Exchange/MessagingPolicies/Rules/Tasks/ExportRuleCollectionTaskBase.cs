using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	public abstract class ExportRuleCollectionTaskBase : GetMultitenancySystemConfigurationObjectTask<RuleIdParameter, TransportRule>
	{
		protected string RuleCollectionName
		{
			get
			{
				return this.ruleCollectionName;
			}
			set
			{
				this.ruleCollectionName = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			ADRuleStorageManager adruleStorageManager = this.RuleStorageManager;
			if (adruleStorageManager == null)
			{
				return;
			}
			using (Stream stream = new MemoryStream())
			{
				using (StreamWriter streamWriter = new StreamWriter(stream))
				{
					adruleStorageManager.LoadRuleCollectionWithoutParsing();
					IEnumerable<Rule> source = adruleStorageManager.WriteToStream(streamWriter, ExportRuleCollectionTaskBase.MaxLegacyFormatVersion, null);
					stream.Seek(0L, SeekOrigin.Begin);
					BinaryReader binaryReader = new BinaryReader(stream);
					BinaryFileDataObject dataObject = new BinaryFileDataObject
					{
						FileData = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length)
					};
					this.WriteResult(dataObject);
					if (source.Any<Rule>())
					{
						this.WriteWarning(Strings.ExportSkippedE15Rules(source.Count<Rule>()));
					}
				}
			}
			try
			{
				adruleStorageManager.ParseRuleCollection();
			}
			catch (ParserException ex)
			{
				this.WriteWarning(Strings.CorruptRuleCollection(ex.Message));
			}
		}

		internal ADRuleStorageManager RuleStorageManager
		{
			get
			{
				if (this.ruleStorageManager == null)
				{
					try
					{
						return new ADRuleStorageManager(this.ruleCollectionName, base.DataSession);
					}
					catch (RuleCollectionNotInAdException)
					{
						this.WriteWarning(Strings.RuleCollectionNotFoundDuringExport(this.RuleCollectionName));
						return null;
					}
				}
				return this.ruleStorageManager;
			}
			set
			{
				this.ruleStorageManager = value;
			}
		}

		private static Version MaxLegacyFormatVersion = new Version("14.00.0000.00");

		private string ruleCollectionName;

		private ADRuleStorageManager ruleStorageManager;
	}
}
