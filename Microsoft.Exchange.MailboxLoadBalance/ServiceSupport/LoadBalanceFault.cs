using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalance.ServiceSupport
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class LoadBalanceFault
	{
		public LoadBalanceFault.LbErrorType ErrorType { get; private set; }

		public LocalizedString Message { get; private set; }

		[DataMember]
		public byte[] MessageData
		{
			get
			{
				return CommonUtils.ByteSerialize(this.Message);
			}
			set
			{
				this.Message = CommonUtils.ByteDeserialize(value);
			}
		}

		[DataMember]
		public string StackTrace { get; private set; }

		[DataMember]
		public string ExceptionType { get; private set; }

		[DataMember(EmitDefaultValue = false)]
		public LoadBalanceFault InnerException { get; private set; }

		public static void Throw(Exception ex)
		{
			LoadBalanceFault loadBalanceFault = LoadBalanceFault.Create(ex);
			throw new FaultException<LoadBalanceFault>(loadBalanceFault, loadBalanceFault.Message);
		}

		public void ReconstructAndThrow()
		{
			throw this.Reconstruct();
		}

		private static LoadBalanceFault Create(Exception ex)
		{
			if (ex == null)
			{
				return null;
			}
			LoadBalanceFault loadBalanceFault = new LoadBalanceFault();
			LocalizedException ex2 = ex as LocalizedException;
			if (ex2 != null)
			{
				loadBalanceFault.Message = ex2.LocalizedString;
			}
			else
			{
				loadBalanceFault.Message = new LocalizedString(ex.Message);
			}
			loadBalanceFault.ExceptionType = CommonUtils.GetFailureType(ex);
			loadBalanceFault.StackTrace = ex.StackTrace;
			loadBalanceFault.ErrorType = (CommonUtils.IsTransientException(ex) ? LoadBalanceFault.LbErrorType.Transient : LoadBalanceFault.LbErrorType.Permanent);
			loadBalanceFault.InnerException = LoadBalanceFault.Create(ex.InnerException);
			return loadBalanceFault;
		}

		private Exception Reconstruct()
		{
			Exception innerException = (this.InnerException != null) ? this.InnerException.Reconstruct() : null;
			LocalizedException result;
			if (this.ErrorType == LoadBalanceFault.LbErrorType.Transient)
			{
				result = new RemoteMailboxLoadBalanceTransientException(this.Message, innerException);
			}
			else
			{
				result = new RemoteMailboxLoadBalancePermanentException(this.Message, innerException);
			}
			return result;
		}

		public enum LbErrorType
		{
			Transient,
			Permanent
		}
	}
}
