using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using DoubCheat;
using Swed64;

public class EspBox
{
    private Swed swed;
    private IntPtr client;
    private Renderer renderer;
    private Vector2 screenSize;

    public EspBox(Swed swed, IntPtr client, Renderer renderer, Vector2 screenSize)
    {
        this.swed = swed;
        this.client = client;
        this.renderer = renderer;
        this.screenSize = screenSize;
    }

    public void RunESP()
    {
        while (true)
        {
            var entities = new List<Entity>();  

            
            IntPtr entityList = swed.ReadPointer(client, Offsets.dwEntityList);
            IntPtr listEntry = swed.ReadPointer(entityList, 0x10);
            IntPtr localPlayerPawn = swed.ReadPointer(client, Offsets.dwLocalPlayerPawn);

            Entity localPlayer = new Entity();
            localPlayer.team = swed.ReadInt(localPlayerPawn, Offsets.m_iTeamNum);

            for (int i = 0; i < 64; i++)
            {
                IntPtr currentController = swed.ReadPointer(listEntry, i * 0x78);
                if (currentController == IntPtr.Zero) continue; 

                int pawnHandle = swed.ReadInt(currentController, Offsets.m_hPlayerPawn);
                if (pawnHandle == 0) continue;

                IntPtr listEntry2 = swed.ReadPointer(entityList, 0x8 * ((pawnHandle & 0x7FFF) >> 9) + 0x10);
                if (listEntry2 == IntPtr.Zero) continue;

                IntPtr currentPawn = swed.ReadPointer(listEntry2, 0x78 * (pawnHandle & 0x1FF));
                if (currentPawn == IntPtr.Zero) continue;

                int lifeState = swed.ReadInt(currentPawn, Offsets.m_lifeState);
                if (lifeState != 256) continue;

                float[] viewMatrix = swed.ReadMatrix(client + Offsets.dwViewMatrix);

                Entity entity = new Entity();

                entity.name = swed.ReadString(currentController, Offsets.m_iszPlayerName, 16).Split("\0")[0];
                entity.team = swed.ReadInt(currentPawn, Offsets.m_iTeamNum);
                entity.spotted = swed.ReadBool(currentPawn, Offsets.m_entitySpottedState + Offsets.m_bSpotted);
                entity.health = swed.ReadInt(currentPawn, Offsets.m_iHealth);
                entity.position = swed.ReadVec(currentPawn, Offsets.m_vOldOrigin);
                entity.viewOffset = swed.ReadVec(currentPawn, Offsets.m_vecViewOffset);
                entity.position2D = Calculate.WorldToScreen(viewMatrix, entity.position, screenSize);
                entity.viewPosition2D = Calculate.WorldToScreen(viewMatrix, Vector3.Add(entity.position, entity.viewOffset), screenSize);

                entities.Add(entity);
            }
            renderer.UpdateLocalPlayer(localPlayer);
            renderer.UpdateEntities(entities);

            
        }
    }
}
