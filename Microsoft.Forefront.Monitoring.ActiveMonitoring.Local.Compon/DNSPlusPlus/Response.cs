using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.DNSPlusPlus
{
	internal class Response
	{
		public Response(byte[] message)
		{
			if (message == null || message.Length < 12 || message.Length > 512)
			{
				throw new FormatException(string.Format("Invalid message length, expected >=12 and <=512, actual ={0}", (message == null) ? 0 : message.Length));
			}
			this.ResponseId = DnsHelper.GetUShort(message, 0);
			byte @byte = DnsHelper.GetByte(message, 2);
			this.IsResponse = ((@byte & 128) > 0);
			if (!this.IsResponse)
			{
				throw new FormatException("Invalid value for IsResponse expected:True, actual:False");
			}
			this.IsAuthoritativeAnswer = ((@byte & 4) > 0);
			this.IsTruncated = ((@byte & 2) > 0);
			this.IsRecursionDesired = ((@byte & 1) > 0);
			byte byte2 = DnsHelper.GetByte(message, 3);
			this.IsRecursionAvailable = ((byte2 & 128) > 0);
			this.Z = (byte)(byte2 >> 4 & 7);
			this.ResponseCode = (QueryResponseCode)(byte2 & 15);
			this.QuestionCount = (int)DnsHelper.GetUShort(message, 4);
			this.AnswerCount = (int)DnsHelper.GetUShort(message, 6);
			this.NameserverCount = (int)DnsHelper.GetUShort(message, 8);
			this.AdditionalRecordCount = (int)DnsHelper.GetUShort(message, 10);
			int position = 12;
			this.Questions = new Question[this.QuestionCount];
			for (int i = 0; i < this.QuestionCount; i++)
			{
				this.Questions[i] = new Question();
				position = this.Questions[i].ProcessResponse(message, position);
			}
			this.Answers = new Answer[this.AnswerCount];
			for (int j = 0; j < this.AnswerCount; j++)
			{
				this.Answers[j] = new Answer();
				position = this.Answers[j].ProcessMessage(message, position);
			}
			this.NSServers = new Answer[this.NameserverCount];
			for (int k = 0; k < this.NameserverCount; k++)
			{
				this.NSServers[k] = new Answer();
				position = this.NSServers[k].ProcessMessage(message, position);
			}
		}

		public QueryResponseCode ResponseCode { get; private set; }

		public bool IsAuthoritativeAnswer { get; private set; }

		public bool IsRecursionAvailable { get; private set; }

		public bool IsRecursionDesired { get; private set; }

		public bool IsTruncated { get; private set; }

		public ushort ResponseId { get; private set; }

		public bool IsResponse { get; private set; }

		public byte Z { get; private set; }

		public int QuestionCount { get; private set; }

		public int AnswerCount { get; private set; }

		public int NameserverCount { get; private set; }

		public int AdditionalRecordCount { get; private set; }

		public Question[] Questions { get; private set; }

		public Answer[] Answers { get; private set; }

		public Answer[] NSServers { get; private set; }
	}
}
