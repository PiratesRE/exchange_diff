using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;

namespace Microsoft.Exchange.Management.Deployment.HybridConfigurationDetection
{
	internal class PowershellHostUI : PSHostUserInterface
	{
		public override PSHostRawUserInterface RawUI
		{
			get
			{
				return null;
			}
		}

		public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions)
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName, PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		public override string ReadLine()
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		public override SecureString ReadLineAsSecureString()
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
		{
		}

		public override void Write(string value)
		{
		}

		public override void WriteDebugLine(string message)
		{
		}

		public override void WriteLine(string value)
		{
		}

		public override void WriteVerboseLine(string message)
		{
		}

		public override void WriteProgress(long value, ProgressRecord record)
		{
		}

		public override void WriteErrorLine(string message)
		{
		}

		public override void WriteWarningLine(string message)
		{
		}
	}
}
