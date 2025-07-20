using System.Collections.ObjectModel;
using System.IO;
using Notes.Model;
using Notes.Services;
using Notes.Core;

namespace Notes.ViewModel
{
    public class ProfileViewModel : BaseViewModel
    {
        private FileService _fileService;
        private MainViewModel _mainViewModel;
        private ObservableCollection<Note> notes;
        private Note selectedNote;
        private ObservableCollection<Folder> folders;
        private Folder selectedFolder;

        public RelayCommand SelectFolderCommand { get; set; }
        public RelayCommand AddFolderCommand { get; set; }
        public RelayCommand RemoveFolderCommand { get; set; }
        public RelayCommand AddNoteCommand { get; set; }
        public RelayCommand RemoveNoteCommand { get; set; }
        public RelayCommand UpdateNoteCommand { get; set; }

        public ProfileViewModel(MainViewModel mainViewModel, FileService fileService)
        {
            _fileService = fileService ?? new FileService();
            _mainViewModel = mainViewModel ?? throw new ArgumentNullException(nameof(mainViewModel));
            notes = new ObservableCollection<Note>();
            selectedNote = new Note();
            folders = new ObservableCollection<Folder>();
            selectedFolder = new Folder();

            SelectFolderCommand = new RelayCommand(selectFolder, canSelectFolder);
            AddFolderCommand = new RelayCommand(addFolder, canAddFolder);
            RemoveFolderCommand = new RelayCommand(removeFolder, canRemoveFolder);
            AddNoteCommand = new RelayCommand(addNote, canAddNote);
            RemoveNoteCommand = new RelayCommand(removeNote, canRemoveNote);
            UpdateNoteCommand = new RelayCommand(updateNote, canUpdateNote);

            LoadFolders();
            LoadNotes();
        }

        public FileService FileService
        {
            get => _fileService;
            set
            {
                _fileService = value ?? new FileService();
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

        public ObservableCollection<Note> Notes
        {
            get => notes;
            set
            {
                notes = value ?? new ObservableCollection<Note>();
                OnPropertyChanged();
            }
        }

        public Note SelectedNote
        {
            get => selectedNote;
            set
            {
                selectedNote = value ?? new Note();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Folder> Folders
        {
            get => folders;
            set
            {
                folders = value ?? new ObservableCollection<Folder>();
                OnPropertyChanged();
            }
        }

        public Folder SelectedFolder
        {
            get => selectedFolder;
            set
            {
                selectedFolder = value ?? new Folder();
                if (!string.IsNullOrWhiteSpace(selectedFolder.Name))
                {
                    _fileService.SetCurrentFolder(selectedFolder.Name);
                    LoadNotes();
                }
                OnPropertyChanged();
            }
        }

        private void addFolder(object parameter)
        {
            if (SelectedFolder != null && _fileService.AddCustomFolder(SelectedFolder.Name))
            {
                LoadFolders();
                LoadNotes();
            }
        }

        private bool canAddFolder(object parameter)
        {
            return SelectedFolder != null && !string.IsNullOrWhiteSpace(SelectedFolder.Name);
        }

        private void removeFolder(object parameter)
        {
            if (SelectedFolder != null && _fileService.RemoveCustomFolder(SelectedFolder.Name))
            {
                LoadFolders();
                LoadNotes();
            }
        }

        private bool canRemoveFolder(object parameter)
        {
            return SelectedFolder != null && !string.IsNullOrWhiteSpace(SelectedFolder.Name);
        }

        private void addNote(object parameter)
        {
            int counter = 1;
            string baseName = "NewNote";
            string noteName = baseName;

            while (File.Exists(Path.Combine(_fileService.CurrentPath, noteName)))
            {
                noteName = $"{baseName}_{counter}";
                counter++;
            }

            var newNote = new Note
            {
                Name = noteName,
                Content = ""
            };

            if (_fileService.AddFile(newNote))
            {
                LoadNotes();
                SelectedNote = newNote;

                // Відкрити вікно редагування відразу
                _mainViewModel.SelectedViewModel = new EditNoteViewModel(_mainViewModel, _fileService, SelectedNote);
            }
        }



        private bool canAddNote(object parameter)
        {
            return SelectedNote != null;
        }

        private void removeNote(object parameter)
        {
            if (SelectedNote != null && _fileService.RemoveFile(SelectedNote.Name))
            {
                LoadNotes();
            }
        }

        private bool canRemoveNote(object parameter)
        {
            return SelectedNote != null && !string.IsNullOrWhiteSpace(SelectedNote.Name);
        }

        private void updateNote(object parameter)
        {
            if (SelectedNote != null)
            {
                _mainViewModel.SelectedViewModel = new EditNoteViewModel(_mainViewModel, _fileService, SelectedNote);
            }
        }

        private bool canUpdateNote(object parameter)
        {
            return SelectedNote != null && !string.IsNullOrWhiteSpace(SelectedNote.Name);
        }

        private void LoadFolders()
        {
            Folders.Clear();
            var list = _fileService.GetFolders();
            if (list != null)
            {
                foreach (var folder in list)
                    Folders.Add(folder);
            }
        }

        private void LoadNotes()
        {
            var allNotes = _fileService.GetAllNotes();
            Notes = new ObservableCollection<Note>(allNotes ?? new List<Note>());
        }

        private void selectFolder(object parameter)
        {
            if (parameter is Folder folder && !string.IsNullOrWhiteSpace(folder.Name))
            {
                _fileService.SetCurrentFolder(folder.Name);
                LoadNotes();
                SelectedFolder = folder;
            }
        }

        private bool canSelectFolder(object parameter)
        {
            return parameter is Folder folder && !string.IsNullOrWhiteSpace(folder.Name);
        }
    }
}
