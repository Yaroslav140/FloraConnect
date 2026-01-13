using System.Windows.Input;

namespace FlowerShop.WpfClient.ViewModel.Base
{
    public interface IViewModelBase
    {
        string Title { get; }
        string? SearchText { get; set; }
        Task LoadAsync();
        ICommand CreateCommand { get; }
        ICommand EditCommand { get; }
        ICommand DeleteCommand { get; }
    }
}
