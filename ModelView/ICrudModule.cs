using System.Windows.Input;

namespace FlowerShop.WpfClient.ModelView
{
    public interface ICrudModule
    {
        string Title { get; }             
        string SearchText { get; set; }   
        object? SelectedItem { get; set; }

        System.Collections.IEnumerable Items { get; } 

        ICommand SearchCommand { get; }
        ICommand AddCommand { get; }
        ICommand EditCommand { get; }
        ICommand DeleteCommand { get; }
    }

}
