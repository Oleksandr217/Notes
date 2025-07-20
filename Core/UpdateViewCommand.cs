using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Notes.Model;
using Notes.ViewModel;

namespace Notes.Core
{
    public class UpdateViewCommand: ICommand
    {
        private MainViewModel _viewModel;

        public UpdateViewCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel;
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            if (parameter is string s && s == "Profile")
                _viewModel.SelectedViewModel = new ProfileViewModel(_viewModel, new Services.FileService());
            else if (parameter is Note note)
                _viewModel.SelectedViewModel = new EditNoteViewModel(_viewModel,new Services.FileService(), note);
        }

    }
}
