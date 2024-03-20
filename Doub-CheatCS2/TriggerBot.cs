using System.Runtime.InteropServices;
using DoubCheat;
using Swed64;

public class TriggerBot
{
    private Swed swed;
    private IntPtr client;
    private Renderer renderer;

    public TriggerBot(Swed swed, IntPtr client, Renderer renderer)
    {
        this.swed = swed;
        this.client = client;
        this.renderer = renderer;

    }

    public void RunTrigger()
    {
        int HOTKEY = 0x06; 

        while (true)
        {
            
            IntPtr entityList = swed.ReadPointer(client, Offsets.dwEntityList);
            IntPtr localPlayerPawn = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn);

            int team = swed.ReadInt(localPlayerPawn, Offsets.m_iTeamNum);
            int entIndex = swed.ReadInt(localPlayerPawn, Offsets.m_iIDEntIndex);

            if (entIndex != -1)
            {  
                IntPtr listEntry = swed.ReadPointer(entityList, 0x8 * ((entIndex & 0x7FFF) >> 9) + 0x10);
                IntPtr currentPawn = swed.ReadPointer(listEntry, 0x78 * (entIndex & 0x1FF));

                int entityTeam = swed.ReadInt(currentPawn, Offsets.m_iTeamNum);

                if (team != entityTeam) 
                {  
                    if (GetAsyncKeyState(HOTKEY) < 0 && renderer.trigerBot)
                    {
                        swed.WriteInt(client, Offsets.dwForceAttack, 65537); 
                        //Thread.Sleep(100); 
                        swed.WriteInt(client, Offsets.dwForceAttack, 256); 
                        //Thread.Sleep(10); 
                    }
                }
            }
        }
    }
    [DllImport("user32.dll")]
    static extern short GetAsyncKeyState(int vKey);
}
