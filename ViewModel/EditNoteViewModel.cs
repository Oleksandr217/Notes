using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Notes.Model;
using Notes.Core;
using Notes.Services;

namespace Notes.ViewModel
{
    public class EditNoteViewModel : BaseViewModel
    {
        private FileService _fileService;
        private MainViewModel _mainViewModel;
        private Note selectedNote;
        private string originalNoteName;

        public RelayCommand SaveChangesCommand { get; set; }

        public EditNoteViewModel(MainViewModel mainViewModel, FileService fileService, Note note)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService)); // <-- Використай наданий сервіс
            _mainViewModel = mainViewModel ?? throw new ArgumentNullException(nameof(mainViewModel));
            SelectedNote = note ?? new Note();
            originalNoteName = note?.Name;

            SaveChangesCommand = new RelayCommand(saveChangesNote, canSaveChangesNote);
        }

        public FileService FileService
        {
            get => _fileService;
            set
            {
                _fileService = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel MainViewModel
        {
            get => _mainViewModel;
            set
            {
                _mainViewModel = value;
                OnPropertyChanged();
            }
        }

        public Note SelectedNote
        {
            get => selectedNote;
            set
            {
                selectedNote = value;
                OnPropertyChanged();
            }
        }

        private void saveChangesNote(object parameter)
        {
            if (SelectedNote != null && !string.IsNullOrWhiteSpace(SelectedNote.Name))
            {
                _fileService.UpdateFile(SelectedNote, originalNoteName);
                _mainViewModel.SelectedViewModel = new ProfileViewModel(_mainViewModel, _fileService);

            }
        }

        private bool canSaveChangesNote(object parameter)
        {
            return SelectedNote != null && !string.IsNullOrWhiteSpace(SelectedNote.Name);
        }
    }

}
