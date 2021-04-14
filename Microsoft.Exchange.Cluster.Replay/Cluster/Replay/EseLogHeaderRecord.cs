using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal sealed class EseLogHeaderRecord : EseLogRecord
	{
		internal static string Identifier
		{
			get
			{
				return "LHGI";
			}
		}

		public override string LogRecType
		{
			get
			{
				return EseLogHeaderRecord.Identifier;
			}
		}

		protected override Regex Regex()
		{
			return EseLogHeaderRecord.regex;
		}

		internal EseLogHeaderRecord(string input)
		{
			Match match = base.Match(input);
			this.m_signature = match.Groups["Signature"].ToString();
			this.m_generation = long.Parse(match.Groups["Generation"].ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			this.m_logFormatVersion = match.Groups["LogFormatVersion"].ToString();
			this.m_creationTime = DateTime.ParseExact(match.Groups["CreationTime"].ToString(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
			if ("00/00/1900 00:00:00" == match.Groups["PreviousGenerationCreationTime"].ToString())
			{
				DiagCore.RetailAssert(1L == this.m_generation, "Generation {0} has a blank PrevGenCreationTime ({1}). input is {2}, regex is {3}", new object[]
				{
					this.m_generation,
					match.Groups["PreviousGenerationCreationTime"].ToString(),
					input,
					EseLogHeaderRecord.regex.ToString()
				});
				this.m_previousGenerationCreationTime = DateTime.MinValue;
			}
			else
			{
				this.m_previousGenerationCreationTime = DateTime.ParseExact(match.Groups["PreviousGenerationCreationTime"].ToString(), "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
			}
			string text = match.Groups["CircularLogging"].ToString();
			if ("0x1" == text)
			{
				this.m_isCircularLoggingOn = true;
			}
			else if ("0x0" == text)
			{
				this.m_isCircularLoggingOn = false;
			}
			else
			{
				ExDiagnostics.FailFast(string.Format(CultureInfo.CurrentCulture, "circular logging field {0} failed to match {1} or {2}. input is {3}, regex is {4}", new object[]
				{
					text,
					"0x1",
					"0x0",
					input,
					EseLogHeaderRecord.regex.ToString()
				}), true);
			}
			this.SectorSize = int.Parse(match.Groups["SectorSizeGroup"].ToString());
		}

		public string Signature
		{
			get
			{
				return this.m_signature;
			}
		}

		public long Generation
		{
			get
			{
				return this.m_generation;
			}
		}

		public DateTime CreationTime
		{
			get
			{
				return this.m_creationTime;
			}
		}

		public DateTime PreviousGenerationCreationTime
		{
			get
			{
				return this.m_previousGenerationCreationTime;
			}
		}

		public string LogFormatVersion
		{
			get
			{
				return this.m_logFormatVersion;
			}
		}

		public bool IsCircularLoggingOn
		{
			get
			{
				return this.m_isCircularLoggingOn;
			}
		}

		public int SectorSize { get; private set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "Signature={0},Generation={1},CreationTime={2},PrevGenCreationTime={3},Format={4},CircularLogging={5},SectorSize={6}", new object[]
			{
				this.m_signature,
				this.m_generation,
				this.m_creationTime,
				this.m_previousGenerationCreationTime,
				this.m_logFormatVersion,
				this.m_isCircularLoggingOn,
				this.SectorSize
			});
		}

		private const string SignatureGroup = "Signature";

		private const string GenerationGroup = "Generation";

		private const string CreationTimeGroup = "CreationTime";

		private const string PreviousGenerationCreationTimeGroup = "PreviousGenerationCreationTime";

		private const string LogFormatVersionGroup = "LogFormatVersion";

		private const string CircularLoggingGroup = "CircularLogging";

		private const string SectorSizeGroup = "SectorSizeGroup";

		private const string EseNullTime = "00/00/1900 00:00:00";

		private const string DateTimeRegex = "[0-1]\\d/[0-3]\\d/\\d\\d\\d\\d [0-2]\\d:[0-5]\\d:[0-5]\\d";

		private const string DateTimeFormat = "MM/dd/yyyy HH:mm:ss";

		private const string CircularLoggingOn = "0x1";

		private const string CircularLoggingOff = "0x0";

		private readonly string m_signature;

		private readonly long m_generation;

		private readonly DateTime m_creationTime;

		private readonly DateTime m_previousGenerationCreationTime;

		private readonly string m_logFormatVersion;

		private readonly bool m_isCircularLoggingOn;

		private static readonly Regex regex = new Regex(string.Format(CultureInfo.InvariantCulture, "^{0}\\s*,\\s*(?<{1}>[^,]+?)\\s*,\\s*(?<{2}>[0-9A-F]+)\\s*,\\s*(?<{3}>{4})\\s*,\\s*(?<{5}>{6})\\s*,\\s*(?<{7}>[^,]+?)\\s*,\\s*(?<{8}>{9}|{10})\\s*,\\s*(?<{11}>[0-9]+)\\s*$", new object[]
		{
			EseLogHeaderRecord.Identifier,
			"Signature",
			"Generation",
			"CreationTime",
			"[0-1]\\d/[0-3]\\d/\\d\\d\\d\\d [0-2]\\d:[0-5]\\d:[0-5]\\d",
			"PreviousGenerationCreationTime",
			"[0-1]\\d/[0-3]\\d/\\d\\d\\d\\d [0-2]\\d:[0-5]\\d:[0-5]\\d",
			"LogFormatVersion",
			"CircularLogging",
			"0x1",
			"0x0",
			"SectorSizeGroup"
		}), RegexOptions.CultureInvariant);
	}
}
