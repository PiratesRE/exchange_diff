using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class VarFilePrompt : VariablePrompt<string>
	{
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Type={0}, Name={1}, File={2}, Value={3}", new object[]
			{
				"wave",
				this.filename,
				this.filename,
				this.filename
			});
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
			string path;
			if (Path.IsPathRooted(base.InitVal))
			{
				path = base.InitVal;
			}
			else
			{
				path = Path.Combine(Util.WavPathFromCulture(base.Culture), base.InitVal);
			}
			if (!File.Exists(path))
			{
				throw new FileNotFoundException(Strings.FileNotFound(path));
			}
			this.filename = path;
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "VarFilePrompt successfully intialized with filename {0}.", new object[]
			{
				this.filename
			});
		}

		private string filename;
	}
}
