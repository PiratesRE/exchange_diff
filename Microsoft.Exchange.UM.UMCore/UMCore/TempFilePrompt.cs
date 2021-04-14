using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class TempFilePrompt : VariablePrompt<ITempWavFile>
	{
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3} Extra={4}", new object[]
			{
				"tempwave",
				base.Config.PromptName,
				this.filename,
				base.Config.PromptName,
				(this.tempWav.ExtraInfo == null) ? "<null>" : this.tempWav.ExtraInfo
			});
		}

		internal string FileName
		{
			get
			{
				return this.filename;
			}
		}

		internal override string ToSsml()
		{
			return FilePrompt.BuildAudioSsml(this.filename, base.ProsodyRate);
		}

		protected override void InternalInitialize()
		{
			if (base.InitVal == null)
			{
				return;
			}
			this.tempWav = base.InitVal;
			this.filename = this.tempWav.FilePath;
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "TempFilePrompt successfully intialized with filename {0}.", new object[]
			{
				this.tempWav.FilePath
			});
		}

		private string filename;

		private ITempWavFile tempWav;
	}
}
