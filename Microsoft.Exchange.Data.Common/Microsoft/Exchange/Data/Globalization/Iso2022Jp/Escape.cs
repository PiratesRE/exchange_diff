using System;

namespace Microsoft.Exchange.Data.Globalization.Iso2022Jp
{
	internal class Escape
	{
		public EscapeState State { get; set; }

		public int BytesInCurrentBuffer { get; set; }

		public int TotalBytes { get; set; }

		public string ErrorMessage { get; set; }

		public Escape()
		{
		}

		public Escape(Escape from)
		{
			this.Sequence = from.Sequence;
			this.State = from.State;
			this.BytesInCurrentBuffer = from.BytesInCurrentBuffer;
			this.TotalBytes = from.TotalBytes;
			this.ErrorMessage = from.ErrorMessage;
		}

		public void Reset()
		{
			this.Sequence = EscapeSequence.None;
			this.State = EscapeState.Begin;
			this.BytesInCurrentBuffer = 0;
			this.TotalBytes = 0;
			this.ErrorMessage = null;
		}

		public bool IsValidEscapeSequence
		{
			get
			{
				return this.Sequence > EscapeSequence.Incomplete;
			}
		}

		public char Abbreviation
		{
			get
			{
				switch (this.Sequence)
				{
				case EscapeSequence.None:
				case EscapeSequence.NotRecognized:
				case EscapeSequence.Incomplete:
					return 'x';
				case EscapeSequence.JisX0208_1978:
					return 'e';
				case EscapeSequence.JisX0208_1983:
					return 'E';
				case EscapeSequence.JisX0201K_1976:
					return 'K';
				case EscapeSequence.JisX0201_1976:
					return 'k';
				case EscapeSequence.JisX0212_1990:
					return 'm';
				case EscapeSequence.JisX0208_Nec:
					return 'n';
				case EscapeSequence.Iso646Irv:
					return 'a';
				case EscapeSequence.ShiftIn:
					return 'i';
				case EscapeSequence.ShiftOut:
					return 'o';
				case EscapeSequence.JisX0208_1990:
					return 'Z';
				case EscapeSequence.Cns11643_1992_1:
					return 'Y';
				case EscapeSequence.Kcs5601_1987:
					return 'R';
				case EscapeSequence.Unknown_1:
					return 'u';
				case EscapeSequence.EucKsc:
					return 'r';
				case EscapeSequence.Gb2312_1980:
					return 'y';
				case EscapeSequence.NECKanjiIn:
					return 'w';
				default:
					return 'X';
				}
			}
		}

		public EscapeSequence Sequence;
	}
}
