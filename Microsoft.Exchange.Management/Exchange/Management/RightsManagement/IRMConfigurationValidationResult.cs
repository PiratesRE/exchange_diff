using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[Serializable]
	public sealed class IRMConfigurationValidationResult : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return IRMConfigurationValidationResult.Schema;
			}
		}

		public IRMConfigurationValidationResult() : base(new SimpleProviderPropertyBag())
		{
			this.overallResult = IRMConfigurationValidationResult.ResultType.OverallPass;
		}

		public string Results
		{
			get
			{
				return (string)this[IRMConfigurationValidationResultSchema.Results];
			}
			private set
			{
				this[IRMConfigurationValidationResultSchema.Results] = value;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal void SetTask(LocalizedString task)
		{
			this.currentTask = task;
		}

		internal bool SetSuccessResult(LocalizedString result)
		{
			this.SetResult(result, IRMConfigurationValidationResult.ResultType.Success, null);
			return true;
		}

		internal bool SetFailureResult(LocalizedString result, Exception ex = null, bool error = true)
		{
			this.SetResult(result, error ? IRMConfigurationValidationResult.ResultType.Error : IRMConfigurationValidationResult.ResultType.Warning, ex);
			if (error)
			{
				this.overallResult = IRMConfigurationValidationResult.ResultType.OverallFail;
			}
			else if (this.overallResult != IRMConfigurationValidationResult.ResultType.OverallFail)
			{
				this.overallResult = IRMConfigurationValidationResult.ResultType.OverallWarning;
			}
			return !error;
		}

		internal void SetOverallResult()
		{
			IRMConfigurationValidationResult.ValidationResultNode value = new IRMConfigurationValidationResult.ValidationResultNode(this.overallResult);
			this.list.AddLast(value);
		}

		private void SetResult(LocalizedString result, IRMConfigurationValidationResult.ResultType type, Exception ex)
		{
			IRMConfigurationValidationResult.ValidationResultNode value = new IRMConfigurationValidationResult.ValidationResultNode(this.currentTask, result, type, ex);
			this.list.AddLast(value);
		}

		internal void PrepareFinalOutput(OrganizationId organizationId)
		{
			this[SimpleProviderObjectSchema.Identity] = organizationId.OrganizationalUnit;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (IRMConfigurationValidationResult.ValidationResultNode validationResultNode in this.list)
			{
				stringBuilder.AppendLine(validationResultNode.ToString());
			}
			this.Results = stringBuilder.ToString();
			base.ResetChangeTracking();
		}

		private static readonly IRMConfigurationValidationResultSchema Schema = ObjectSchema.GetInstance<IRMConfigurationValidationResultSchema>();

		private IRMConfigurationValidationResult.ResultType overallResult = IRMConfigurationValidationResult.ResultType.OverallPass;

		private LinkedList<IRMConfigurationValidationResult.ValidationResultNode> list = new LinkedList<IRMConfigurationValidationResult.ValidationResultNode>();

		private LocalizedString currentTask;

		private enum ResultType
		{
			Success,
			Warning,
			Error,
			OverallPass,
			OverallWarning,
			OverallFail
		}

		[Serializable]
		private sealed class ValidationResultNode
		{
			public ValidationResultNode(LocalizedString task, LocalizedString result, IRMConfigurationValidationResult.ResultType type, Exception exception)
			{
				this.Task = task;
				this.Result = result;
				this.Type = type;
				this.Exception = exception;
			}

			public ValidationResultNode(IRMConfigurationValidationResult.ResultType type)
			{
				this.Task = LocalizedString.Empty;
				this.Result = LocalizedString.Empty;
				this.Type = type;
				this.Exception = null;
			}

			public override string ToString()
			{
				string text = string.Empty;
				if (this.Exception != null)
				{
					text = string.Format(CultureInfo.CurrentUICulture, "{0}{1}{2}{3}{4}", new object[]
					{
						"----------------------------------------",
						Environment.NewLine,
						this.Exception,
						Environment.NewLine,
						"----------------------------------------"
					});
				}
				switch (this.Type)
				{
				case IRMConfigurationValidationResult.ResultType.Warning:
					return string.Format(CultureInfo.CurrentUICulture, "{0}{1}    - {2}{3}{4}", new object[]
					{
						this.Task,
						Environment.NewLine,
						Strings.InfoWarning(this.Result),
						Environment.NewLine,
						text
					});
				case IRMConfigurationValidationResult.ResultType.Error:
					return string.Format(CultureInfo.CurrentUICulture, "{0}{1}    - {2}{3}{4}", new object[]
					{
						this.Task,
						Environment.NewLine,
						Strings.InfoError(this.Result),
						Environment.NewLine,
						text
					});
				case IRMConfigurationValidationResult.ResultType.OverallPass:
					return string.Format(CultureInfo.CurrentUICulture, "{0}{1}{2}", new object[]
					{
						Environment.NewLine,
						Strings.InfoOverallPass,
						Environment.NewLine
					});
				case IRMConfigurationValidationResult.ResultType.OverallWarning:
					return string.Format(CultureInfo.CurrentUICulture, "{0}{1}{2}", new object[]
					{
						Environment.NewLine,
						Strings.InfoOverallWarning,
						Environment.NewLine
					});
				case IRMConfigurationValidationResult.ResultType.OverallFail:
					return string.Format(CultureInfo.CurrentUICulture, "{0}{1}{2}", new object[]
					{
						Environment.NewLine,
						Strings.InfoOverallFail,
						Environment.NewLine
					});
				}
				return string.Format(CultureInfo.CurrentUICulture, "{0}{1}    - {2}", new object[]
				{
					this.Task,
					Environment.NewLine,
					Strings.InfoSuccess(this.Result)
				});
			}

			private const string Separator = "----------------------------------------";

			public readonly LocalizedString Task;

			public readonly LocalizedString Result;

			public readonly IRMConfigurationValidationResult.ResultType Type;

			public readonly Exception Exception;
		}
	}
}
