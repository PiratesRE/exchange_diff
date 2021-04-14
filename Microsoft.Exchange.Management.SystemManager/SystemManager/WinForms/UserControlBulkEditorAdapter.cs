using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class UserControlBulkEditorAdapter : BulkEditorAdapter
	{
		public UserControlBulkEditorAdapter(ExchangeUserControl control) : base(control)
		{
		}

		protected override void OnStateChanged(BulkEditorAdapter sender, BulkEditorStateEventArgs e)
		{
			base.OnStateChanged(sender, e);
			if (base[e.PropertyName] == 2 || base[e.PropertyName] == 3)
			{
				ExchangeUserControl exchangeUserControl = base.HostControl as ExchangeUserControl;
				foreach (Control control in exchangeUserControl.ExposedPropertyRelatedControls[e.PropertyName])
				{
					control.Enabled = false;
				}
			}
		}

		protected override void InnerSetState(string propertyName, BulkEditorState state)
		{
			base.InnerSetState(propertyName, state);
			ExchangeUserControl exchangeUserControl = base.HostControl as ExchangeUserControl;
			foreach (BulkEditorAdapter bulkEditorAdapter in UserControlBulkEditorAdapter.GetBulkEditorAdapters(exchangeUserControl.ExposedPropertyRelatedControls[propertyName]))
			{
				bulkEditorAdapter.SetPropertiesState(state);
				bulkEditorAdapter.StateChanged += new BulkEditorAdapter.BulkEditorStateChangedEventHandler(this.BulkEditorAdapter_StateChanged);
			}
		}

		private void BulkEditorAdapter_StateChanged(BulkEditorAdapter sender, BulkEditorStateEventArgs e)
		{
			ExchangeUserControl exchangeUserControl = base.HostControl as ExchangeUserControl;
			foreach (KeyValuePair<string, HashSet<Control>> keyValuePair in exchangeUserControl.ExposedPropertyRelatedControls)
			{
				if (exchangeUserControl.ExposedPropertyRelatedControls[keyValuePair.Key].Contains(sender.HostControl))
				{
					base[keyValuePair.Key] = sender[e.PropertyName];
				}
			}
		}

		protected override BulkEditorState InnerGetState(string propertyName)
		{
			return base.InnerGetState(propertyName);
		}

		protected override IList<string> InnerGetManagedProperties()
		{
			IList<string> list = base.InnerGetManagedProperties();
			ExchangeUserControl exchangeUserControl = base.HostControl as ExchangeUserControl;
			foreach (string item in exchangeUserControl.ExposedPropertyRelatedControls.Keys)
			{
				list.Add(item);
			}
			return list;
		}

		private static HashSet<BulkEditorAdapter> GetBulkEditorAdapters(HashSet<Control> controlSet)
		{
			HashSet<BulkEditorAdapter> hashSet = new HashSet<BulkEditorAdapter>();
			foreach (Control control in controlSet)
			{
				IBulkEditor bulkEditor = control as IBulkEditor;
				if (bulkEditor != null)
				{
					hashSet.Add(bulkEditor.BulkEditorAdapter);
				}
			}
			return hashSet;
		}
	}
}
