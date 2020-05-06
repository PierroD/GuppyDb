using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GuppyDbConsole
{
  public  class DataCommands
    {
       private static Regex rx;

        #region  Create
        public static void Insert(string command)
        {
            rx = new Regex("(?<=insert into\\s).*?[(]");
            string insert_table = "\\" + rx.Match(command).Value.Replace("(", String.Empty);
            if (Directory.Exists(Program.DatabaseFolder + insert_table))
            {
                rx = new Regex("[(].*?[)]");
                string[] fields = rx.Matches(command)[0].ToString().Replace(" ", String.Empty).Replace("(", String.Empty).Replace(")", String.Empty).Split(',');
                string[] values = rx.Matches(command)[1].ToString().Replace(" ", String.Empty).Replace("(", String.Empty).Replace(")", String.Empty).Split(',');
                bool insert = true;
                foreach (string file in fields)
                {
                    if (!File.Exists(Program.DatabaseFolder + insert_table + "\\" + file + ".gpx"))
                        insert = false;
                }
                if (new DirectoryInfo(Program.DatabaseFolder + insert_table).GetFiles().Count() - 1 != fields.Count())
                    insert = false;

                if (insert)
                {
                    int new_id;
                    if (File.ReadLines(Program.DatabaseFolder + insert_table + "\\id.gpx").Count() != 0)
                        new_id = int.Parse(File.ReadLines(Program.DatabaseFolder + insert_table + "\\id.gpx").Last()) + 1;
                    else
                        new_id = 0;

                    using (StreamWriter sw = File.AppendText(Program.DatabaseFolder + insert_table + "\\id.gpx"))
                    {
                        sw.WriteLine(new_id);
                        sw.Close();
                    }
                    for (int i = 0; i < fields.Length; i++)
                    {
                        using (StreamWriter sw = File.AppendText(Program.DatabaseFolder + insert_table + "\\" + fields[i] + ".gpx"))
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

        }
        #endregion

        #region Read
        public static void Select(string command)
        {
            try
            {
                rx = new Regex("^select.*?from");
                string[] select_values = rx.Match(command).Value.Replace("select", String.Empty).Replace("from", String.Empty).Replace(" ", String.Empty).Split(',');
                rx = new Regex("[from]((?!where).)*$"); // ex : select * from users, etc (récupère )
                string[] select_tables = rx.Match(command).Value.Replace("from", String.Empty).Replace(" ", String.Empty).Split(',');
                string select_result = "";
                if (select_values[0].Contains("*"))
                {
                    foreach (string select_table in select_tables)
                    {
                        FileInfo[] select_files = new DirectoryInfo(Program.DatabaseFolder + "\\" + select_table).GetFiles().OrderBy(o => o.CreationTime).ToArray();
                        int table_size = File.ReadAllLines(Program.DatabaseFolder + "\\" + select_table + "\\" + select_files[0]).Length;

                        if (table_size != 0)
                        {
                            for (int i = 0; i < table_size; i++)
                            {
                                select_result += "{ ";
                                foreach (FileInfo select_file in select_files)
                                {
                                    List<string> datas = File.ReadAllLines(Program.DatabaseFolder + "\\" + select_table + "\\" + select_file.Name).ToList();
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
        }
        #endregion

        #region Delete
        public static void Delete(string command)
        {
            rx = new Regex("(?<=from\\s).*?\\s");
            string delete_table = "\\" + rx.Match(command).Value.Replace(" ", String.Empty);
            if (Directory.Exists(Program.DatabaseFolder + delete_table))
            {
                rx = new Regex("(?<=where\\s).*");
                string[] delete_values = rx.Match(command).Value.Split(' ');
                bool delete = true;
                string path = Program.DatabaseFolder + delete_table + "\\" + delete_values[0] + ".gpx";
                if (!File.Exists(path))
                    delete = false;

                if (delete)
                {
                    List<string> quotelist = File.ReadAllLines(Program.DatabaseFolder + delete_table + "\\" + delete_values[0] + ".gpx").ToList();
                    List<int> line_to_delete = new List<int>();
                    for (int i = 0; i < quotelist.Count(); i++)
                    {
                        if (Compare(delete_values[1], quotelist[i], delete_values[2])) // operator, left, right (>, 7, 8)
                        {
                            line_to_delete.Add(i);
                        }
                    }
                    File.WriteAllLines(Program.DatabaseFolder + delete_table + "\\" + delete_values[0] + ".gpx", quotelist.ToArray());

                    foreach (FileInfo file in new DirectoryInfo(Program.DatabaseFolder + delete_table).GetFiles())
                    {
                        quotelist = File.ReadAllLines(file.FullName).ToList();
                        foreach (int delete_index in line_to_delete.OrderByDescending(key => key))
                        {
                            quotelist.RemoveAt(delete_index);
                        }
                        File.WriteAllLines(Program.DatabaseFolder + delete_table + "\\" + file, quotelist.ToArray());

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


        }
        #endregion

        #region Where
        private static void Where(string whereCommand)
        {

        }
        #endregion

        #region Compare(operator, left, right) ex : (>, 7, 8) return false : 7 > 8 is false
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

        #endregion
    }
}
