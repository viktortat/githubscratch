﻿using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace ConsoleApplication10 {
    class NetworkStuff {
        static void Main5(){
            //MainA().Wait();
            Task t = MainA();
            t.Wait();
        }

        static async Task MainA(){
            var time = DateTime.Now;
            Task<string> t1 = DoSomething("t1", 4000);
            var t2 = DoSomething("t2", 3000);
            var t3 = DoSomethingTaskRun("t3", 2000);
            var t4 = Task.Delay(6000);
            var t5 = DoSomethingError("t5", 2000);

            // Wait for t1 which is 4s, then t2 and t3 will have finished
            await t1;
            Console.WriteLine("after t1 - 4s - {0}", Actual(time));
            await t2;
            Console.WriteLine("after t2 - 3s - {0}", Actual(time));
            await t3;
            Console.WriteLine("after t3 - 2s - {0}", Actual(time));
            // after another 2s t4 will have finished
            await t4;
            Console.WriteLine("after t4 - 6s - {0}", Actual(time));
            // then immediately t5 will error
            try {
                await t5;
            }
            catch (Exception ex) {
                Console.WriteLine("error t5 - 2s - {0} - {1}", Actual(time), ex.Message);
            }

            // Returning data from an async method
            Console.WriteLine("Done: {0}", String.Join(", ", t1.Result, t2.Result, t3.Result));
            Console.WriteLine("Total: " + Actual(time));
        }

        static async Task<string> DoSomething(string name, int timeout){
            var sw = new Stopwatch();
            sw.Start();
            await Task.Delay(timeout);
            var message = "Exit " + name + " in: " + sw.ElapsedMilliseconds;
            Console.WriteLine(message);
            return message;
        }

        static async Task<string> DoSomethingTaskRun(string name, int timeout) {
            var sw = new Stopwatch();
            sw.Start();
            // Await the execution until the sleep completes
            await Task.Run(() => System.Threading.Thread.Sleep(timeout));
            var message = "Exit " + name + " in: " + sw.ElapsedMilliseconds;
            Console.WriteLine(message);
            return message;
        }

        static async Task<string> DoSomethingError(string name, int timeout) {
            var sw = new Stopwatch();
            sw.Start();
            await Task.Delay(timeout);
            throw new Exception("test error");
        }

        static string Actual(DateTime time) {
            return (DateTime.Now - time).TotalMilliseconds.ToString();
        }


        //http://stackoverflow.com/questions/18310996/why-should-i-prefer-single-await-task-whenall-over-multiple-awaits
        static async Task<string> DoTaskAsync(string name, int timeout) {
            var start = DateTime.Now;
            Console.WriteLine("Enter {0}, {1}", name, timeout);
            await Task.Delay(timeout);
            Console.WriteLine("Exit {0}, {1}", name, (DateTime.Now - start).TotalMilliseconds);
            return name;
        }

        static async Task<int> Something(){
            await Task.Run(() => System.Threading.Thread.Sleep(4000));
            Console.WriteLine("exit something1");
            return 1;
        }

        static async Task<int> Something2() {
            await Task.Run(() => System.Threading.Thread.Sleep(2000));
            Console.WriteLine("exit something2");
            return 2;
        }

        static async Task DoWork1(){
            var sw = new Stopwatch();
            sw.Start();

            var t1 = DoTaskAsync("t1", 3000);
            var t2 = DoTaskAsync("t2", 2000);
            var t3 = DoTaskAsync("t3", 1000);
            Task<int> t5 = Something2();
            Task<int> t6 = Something2();

            //http://blog.stephencleary.com/2012/02/async-and-await.html - **HERE** grok**outl
            await t1;
            await t2;
            await t3;
            await t5;
            await t6;

            // seems to accomplish the same thing
            // but don't have access to the task.. for error handling?
            // no this waits for everything above to finish
            Console.WriteLine("here");
            int result = await Task.Run(() => Something());
            int result2 = await Task.Run(() => Something());
            

            Console.WriteLine("DoWork1 results: {0}", String.Join(", ", t1.Result, t2.Result, t3.Result, result, t5.Result));
            Console.WriteLine("Total time: " + sw.Elapsed.TotalMilliseconds);
        }

        static async Task DoWork2() {
            var t1 = DoTaskAsync("t2.1", 3000);
            var t2 = DoTaskAsync("t2.2", 2000);
            var t3 = DoTaskAsync("t2.3", 1000);

            // will propogate all errors at once
            // will wait for all tasks to complete even if one throws or is cancelled
            await Task.WhenAll(t1, t2, t3);

            Console.WriteLine("DoWork2 results: {0}", String.Join(", ", t1.Result, t2.Result, t3.Result));
        }


        static void Main4(string[] args) {

            //Task.WhenAll(DoWork2()).Wait();
            //Task.WhenAll(DoWork1()).Wait();
            DoWork1().Wait();
        }



        //http://blogs.msdn.com/b/henrikn/archive/2012/02/16/httpclient-is-here.aspx
        //http://blogs.msdn.com/b/webdev/archive/2012/08/26/asp-net-web-api-and-httpclient-samples.aspx
        //static string _address = "http://api.worldbank.org/countries?format=json";
        //static string _address = "http://www.programgood.netXXXX/CategoryView,category,c.aspx";

        // davemateeroutlook
        //9CO7ZPJ3MTA2LGSUN 
        private static string _address = "http://developer.echonest.com/api/v4/artist/biographies?api_key=9CO7ZPJ3MTA2LGSUN&id=spotify:artist:4Z8W4fKeB5YxbusRsdQVPb";

        // this is their dev API key
        //private static string _address = "http://developer.echonest.com/api/v4/artist/biographies?api_key=FILDTEOIK2HBORODV&id=spotify:artist:4Z8W4fKeB5YxbusRsdQVPb";

        static void Main3(){
            MainAsync();
            Console.WriteLine("Done");
        }

        static async void MainAsync(){
            var info1 = GetHttpWithTimingInfo("http://google.com");
            var info2 = GetHttpWithTimingInfo("http://stackoverflow.com");
            var info3 = GetHttpWithTimingInfo("http://twitter.com");
            var info4 = GetHttpWithTimingInfo("http://www.programgood.net");
            var info5 = GetHttpWithTimingInfo("http://www.davemateer.com");
            var info6 = GetHttpWithTimingInfo("http://www.cnn.com");
            var info7 = GetHttpWithTimingInfo("http://www.bbc.co.uk");

            
            Task.WhenAll(info1, info2, info3,info4,info5,info6,info7);
            Console.WriteLine("Request1 took {0}", info1.Result.Item2);
            Console.WriteLine("Request2 took {0}", info2.Result.Item2);
            Console.WriteLine("Request3 took {0}", info3.Result.Item2);
            Console.WriteLine("Request4 took {0}", info4.Result.Item2);
            Console.WriteLine("Request5 took {0}", info5.Result.Item2);
            Console.WriteLine("Request6 took {0}", info6.Result.Item2);
            Console.WriteLine("Request7 took {0}", info7.Result.Item2);
        }

        private static async Task<Tuple<HttpResponseMessage, TimeSpan>> GetHttpWithTimingInfo(string url) {
            var stopWatch = Stopwatch.StartNew();
            using (var client = new HttpClient()) {
                var result = await client.GetAsync(url);
                return new Tuple<HttpResponseMessage, TimeSpan>(result, stopWatch.Elapsed);
            }
        }

        static void Main1(){
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i = 0; i < 10; i++) {
                Console.WriteLine("Starting " + i);
                var request = HttpWebRequest.CreateHttp(_address);
                string text;
                using (var response = request.GetResponse()) {
                    using (var sr = new StreamReader(response.GetResponseStream())) {
                        text = sr.ReadToEnd();
                    }
                }
                Console.WriteLine("result of " + i + " " + text.Substring(0, 100));
            }
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:0}", ts.TotalMilliseconds);
            Console.WriteLine("Total: " + elapsedTime);
        }

        static void Main(){
            Task t = MainAA();
            t.Wait();
        }

        static async Task MainAA(){
            var time = DateTime.Now;
            ServicePointManager.DefaultConnectionLimit = 5;
            const int n = 20;
            var tasks = new Task<string>[n];
            for (int i = 0; i < n; i++) {
                tasks[i] = CallAPI(i);
            }

            for (int i = 0; i < n; i++){
                // Will await the tasks in order
                await tasks[i];
                Console.WriteLine("{0} - {1}",i, Actual(time));
            }

            //Task.WaitAll(tasks);
            Console.WriteLine("after all tasks - {0}", Actual(time));
            Console.ReadLine();
        }

        static async Task<string> CallAPI(int i){
            var keepTrying = true;
            int totalErrors = 0;
            while (keepTrying){
                try{
                    //var stopWatch = new Stopwatch();
                    //stopWatch.Start();
                    var client = new HttpClient();
                    HttpResponseMessage response = await client.GetAsync(_address);
                    response.EnsureSuccessStatusCode();

                    var result = await response.Content.ReadAsStringAsync();
                    //if (totalErrors > 0)
                    //    Console.WriteLine(i + ": " + totalErrors);
                    //Console.WriteLine(i + result.Substring(0, 100));
                    keepTrying = false;
                    //stopWatch.Stop();
                    //TimeSpan ts = stopWatch.Elapsed;
                    //string elapsedTime = String.Format("{0:0}", ts.TotalMilliseconds);
                    //Console.WriteLine("Total: " + elapsedTime);

                    return result;
                }
                catch (HttpRequestException hre){
                    //Console.WriteLine(i + hre.Message);
                    totalErrors ++;
                    System.Threading.Thread.Sleep(500);
                }
                catch (Exception ex){
                    Console.WriteLine("here" + i + ex.Message);
                }
            }
            return null;
        }
    }
}
