using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LocationIdentifierHelper : ILocationIdentifierSetter, IEnumerable<LocationIdentifier>, IEnumerable
	{
		public LocationIdentifierHelper()
		{
			this.PrivateResetChangeList();
		}

		public static byte[] LocationIdentifierBufferIdentifier
		{
			get
			{
				if (LocationIdentifierHelper.locationIdentifierBufferIdentifier == null)
				{
					LocationIdentifierHelper.locationIdentifierBufferIdentifier = CTSGlobals.AsciiEncoding.GetBytes("v2CalendarLogging");
				}
				return LocationIdentifierHelper.locationIdentifierBufferIdentifier;
			}
		}

		public static int LocationIdentifierBufferIdentifierSize
		{
			get
			{
				return LocationIdentifierHelper.LocationIdentifierBufferIdentifier.Length;
			}
		}

		public long ChangeBufferSize
		{
			get
			{
				if (this.changeList != null)
				{
					return (long)LocationIdentifierHelper.LocationIdentifierBufferIdentifierSize + (long)LocationIdentifier.ByteArraySize * (long)this.changeList.Count;
				}
				return 0L;
			}
		}

		public byte[] ChangeBuffer
		{
			get
			{
				if (this.changeList == null)
				{
					return null;
				}
				byte[] array = new byte[this.ChangeBufferSize];
				Array.Copy(LocationIdentifierHelper.LocationIdentifierBufferIdentifier, array, LocationIdentifierHelper.LocationIdentifierBufferIdentifierSize);
				int num = LocationIdentifierHelper.LocationIdentifierBufferIdentifierSize;
				foreach (LocationIdentifier locationIdentifier in this.changeList)
				{
					byte[] byteArray = locationIdentifier.ByteArray;
					Array.Copy(byteArray, 0, array, num, byteArray.Length);
					num += byteArray.Length;
				}
				return array;
			}
			set
			{
				if (value == null)
				{
					this.changeList = null;
					return;
				}
				int i = LocationIdentifierHelper.LocationIdentifierBufferIdentifierSize;
				int num = (value.Length - i) / LocationIdentifier.ByteArraySize;
				int capacity = (num > int.MaxValue) ? int.MaxValue : num;
				this.changeList = new List<LocationIdentifier>(capacity);
				while (i < value.Length)
				{
					byte[] array = new byte[LocationIdentifier.ByteArraySize];
					Array.Copy(value, i, array, 0, array.Length);
					LocationIdentifier item = new LocationIdentifier(array);
					this.changeList.Add(item);
					i += array.Length;
				}
			}
		}

		public string ChangeListToString()
		{
			if (this.changeList == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (LocationIdentifier arg in this.changeList)
			{
				stringBuilder.AppendFormat("{0};", arg);
			}
			return stringBuilder.ToString();
		}

		public void ParseChangeList(string str)
		{
			if (str == null)
			{
				this.changeList = null;
				return;
			}
			if (str.Length != 0)
			{
				string[] array = str.Split(new char[]
				{
					';'
				});
				if (this.changeList == null)
				{
					this.changeList = new List<LocationIdentifier>(array.Length);
				}
				else
				{
					this.changeList.Clear();
					if (this.changeList.Capacity < array.Length)
					{
						this.changeList.Capacity = array.Length;
					}
				}
				foreach (string text in array)
				{
					if (text.Length == 0)
					{
						return;
					}
					LocationIdentifier item;
					try
					{
						item = LocationIdentifier.Parse(text);
					}
					catch (ArgumentException innerException)
					{
						throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "'{0}' is not a valid Location Identifier representaion.", new object[]
						{
							text
						}), innerException);
					}
					this.changeList.Add(item);
				}
				return;
			}
			if (this.changeList == null)
			{
				this.changeList = new List<LocationIdentifier>(0);
				return;
			}
			this.changeList.Clear();
		}

		public void SetLocationIdentifier(uint id)
		{
			LocationIdentifier locationIdentifier = new LocationIdentifier(id);
			this.SetLocationIdentifier(locationIdentifier);
		}

		public void SetLocationIdentifier(uint id, LastChangeAction action)
		{
			EnumValidator.ThrowIfInvalid<LastChangeAction>(action);
			LocationIdentifier locationIdentifier = new LocationIdentifier(id, action);
			this.SetLocationIdentifier(locationIdentifier);
		}

		IEnumerator<LocationIdentifier> IEnumerable<LocationIdentifier>.GetEnumerator()
		{
			return ((IEnumerable<LocationIdentifier>)this.changeList).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.changeList).GetEnumerator();
		}

		internal virtual void ResetChangeList()
		{
			this.PrivateResetChangeList();
		}

		protected virtual void SetLocationIdentifier(LocationIdentifier locationIdentifier)
		{
			if (this.changeList == null)
			{
				this.ResetChangeList();
			}
			this.changeList.Add(locationIdentifier);
		}

		private void PrivateResetChangeList()
		{
			if (this.changeList == null)
			{
				this.changeList = new List<LocationIdentifier>(1);
				return;
			}
			this.changeList.Clear();
		}

		private const string LocationIdentifierBufferHeader = "v2CalendarLogging";

		private static byte[] locationIdentifierBufferIdentifier;

		private List<LocationIdentifier> changeList;
	}
}
