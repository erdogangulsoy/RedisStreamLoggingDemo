using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LogProcessor
{
    class Program
    {
        static readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        static IConnectionMultiplexer connection;
        static IServer server;
        static IDatabase db;
        static string position = "0-0";

        static Program()
        {
            connection = ConnectionMultiplexer.Connect("127.0.0.1");
            db = connection.GetDatabase();
            server = connection.GetServer("127.0.0.1:6379");
        }
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += ExitService;
            Console.WriteLine("------------------- SERVICE START ------------------- ");
            while (!tokenSource.Token.IsCancellationRequested)
            {
                RunProcess(null);
                //Thread.Sleep(2000);
            }
            Console.WriteLine("------------------- SERVICE STOP ------------------- ");
        }
        public static void RunProcess(Object stateInfo)
        {

            List<Log> logs = new List<Log>();
            StreamEntry[] entries = db.StreamRead("Stream:Customer", position);
            foreach (var entry in entries)
            {

                string entityKey = ((NameValueEntry)entry.Values.GetValue(0)).Value.ToString();
                string logKey = ((NameValueEntry)entry.Values.GetValue(1)).Value.ToString();



                Log logItem = new Log();
                logs.Add(logItem);
                logItem.Current = db.HashGetAll(entityKey);

                //search for the key and get the most up to date one
                var keys = server.Keys(pattern: $"log:{entityKey}*", pageSize: 10000);
                string previousSnapshotKey = "";


                List<string> keyList = keys.Select(k => k.ToString()).OrderByDescending(o => o.ToString()).ToList();

                foreach (var k in keyList)
                {
                    if (String.Compare(logKey, k) > 0)
                    {
                        //logKey the most up to date record. 
                        //we will take the one below 
                        previousSnapshotKey = k;
                        break;
                    }
                }




                #region New Record
                if (previousSnapshotKey == String.Empty)
                {
                    //it is a new record.
                    foreach (var item in logItem.Current)
                    {
                        logItem.Added.Add(item.Name);
                    }

                    Console.WriteLine("== Added Items ==");
                    foreach (var d in logItem.Added)
                    {
                        Console.WriteLine($"{d}={logItem.Current.ToDictionary()[d]}");
                    }

                    position = entry.Id;
                    continue;
                }

                #endregion

                logItem.Snapshot = db.HashGetAll(previousSnapshotKey);

                #region Added/Updated Action
                foreach (var item in logItem.Current)
                {

                    HashEntry snapshotItem = Array.Find(logItem.Snapshot, s => s.Name == item.Name);

                    if (snapshotItem != null)
                    {
                        //key exists
                        if (snapshotItem.Value != item.Value) logItem.Updated.Add(item.Name); //value has been updated
                    }
                    else
                    {
                        //key not exists. It must be new field
                        logItem.Added.Add(item.Name); //new value

                    }

                }

                Console.WriteLine("== Updated Items ==");
                foreach (var d in logItem.Updated)
                {
                    Console.WriteLine($"{d}={logItem.Current.ToDictionary()[d]}");
                }

                #endregion

                #region Deleted Action
                foreach (var item in logItem.Snapshot)
                {
                    if (!Array.Exists(logItem.Current, a => a.Name == item.Name))
                    {
                        //new set has no such key
                        logItem.Removed.Add(item.Name);
                    }

                }
                #endregion

                position = entry.Id;
            }

        }
        private static void ExitService(object sender, EventArgs e)
        {
            Console.WriteLine("------------------ - PROC EXIT SIGNAL------------------ - ");
            tokenSource.Cancel();
        }
    }
}
