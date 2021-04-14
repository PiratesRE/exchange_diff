using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class KeyMappings
	{
		internal KeyMappings()
		{
			this.binding = new Dictionary<int, KeyMappingBase>();
		}

		internal KeyMappingBase[] Menu
		{
			get
			{
				KeyMappingBase[] array = new KeyMappingBase[this.binding.Keys.Count];
				this.binding.Values.CopyTo(array, 0);
				return array;
			}
		}

		internal int Count
		{
			get
			{
				return this.Menu.Length;
			}
		}

		internal List<KeyMappingBase> SortedMenu
		{
			get
			{
				List<KeyMappingBase> list = new List<KeyMappingBase>();
				list.AddRange(this.Menu);
				list.Sort((KeyMappingBase left, KeyMappingBase right) => left.Key - right.Key);
				return list;
			}
		}

		internal void AddTransferToADContactMailbox(int key, string context, string legacyExchangeDN)
		{
			if (key < 1 || key > 9)
			{
				throw new ArgumentOutOfRangeException("key");
			}
			if (legacyExchangeDN == null)
			{
				throw new ArgumentNullException("legacyExchangeDN");
			}
			KeyMappingBase value = null;
			if (this.binding.TryGetValue(key, out value))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Key {0} is already bound", new object[]
				{
					key
				}));
			}
			value = KeyMappingBase.CreateTransferToADContactMailbox(key, context, legacyExchangeDN);
			this.binding[key] = value;
		}

		internal void AddTransferToADContactPhone(int key, string context, string legacyExchangeDN)
		{
			if (key < 1 || key > 9)
			{
				throw new ArgumentOutOfRangeException("key");
			}
			if (legacyExchangeDN == null)
			{
				throw new ArgumentNullException("legacyExchangeDN");
			}
			KeyMappingBase value = null;
			if (this.binding.TryGetValue(key, out value))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Key {0} is already bound", new object[]
				{
					key
				}));
			}
			value = KeyMappingBase.CreateTransferToADContactPhone(key, context, legacyExchangeDN);
			this.binding[key] = value;
		}

		internal void AddTransferToNumber(int key, string context, string number)
		{
			if (key < 1 || key > 9)
			{
				throw new ArgumentOutOfRangeException("key");
			}
			if (number == null)
			{
				throw new ArgumentNullException("number");
			}
			KeyMappingBase value = null;
			if (this.binding.TryGetValue(key, out value))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Key {0} is already bound", new object[]
				{
					key
				}));
			}
			value = KeyMappingBase.CreateTransferToNumber(key, context, number);
			this.binding[key] = value;
		}

		internal void AddTransferToVoicemail(string context)
		{
			KeyMappingBase keyMappingBase = KeyMappingBase.CreateTransferToVoicemail(context);
			this.binding[keyMappingBase.Key] = keyMappingBase;
		}

		internal void AddFindMe(int key, string context, string number, int timeout)
		{
			this.AddFindMe(key, context, number, timeout, string.Empty);
		}

		internal void AddFindMe(int key, string context, string number, int timeout, string label)
		{
			if (key < 1 || key > 9)
			{
				throw new ArgumentOutOfRangeException("key");
			}
			if (number == null)
			{
				throw new ArgumentNullException("number");
			}
			if (timeout < 0)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			KeyMappingBase keyMappingBase = null;
			if (!this.binding.TryGetValue(key, out keyMappingBase))
			{
				keyMappingBase = KeyMappingBase.CreateFindMe(key, context, number, timeout, label);
				this.binding[key] = keyMappingBase;
				return;
			}
			if (keyMappingBase.KeyMappingType != KeyMappingTypeEnum.FindMe)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Key {0} is already bound", new object[]
				{
					key
				}));
			}
			((TransferToFindMe)keyMappingBase).AddFindMe(number, timeout, label);
		}

		internal void Remove(int key)
		{
			KeyMappingBase keyMappingBase = null;
			if (!this.binding.TryGetValue(key, out keyMappingBase))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Menu with key {0} does not exist", new object[]
				{
					key
				}));
			}
			this.binding.Remove(key);
		}

		private Dictionary<int, KeyMappingBase> binding;
	}
}
