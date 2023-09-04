using MSDHSystem.Models;
using MSDHSystem.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MSDHSystem.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FlyoutItemTemplate : Grid
	{
		public FlyoutItemTemplate ()
		{
			InitializeComponent ();
		}

        private void lstMenus_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var selectedProduct = (Menus)e.Item;
            DependencyService.Get<Toast>().Show(selectedProduct.menuTitle);
        }
    }
}