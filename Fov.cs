using System.Numerics;
using DoubCheat;
using Swed64;

class Fov
{
    private Swed swed;
    private IntPtr client;
    private Renderer renderer;

    public Fov(Swed swed, IntPtr client, Renderer renderer, Vector2 screenSize)
    {
        this.swed = swed;
        this.client = client;
        this.renderer = renderer;
    }

    public void RunFov()
    {
        while (true)
        {
            uint desiredFov = (uint)renderer.fov;
            IntPtr localPlayerPawn = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn);
            IntPtr cameraServices = swed.ReadPointer(localPlayerPawn, Offsets.m_pCameraServices);
            uint currentFov = swed.ReadUInt(cameraServices + Offsets.m_iFOV);
            bool isScoped = swed.ReadBool(localPlayerPawn, Offsets.m_bIsScoped);

            if (!isScoped && currentFov != desiredFov)
            {
                swed.WriteUInt(cameraServices + Offsets.m_iFOV, desiredFov); 
            }
        }
    }
}
