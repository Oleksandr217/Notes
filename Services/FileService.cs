using System;
using System.IO;
using Notes.Model;

namespace Notes.Services
{
    public class FileService
    {
        public string MainPath { get; set; }
        private string _currentPath;
        public string CurrentPath
        {
            get => _currentPath;
            private set => _currentPath = value;
        }

        public FileService()
        {
            InitializeMainFolder();
        }

        private void InitializeMainFolder()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            MainPath = Path.Combine(appData, "MainFolder");

            if (!Directory.Exists(MainPath))
                Directory.CreateDirectory(MainPath);

            CurrentPath = MainPath;
        }
        public void ResetToMain()
        {
            CurrentPath = MainPath;
        }
        public List<Folder> GetFolders()
        {
            var folders = new List<Folder>();

            folders.Add(new Folder { Name = Path.GetFileName(MainPath) });

            if (!Directory.Exists(MainPath))
                return folders;

            folders.AddRange(
                Directory.GetDirectories(MainPath)
                    .Select(dir => new Folder
                    {
                        Name = Path.GetFileName(dir)
                    }));

            return folders;
        }
        public bool AddCustomFolder(string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderName))
                return false;

            string folderPath = Path.Combine(MainPath, folderName);
            if (Directory.Exists(folderPath)) return false;

            Directory.CreateDirectory(folderPath);
            CurrentPath = folderPath;
            return true;
        }

        public bool RemoveCustomFolder(string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderName))
                return false;

            string folderPath = Path.Combine(MainPath, folderName);
            if (!Directory.Exists(folderPath)) return false;

            Directory.Delete(folderPath, true);
            ResetToMain();
            return true;
        }

        public bool AddFile(Note note)
        {
            if (note == null || string.IsNullOrWhiteSpace(note.Name))
                return false;

            string filePath = Path.Combine(CurrentPath, note.Name);
            if (File.Exists(filePath)) return false;

            File.WriteAllText(filePath, note.Content ?? string.Empty);
            return true;
        }

        public bool RemoveFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            string filePath = Path.Combine(CurrentPath, fileName);
            if (!File.Exists(filePath)) return false;

            File.Delete(filePath);
            return true;
        }

        public bool UpdateFile(Note updatedNote, string oldFileName = null)
        {
            if (updatedNote == null || string.IsNullOrWhiteSpace(updatedNote.Name))
                return false;

            string oldPath = Path.Combine(CurrentPath, oldFileName ?? updatedNote.Name);
            string newPath = Path.Combine(CurrentPath, updatedNote.Name);


            if (!string.Equals(oldPath, newPath, StringComparison.OrdinalIgnoreCase) && File.Exists(oldPath))
            {
                File.Move(oldPath, newPath);
            }

            File.WriteAllText(newPath, updatedNote.Content ?? string.Empty);
            return true;
        }

        public List<Note> GetAllNotes()
        {
            var notes = new List<Note>();
            if (Directory.Exists(CurrentPath))
            {
                foreach (var file in Directory.GetFiles(CurrentPath))
                {
                    var content = File.ReadAllText(file);
                    notes.Add(new Note
                    {
                        Name = Path.GetFileName(file),
                        Content = content ?? string.Empty
                    });
                }
            }
            return notes;
        }
        public void SetCurrentFolder(string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderName))
                return;

            var path = Path.Combine(MainPath, folderName);

            if (Directory.Exists(path))
            {
                CurrentPath = path;
            }
            else
            {
                if (folderName == Path.GetFileName(MainPath))
                {
                    ResetToMain();
                }
            }
        }


    }
}
