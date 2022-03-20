using final.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final
{
    class Program
    {
        static void Main(string[] args)
        {
            //Восстановление настроек
            string path = @Settings.Default.path;

            //Настройки по умолчанию
            if (path == "")
            {
                path = @"C:\";
            }

            //Инициализация массива команд
            string[] arrayCmd = { };

            //Индекс выбранного каталога или файла
            int currentIndex = 0;

            //Вывод списка каталогов и файлов при инициализации
            PrintFiles(0, path);

            while (true)
            {
                //Считываем команду
                ConsoleKeyInfo info = Console.ReadKey();

                switch (info.Key)
                {
                    //Навигация по списку каталогов и файлов, стрелка ВВЕРХ
                    case ConsoleKey.UpArrow:
                        {
                            if (currentIndex > 0)
                            {
                                currentIndex--;
                            }
                            PrintFiles(currentIndex, path);
                        }
                        break;
                    //Навигация по списку каталогов и файлов, стрелка ВНИЗ
                    case ConsoleKey.DownArrow:
                        {
                            string[] files = Directory.GetFileSystemEntries(path);

                            if (currentIndex < files.Length - 1)
                            {
                                currentIndex++;
                            }

                            PrintFiles(currentIndex, path);
                        }
                        break;
                    //Навигация по списку каталогов и файлов, стрелка ВЛЕВО, переход на уровень выше по дереву каталогов
                    case ConsoleKey.LeftArrow:
                        {
                            try
                            {
                                if (Directory.GetParent(path) != null)
                                {
                                    path = Directory.GetParent(path).FullName;
                                    currentIndex = 0;
                                    PrintFiles(currentIndex, path);
                                }
                            }
                            catch (Exception)
                            {

                                //throw;
                            }
                            PrintFiles(currentIndex, path);
                        }
                        break;
                    //Навигация по списку каталогов и файлов, стрелка ВПРАВО, переход на уровень ниже по дереву каталогов
                    case ConsoleKey.RightArrow:
                        {
                            try
                            {
                                string[] entries = Directory.GetFileSystemEntries(path);

                                if (entries.Length > 0)
                                {
                                    string newPath = Directory.GetFileSystemEntries(path)[currentIndex];

                                    //Определение каталога или файла
                                    if (IsDirectory(newPath))
                                    {
                                        path = newPath;
                                        currentIndex = 0;
                                        PrintFiles(currentIndex, path);
                                    }
                                    else
                                    {
                                        PrintFiles(currentIndex, path);
                                    }
                                }
                                else
                                {
                                    PrintFiles(currentIndex, path);
                                }
                            }
                            catch (Exception)
                            {
                                
                                //throw;
                            }
                            PrintFiles(currentIndex, path);
                        }
                        break;
                    //Открытие текущего файла
                    case ConsoleKey.Enter:
                        {
                            try
                            {
                                string file = Directory.GetFiles(path)[currentIndex];
                                Process.Start(new ProcessStartInfo() { FileName = file, UseShellExecute = true });
                            }
                            catch (Exception)
                            {

                                //throw;
                            }
                        }
                        break;
                    //Переход в режим командной строки
                    case ConsoleKey.F3:
                        {
                            int cmdIndex = arrayCmd.Length;

                            int cursorLeft = 15;

                            PrintFiles(currentIndex, path);

                            Console.Write("Command Line " + (char)16 + " ");

                            string currentCmd = string.Empty;

                            while (true)
                            {
                                ConsoleKeyInfo infoCmd = Console.ReadKey();

                                switch (infoCmd.Key)
                                {
                                    //Навигация по истории команд, стрелка ВВЕРХ
                                    case ConsoleKey.UpArrow:
                                        {
                                            PrintFiles(currentIndex, path);
                                            if (arrayCmd.Length > 0)
                                            {
                                                cmdIndex = (cmdIndex - 1) >= 0 ? (cmdIndex - 1) : cmdIndex;
                                                currentCmd = arrayCmd[cmdIndex];
                                            }
                                            Console.Write("Command Line " + (char)16 + " " + currentCmd);

                                            cursorLeft = 15;
                                            Console.SetCursorPosition(cursorLeft, 23);
                                        }
                                        break;
                                    //Навигация по истории команд, стрелка ВНИЗ
                                    case ConsoleKey.DownArrow:
                                        {
                                            PrintFiles(currentIndex, path);
                                            if (arrayCmd.Length > 0)
                                            {
                                                cmdIndex = (cmdIndex + 1) < arrayCmd.Length ? (cmdIndex + 1) : cmdIndex;
                                                currentCmd = arrayCmd[cmdIndex];
                                            }
                                            Console.Write("Command Line " + (char)16 + " " + currentCmd);

                                            cursorLeft = 15;
                                            Console.SetCursorPosition(cursorLeft, 23);
                                        }
                                        break;
                                    //Навигация по строке команд, стрелка ВЛЕВО
                                    case ConsoleKey.LeftArrow:
                                        {
                                            PrintFiles(currentIndex, path);
                                            Console.Write("Command Line " + (char)16 + " " + currentCmd);

                                            cursorLeft = cursorLeft > 15 ? cursorLeft - 1 : cursorLeft;
                                            Console.SetCursorPosition(cursorLeft, 23);
                                        }
                                        break;
                                    //Навигация по строке команд, стрелка ВПРАВО
                                    case ConsoleKey.RightArrow:
                                        {
                                            PrintFiles(currentIndex, path);
                                            Console.Write("Command Line " + (char)16 + " " + currentCmd);

                                            cursorLeft = cursorLeft < (currentCmd.Length + 15) ? cursorLeft + 1 : cursorLeft;
                                            Console.SetCursorPosition(cursorLeft, 23);
                                        }
                                        break;
                                    //Навигация по строке команд, перевод курсора в конец строки команды
                                    case ConsoleKey.End:
                                        {
                                            PrintFiles(currentIndex, path);
                                            Console.Write("Command Line " + (char)16 + " " + currentCmd);

                                            cursorLeft = currentCmd.Length + 15;
                                            Console.SetCursorPosition(cursorLeft, 23);
                                        }
                                        break;
                                    //Навигация по строке команд, перевод курсора в начало строки команды
                                    case ConsoleKey.Home:
                                        {
                                            PrintFiles(currentIndex, path);
                                            Console.Write("Command Line " + (char)16 + " " + currentCmd);

                                            cursorLeft = 15;
                                            Console.SetCursorPosition(cursorLeft, 23);
                                        }
                                        break;
                                    //Обработка исключений
                                    case ConsoleKey.Escape:
                                        {
                                            PrintFiles(currentIndex, path);
                                            Console.Write("Command Line " + (char)16 + " " + currentCmd);
                                        }
                                        break;
                                    //Удаление элемента слева от текущей позиции в строке команд
                                    case ConsoleKey.Backspace:
                                        {
                                            if (currentCmd.Length > 0)
                                            {
                                                currentCmd = currentCmd.Substring(0, cursorLeft - 15 - 1) + currentCmd.Substring(cursorLeft - 15);
                                            }
                                            PrintFiles(currentIndex, path);
                                            Console.Write("Command Line " + (char)16 + " " + currentCmd);

                                            cursorLeft = cursorLeft > 15 ? cursorLeft - 1 : cursorLeft;
                                            Console.SetCursorPosition(cursorLeft, 23);
                                        }
                                        break;
                                    //Удаление элемента справа от текущей позиции в строке команд
                                    case ConsoleKey.Delete:
                                        {
                                            if (currentCmd.Length > 0)
                                            {
                                                currentCmd = currentCmd.Substring(0, cursorLeft - 15) + currentCmd.Substring(cursorLeft - 15 + 1);
                                            }
                                            PrintFiles(currentIndex, path);
                                            Console.Write("Command Line " + (char)16 + " " + currentCmd);

                                            Console.SetCursorPosition(cursorLeft, 23);
                                        }
                                        break;
                                    //Выполнение команды
                                    case ConsoleKey.Enter:
                                        {
                                            //string currentCmd = Console.ReadLine();

                                            string[] paramCmd = CommandParse(currentCmd);

                                            if (paramCmd.Length > 0)
                                            {
                                                //Обработка команды смены пути: пример, cd "D:\"
                                                if (paramCmd.Length == 2 && paramCmd[0] == "cd")
                                                {
                                                    path = paramCmd[1];

                                                    currentIndex = 0;
                                                    PrintFiles(currentIndex, path);

                                                    Console.Write(" Completed Successfully!");
                                                    break;
                                                }

                                                //Обработка команды копирования: пример, cp "C:\Source" "C:\Target" или cp "C:\source.txt" "C:\target.txt"
                                                if (paramCmd.Length == 3 && paramCmd[0] == "cp")
                                                {
                                                    //Копирование директории и всех файлов (рекурсивно)
                                                    if (IsDirectory(paramCmd[1]))
                                                    {
                                                        try
                                                        {
                                                            CopyDirectory(paramCmd[1], paramCmd[2]);
                                                            PrintFiles(currentIndex, path);
                                                            Console.Write(" Completed Successfully!");
                                                        }
                                                        catch (Exception)
                                                        {
                                                            PrintFiles(currentIndex, path);
                                                            Console.Write(" Command Error!");
                                                            //throw;
                                                        }
                                                        break;
                                                    }

                                                    //Копирование файла
                                                    if (!IsDirectory(paramCmd[1]))
                                                    {
                                                        try
                                                        {
                                                            File.Copy(paramCmd[1], paramCmd[2], true);
                                                            PrintFiles(currentIndex, path);
                                                            Console.Write(" Completed Successfully!");
                                                            break;
                                                        }
                                                        catch (Exception)
                                                        {
                                                            PrintFiles(currentIndex, path);
                                                            Console.Write(" Command Error!");
                                                            //throw;
                                                        }
                                                        break;
                                                    }
                                                }
                                            }

                                            PrintFiles(currentIndex, path);
                                            Console.Write(" Command Error!");
                                        }
                                        break;
                                    default:
                                        {
                                            //Добавляем элемент к строке
                                            currentCmd = currentCmd.Insert(cursorLeft - 15, infoCmd.KeyChar.ToString());

                                            cursorLeft = cursorLeft + 1;

                                            PrintFiles(currentIndex, path);
                                            Console.Write("Command Line " + (char)16 + " " + currentCmd);

                                            //Восстанавливаем позицию курсора
                                            Console.SetCursorPosition(cursorLeft, 23);
                                        }
                                        break;
                                }

                                if (infoCmd.Key == ConsoleKey.Enter)
                                {
                                    Array.Resize(ref arrayCmd, arrayCmd.Length + 1);
                                    arrayCmd[arrayCmd.Length - 1] = currentCmd;
                                    break;
                                }
                            }
                        }
                        break;
                    //Удаление каталога (рекурсивно) или файла
                    case ConsoleKey.Delete:
                        {
                            PrintFiles(currentIndex, path);

                            string[] entries = Directory.GetFileSystemEntries(path);

                            if (entries.Length > 0)
                            {
                                //Подтверждение удаления каталога (рекурсивно) или файла
                                Console.Write(" Delete Directory or File? (Y/N) " + (char)16 + " ");

                                ConsoleKeyInfo answer = Console.ReadKey();

                                if (answer.Key == ConsoleKey.Y)
                                {
                                    try
                                    {
                                        //Процедура удаления каталога (рекурсивно) или файла
                                        DeleteFileOrDirectory(Directory.GetFileSystemEntries(path)[currentIndex]);

                                        currentIndex = 0;
                                        PrintFiles(currentIndex, path);

                                        Console.Write(" Deleted successfully!");
                                    }
                                    catch (Exception)
                                    {
                                        Console.Write(" Delete error!");
                                        //throw;
                                    }
                                }
                                else
                                {
                                    PrintFiles(currentIndex, path);
                                    Console.Write(" Delete cancelled!");
                                }
                            }
                        }
                        break;
                    //Выход
                    case ConsoleKey.Q:
                        {
                            //Запись настроек
                            Settings.Default.path = path;
                            Settings.Default.Save();

                            return;
                        }
                    //Действие во всех остальных случаях
                    default:
                        {
                            PrintFiles(currentIndex, path);
                        }
                        break;
                }
            }
        }

        //Постраничный вывод списка каталогов и файлов
        public static void PrintFiles(int currentIndex, string path)
        {
            Console.Clear();

            Console.WriteLine("..");

            //Инициализация массива каталогов и файлов
            string[] files = { };

            try
            {
                files = Directory.GetFileSystemEntries(path);
            }
            catch (Exception)
            {

                //throw;
            }

            //Количество элементов на странице
            int maxRow = 15;

            int page = currentIndex / maxRow;

            for (int i = page * maxRow; i < page * maxRow + maxRow; i++)
            {
                if (i >= files.Length)
                {
                    Console.WriteLine();
                    continue;
                }

                string marker;

                FileInfo info = new FileInfo(files[i]);

                //Установка маркера для каталога и файла
                if ((info.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    marker = "[+] ";
                }
                else
                {
                    marker = "    ";
                }

                if (currentIndex == i)
                {
                    ConsoleColor current = Console.BackgroundColor;

                    //Выделение текущего элемента
                    Console.BackgroundColor = ConsoleColor.DarkYellow;

                    Console.WriteLine(marker + files[i]);

                    Console.BackgroundColor = current;
                }
                else
                {
                    Console.WriteLine(marker + files[i]);
                }
            }

            //Вывод разделителя
            Console.WriteLine(new string('-', 50));

            if (files.Length > 0)
            {
                //Вывод информации о каталоге или файле
                PrintFile(files[currentIndex]);
            }
            else
            {
                //Сообщение о пустом каталоге или ошибке доступа
                Console.WriteLine("Empty Directory or Access Denied");
                Console.WriteLine("\n\n\n");
            }

            //Вывод разделителя
            Console.WriteLine(new string('-', 50));
        }

        //Вывод информации о каталоге или файле
        public static void PrintFile(string file)
        {
            FileInfo info = new FileInfo(file);

            Console.WriteLine("File or Directory Information:");

            if ((info.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                Console.WriteLine($"CreationTime: {info.CreationTime}");
                Console.WriteLine($"LastAccessTime: {info.LastAccessTime}");
                Console.WriteLine("\n");
            }
            else
            {
                Console.WriteLine($"CreationTime: {info.CreationTime}");
                Console.WriteLine($"LastAccessTime: {info.LastAccessTime}");
                Console.WriteLine($"Extension: {info.Extension}");
                Console.WriteLine($"Length: {info.Length} bytes");
            }
        }

        //Удаление каталога (рекурсивно) или файла
        public static void DeleteFileOrDirectory(string path)
        {
            if (IsDirectory(path))
            {
                //Удаление каталога (рекурсивно)

                //Можно сделать так, но реализуем рекурсивное удаление вручную
                //Directory.Delete(path, true);

                //Реализация рекурсивного удаления каталога
                DeleteDirectory(path);
            }
            else
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception)
                {

                    //throw;
                }
            }
        }

        //Опеределение каталога по пути
        public static bool IsDirectory(string path)
        {
            try
            {
                FileAttributes attr = File.GetAttributes(path);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    return true;
                }
            }
            catch (Exception)
            {

                //throw;
            }
            return false;
        }

        //Рекурсивное удаление каталога
        public static void DeleteDirectory(string path)
        {
            string[] files = { };

            try
            {
                files = Directory.GetFileSystemEntries(path);
            }
            catch (Exception)
            {

                //throw;
            }

            for (int i = 0; i < files.Length; i++)
            {
                if (IsDirectory(files[i]))
                {
                    DeleteDirectory(files[i]);
                    try
                    {
                        Directory.Delete(files[i]);
                    }
                    catch (Exception)
                    {

                        //throw;
                    }
                }
                else
                {
                    try
                    {
                        File.Delete(files[i]);
                    }
                    catch (Exception)
                    {

                        //throw;
                    }
                }
            }

            try
            {
                Directory.Delete(path);
            }
            catch (Exception)
            {

                //throw;
            }
        }

        //Копирование директории и всех файлов (рекурсивно)
        public static void CopyDirectory(string sourcePath, string targetPath)
        {
            Directory.CreateDirectory(targetPath);

            string[] files = { };

            try
            {
                files = Directory.GetFileSystemEntries(sourcePath);
            }
            catch (Exception)
            {

                //throw;
            }

            for (int i = 0; i < files.Length; i++)
            {
                FileInfo info = new FileInfo(files[i]);

                if (IsDirectory(files[i]))
                {
                    CopyDirectory(files[i], Path.Combine(targetPath, info.Name));
                }
                else
                {
                    File.Copy(files[i], Path.Combine(targetPath, info.Name), true);
                }
            }
        }

        //Парсер команд
        public static string[] CommandParse(string currentCmd)
        {
            string[] result = { };

            string temp = string.Empty;

            char separator = ' ';

            for (int i = 0; i < currentCmd.Length; i++)
            {
                if (currentCmd[i]==separator)
                {
                    if (temp!=string.Empty)
                    {
                        Array.Resize(ref result, result.Length + 1);
                        result[result.Length - 1] = temp;
                        temp = string.Empty;
                    }

                    separator = ' ';
                    continue;
                }

                if (currentCmd[i] == '"' && i>1 && currentCmd[i-1] == ' ')
                {
                    separator = '"';
                    continue;
                }

                temp = temp + currentCmd[i];
            }

            return result;
        }
    }
}
