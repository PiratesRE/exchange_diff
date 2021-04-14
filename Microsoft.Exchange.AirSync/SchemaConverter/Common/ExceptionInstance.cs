using System;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync.SchemaConverter.Common
{
	[Serializable]
	internal class ExceptionInstance
	{
		public ExceptionInstance(ExDateTime exceptionStartTime, byte deleted)
		{
			AirSyncDiagnostics.TraceInfo<ExDateTime, byte>(ExTraceGlobals.CommonTracer, this, "ExceptionInstance Created exceptionStartTime={0} deleted={1}", exceptionStartTime, deleted);
			this.exceptionStartTime = exceptionStartTime;
			this.deleted = deleted;
		}

		public IPropertyContainer ModifiedException
		{
			get
			{
				return this.modifiedException;
			}
			set
			{
				this.modifiedException = value;
			}
		}

		public ExDateTime ExceptionStartTime
		{
			get
			{
				return this.exceptionStartTime;
			}
		}

		public byte Deleted
		{
			get
			{
				return this.deleted;
			}
		}

		private byte deleted;

		private ExDateTime exceptionStartTime;

		private IPropertyContainer modifiedException;
	}
}
