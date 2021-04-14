using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class ResolverLog
	{
		public ResolverLog(string path, long correlator, bool logContent)
		{
			this.correlator = correlator;
			this.logContent = logContent;
			this.path = path;
			this.unique = Guid.NewGuid();
			string path2 = string.Format("resolver-{0}-{1}.log", correlator, this.unique);
			try
			{
				this.log = new StreamWriter(Path.Combine(path, path2), false, Encoding.UTF8);
			}
			catch (DirectoryNotFoundException)
			{
				Microsoft.Exchange.Diagnostics.Log.CreateLogDirectory(path);
				this.log = new StreamWriter(Path.Combine(path, path2), false, Encoding.UTF8);
			}
		}

		public StreamWriter Log
		{
			get
			{
				return this.log;
			}
		}

		public void LogMailItem(string title, TransportMailItem mailItem)
		{
			if (this.logContent)
			{
				string contentFileName = string.Format("content-{0}-{1}-{2}.eml", this.correlator, this.unique, this.counter);
				this.LogMailItem(title, mailItem, this.path, contentFileName);
			}
			else
			{
				this.LogMailItem(title, mailItem, null, null);
			}
			this.log.Flush();
			this.counter++;
		}

		public void Close()
		{
			this.log.Close();
		}

		private static void LogMailItemContent(TransportMailItem mailItem, string filename)
		{
			Stream stream = mailItem.OpenMimeReadStream();
			Stream stream2 = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
			byte[] array = new byte[65536];
			for (int i = stream.Read(array, 0, array.Length); i > 0; i = stream.Read(array, 0, array.Length))
			{
				stream2.Write(array, 0, i);
			}
			stream.Close();
			stream2.Close();
		}

		private void WriteLinesIndented(string indent, string data)
		{
			if (data == null)
			{
				this.log.WriteLine();
				return;
			}
			using (StringReader stringReader = new StringReader(data))
			{
				for (string value = stringReader.ReadLine(); value != null; value = stringReader.ReadLine())
				{
					this.log.Write(indent);
					this.log.WriteLine(value);
				}
			}
		}

		private void WriteBytesAsAscii(byte[] data, int start, int count)
		{
			for (int i = start; i < start + count; i++)
			{
				if (data[i] < 32 || data[i] > 127)
				{
					this.log.Write('.');
				}
				else
				{
					this.log.Write((char)data[i]);
				}
			}
		}

		private void WriteBytesIndented(string indent, byte[] data)
		{
			int i;
			for (i = 0; i < data.Length - 16; i += 16)
			{
				this.log.Write(indent);
				this.log.Write(HexConverter.ByteArrayToHexString(data, i, 16));
				this.log.Write(' ');
				this.WriteBytesAsAscii(data, i, 16);
				this.log.WriteLine();
			}
			if (i < data.Length)
			{
				this.log.Write(indent);
				this.log.Write(HexConverter.ByteArrayToHexString(data, i, data.Length - i));
				for (int j = data.Length - i; j < 16; j++)
				{
					this.log.Write("   ");
				}
				this.log.Write(' ');
				this.WriteBytesAsAscii(data, i, data.Length - i);
				this.log.WriteLine();
			}
		}

		private void LogMailItem(string title, TransportMailItem mailItem, string contentFilePath, string contentFileName)
		{
			this.log.WriteLine(title);
			this.log.WriteLine("  Mail item ({0})", contentFileName ?? "content not logged");
			this.log.WriteLine("    Id: {0}", mailItem.RecordId);
			this.log.WriteLine("    InternetMessageId: {0}", mailItem.InternetMessageId);
			this.log.WriteLine("    ReversePath: {0}", mailItem.From);
			this.log.WriteLine("    Subject: {0}", mailItem.Subject);
			this.log.WriteLine("    MimeSize: {0}", mailItem.MimeSize);
			this.LogP1Recipients(mailItem);
			this.log.WriteLine("---------------------------------------------------------------------");
			if (contentFileName != null)
			{
				ResolverLog.LogMailItemContent(mailItem, Path.Combine(contentFilePath, contentFileName));
			}
		}

		private void WriteValuesIndented<T>(string indent, IEnumerable<T> values)
		{
			foreach (T t in values)
			{
				this.log.WriteLine("{0}{1}", indent, t);
			}
		}

		private void LogP1Recipients(TransportMailItem mailItem)
		{
			this.log.WriteLine("    Recipients:");
			foreach (MailRecipient mailRecipient in mailItem.Recipients)
			{
				this.log.WriteLine("      P1 recipient:");
				this.log.WriteLine("        Email: {0}", mailRecipient.Email);
				this.log.WriteLine("        Id: {0}", mailRecipient.MsgRecordId);
				this.log.WriteLine("        AckStatus: {0}", mailRecipient.AckStatus);
				this.log.WriteLine("        Status: {0}", mailRecipient.Status);
				this.log.WriteLine("        SmtpResponse:");
				this.WriteLinesIndented("            ", mailRecipient.SmtpResponse.ToString());
				this.log.WriteLine("        DsnRequested: {0}", mailRecipient.DsnRequested);
				this.log.WriteLine("        DsnMessageId: {0}", mailRecipient.DsnMessageId);
				this.log.WriteLine("        ORcpt: {0}", mailRecipient.ORcpt);
			}
		}

		private StreamWriter log;

		private long correlator;

		private Guid unique;

		private string path;

		private bool logContent;

		private int counter;
	}
}
