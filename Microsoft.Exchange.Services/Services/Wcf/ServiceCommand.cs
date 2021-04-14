using System;
using System.ServiceModel;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal abstract class ServiceCommand<T> : WcfServiceCommandBase
	{
		protected ServiceCommand(CallContext callContext) : base(callContext)
		{
		}

		internal T Execute()
		{
			base.PrepareBudgetAndActivityScope();
			T result;
			if (this.scope != null)
			{
				using (CpuTracker.StartCpuTracking("CMD"))
				{
					base.PreExecute();
					try
					{
						result = this.DetectDuplicatedCallOrExecute();
					}
					finally
					{
						base.PostExecute();
					}
					return result;
				}
			}
			result = this.DetectDuplicatedCallOrExecute();
			return result;
		}

		protected abstract T InternalExecute();

		private T DetectDuplicatedCallOrExecute()
		{
			T t;
			if (base.CallContext.TryGetResponse<T>(out t))
			{
				return t;
			}
			T result;
			try
			{
				t = this.InternalExecute();
				base.CallContext.SetResponse<T>(t, null);
				result = t;
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
			return result;
		}

		public IdAndSession GetIdAndSession(BaseFolderId folder)
		{
			return new IdConverter(base.CallContext).ConvertFolderIdToIdAndSession(folder, IdConverter.ConvertOption.IgnoreChangeKey);
		}
	}
}
