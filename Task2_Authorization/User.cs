using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task2_Authorization
{
    // класс пользователя
    class User
    {
        public int ID { get; set; }
        public string Name { get; set; } // имя
        public string Password { get; set; } // пароль
    }
}
