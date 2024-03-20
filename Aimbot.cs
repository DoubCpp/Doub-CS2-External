using System.Numerics;
using System.Runtime.InteropServices;
using DoubCheat;
using Swed64;

class Aimbot
{
    static List<Entity> entities = new List<Entity>(); 
    static Entity localPlayer = new Entity(); 
    private Swed swed;
    private IntPtr client;
    private Renderer renderer;

    public Aimbot(Swed swed, IntPtr client, Renderer renderer, Vector2 screenSize)
    {
        this.swed = swed;
        this.client = client;
        this.renderer = renderer;
    }

    public void RunAimbot()
    {

        const int HOTKEY = 0x06; 

        while (true)
        {
            entities.Clear();

            IntPtr entityList = swed.ReadPointer(client, Offsets.dwEntityList);

            IntPtr listEntry = swed.ReadPointer(entityList, 0x10);

            localPlayer.pawnAddress = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn);
            localPlayer.team = swed.ReadInt(localPlayer.pawnAddress, Offsets.m_iTeamNum);
            localPlayer.origin = swed.ReadVec(localPlayer.pawnAddress, Offsets.m_vOldOrigin);
            localPlayer.view = swed.ReadVec(localPlayer.pawnAddress, Offsets.m_vecViewOffset);


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

                if (currentPawn == localPlayer.pawnAddress) 
                    continue;

                int health = swed.ReadInt(currentPawn, Offsets.m_iHealth);
                int team = swed.ReadInt(currentPawn, Offsets.m_iTeamNum);
                uint lifeState = swed.ReadUInt(currentPawn, Offsets.m_lifeState);

                if (lifeState != 256)
                    continue;
                if (team == localPlayer.team && !renderer.aimOnTeam)
                    continue;

                Entity entity = new Entity();

                entity.pawnAddress = currentPawn;
                entity.controllerAddress = currentController;
                entity.health = health;
                entity.lifeState = lifeState;
                entity.origin = swed.ReadVec(currentPawn, Offsets.m_vOldOrigin);
                entity.view = swed.ReadVec(currentPawn, Offsets.m_vecViewOffset);
                entity.distance = Vector3.Distance(entity.origin, localPlayer.origin);

                entities.Add(entity);

            }


            // sort entities and aim

            entities = entities.OrderBy(o => o.distance).ToList(); // closest

            if (entities.Count > 0 && GetAsyncKeyState(HOTKEY) < 0 && renderer.aimbot) // count, hotkey and checkbox
            {
                // get view pos
                Vector3 playerView = Vector3.Add(localPlayer.origin, localPlayer.view);
                Vector3 entityView = Vector3.Add(entities[0].origin, entities[0].view);

                // get angles 
                Vector2 newAngles = Calculate.CalculateAngles(playerView, entityView);
                Vector3 newAnglesVec3 = new Vector3(newAngles.Y, newAngles.X, 0.0f); // set y before x. 

                // force new angles 
                swed.WriteVec(client, Offsets.dwViewAngles, newAnglesVec3);
            }

        }
    }

    [DllImport("user32.dll")]
    static extern short GetAsyncKeyState(int vKey);
}