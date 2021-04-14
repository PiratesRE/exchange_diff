using System;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	internal sealed class StringLogger : ChainedLogger
	{
		internal StringLogger()
		{
			this.instance = new StringBuilder();
		}

		protected override void InternalWriteVerbose(LocalizedString message)
		{
			this.WriteLine(Strings.Verbose, message);
		}

		protected override void InternalWriteDebug(LocalizedString message)
		{
			this.WriteLine(Strings.Debug, message);
		}

		protected override void InternalWriteWarning(LocalizedString message)
		{
			this.WriteLine(Strings.Warning, message);
		}

		private void WriteLine(LocalizedString prefix, LocalizedString message)
		{
			this.instance.AppendLine(string.Format("{0}{1}", prefix, message));
		}

		protected override string InternalGetDiagnosticInfo(string prefix)
		{
			return string.Format("{0}{1}{2}", prefix, Environment.NewLine, this.instance);
		}

		private readonly StringBuilder instance;
	}
}
