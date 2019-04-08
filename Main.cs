using System;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Wox.Plugin.mpv
{
    public class Main : IPlugin
    {
        Dictionary<string, string> mpvDic = new Dictionary<string, string>();

        public void Init(PluginInitContext context) => Populate();

        public List<Result> Query(Query query)
        {
            List<Result> results = new List<Result>();

            bool isRunning = false;
            foreach (Process proc in Process.GetProcesses())
                if (proc.ProcessName == "mpv" || proc.ProcessName == "mpvnet")
                    isRunning = true;
            if (!isRunning)
                return results;

            foreach (var pair in mpvDic)
            {
                if (pair.Key.ToLower().Contains(query.Search) ||
                    pair.Value.ToLower().Contains(query.Search))
                {
                    results.Add(new Result()
                    {
                        Title = pair.Value,
                        SubTitle = pair.Key,
                        IcoPath = "mpv.ico",
                        Action = e =>
                        {
                            using (var pipe = new NamedPipeClientStream(
                                ".", "mpvsocket", PipeDirection.Out,
                                PipeOptions.Asynchronous,
                                TokenImpersonationLevel.Anonymous))
                            {
                                pipe.Connect(2000);
                                StreamWriter pipeOut = new StreamWriter(pipe) { AutoFlush = true };
                                pipeOut.WriteAsync(pair.Key + "\n");
                            }
                            return true;
                        }
                    });
                }
            }
            return results;
        }

        void Populate()
        {
            string inputConfPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\mpv\\input.conf";
            if (!File.Exists(inputConfPath)) return;
            string content = File.ReadAllText(inputConfPath);
            List<string> lines = null;
            Dictionary<string, string> commandInputDic = new Dictionary<string, string>();

            if (content.Contains("#menu:"))
                lines = content.Split("\r\n".ToCharArray()).ToList();
            else
            {
                lines = Properties.Resources.input_conf.Split("\r\n".ToCharArray()).ToList();

                foreach (string i in content.Split("\r\n".ToCharArray()))
                {
                    string line = i.Trim();
                    if (line.StartsWith("#") || !line.Contains(" ")) continue;
                    string input = line.Substring(0, line.IndexOf(" ")).Trim();
                    string command = line.Substring(line.IndexOf(" ") + 1).Trim();
                    commandInputDic[command] = input;
                }
            }

            foreach (string i in lines)
            {
                if (!i.Contains("#menu:")) continue;
                string left = i.Substring(0, i.IndexOf("#menu:")).Trim();
                if (left.StartsWith("#")) continue;
                string command = left.Substring(left.IndexOf(" ") + 1).Trim();
                if (command == "ignore") continue;
                string menu = i.Substring(i.IndexOf("#menu:") + "#menu:".Length).Trim();
                string input = left.Substring(0, left.IndexOf(" "));
                if (input == "_") input = "";
                if (menu.Contains(";")) input = menu.Substring(0, menu.IndexOf(";")).Trim();
                string path = menu.Substring(menu.IndexOf(";") + 1).Trim().Replace("&", "&&").Replace("...", "");
                if (path == "" || command == "") continue;

                if (commandInputDic.Count > 0)
                    if (commandInputDic.ContainsKey(command))
                        input = commandInputDic[command];
                    else
                        input = "";

                if (!string.IsNullOrEmpty(input))
                    path += $" ({input})";

                mpvDic[command] = path;
            }
        }
    }
}