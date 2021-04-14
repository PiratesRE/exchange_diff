using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.DxStore.Common
{
	[DataContract(Namespace = "http://www.outlook.com/highavailability/dxstore/v1/")]
	public class DxStoreServerFault
	{
		public DxStoreServerFault()
		{
		}

		public DxStoreServerFault(Exception ex, DxStoreFaultCode faultCode, bool isTransientError, bool isLocalized)
		{
			this.IsLocalizedException = isLocalized;
			this.FaultCode = faultCode;
			this.IsTransientError = isTransientError;
			this.TypeName = ex.GetType().FullName;
			this.Message = ex.Message;
			this.StackTrace = ex.StackTrace;
		}

		[DataMember]
		public bool IsTransientError { get; set; }

		[DataMember]
		public string TypeName { get; set; }

		[DataMember]
		public string Message { get; set; }

		[DataMember]
		public string StackTrace { get; set; }

		[DataMember]
		public string Context { get; set; }

		[DataMember]
		public int FaultCodeAsInt { get; private set; }

		[DataMember]
		public bool IsLocalizedException { get; set; }

		[IgnoreDataMember]
		public DxStoreFaultCode FaultCode
		{
			get
			{
				if (this.faultCode == null)
				{
					if (Enum.IsDefined(typeof(DxStoreFaultCode), this.FaultCodeAsInt))
					{
						this.faultCode = new DxStoreFaultCode?((DxStoreFaultCode)this.FaultCodeAsInt);
					}
					else
					{
						this.faultCode = new DxStoreFaultCode?(DxStoreFaultCode.Unknown);
					}
				}
				return this.faultCode.Value;
			}
			set
			{
				this.faultCode = new DxStoreFaultCode?(value);
				this.FaultCodeAsInt = (int)this.faultCode.Value;
			}
		}

		[IgnoreDataMember]
		private DxStoreFaultCode? faultCode;
	}
}
