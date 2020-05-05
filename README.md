# GuppyDb

## About

GuppyDb is a C# console application.

GuppyDb is based on [FreeGuppy](https://www.freeguppy.org) database system.

- **Quick reminder :**

Guppy is a benchmark database free CMS created by Aldweb on September 24th, 2003 (based on miniPortail).

Guppy is using .txt files to store the CMS's data.

## Summary

- [How to install](#install)
- [All the features](#features)

## <a name="install"></a>How to install

Download the .exe file <a href="https://github.com/PierroD/GuppyDb/raw/master/GuppyDbConsole/bin/Debug/GuppyDbConsole.exe">here</a> (your windows defender could refuse to download it, you just have to allow it on your computer)

Put it somewhere on your computer double-click on the .exe file and there you go.

## <a name="features"></a>All the features

You can also see all the features directly by typing : 'help' in GuppyDb's console

- `help` : is providing all the commands includes in GuppyDb
- `cls` : to clear the console
- `exit` : close the console
- `show databases` : to show all the databases
- `show tables` : to show all the tables
- `use dbName` : to use or create a database
- `create table tbName`: to create a table (with a default field named ' id' use as a primary key)
- `switch table tbName` : allow you to go inside the table to add fields
- `add fields fName` : to add a field
- `desc tbName` : to see the table structure
- `drop database dbName` : to drop the database

Few commands are available :

- Select all :

```
select * from tableNames
```

- Insert data :

```
insert into tableNames(name, email, phone) values("pierro", "pierro@gmail.com", "0606060606")
```

- Delete data

```
delete from tableName where name = "pierro" // equal
delete from tableName where id <> "pierro" // not-equal
delete from tableName where id > 1
delete from tableName where id < 3
delete from tableName where id >= 2
delete from tableName where id <= 2
// spaces between field, the operator and the value are important
```
