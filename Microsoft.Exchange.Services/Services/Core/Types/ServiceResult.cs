using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ServiceResult<TValue>
	{
		public ServiceResult(TValue value, ServiceError error)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (error == null)
			{
				throw new ArgumentNullException("error");
			}
			this.resultValue = value;
			this.error = error;
			this.code = ServiceResultCode.Warning;
		}

		public ServiceResult(TValue value)
		{
			this.resultValue = value;
			this.code = ServiceResultCode.Success;
		}

		public ServiceResult(ServiceError error)
		{
			if (error == null)
			{
				throw new ArgumentNullException("error");
			}
			this.error = error;
			this.code = ServiceResultCode.Error;
		}

		public ServiceResult(ServiceResultCode code, TValue value, ServiceError error)
		{
			bool flag = code == ServiceResultCode.Success;
			bool flag2 = code == ServiceResultCode.Error || code == ServiceResultCode.Warning;
			if (flag && value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (flag2 && error == null)
			{
				throw new ArgumentNullException("error");
			}
			if (!flag2 && error != null)
			{
				throw new ArgumentException("Must be null", "error");
			}
			this.resultValue = value;
			this.error = error;
			this.code = code;
		}

		public TValue Value
		{
			get
			{
				return this.resultValue;
			}
		}

		public ServiceError Error
		{
			get
			{
				return this.error;
			}
			set
			{
				this.error = value;
			}
		}

		public ServiceResultCode Code
		{
			get
			{
				return this.code;
			}
			set
			{
				this.code = value;
			}
		}

		public bool IsStopBatchProcessingError
		{
			get
			{
				return this.error != null && this.error.StopsBatchProcessing;
			}
		}

		internal Dictionary<string, object> RequestData
		{
			get
			{
				return this.requestData;
			}
		}

		public static void ProcessServiceResults(ServiceResult<TValue>[] results, ProcessServiceResult<TValue> processDelegate)
		{
			ServiceError serviceError = null;
			bool flag = false;
			ServiceResult<TValue> serviceResult = null;
			for (int i = 0; i < results.Length; i++)
			{
				ServiceResult<TValue> serviceResult2 = results[i];
				if (serviceError == null)
				{
					serviceError = serviceResult2.Error;
				}
				if (flag)
				{
					if (serviceResult == null)
					{
						serviceResult = new ServiceResult<TValue>(ServiceResultCode.Warning, default(TValue), ServiceError.CreateBatchProcessingStoppedError());
					}
					serviceResult2 = serviceResult;
					results[i] = serviceResult2;
				}
				else
				{
					flag = serviceResult2.IsStopBatchProcessingError;
				}
				if (processDelegate != null)
				{
					processDelegate(serviceResult2);
				}
			}
			if (results.Length == 1 && CallContext.Current != null && CallContext.Current.IsOwa)
			{
				ServiceResult<TValue> serviceResult3 = results[0];
				if (serviceResult3.Error != null && serviceResult3.Error.IsTransient)
				{
					CallContext.Current.IsTransientErrorResponse = true;
				}
			}
		}

		private TValue resultValue = default(TValue);

		private ServiceError error;

		private ServiceResultCode code;

		private Dictionary<string, object> requestData = new Dictionary<string, object>();
	}
}
