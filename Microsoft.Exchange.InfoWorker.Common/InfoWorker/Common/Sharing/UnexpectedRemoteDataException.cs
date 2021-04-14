using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	[Serializable]
	public sealed class UnexpectedRemoteDataException : SharingSynchronizationException
	{
		public UnexpectedRemoteDataException() : base(Strings.UnexpectedRemoteDataException)
		{
		}

		public UnexpectedRemoteDataException(Exception innerException) : base(Strings.UnexpectedRemoteDataException, innerException)
		{
		}

		public UnexpectedRemoteDataException(params ResponseMessageType[] items) : this()
		{
			this.items = items;
			int num = 0;
			foreach (ResponseMessageType item in items)
			{
				string key = string.Format("Item{0}", num++);
				this.Data[key] = UnexpectedRemoteDataException.ResponseMessageTypeToString(item);
			}
		}

		internal ResponseMessageType[] Items
		{
			get
			{
				return this.items;
			}
		}

		private static string ResponseMessageTypeToString(ResponseMessageType item)
		{
			if (item == null)
			{
				return "<null>";
			}
			XmlSerializer xmlSerializer = new XmlSerializer(item.GetType());
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				xmlSerializer.Serialize(memoryStream, item);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				using (StreamReader streamReader = new StreamReader(memoryStream))
				{
					result = streamReader.ReadToEnd();
				}
			}
			return result;
		}

		private readonly ResponseMessageType[] items;
	}
}
