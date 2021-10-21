using System;
using System.Collections.Generic;
using System.Linq;

namespace Task2_Authorization
{
    using OuterFile = System.IO.File;

    class Program
    {
        static string[] ReadCommand(string invitation)
        {
            Console.Write(invitation + ">");
            var s = Console.ReadLine();

            int ptr = 0;
            int n = s.Length;

            List<string> args = new();

            while(ptr < n)
            {
                if(s[ptr] == ' ')
                {
                    ptr++;
                    continue;
                }

                if(s[ptr] == '"')
                {
                    int end = ++ptr;
                    while (end < n && s[end] != '"')
                        end++;
                    args.Add(s[ptr..end]);
                    ptr = end + 1;
                    continue;
                }

                // otherwise
                {
                    int end = ptr;
                    while (end < n && s[end] != ' ')
                        end++;
                    args.Add(s[ptr..end]);
                    ptr = end;
                    continue;
                }
            }

            return args.ToArray();
        }

        static void RunSession(Session session)
        {
            bool run = true;
            while (run)
            {
                var cmd = ReadCommand(session.CurrentDirectory);
                if (cmd.Length == 0)
                {
                    Console.WriteLine("Empty input. Try again");
                    continue;
                }
                switch (cmd[0])
                {
                    case "exit":
                        run = false;
                        break;
                    case "pwd":
                        session.PWD();
                        break;
                    case "ls":
                        session.LS();
                        break;
                    case "cd":
                        if (cmd.Length < 2)
                            Console.WriteLine("Expected 2nd arg");
                        else
                            session.CD(cmd[1]);
                        break;
                    case "cdback":
                        session.CDback();
                        break;
                    case "cat":
                        if (cmd.Length < 2)
                            Console.WriteLine("Expected 2nd arg");
                        else
                            session.Cat(cmd[1]);
                        break;
                    case "load":
                        if (cmd.Length < 2)
                            Console.WriteLine("Expected 2nd arg");
                        else if(cmd.Length < 3)
                            Console.WriteLine("Expected 3rd arg");
                        else
                            session.Load(cmd[1], cmd[2]);
                        break;
                    case "store":
                        if (cmd.Length < 2)
                            Console.WriteLine("Expected 2nd arg");
                        else if (cmd.Length < 3)
                            Console.WriteLine("Expected 3rd arg");
                        else
                            session.Store(cmd[1], cmd[2]);
                        break;
                    case "copy":
                        if (cmd.Length < 2)
                            Console.WriteLine("Expected 2nd arg");
                        else if (cmd.Length < 3)
                            Console.WriteLine("Expected 3rd arg");
                        else
                            session.Copy(cmd[1], cmd[2]);
                        break;
                    case "move":
                        if (cmd.Length < 2)
                            Console.WriteLine("Expected 2nd arg");
                        else if (cmd.Length < 3)
                            Console.WriteLine("Expected 3rd arg");
                        else
                            session.Move(cmd[1], cmd[2]);
                        break;
                    case "del":
                        if (cmd.Length < 2)
                            Console.WriteLine("Expected 2nd arg");
                        else
                            session.Del(cmd[1]);
                        break;
                    case "mkdir":
                        if (cmd.Length < 2)
                            Console.WriteLine("Expected 2nd arg");
                        else
                            session.Mkdir(cmd[1]);
                        break;
                    case "getacc":
                        if (cmd.Length < 2)
                            Console.WriteLine("Expected 2nd arg");
                        else if (cmd.Length < 3)
                            Console.WriteLine("Expected 3rd arg");
                        else
                            session.GetAcc(cmd[1], cmd[2]);
                        break;
                    case "getfullacc":
                        if (cmd.Length < 2)
                            Console.WriteLine("Expected 2nd arg");
                        else if (cmd.Length < 3)
                            Console.WriteLine("Expected 3rd arg");
                        else
                            session.GetFullAcc(cmd[1], cmd[2]);
                        break;
                    case "delacc":
                        if (cmd.Length < 2)
                            Console.WriteLine("Expected 2nd arg");
                        else if (cmd.Length < 3)
                            Console.WriteLine("Expected 3rd arg");
                        else
                            session.DelAcc(cmd[1], cmd[2]);
                        break;
                    case "cru":
                        if (cmd.Length < 2)
                            Console.WriteLine("Expected 2nd arg");
                        else if (cmd.Length < 3)
                            Console.WriteLine("Expected 3rd arg");
                        else
                            session.CRU(cmd[1], cmd[2]);
                        break;
                }
            }
        }

        static void RunFS(FileSystem fs)
        {
            bool run = true;
            while (run)
            {
                var cmd = ReadCommand("FS");
                if (cmd.Length == 0)
                {
                    Console.WriteLine("Empty input. Try again");
                    continue;
                }
                switch(cmd[0])
                {
                    case "exit":
                        run = false;
                        break;
                    case "store":
                        if (cmd.Length < 2)
                            Console.WriteLine("Expected 2nd arg");
                        else
                        {
                            try
                            {
                                OuterFile.WriteAllBytes(cmd[1], Encoder.EncodeObject(fs));
                            }
                            catch (System.IO.IOException)
                            {
                                Console.WriteLine("Error while writing file");
                            }
                        }
                        break;
                    case "enter":
                        if (cmd.Length < 2)
                            Console.WriteLine("Expected 2nd arg");
                        else if (cmd.Length < 3)
                            Console.WriteLine("Expected 3rd arg");
                        else
                        {
                            var username = cmd[1];
                            var password = cmd[2];
                            var user = fs.Users.FirstOrDefault(u => u.Name == username && u.Password == password);
                            if (user is null)
                                Console.WriteLine("Invalid username or password");
                            else
                                RunSession(new Session
                                {
                                    CurrentDirectory = "/",
                                    FS = fs,
                                    UserID = user.ID
                                });
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid command");
                        break;
                }
            }
        }

        static void Main(string[] args)
        {
            bool run = true;
            while(run)
            {
                var cmd = ReadCommand("FSMANAGER");
                if(cmd.Length == 0)
                {
                    Console.WriteLine("Empty input. Try again");
                    continue;
                }

                switch(cmd[0])
                {
                    case "exit":
                        run = false;
                        break;
                    case "load":
                        if (cmd.Length < 2)
                            Console.WriteLine("Expected 2nd arg");
                        else if (OuterFile.Exists(cmd[1]))
                        {
                            FileSystem fs = null;
                            try
                            {
                                fs = Encoder.DecodeObject<FileSystem>(OuterFile.ReadAllBytes(cmd[1]));
                            }
                            catch(Exception)
                            {
                                Console.WriteLine("Error while reading file");
                            }
                            if (fs is object)
                                RunFS(fs);
                        }
                        else
                            Console.WriteLine("File doesn't exist");
                        break;
                    case "new":
                        if (cmd.Length < 2)
                            Console.WriteLine("Expected 2nd arg");
                        else if (cmd.Length < 3)
                            Console.WriteLine("Expected 3rd arg");
                        else
                            RunFS(FileSystem.MakeDefault(cmd[1], cmd[2]));
                        break;
                    default:
                        Console.WriteLine("Invalid command");
                        break;
                }
            }
        }
    }
}
