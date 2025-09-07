using System.Collections.ObjectModel;

namespace FlowerShop.WpfClient.ModelView
{
    public class MainViewModel
    {
        public ObservableCollection<ICrudModule> Modules { get; }
        public ICrudModule? CurrentModule { get; set; } 

        public MainViewModel(OrdersModuleViewModel orders, UsersModuleViewModel users, BouquetsModuleViewModel bouquets, FlowersModuleViewModel flowers)
        {
            Modules = new ObservableCollection<ICrudModule>
            {
                orders, users, bouquets, flowers
            };
            CurrentModule = Modules.FirstOrDefault();
        }
    }

}
