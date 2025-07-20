using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Notes.Core;
using Notes.Services;

namespace Notes.ViewModel
{
    public class MainViewModel: BaseViewModel
    {
        private BaseViewModel selectedViewModel;
        public ICommand UpdateViewCommand { get; set; }
        public MainViewModel()
        {
            UpdateViewCommand = new UpdateViewCommand(this);
            selectedViewModel = new ProfileViewModel(this, new FileService());
        }
        public BaseViewModel SelectedViewModel
        {
            get { return selectedViewModel; }
            set 
            {  
                selectedViewModel = value; 
                OnPropertyChanged(nameof(selectedViewModel));
            }
        }
    }
}
