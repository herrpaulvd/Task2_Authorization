using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task2_Authorization
{
    using OuterFile = System.IO.File;
    using OuterFileInfo = System.IO.FileInfo;
    class Session
    {
        public int UserID { get; set; }
        public FileSystem FS { get; set; }
        public string CurrentDirectory { get; set; }

        // Navigation
        public void PWD()
        {
            Console.WriteLine(CurrentDirectory);
        }

        public void LS()
        {
            foreach (var e in FS.GetFilesByFullName(CurrentDirectory, UserID)[^1].AccessableChildren(UserID))
            {
                Console.WriteLine(
                    (e.IsDirectory ? "Directory " : "File ")
                    + e.Name
                    + (e.IsDirectory ? " filecount " + e.Children.Count(f => f.Users.Contains(UserID)) : " sizeof " + e.Content.Length + " bytes"));
            }
        }

        private string PrepareFilename(string filename)
        {
            if (filename[^1] != '/') filename += "/";
            if (filename[0] != '/') filename = CurrentDirectory + filename;
            return filename;
        }

        public void CD(string filename)
        {
            if(filename.Length == 0)
            {
                Console.WriteLine("Empty name");
                return;
            }
            filename = PrepareFilename(filename);
            var f = FS.GetFilesByFullName(filename, UserID);
            if (f is object && f[^1].IsDirectory)
                CurrentDirectory = filename;
            else
                Console.WriteLine("Invalid path");
        }

        public void CDback()
        {
            if(CurrentDirectory != "/")
            {
                int end = CurrentDirectory.Length - 1;
                while (CurrentDirectory[end - 1] != '/') end--;
                CurrentDirectory = CurrentDirectory.Substring(0, end);
            }
        }

        // I/O
        public void Cat(string filename)
        {
            if (filename.Length == 0)
            {
                Console.WriteLine("Empty name");
                return;
            }
            filename = PrepareFilename(filename);
            var f = FS.GetFilesByFullName(filename, UserID);
            if (f is object && !f[^1].IsDirectory)
            {
                Console.WriteLine(Encoding.UTF8.GetString(f[^1].Content));
            }
            else
                Console.WriteLine("Invalid path");
        }

        public void Load(string start, string end)
        {
            if(!OuterFile.Exists(start))
            {
                Console.WriteLine("Outer file {0} doesn't exist", start);
                return;
            }

            if (end.Length == 0)
            {
                Console.WriteLine("Empty name");
                return;
            }
            end = PrepareFilename(end);
            var f = FS.GetFilesByNewFullName(end, UserID, false);
            if (f is null)
            {
                Console.WriteLine("Invalid path");
                return;
            }
            f[^1].Content = OuterFile.ReadAllBytes(start);
        }

        public void Store(string start, string end)
        {
            if (start.Length == 0)
            {
                Console.WriteLine("Empty name");
                return;
            }
            start = PrepareFilename(start);
            var f = FS.GetFilesByFullName(start, UserID);
            if (f is null || f[^1].IsDirectory)
            {
                Console.WriteLine("Invalid path");
                return;
            }
            try
            {
                OuterFile.WriteAllBytes(end, f[^1].Content);
            }
            catch(System.IO.IOException)
            {
                Console.WriteLine("Cannot store to the outer file");
            }
        }

        // copy & move & delete
        public void Copy(string start, string end)
        {
            if (start.Length == 0)
            {
                Console.WriteLine("Empty name");
                return;
            }
            start = PrepareFilename(start);
            var fStart = FS.GetFilesByFullName(start, UserID);
            if (fStart is null || fStart[^1].IsDirectory)
            {
                Console.WriteLine("Invalid path");
                return;
            }

            if (end.Length == 0)
            {
                Console.WriteLine("Empty name");
                return;
            }

            end = PrepareFilename(end);
            var fEnd = FS.GetFilesByNewFullName(end, UserID, false);
            if (fEnd is null)
            {
                Console.WriteLine("Invalid path");
                return;
            }

            fEnd[^1].Content = fStart[^1].Content;
        }

        public void Move(string start, string end)
        {
            if (start.Length == 0)
            {
                Console.WriteLine("Empty name");
                return;
            }
            start = PrepareFilename(start);
            var fStart = FS.GetFilesByFullName(start, UserID);
            if (fStart is null || fStart[^1].IsDirectory)
            {
                Console.WriteLine("Invalid path");
                return;
            }

            if (end.Length == 0)
            {
                Console.WriteLine("Empty name");
                return;
            }

            end = PrepareFilename(end);
            var fEnd = FS.GetFilesByNewFullName(end, UserID, false);
            if (fEnd is null)
            {
                Console.WriteLine("Invalid path");
                return;
            }

            fEnd[^1].Content = fStart[^1].Content;
            fStart[^2].Children = fStart[^2].Children.Where(c => c.Name != fStart[^1].Name).ToArray();
        }

        public void Del(string filename)
        {
            if (filename.Length == 0)
            {
                Console.WriteLine("Empty name");
                return;
            }
            filename = PrepareFilename(filename);
            var f = FS.GetFilesByFullName(filename, UserID);
            if (f is object)
            {
                f[^2].Children = f[^2].Children.Where(c => c.Name != f[^1].Name).ToArray();
            }
            else
                Console.WriteLine("Invalid path");
        }

        public void Mkdir(string filename)
        {
            if (filename.Length == 0)
            {
                Console.WriteLine("Empty name");
                return;
            }
            filename = PrepareFilename(filename);
            var f = FS.GetFilesByNewFullName(filename, UserID, true);
            if (f is null)
                Console.WriteLine("Invalid path");
        }

        // manage access
        public void GetAcc(string filename, string username)
        {
            var u = FS.Users.FirstOrDefault(u => u.Name == username);
            if(u is null)
            {
                Console.WriteLine("Invalid username");
                return;
            }

            if (filename.Length == 0)
            {
                Console.WriteLine("Empty name");
                return;
            }
            filename = PrepareFilename(filename);
            var f = FS.GetFilesByFullName(filename, UserID);
            if (f is null)
                Console.WriteLine("Invalid path");

            foreach(var e in f)
                e.Users = e.Users.Append(u.ID).Distinct().ToArray();
        }

        // manage access
        public void GetFullAcc(string filename, string username)
        {
            var u = FS.Users.FirstOrDefault(u => u.Name == username);
            if (u is null)
            {
                Console.WriteLine("Invalid username");
                return;
            }

            if (filename.Length == 0)
            {
                Console.WriteLine("Empty name");
                return;
            }
            filename = PrepareFilename(filename);
            var f = FS.GetFilesByFullName(filename, UserID);
            if (f is null)
                Console.WriteLine("Invalid path");

            foreach (var e in f)
                e.Users = e.Users.Append(u.ID).Distinct().ToArray();
            void RecAddUser(File file)
            {
                file.Users = file.Users.Append(u.ID).Distinct().ToArray();
                foreach (var e in file.Children)
                    RecAddUser(e);
            }
            RecAddUser(f[^1]);
        }

        public void DelAcc(string filename, string username)
        {
            var u = FS.Users.FirstOrDefault(u => u.Name == username);
            if (u is null)
            {
                Console.WriteLine("Invalid username");
                return;
            }

            if (filename.Length == 0)
            {
                Console.WriteLine("Empty name");
                return;
            }
            filename = PrepareFilename(filename);
            var f = FS.GetFilesByFullName(filename, UserID);
            if (f is null)
                Console.WriteLine("Invalid path");

            void RecDelUser(File file)
            {
                file.Users = file.Users.Where(id => id != u.ID).ToArray();
                foreach (var e in file.Children)
                    RecDelUser(e);
            }
            RecDelUser(f[^1]);
        }

        private static int MEX(IEnumerable<int> ids)
        {
            var ss = new SortedSet<int>(ids);
            for (int id = 0; ; id++)
                if (!ids.Contains(id))
                    return id;
        }

        // create user
        public void CRU(string username, string password)
        {
            if(FS.Users.Any(u => u.Name == username))
            {
                Console.WriteLine("User already exists");
                return;
            }

            FS.Users = FS.Users.Append(new User { ID = MEX(FS.Users.Select(u => u.ID)), Name = username, Password = password }).ToArray();
        }
    }
}
