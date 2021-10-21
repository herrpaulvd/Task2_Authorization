using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task2_Authorization
{
    class File
    {
        public int[] Users { get; set; }
        public bool IsDirectory { get; set; }
        public string Name { get; set; }
        public File[] Children { get; set; }
        //public string Base64 { get; set; }
        public byte[] Content { get; set; }

        public IEnumerable<File> AccessableChildren(int userID)
            => Children.Where(c => c.Users.Contains(userID));
    }
}
