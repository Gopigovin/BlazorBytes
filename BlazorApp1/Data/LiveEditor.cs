using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.JSInterop;
using System.ServiceProcess;

namespace BlazorApp1.Data
{
    public class LiveEditor
    {
        string guid = Guid.NewGuid().ToString().Substring(0, 5);
        IJSRuntime jsrun { get; set; }
        private bool isInstalledPackage { get; set; }
        private bool serviceIsRunning { get; set; }
        private string syncPackName { get; set; } = "Syncfusion.EJ2.Blazor";
        public string getPageText()
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pages", "pagecode.txt");
            string text = File.ReadAllText(file);
            return text;
        }
        public string getHostHtmlText()
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pages", "host.txt");
            string text = File.ReadAllText(file);
            return text;
        }
        public string getStartUpText()
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pages", "startup.txt");
            string text = File.ReadAllText(file);
            return text;
        }

        public string templateLocation = @"D:\playground\BlazorApp1\BlazorApp1\EJSBlazorEditor\EJSBlazorEditor";

        Process process = new Process();

        internal void BuildEngine1(DataValue data, IJSRuntime jsRuntime = null)
        {
            this.jsrun = jsRuntime;
            this.isAlreadyInstalled();
            var startUpCode = data.startup;
            var hostCode = data.host;
            var pageCode = data.pages;
            System.IO.File.WriteAllText(templateLocation + @"\Startup.cs", startUpCode);
            System.IO.File.WriteAllText(templateLocation + @"\Pages\_Host.cshtml", hostCode);
            System.IO.File.WriteAllText(templateLocation + @"\Pages\Index.razor", pageCode);
            this.jsrun.InvokeVoidAsync("projectCompilation");
        }
            internal void BuildEngine(DataValue data, IJSRuntime jsRuntime=null)
        {
            this.jsrun = jsRuntime;
            this.isAlreadyInstalled();
            var startUpCode = data.startup;
            var hostCode = data.host;
            var pageCode = data.pages;
            System.IO.File.WriteAllText(templateLocation + @"\Startup.cs", startUpCode);
            System.IO.File.WriteAllText(templateLocation + @"\Pages\_Host.cshtml", hostCode);
            System.IO.File.WriteAllText(templateLocation + @"\Pages\Index.razor", pageCode);
            
            List<string> cmds = new List<string>();

            Process process = new Process();
            string dllVersion = "17.4.0.39";
            string version = " -v " + dllVersion;
            string dotnetPack = "dotnet add package" + " " + this.syncPackName + version;
            string dotnetRestore = "dotnet restore";
            string dotnetBuild = "dotnet build";
            string dotnetRun = "dotnet run";
            string publishFile = Path.Combine(Directory.GetCurrentDirectory(), "Publish/" + "Result");
            string dotnetPublish = "dotnet publish -c Release -o " + publishFile;

            if (Directory.Exists(publishFile))
            {
                string pathFile = Path.Combine(Directory.GetCurrentDirectory(), "Publish" + "\\" + "Result");
                //List<string> stop = new List<string>() { "iisreset /stop" };
                //var psii = new ProcessStartInfo();
                //psii.FileName = @"C:\Windows\System32\cmd.exe";
                //psii.RedirectStandardInput = true;
                //psii.RedirectStandardOutput = true;
                //psii.RedirectStandardError = true;
                //psii.UseShellExecute = false;
                //psii.WorkingDirectory = @"C:\Windows\system32";
                //process.StartInfo = psii;
                //process.Start();
                //process.OutputDataReceived += (sender, e) =>
                //{
                //    //var path = Path.Combine(Directory.GetCurrentDirectory(), "Publish") + "\\" + "Result";
                //    //if (e.Data != null && e.Data.Trim() == "EJSBlazorEditor -> " + path + "\\")
                //    //{
                //    //    this.jsrun.InvokeVoidAsync("exampleJsFunctions.renderIframe", DotNetObjectReference.Create(this));
                //    //}
                //};
                //process.ErrorDataReceived += (sender, e) =>
                //{
                //    Console.WriteLine(e.Data);
                //};
                //process.BeginOutputReadLine();
                //process.BeginErrorReadLine();
                //using (StreamWriter sw = process.StandardInput)
                //{
                //    foreach (var cmd in stop)
                //    {
                //        sw.WriteLine(cmd);
                //    }
                //}
                //process.WaitForExit();

                //File.SetAttributes(pathFile, FileAttributes.Normal);
                ClearAttributes(pathFile);
                Directory.Delete(pathFile, true);
                publishFile = Path.Combine(Directory.GetCurrentDirectory(), "Publish/" + "Result");
                //proc1.UseShellExecute = true;
                //proc1.WorkingDirectory = @"C:\Windows\System32";
                //proc1.FileName = @"C:\Windows\System32\cmd.exe";
                //proc1.Arguments = "/c " + "iisreset /start";
                //proc1.WindowStyle = ProcessWindowStyle.Hidden;
                //Process.Start(proc1);
            }
            //if (this.isInstalledPackage)
            //{
            cmds.Add(dotnetPublish);
            //} else
            //{
            //    cmds.Add(dotnetPack);
            //    cmds.Add(dotnetRestore);
            //}
            if (this.serviceIsRunning)
            {
                this.serviceIsRunning = false;
                process.Kill();
            }
            var psi = new ProcessStartInfo();
            psi.FileName = @"C:\Windows\System32\cmd.exe";
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.WorkingDirectory = templateLocation;
            process.StartInfo = psi;
            process.Start();
            process.OutputDataReceived += (sender, e) =>
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "Publish") + "\\" + "Result";
                if (e.Data !=null && e.Data.Trim() == "EJSBlazorEditor -> "+ path + "\\")
                {
                    this.jsrun.InvokeVoidAsync("exampleJsFunctions.renderIframe", DotNetObjectReference.Create(this));
                }
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                Console.WriteLine(e.Data);
            };
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            using (StreamWriter sw = process.StandardInput)
            {
                foreach (var cmd in cmds)
                {
                    sw.WriteLine(cmd);
                }
            }
            process.WaitForExit();
        }

        public static void ClearAttributes(string currentDir)
        {
            if (Directory.Exists(currentDir))
            {
                File.SetAttributes(currentDir, FileAttributes.Normal);

                string[] subDirs = Directory.GetDirectories(currentDir);
                foreach (string dir in subDirs)
                {
                    ClearAttributes(dir);
                }

                string[] files = files = Directory.GetFiles(currentDir);
                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                }
            }
        }
        private void executeProject()
        {
            try
            {
                process.Close();
                List<string> cmds = new List<string>();
                string dotnetRun = "dotnet run";
                cmds.Add(dotnetRun);
                var psi = new ProcessStartInfo();
                psi.FileName = @"C:\Windows\System32\cmd.exe";
                psi.RedirectStandardInput = true;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;
                psi.UseShellExecute = false;
                psi.WorkingDirectory = templateLocation;
                process.StartInfo = psi;
                process.Start();
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null && e.Data.Contains("localhost:5001"))
                    {
                        this.serviceIsRunning = true;
                        process.CancelOutputRead();
                        process.CancelErrorRead();
                        process.WaitForExit(0);
                        string hostId = "https://localhost:5001";
                        this.jsrun.InvokeVoidAsync("exampleJsFunctions.renderIframe", DotNetObjectReference.Create(this), hostId);
                        Console.WriteLine(e.Data);
                    }
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    Console.WriteLine(e.Data);
                };
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                using (StreamWriter sw = process.StandardInput)
                {
                    foreach (var cmd in cmds)
                    {
                        sw.WriteLine(cmd);
                    }
                }
                process.WaitForExit();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async void isAlreadyInstalled()
        {
            try
            {
                process.Close();
                string packList = "dotnet list package";
                List<string> cmds = new List<string>();
                cmds.Add(packList);
                var psi = new ProcessStartInfo();
                psi.FileName = @"C:\Windows\System32\cmd.exe";
                psi.RedirectStandardInput = true;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;
                psi.UseShellExecute = false;
                psi.WorkingDirectory = templateLocation;
                process.StartInfo = psi;
                process.Start();
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null && e.Data.Contains(this.syncPackName))
                    {
                        this.isInstalledPackage = true;
                        process.CancelOutputRead();
                        process.CancelErrorRead();
                        process.WaitForExit(0);
                        process.Close();
                        Console.WriteLine(e.Data);
                    }
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    Console.WriteLine(e.Data);
                };
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                using (StreamWriter sw = process.StandardInput)
                {
                    foreach (var cmd in cmds)
                    {
                        sw.WriteLine(cmd);
                    }
                }
                process.WaitForExit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    

    public class DataValue
    {
        public string pages { get; set; }
        public string startup { get; set; }
        public string host { get; set; }
    }
}
