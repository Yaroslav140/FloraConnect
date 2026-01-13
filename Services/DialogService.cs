using FlowerShop.WpfClient.Views;
using System.Windows;

namespace FlowerShop.WpfClient.Services
{
    public interface IDialogService
    {
        bool? ShowDialog(object viewModel);
    }
    public interface IRequestClose
    {
        event Action<bool?> RequestClose;
    }
    public sealed class DialogService : IDialogService
    {
        public bool? ShowDialog(object viewModel)
        {
            var window = new EntityDialogWindow
            {
                DataContext = viewModel,
                Owner = Application.Current.MainWindow
            };

            if (viewModel is IRequestClose rc)
            {
                rc.RequestClose += result =>
                {
                    window.DialogResult = result;
                    window.Close();
                };
            }

            return window.ShowDialog();
        }
    }
}
