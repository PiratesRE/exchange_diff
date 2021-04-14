using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMPhoneSession
{
	[Serializable]
	public class UMPhoneSession : ConfigurableObject
	{
		public UMPhoneSession() : base(new SimpleProviderPropertyBag())
		{
		}

		internal static UMPhoneSessionSchema Schema
		{
			get
			{
				return UMPhoneSession.schema;
			}
		}

		public UMCallState CallState
		{
			get
			{
				return (UMCallState)this[UMPhoneSessionSchema.CallState];
			}
			set
			{
				this[UMPhoneSessionSchema.CallState] = value;
			}
		}

		public UMOperationResult OperationResult
		{
			get
			{
				return (UMOperationResult)this[UMPhoneSessionSchema.OperationResult];
			}
			set
			{
				this[UMPhoneSessionSchema.OperationResult] = value;
			}
		}

		public UMEventCause EventCause
		{
			get
			{
				return (UMEventCause)this[UMPhoneSessionSchema.EventCause];
			}
			set
			{
				this[UMPhoneSessionSchema.EventCause] = value;
			}
		}

		internal string PhoneNumber
		{
			get
			{
				return (string)this[UMPhoneSessionSchema.PhoneNumber];
			}
			set
			{
				this[UMPhoneSessionSchema.PhoneNumber] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return UMPhoneSession.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static UMPhoneSessionSchema schema = new UMPhoneSessionSchema();
	}
}
