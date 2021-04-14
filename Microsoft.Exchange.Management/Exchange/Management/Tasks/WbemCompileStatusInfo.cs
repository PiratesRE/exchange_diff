using System;

namespace Microsoft.Exchange.Management.Tasks
{
	internal struct WbemCompileStatusInfo
	{
		internal void InitializeStatusInfo()
		{
			this.phaseError = 0;
			this.hresult = 0;
			this.objectNum = 0;
			this.firstLine = 0;
			this.lastLine = 0;
			this.outFlags = 0;
		}

		internal int PhaseError
		{
			get
			{
				return this.phaseError;
			}
		}

		internal int HResult
		{
			get
			{
				return this.hresult;
			}
		}

		internal int FirstLine
		{
			get
			{
				return this.firstLine;
			}
		}

		internal int LastLine
		{
			get
			{
				return this.lastLine;
			}
		}

		internal int OutFlags
		{
			get
			{
				return this.outFlags;
			}
		}

		private int phaseError;

		private int hresult;

		private int objectNum;

		private int firstLine;

		private int lastLine;

		private int outFlags;
	}
}
