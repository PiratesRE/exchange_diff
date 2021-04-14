using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Exchange.Data.Transport.Interop
{
	[ComVisible(false)]
	internal sealed class ComArguments
	{
		public byte[] this[int id]
		{
			get
			{
				return (byte[])this.props[id];
			}
			set
			{
				if (this.props.Count >= 64 && this.props[id] == null && value != null)
				{
					throw new ArgumentOutOfRangeException(string.Concat(new object[]
					{
						"Reaching the capacity of the property bag (",
						64,
						" properties) while adding property ",
						id,
						"."
					}));
				}
				if (value == null)
				{
					this.props.Remove(id);
					return;
				}
				if (value.Length > 65536)
				{
					string message = string.Concat(new object[]
					{
						"Error trying to set property ",
						id,
						" with size ",
						value.Length,
						". (Maximum: ",
						65536,
						" bytes)"
					});
					throw new ArgumentOutOfRangeException("value", message);
				}
				this.props[id] = value;
			}
		}

		public int GetInt32(int propertyId)
		{
			return BitConverter.ToInt32(this[propertyId], 0);
		}

		public string GetString(int propertyId)
		{
			byte[] array = this[propertyId];
			if (array == null)
			{
				return string.Empty;
			}
			return Encoding.Unicode.GetString(array);
		}

		public void SetBool(int propertyId, bool value)
		{
			this[propertyId] = (value ? ComArguments.TrueBytes : ComArguments.FalseBytes);
		}

		private const int MaxPropertySize = 65536;

		private const int MaxNumberOfProperties = 64;

		private static readonly byte[] FalseBytes = BitConverter.GetBytes(false);

		private static readonly byte[] TrueBytes = BitConverter.GetBytes(true);

		private SortedList props = new SortedList();
	}
}
