using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Transport.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AsyncDecryptionOperationResult<TData> : AsyncOperationResult<TData>
	{
		public AsyncDecryptionOperationResult(TData data, Exception exception) : base(data, exception)
		{
			if (base.Exception != null)
			{
				if (base.Exception is StorageTransientException)
				{
					this.isTransientException = true;
					this.isKnownException = true;
					return;
				}
				if (base.Exception is StoragePermanentException)
				{
					this.isTransientException = false;
					this.isKnownException = true;
					return;
				}
				if (base.Exception is RightsManagementException)
				{
					this.isTransientException = !(base.Exception as RightsManagementException).IsPermanent;
					this.isKnownException = true;
					return;
				}
				if (base.Exception is ExchangeConfigurationException)
				{
					this.isTransientException = true;
					this.isKnownException = true;
					return;
				}
				this.isTransientException = false;
				this.isKnownException = false;
			}
		}

		public override bool IsTransientException
		{
			get
			{
				return this.isTransientException;
			}
		}

		public bool IsKnownException
		{
			get
			{
				return this.isKnownException;
			}
		}

		private readonly bool isTransientException;

		private readonly bool isKnownException;
	}
}
