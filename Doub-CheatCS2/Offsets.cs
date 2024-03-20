using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoubCheat
{
    public static class Offsets
    {
        // offsets

        // offsets.cs
        public static int dwEntityList = 0x18C2D58;
        public static int dwViewMatrix = 0x19241A0;
        public static int dwLocalPlayerPawn = 0x17371A8;
        public static int dwViewAngles = 0x19309B0;
        public static int dwForceAttack = 0x1730020;
        

        //client.dll.cs
        public static int m_vOldOrigin = 0x127C;
        public static int m_iTeamNum = 0x3CB;
        public static int m_lifeState = 0x338;
        public static int m_hPlayerPawn = 0x7E4;
        public static int m_vecViewOffset = 0xC58;
        public static int m_iHealth = 0x334;
        public static int m_iszPlayerName = 0x638;
        public static int m_entitySpottedState = 0x1698;
        public static int m_bSpotted = 0x8;
        public static int m_pCameraServices = 0x1138;
        public static int m_iFOV = 0x210;
        public static int m_flFlashBangTime = 0x14B8;
        public static int m_bIsScoped = 0x1400;
        public static int m_iIDEntIndex = 0x15A4;
    }
}

