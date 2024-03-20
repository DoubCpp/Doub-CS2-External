using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoubCheat;
using Swed64;

public class AntiFlash
{
    private Swed swed;
    private IntPtr client;
    private Renderer renderer;

    public AntiFlash(Swed swed, IntPtr client, Renderer renderer)
    {
        this.swed = swed;
        this.client = client;
        this.renderer = renderer;

    }

    public void RunAntiFlash()
    {

        while (true) 
        {
            if (renderer.antiFlash)
            {
                IntPtr localPlayerPawn = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn);

                float flashDuration = swed.ReadFloat(localPlayerPawn, Offsets.m_flFlashBangTime); 

                if (flashDuration > 0) 
                {
                    swed.WriteFloat(localPlayerPawn, Offsets.m_flFlashBangTime, 0); 
                }


            }
        }
    }
}
