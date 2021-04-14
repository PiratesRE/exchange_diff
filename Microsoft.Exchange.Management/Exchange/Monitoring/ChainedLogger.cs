using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring
{
	internal abstract class ChainedLogger
	{
		public void WriteVerbose(LocalizedString message)
		{
			this.InternalWriteVerbose(message);
			if (this.nextLogger != null)
			{
				this.nextLogger.WriteVerbose(message);
			}
		}

		public void WriteDebug(LocalizedString message)
		{
			this.InternalWriteDebug(message);
			if (this.nextLogger != null)
			{
				this.nextLogger.WriteDebug(message);
			}
		}

		public void WriteWarning(LocalizedString message)
		{
			this.InternalWriteWarning(message);
			if (this.nextLogger != null)
			{
				this.nextLogger.WriteWarning(message);
			}
		}

		public void Append(ChainedLogger nextLogger)
		{
			if (this.nextLogger == null)
			{
				this.nextLogger = nextLogger;
				return;
			}
			this.nextLogger.Append(nextLogger);
		}

		public string GetDiagnosticInfo(string prefix)
		{
			string text = this.InternalGetDiagnosticInfo(prefix);
			if (this.nextLogger != null)
			{
				text = string.Format("{0}{1}", text, this.nextLogger.GetDiagnosticInfo(string.Empty));
			}
			return text;
		}

		protected abstract void InternalWriteVerbose(LocalizedString message);

		protected abstract void InternalWriteDebug(LocalizedString message);

		protected abstract void InternalWriteWarning(LocalizedString message);

		protected abstract string InternalGetDiagnosticInfo(string prefix);

		private ChainedLogger nextLogger;
	}
}
