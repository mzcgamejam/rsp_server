using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aws.GameLift.Server;
using BattleServer.GameLift;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using BattleServer.Config;

namespace BattleServer
{
    class Program
    {
        static AutoResetEvent terminatingEvent = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            Console.CancelKeyPress += OnConsoleKeyPress;

            Config.ConfigManager.SetConfiguration();

            //https://docs.supersocket.net/v1-6/en-US/SuperSocket-Basic-Configuration
            //https://www.slideshare.net/jacking/kgc-2016-super-socket

            var result = Server.BattleServer.Instance.Setup(new ServerConfig
            {
                Ip = "0.0.0.0",
                Port = int.Parse(args[0]),
                Mode = SocketMode.Tcp,
                MaxConnectionNumber = 3000,
                ReceiveBufferSize = 4096,
                MaxRequestLength = 4096,
                SendBufferSize = 4096,
                SendingQueueSize = 512,
                ClearIdleSession = true,
                ClearIdleSessionInterval = 120,
                IdleSessionTimeOut = 600,
                SyncSend = false,
                Name = "Battle Server"
            });

            if (false == result)
            {
                Console.WriteLine("Battle Server Setup Fail...");
            }

            Console.WriteLine("Battle Server Start...");
            Server.BattleServer.Instance.Start();
            
            var gamelift = new GameLiftNetwork();
            gamelift.Init(int.Parse(args[0]));
            terminatingEvent.WaitOne();
            gamelift.Ending();
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            terminatingEvent.Set();
        }

        static void OnConsoleKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (e.SpecialKey == ConsoleSpecialKey.ControlC)
            {
                e.Cancel = true;
                terminatingEvent.Set();

                var gamelift = new GameLiftNetwork();
                gamelift.Ending();
            }
        }
    }
}
