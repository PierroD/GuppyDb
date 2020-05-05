using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;

namespace GuppyDbConsole
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "GuppyDb - Console";
            string rootFolder = Directory.GetCurrentDirectory() + "\\db";
            string DatabaseFolder = rootFolder;
            string TableFolder = "";

            if (!Directory.Exists(DatabaseFolder))
                Directory.CreateDirectory(DatabaseFolder);

            #region introduction
            Console.WriteLine(@"
 ________  ___  ___  ________  ________  ___    ___ ________  ________     
|\   ____\|\  \|\  \|\   __  \|\   __  \|\  \  /  /|\   ___ \|\   __  \    
\ \  \___|\ \  \\\  \ \  \|\  \ \  \|\  \ \  \/  / | \  \_|\ \ \  \|\ /_   
 \ \  \  __\ \  \\\  \ \   ____\ \   ____\ \    / / \ \  \ \\ \ \   __  \  
  \ \  \|\  \ \  \\\  \ \  \___|\ \  \___|\/  /  /   \ \  \_\\ \ \  \|\  \ 
   \ \_______\ \_______\ \__\    \ \__\ __/  / /      \ \_______\ \_______\
    \|_______|\|_______|\|__|     \|__||\___/ /        \|_______|\|_______|
                                       \|___|/                             
                                             
Welcome to the world first file database console manager (v:1.0)
Use 'help' to discover all the commands you need
");

            #endregion

            #region readline prefix
            Console.WriteLine();
            Console.Write("GuppyDb> ");
            #endregion
            bool running = true;
            while (running)
            {
                string line = Console.ReadLine().Replace("Guppydb> ", String.Empty).ToLower();
                switch (line)
                {
                    #region help
                    case "help":
                        Console.WriteLine(@"
+----------------+
| HELP
+----------------+
help                    to see all the commands
cls                     to clear the console 
exit                    to exit GuppyDb
show databases          show all the databases
show tables             show all the tables inside the selected database
use <dbName>            switch to the 'databaseName', if the database doesn't exist it will create one automatically
create table <tbName>   create a table called 'tbName' with a default field (id)
switch table <tbName>   switch to the table name 'tbName'
add field <fieldName>   create a field inside the table
desc <tbName>           show all the fields of the table inside the selected database
drop database <dbName>  delete the whole database
+----------------+
| Example
+----------------+
insert into users(name, email) values(""test"", 'test@mail.com')
delete from users where id = 1
select * from users
");
                        break;
                    #endregion

                    #region clear
                    case "cls":
                        Console.Clear();
                        break;
                    #endregion

                    #region exit
                    case "exit":
                        running = !running;
                        break;
                    #endregion

                    #region show databases
                    case "show databases":
                        if (Directory.GetDirectories(rootFolder).Length != 0)
                        {
                            Console.WriteLine(@"
+----------------+
| Databases   
+----------------+");
                            foreach (DirectoryInfo database in new DirectoryInfo(rootFolder).GetDirectories())
                            {
                                Console.WriteLine($"| {database.Name} ");
                            }
                            Console.Write($"+----------------+");
                        }
                        else
                            OutMessage.Error(@"GuppyDb is empty
-> to create a database, type : 'use <dbName>'
");
                        break;
                    #endregion

                    #region use
                    case string u when u.StartsWith("use"):
                        DatabaseFolder = rootFolder + $"\\{u.Remove(0, 4).Replace(" ", String.Empty)}";
                        if (!Directory.Exists(DatabaseFolder))
                        {
                            Directory.CreateDirectory(DatabaseFolder);
                            OutMessage.Success($"Database created : {u.Remove(0, 4)}");
                        }
                        OutMessage.Success($"Database changed to : {u.Remove(0, 4)}");
                        break;
                    #endregion

                    #region create table
                    case string ct when ct.StartsWith("create table"):
                        TableFolder = DatabaseFolder + $"\\{ct.Remove(0, 13)}";
                        if (!Directory.Exists(TableFolder) && !DatabaseFolder.EndsWith("db"))
                        {
                            Directory.CreateDirectory(TableFolder);
                            string idFile = TableFolder + "\\id.gpx";
                            if (!File.Exists(idFile))
                            {
                                FileStream f = System.IO.File.Create(idFile);
                                f.Close();
                            }
                            OutMessage.Success($"Add a table called {ct.Remove(0, 13)} inside {new DirectoryInfo(DatabaseFolder).Name} database ");
                        }
                        else
                            OutMessage.Error($@"{ct.Remove(0, 13)} allready exist or you aren't inside a database");



                        break;
                    #endregion

                    #region switch table
                    case string st when st.StartsWith("switch table"):
                        string table = DatabaseFolder + $"\\{st.Remove(0, 13)}";
                        if (Directory.Exists(table) && !DatabaseFolder.EndsWith("db"))
                        {
                            TableFolder = table;
                            OutMessage.Success($"Table changed to : {st.Remove(0, 13)}");
                        }
                        else
                            OutMessage.Error($@"{st.Remove(0, 13)} table doesn't exist or you aren't inside a database
to see all the commands type 'help'
");

                        break;
                    #endregion

                    #region add field
                    case string af when af.StartsWith("add field"):
                        string field = TableFolder + $"\\{af.Remove(0, 10)}.gpx";
                        if (!File.Exists(field) && TableFolder != String.Empty)
                        {
                            FileStream f = System.IO.File.Create(field);
                            f.Close();
                            if (File.ReadLines(TableFolder + "\\id.gpx").Count() > 0)
                            {
                                using (StreamWriter sw = File.AppendText(field))
                                {
                                    for (int i = 0; i < File.ReadLines(TableFolder + "\\id.gpx").Count(); i++)
                                    {
                                        sw.WriteLine("NULL");
                                    }
                                }
                            }
                            OutMessage.Success($"{new FileInfo(field).Name.Replace(".gpx", String.Empty)} has been added to the table {new DirectoryInfo(TableFolder).Name}, inside {new DirectoryInfo(DatabaseFolder).Name} database");
                        }
                        else
                        {
                            OutMessage.Error($@"
{new FileInfo(field).Name.Replace(".gpx", String.Empty)} already exist or you aren't inside a table
-> to select the table use 'switch table <tbName>'
");
                        }

                        break;
                    #endregion

                    #region show tables
                    case "show tables":
                        if (!DatabaseFolder.EndsWith("db"))
                        {
                            if (Directory.GetDirectories(DatabaseFolder).Length != 0)
                            {
                                Console.WriteLine($@"
+----------------+
| {new DirectoryInfo(DatabaseFolder).Name} 
+----------------+");
                                foreach (DirectoryInfo directory in new DirectoryInfo(DatabaseFolder).GetDirectories())
                                {
                                    Console.WriteLine($"| {directory.Name}");
                                }
                                Console.Write($"+----------------+");
                            }
                            else
                                OutMessage.Success("This database is Empty.");
                        }
                        else
                            OutMessage.Error(@"You aren't inside a database
-> type 'use <dbName>' to go inside a database");

                        break;
                    #endregion

                    #region desc
                    case string d when d.StartsWith("desc"):
                        if (Directory.Exists(DatabaseFolder + $"\\{d.Remove(0, 5)}"))
                        {
                            Console.WriteLine($@"
+----------------+
| {d.Remove(0, 5)}
+----------------+");
                            foreach (FileInfo file in new DirectoryInfo(DatabaseFolder + $"\\{d.Remove(0, 5)}").GetFiles().OrderBy(o => o.CreationTime))
                            {
                                Console.WriteLine($"| {file.Name.Replace(".gpx", String.Empty)}");
                            }
                            Console.Write($"+----------------+");
                        }
                        else
                            OutMessage.Warning($@"{d.Remove(0, 5)} table doesn't exist or you aren't inside a database
-> type 'use <dbName>' to go inside a database
-> type 'show tables' to see all the tables inside the database");

                        break;
                    #endregion

                    #region drop database
                    case string dd when dd.Contains("drop database"):
                        string drop_database = "\\" + dd.Replace("drop database", String.Empty).Replace(" ", String.Empty);
                        if (Directory.Exists(rootFolder + drop_database))
                        {
                            Directory.Delete(rootFolder + drop_database, true);
                            OutMessage.Success($"The database named : {drop_database.Remove(0, 1)} has been dropped");
                        }
                        else
                            OutMessage.Warning($@"{drop_database.Remove(0, 1)} you can't drop a database which doesn't exist
-> type 'show databases' to see all the databases");
                        break;
                    #endregion

                    #region insert into
                    case string it when it.Contains("insert into"):
                        Regex rx;
                        rx = new Regex("(?<=insert into\\s).*?[(]");
                        string insert_table = "\\" + rx.Match(it).Value.Replace("(", String.Empty);
                        if (Directory.Exists(DatabaseFolder + insert_table))
                        {
                            rx = new Regex("[(].*?[)]");
                            string[] fields = rx.Matches(it)[0].ToString().Replace(" ", String.Empty).Replace("(", String.Empty).Replace(")", String.Empty).Split(',');
                            string[] values = rx.Matches(it)[1].ToString().Replace(" ", String.Empty).Replace("(", String.Empty).Replace(")", String.Empty).Split(',');
                            bool insert = true;
                            foreach (string file in fields)
                            {
                                if (!File.Exists(DatabaseFolder + insert_table + "\\" + file + ".gpx"))
                                    insert = false;
                            }
                            if (new DirectoryInfo(DatabaseFolder + insert_table).GetFiles().Count()-1 != fields.Count())
                                insert = false;

                            if (insert)
                            {
                                int new_id;
                                if (File.ReadLines(DatabaseFolder + insert_table + "\\id.gpx").Count() != 0)
                                    new_id = int.Parse(File.ReadLines(DatabaseFolder + insert_table + "\\id.gpx").Last()) + 1;
                                else
                                    new_id = 0;

                                using (StreamWriter sw = File.AppendText(DatabaseFolder + insert_table + "\\id.gpx"))
                                {
                                    sw.WriteLine(new_id);
                                    sw.Close();
                                }
                                for (int i = 0; i < fields.Length; i++)
                                {
                                    using (StreamWriter sw = File.AppendText(DatabaseFolder + insert_table + "\\" + fields[i] + ".gpx"))
                                    {
                                        sw.WriteLine(values[i]);
                                        sw.Close();
                                    }
                                }
                                OutMessage.Success("Data inserted successfully");
                            }
                            else
                                OutMessage.Error("A field is missing or isn't matching with table's one");
                        }
                        else
                            OutMessage.Error($@"{insert_table.Remove(0, 1)} doesn't exist or you aren't inside a database
-> type 'use <dbName>' to go inside a database
-> type 'show tables' to see all the tables inside the database");
                        break;
                    #endregion

                    #region delete 
                    case string d when d.Contains("delete"):
                        rx = new Regex("(?<=from\\s).*?\\s");
                        string delete_table = "\\" + rx.Match(d).Value.Replace(" ", String.Empty);
                        if (Directory.Exists(DatabaseFolder + delete_table))
                        {
                            rx = new Regex("(?<=where\\s).*");
                            string[] delete_values = rx.Match(d).Value.Split(' ');
                            bool delete = true;
                            string path = DatabaseFolder + delete_table + "\\" + delete_values[0] + ".gpx";
                            if (!File.Exists(path))
                                delete = false;

                            if (delete)
                            {
                                List<string> quotelist = File.ReadAllLines(DatabaseFolder + delete_table + "\\" + delete_values[0] + ".gpx").ToList();
                                List<int> line_to_delete = new List<int>();
                                for (int i = 0; i < quotelist.Count(); i++)
                                {
                                    if (Compare(delete_values[1], quotelist[i], delete_values[2])) // operator, left, right (>, 7, 8)
                                    {
                                        line_to_delete.Add(i);
                                    }
                                }
                                File.WriteAllLines(DatabaseFolder + delete_table + "\\" + delete_values[0] + ".gpx", quotelist.ToArray());

                                foreach (FileInfo file in new DirectoryInfo(DatabaseFolder + delete_table).GetFiles())
                                {
                                    quotelist = File.ReadAllLines(file.FullName).ToList();
                                    foreach (int delete_index in line_to_delete.OrderByDescending(key => key))
                                    {
                                        quotelist.RemoveAt(delete_index);
                                    }
                                    File.WriteAllLines(DatabaseFolder + delete_table + "\\" + file, quotelist.ToArray());

                                }
                                OutMessage.Success("Data(s) deleted successfully");
                            }
                            else
                                OutMessage.Warning("The delete condition is false");
                        }
                        else
                            OutMessage.Warning($@"{delete_table.Remove(0, 1)} doesn't exist or you aren't inside a database
-> type 'use <dbName>' to go inside a database
-> type 'show tables' to see all the tables inside the database");

                        break;
                    #endregion

                    #region select 
                    case string s when s.StartsWith("select"):
                        try
                        {
                            rx = new Regex("^select.*?from");
                            string[] select_values = rx.Match(s).Value.Replace("select", String.Empty).Replace("from", String.Empty).Replace(" ", String.Empty).Split(',');
                            rx = new Regex("[from]((?!where).)*$"); // ex : select * from users, etc (récupère )
                            string[] select_tables = rx.Match(s).Value.Replace("from", String.Empty).Replace(" ", String.Empty).Split(',');
                            string select_result = "";
                            if (select_values[0].Contains("*"))
                            {
                                foreach (string select_table in select_tables)
                                {
                                    FileInfo[] select_files = new DirectoryInfo(DatabaseFolder + "\\" + select_table).GetFiles().OrderBy(o => o.CreationTime).ToArray();
                                    int table_size = File.ReadAllLines(DatabaseFolder + "\\" + select_table + "\\" + select_files[0]).Length;

                                    if (table_size != 0)
                                    {
                                        for (int i = 0; i < table_size; i++)
                                        {
                                            select_result += "{ ";
                                            foreach (FileInfo select_file in select_files)
                                            {
                                                List<string> datas = File.ReadAllLines(DatabaseFolder + "\\" + select_table + "\\" + select_file.Name).ToList();
                                                select_result += select_file.Name.Replace(".gpx", String.Empty) + " : " + datas[i] + ", ";
                                            }
                                            select_result = select_result.Remove(select_result.Length - 2) + " }" + Environment.NewLine;
                                        }
                                    }
                                    else
                                        Console.WriteLine($"{select_table} is empty");
                                }
                                Console.WriteLine(select_result);
                            }
                        }
                        catch (Exception)
                        {
                            OutMessage.Error(@"
-> You aren't inside database
-> The selected table doesn't exist");
                        }
                        break;
                    #endregion

                    #region default
                    default:
                        OutMessage.Error("La commande demandé n'existe pas");
                        break;
                        #endregion
                }
                #region readline prefix
                Console.WriteLine();
                Console.Write("GuppyDb> ");
                #endregion
            }

        }

        public static bool Compare<T>(string op, T left, T right) where T : IComparable<T>
        {
            switch (op)
            {
                case "<": return left.CompareTo(right) < 0;
                case ">": return left.CompareTo(right) > 0;
                case "<=": return left.CompareTo(right) <= 0;
                case ">=": return left.CompareTo(right) >= 0;
                case "=": return left.Equals(right);
                case "<>": return !left.Equals(right);
                default: throw new ArgumentException("Invalid comparison operator: {0}", op);
            }
        }
    }
}
