using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Common.Useful
{
	public interface IShowable
	{
		void Show();
		void Hide();
		bool Visible { get; }

		/*
		#region IShowable
		private bool visible;
		public bool Visible { get { return visible; } }
		public void Show()
		{
			visible = true;
		}
		public void Hide()
		{
			visible = false;
		}
		#endregion
		*/
	}

}
