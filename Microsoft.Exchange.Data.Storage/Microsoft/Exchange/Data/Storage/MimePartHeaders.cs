using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MimePartHeaders
	{
		public MimePartHeaders(Charset charset)
		{
			this.headerDictionary = new Dictionary<string, Header>();
			this.headerByIdDictionary = new Dictionary<HeaderId, Header>();
			this.headerList = new List<Header>();
			this.charset = charset;
		}

		public void AddHeader(Header header)
		{
			this.headerList.Add(header);
			if (header.HeaderId != HeaderId.Received)
			{
				if (header.HeaderId != HeaderId.Unknown)
				{
					this.headerByIdDictionary[header.HeaderId] = header;
					return;
				}
				this.headerDictionary[header.Name.ToLowerInvariant()] = header;
			}
		}

		public Header this[string headerName]
		{
			get
			{
				headerName = headerName.ToLowerInvariant();
				HeaderId headerId = Header.GetHeaderId(headerName);
				Header result;
				if (headerId == HeaderId.Unknown)
				{
					this.headerDictionary.TryGetValue(headerName, out result);
				}
				else
				{
					this.headerByIdDictionary.TryGetValue(headerId, out result);
				}
				return result;
			}
		}

		public Header this[HeaderId id]
		{
			get
			{
				EnumValidator.ThrowIfInvalid<HeaderId>(id, "id");
				Header result;
				this.headerByIdDictionary.TryGetValue(id, out result);
				return result;
			}
		}

		public int Count
		{
			get
			{
				return this.headerList.Count;
			}
		}

		public IEnumerator<Header> GetEnumerator()
		{
			return this.headerList.GetEnumerator();
		}

		public Charset Charset
		{
			get
			{
				return this.charset;
			}
		}

		private Dictionary<string, Header> headerDictionary;

		private Dictionary<HeaderId, Header> headerByIdDictionary;

		private List<Header> headerList;

		private Charset charset;
	}
}
