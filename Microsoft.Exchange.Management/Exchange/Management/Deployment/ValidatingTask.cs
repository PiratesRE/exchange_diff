using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public abstract class ValidatingTask : Task
	{
		internal ValidatingCondition[] ValidationTests
		{
			get
			{
				return this.validationTests;
			}
			set
			{
				this.validationTests = value;
			}
		}

		internal string[] GetTestDescriptions()
		{
			TaskLogger.LogEnter();
			string[] array = new string[this.ValidationTests.Length];
			for (int i = 0; i < this.ValidationTests.Length; i++)
			{
				array[i] = this.ValidationTests[i].Description.ToString();
			}
			TaskLogger.LogExit();
			return array;
		}

		protected sealed override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			bool flag = true;
			foreach (ValidatingCondition validatingCondition in this.ValidationTests)
			{
				ValidatingTaskResult validatingTaskResult = new ValidatingTaskResult();
				validatingTaskResult.ConditionDescription = validatingCondition.Description.ToString();
				if (flag)
				{
					bool flag2 = false;
					try
					{
						flag2 = validatingCondition.Validate();
					}
					catch (LocalizedException ex)
					{
						TaskLogger.LogError(new LocalizedException(Strings.ExceptionValidatingConditionFailed(ex.Message), ex));
						validatingTaskResult.FailureDetails = ex;
						flag2 = false;
					}
					validatingTaskResult.Result = (flag2 ? ValidatingTaskResult.ResultType.Passed : ValidatingTaskResult.ResultType.Failed);
					if (!flag2 && validatingCondition.AbortValidationIfFailed)
					{
						flag = false;
					}
				}
				base.WriteObject(validatingTaskResult);
			}
			TaskLogger.LogExit();
		}

		private ValidatingCondition[] validationTests;
	}
}
