using DoubCheat;
using Swed64;

public class RadarHack
{
    private Swed swed;
    private IntPtr client;
    private Renderer renderer;

    public RadarHack(Swed swed, IntPtr client, Renderer renderer)
    {
        this.swed = swed;
        this.client = client;
        this.renderer = renderer;

    }

    public void RunRadar()
    {
        while (true)
        {
            if (renderer.radar) 
            {
                IntPtr entityList = swed.ReadPointer(client, Offsets.dwEntityList);
                IntPtr listEntry = swed.ReadPointer(entityList, 0x10);

                for (int i = 0; i < 64; i++)
                {
                    if (listEntry == IntPtr.Zero) 
                        continue;

                    IntPtr currentController = swed.ReadPointer(listEntry, i * 0x78);

                    if (currentController == IntPtr.Zero)
                        continue;

                    int pawnHandle = swed.ReadInt(currentController, Offsets.m_hPlayerPawn);
                    if (pawnHandle == 0)
                        continue;
                    IntPtr listEntry2 = swed.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);
                    IntPtr currentPawn = swed.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));

                    string name = swed.ReadString(currentController, Offsets.m_iszPlayerName, 16);
                    bool spotted = swed.ReadBool(currentPawn, Offsets.m_entitySpottedState + Offsets.m_bSpotted);

                    swed.WriteBool(currentPawn, Offsets.m_entitySpottedState + Offsets.m_bSpotted, true); 

                    string spottedStatus = spotted ? "spotted" : ""; 
                }
            }
        }
    }
}
            
        