using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task2_Authorization
{
    class FileSystem
    {
        public User[] Users { get; set; }
        public File RootDirectory { get; set; }

        public static FileSystem MakeDefault(string username, string password)
        {
            var admin = new User { Name = username, Password = password, ID = 0 };
            var root = new File { Name = "", Content = Array.Empty<byte>(), Children = Array.Empty<File>(), IsDirectory = true, Users = new int[] { 0 } };
            return new FileSystem { Users = new User[] { admin }, RootDirectory = root };
        }

        private static string[] GetNamesWithoutSlashes(string path)
            => path.Split('/');

        public List<File> GetFilesByFullName(string filename, int user)
        {
            filename = filename.Trim('/');
            File curr = RootDirectory;
            List<File> result = new() { curr };
            var components = filename.Length == 0 ? Array.Empty<string>() : GetNamesWithoutSlashes(filename);

            for(int i = 0; i < components.Length; i++)
            {
                curr = curr.Children.Where(f => f.Name == components[i] && f.Users.Contains(user)).FirstOrDefault();
                if (curr is null) return null;
                result.Add(curr);
            }

            return result;
        }

        public List<File> GetFilesByNewFullName(string filename, int user, bool dir)
        {
            filename = filename.Trim('/');
            File curr = RootDirectory;
            List<File> result = new() { curr };
            var components = filename.Length == 0 ? Array.Empty<string>() : GetNamesWithoutSlashes(filename);

            for (int i = 0; i < components.Length - 1; i++)
            {
                curr = curr.Children.Where(f => f.Name == components[i] && f.Users.Contains(user)).FirstOrDefault();
                if (curr is null) return null;
                result.Add(curr);
            }

            if (!curr.IsDirectory) return null;
            if (curr.Children.Any(c => c.Name == components[^1]))
                return null;

            var newf = new File
            { 
                Children = Array.Empty<File>(), 
                IsDirectory = dir, 
                Content = Array.Empty<byte>(), 
                Name = components[^1], 
                Users = new int[] { user } 
            };
            result.Add(newf);
            curr.Children = curr.Children.Append(newf).ToArray();

            return result;
        }
    }
}
