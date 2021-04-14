using System;
using System.IO;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.ObjectModel.EventLog;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class PiiRedactionModuleBase : TaskIOPipelineBase, ITaskModule
	{
		private protected TaskContext CurrentTaskContext { protected get; private set; }

		private PiiMap CurrentPiiMap
		{
			get
			{
				if (this.CurrentTaskContext.ExchangeRunspaceConfig != null && this.CurrentTaskContext.ExchangeRunspaceConfig.EnablePiiMap)
				{
					return PiiMapManager.Instance.GetOrAdd(this.CurrentTaskContext.ExchangeRunspaceConfig.PiiMapId);
				}
				return null;
			}
		}

		public PiiRedactionModuleBase(TaskContext context)
		{
			this.CurrentTaskContext = context;
		}

		public virtual void Init(ITaskEvent task)
		{
			task.PreInit += this.Task_PreInit;
		}

		public virtual void Dispose()
		{
		}

		private void Task_PreInit(object sender, EventArgs e)
		{
			if (this.CurrentTaskContext.ExchangeRunspaceConfig != null && this.CurrentTaskContext.ExchangeRunspaceConfig.NeedSuppressingPiiData)
			{
				if (!SuppressingPiiProperty.Initialized)
				{
					this.InitializePiiRedaction();
				}
				this.CurrentTaskContext.CommandShell.PrependTaskIOPipelineHandler(this);
			}
		}

		protected IDisposable CreatePiiSuppressionContext(IConfigurable outputObject)
		{
			ConfigurableObject configurableObject = outputObject as ConfigurableObject;
			if (configurableObject == null)
			{
				return null;
			}
			if (configurableObject.SkipPiiRedaction || SuppressingPiiProperty.IsExcludedSchemaType(configurableObject.ObjectSchema.GetType()) || (VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.SkipPiiRedactionForForestWideObject.Enabled && TaskHelper.IsForestWideADObject(outputObject as ADObject)))
			{
				return null;
			}
			CmdletLogger.SafeSetLogger(this.CurrentTaskContext.UniqueId, RpsCmdletMetadata.IsOutputObjectRedacted, true);
			return SuppressingPiiContext.Create(true, this.CurrentPiiMap);
		}

		public override bool WriteVerbose(LocalizedString input, out LocalizedString output)
		{
			return this.RedactLocString(input, true, out output);
		}

		public override bool WriteDebug(LocalizedString input, out LocalizedString output)
		{
			return this.RedactLocString(input, true, out output);
		}

		public override bool WriteWarning(LocalizedString input, string helpUrl, out LocalizedString output)
		{
			return this.RedactLocString(input, false, out output);
		}

		private bool RedactLocString(LocalizedString input, bool hideNonRedacted, out LocalizedString output)
		{
			output = input;
			return input.FormatParameters == null || input.FormatParameters.Count <= 0 || SuppressingPiiData.TryRedactPiiLocString(input, this.CurrentPiiMap, out output) || !hideNonRedacted;
		}

		private void InitializePiiRedaction()
		{
			string text = Path.Combine(ExchangeSetupContext.InstallPath, "ClientAccess\\PowerShell-Proxy\\CmdletDataRedaction.xml");
			Exception ex = null;
			string text2;
			try
			{
				text2 = SuppressingPiiProperty.Initialize(text);
			}
			catch (IOException ex2)
			{
				ex = ex2;
				text2 = string.Format("IOException occurred while loading the configuration file. Please make sure the CmdletDataRedaction.xml is accessible. Expected path: {0}, Details: {1}", text, ex2);
			}
			catch (InvalidOperationException ex3)
			{
				ex = ex3;
				text2 = string.Format("An error occurred during deserializing file {0}, please make sure the XML file is well-formatted and complies with the XML schema definition. Details: {1}", text, ex3);
			}
			if (text2 != null)
			{
				if (ex != null)
				{
					this.CurrentTaskContext.CommandShell.WriteError(new LocalizedException(LocalizedString.Empty, ex), ExchangeErrorCategory.ServerOperation, text);
				}
				else
				{
					this.CurrentTaskContext.CommandShell.WriteWarning(Strings.PiiRedactionInitializationFailed(text2));
				}
				TaskLogger.LogEvent("All", TaskEventLogConstants.Tuple_FailedToInitailizeCmdletDataRedactionConfiguration, new object[]
				{
					text2
				});
			}
		}
	}
}
