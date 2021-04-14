using System;
using System.CodeDom.Compiler;

namespace Microsoft.WindowsAzure.ActiveDirectoryV142
{
	public class InvitationTicket
	{
		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string Type
		{
			get
			{
				return this._Type;
			}
			set
			{
				this._Type = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		public string Ticket
		{
			get
			{
				return this._Ticket;
			}
			set
			{
				this._Ticket = value;
			}
		}

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _Type;

		[GeneratedCode("System.Data.Services.Design", "1.0.0")]
		private string _Ticket;
	}
}
