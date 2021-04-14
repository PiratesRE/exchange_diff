using System;
using System.Collections;
using System.ComponentModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ObjectChangeTrackListEditor : ObjectListEditor
	{
		public ObjectChangeTrackListEditor()
		{
			base.Name = "ObjectChangeTrackListEditor";
		}

		protected override void TrackResolvedObjects(ICollection identities)
		{
			if (identities != null)
			{
				this.resolvedObjects.Clear();
				foreach (object obj in identities)
				{
					ADObjectId item = (ADObjectId)obj;
					this.resolvedObjects.Add(item);
				}
				this.resolvedObjects.ResetChangeTracking();
			}
		}

		protected override void AddToIdentityList(ICollection identities)
		{
			this.UpdateIdentityList(identities, false);
		}

		protected override void RemoveFromIdentityList(ICollection identities)
		{
			this.UpdateIdentityList(identities, true);
		}

		private void UpdateIdentityList(ICollection identities, bool removing)
		{
			if (identities != null && this.resolvedObjects != null)
			{
				int num = this.resolvedObjects.Added.Length;
				int num2 = this.resolvedObjects.Removed.Length;
				foreach (object obj in identities)
				{
					ADObjectId item = (ADObjectId)obj;
					try
					{
						if (removing)
						{
							this.resolvedObjects.Remove(item);
						}
						else
						{
							this.resolvedObjects.Add(item);
						}
					}
					catch (InvalidOperationException)
					{
					}
				}
				if (num != this.resolvedObjects.Added.Length)
				{
					this.OnAddedIdentityListChanged(EventArgs.Empty);
				}
				if (num2 != this.resolvedObjects.Removed.Length)
				{
					this.OnRemovedIdentityListChanged(EventArgs.Empty);
				}
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(null)]
		public object[] AddedIdentityList
		{
			get
			{
				return this.resolvedObjects.Added;
			}
			set
			{
			}
		}

		public event EventHandler AddedIdentityListChanged;

		protected virtual void OnAddedIdentityListChanged(EventArgs e)
		{
			if (this.AddedIdentityListChanged != null)
			{
				this.AddedIdentityListChanged(this, e);
			}
		}

		[DefaultValue(null)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object[] RemovedIdentityList
		{
			get
			{
				return this.resolvedObjects.Removed;
			}
			set
			{
			}
		}

		public event EventHandler RemovedIdentityListChanged;

		protected virtual void OnRemovedIdentityListChanged(EventArgs e)
		{
			if (this.RemovedIdentityListChanged != null)
			{
				this.RemovedIdentityListChanged(this, e);
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(null)]
		public new MultiValuedProperty<ADObjectId> IdentityList
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		private MultiValuedProperty<ADObjectId> resolvedObjects = new MultiValuedProperty<ADObjectId>();
	}
}
