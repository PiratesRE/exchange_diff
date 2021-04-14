using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal abstract class AsyncServiceCommand<T> : WcfServiceCommandBase
	{
		protected AsyncServiceCommand(CallContext callContext) : base(callContext)
		{
		}

		internal async Task<T> Execute()
		{
			base.PrepareBudgetAndActivityScope();
			T result;
			if (this.scope != null)
			{
				base.PreExecute();
				try
				{
					result = await this.DetectDuplicatedCallOrExecute();
					goto IL_149;
				}
				finally
				{
					base.PostExecute();
				}
			}
			result = await this.DetectDuplicatedCallOrExecute();
			IL_149:
			return result;
		}

		protected abstract Task<T> InternalExecute();

		private async Task<T> DetectDuplicatedCallOrExecute()
		{
			T results;
			T result;
			if (base.CallContext.TryGetResponse<T>(out results))
			{
				result = results;
			}
			else
			{
				try
				{
					results = await this.InternalExecute();
					base.CallContext.SetResponse<T>(results, null);
					result = results;
				}
				catch (FaultException exception)
				{
					base.CallContext.SetResponse<T>(default(T), exception);
					throw;
				}
				catch (LocalizedException exception2)
				{
					base.CallContext.SetResponse<T>(default(T), exception2);
					throw;
				}
				finally
				{
					base.LogRequestTraces();
				}
			}
			return result;
		}
	}
}
