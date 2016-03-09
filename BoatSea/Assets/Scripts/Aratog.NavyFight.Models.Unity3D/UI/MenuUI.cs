using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;
using Aratog.NavyFight.Models.UI;
using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.UI {
	public class MenuUI {

		#region Variables

		//Идентификатор меню, в зависимости от типа меню, может содержать несколько меню одновременно
		public MenuID ID;

		//Тип меню: классический, панель вкладок
		public MenuType Type;


		//Отображается ли вданный момент меню
		public bool IsShowing;

		//Отображать меню модальным, так же это подразумевает полноэкранную кнопку триггер по нажатию 
		//на которую во вне меню, текущее меню закрывается
		public bool IsModal;

		//Является ли меню активным
		public bool IsActive;

		
		//Переменная для проверки касания по экрану во вне меню
		public Rect FullscreenTrigger;
		
		#endregion

		#region Constructor
		public MenuUI () {
			ID = MenuID.None;
			Type = MenuType.ClassicMenu;
			IsShowing = false;
			IsActive = false;
			IsModal = false;
		}
		#endregion

		#region Events

		public virtual void OnShow () {
			IsShowing = true;
			IsActive = false;
		}

		public virtual void OnHide () {
			IsShowing = false;
		}

		public virtual void OnClose () {
			IsShowing = false;
			IsActive = false;
		}

		public virtual void OnOtherMenuSelect () {
			IsActive = false;
		}
		#endregion


	}
}
