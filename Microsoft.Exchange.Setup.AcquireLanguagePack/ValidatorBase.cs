using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal abstract class ValidatorBase : IDisposable
	{
		protected ValidatorBase()
		{
			this.validatedFiles = new List<string>();
			this.disposed = false;
		}

		public List<string> ValidatedFiles
		{
			get
			{
				return this.validatedFiles;
			}
		}

		protected Action<object> Callback { get; set; }

		protected void InvokeCallback(object obj)
		{
			if (this.Callback != null && obj != null)
			{
				this.Callback(obj);
			}
		}

		public abstract bool Validate();

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing && Directory.Exists(ValidatorBase.TempPath))
				{
					Directory.Delete(ValidatorBase.TempPath, true);
				}
				this.disposed = true;
			}
		}

		internal static readonly string TempPath = Path.Combine(Path.GetTempPath(), "ExchangeSetupValidatorTemp");

		protected List<string> validatedFiles;

		private bool disposed;
	}
}
