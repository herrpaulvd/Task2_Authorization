using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task2_Authorization
{
    class File
    {
        public int[] Users { get; set; } // ID имеющих доступ пользователей
        public bool IsDirectory { get; set; } // папка | файл
        public string Name { get; set; } // имя (неполное)
        public File[] Children { get; set; } // дочерние элементы
        //public string Base64 { get; set; }
        public byte[] Content { get; set; } // содержимое файла

        // метод, возвращающий только тех детей, которые доступны текущему пользователю
        public IEnumerable<File> AccessableChildren(int userID)
            => Children.Where(c => c.Users.Contains(userID));
    }
}
