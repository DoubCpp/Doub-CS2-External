using DoubCheat;
using Swed64;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        Swed swed = new Swed("cs2");

        IntPtr client = swed.GetModuleBase("client.dll");

        Renderer renderer = new Renderer();
        Thread renderThread = new Thread(new ThreadStart(renderer.Start().Wait));
        renderThread.Start();

        Vector2 screenSize = renderer.screenSize;

        Aimbot aimbot = new Aimbot(swed, client, renderer, screenSize);
        Thread aimbotThread = new Thread(new ThreadStart(aimbot.RunAimbot));
        aimbotThread.Start();

        EspBox espBox = new EspBox(swed, client, renderer, screenSize);
        Thread espThread = new Thread(new ThreadStart(espBox.RunESP));
        espThread.Start();

        RadarHack radarHack = new RadarHack(swed, client, renderer);
        Thread radarThread = new Thread(new ThreadStart(radarHack.RunRadar));
        radarThread.Start();

        Fov fov = new Fov(swed, client, renderer, screenSize);
        Thread fovThread = new Thread(new ThreadStart(fov.RunFov));
        fovThread.Start();

        TriggerBot triggerBot = new TriggerBot(swed, client, renderer);
        Thread triggerThread = new Thread(new ThreadStart(triggerBot.RunTrigger));
        triggerThread.Start();

        AntiFlash antiFlash = new AntiFlash(swed, client, renderer);
        Thread antiFlashThread = new Thread(new ThreadStart(antiFlash.RunAntiFlash));
        antiFlashThread.Start();

        while (true)
        {
            //Thread.Sleep(1000);
        }
    }
}