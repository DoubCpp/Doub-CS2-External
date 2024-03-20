using System.Collections.Concurrent;
using System.Numerics;
using ClickableTransparentOverlay;
using ImGuiNET;

namespace DoubCheat
{
    public class Renderer : Overlay
    {
        public Vector2 screenSize = new Vector2(1920, 1080); 

        private ConcurrentQueue<Entity> entities = new ConcurrentQueue<Entity>();
        private Entity localPlayer = new Entity();
        private readonly object entityLock = new object();

        private bool enableESP = false;
        public bool enableName = false;
        public bool enableVisibilityCheck = false;
        public bool aimbot = false;
        public bool trigerBot = false;
        public bool aimOnTeam = false;
        public bool radar = false;
        public bool antiFlash = false;
        public int smooth = 0;
        public int fov = 90;
        private Vector4 enemyColor = new Vector4(1, 0, 0, 1); // default red
        private Vector4 teamColor = new Vector4(0, 1, 0, 1); // default green
        private Vector4 hiddenColor = new Vector4(0, 0, 0, 1); // default black
        private Vector4 nameColor = new Vector4(1, 1, 1, 1); // default white

        ImDrawListPtr drawList;

        private bool menuVisible = false;

        protected override void Render()
        {
            if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(ImGuiKey.Insert)))
                menuVisible = !menuVisible;

            if (menuVisible)
            {
                ImGui.SetNextWindowSize(new Vector2(300, 200), ImGuiCond.FirstUseEver);
                ImGui.SetNextWindowPos(new Vector2(10, 10), ImGuiCond.FirstUseEver);
                ImGui.Begin("Doub Menu", ref menuVisible);

                if (ImGui.BeginTabBar("Tabs"))
                {
                    if (ImGui.BeginTabItem("View"))
                    {
                        ImGui.Text("Esp");
                        ImGui.Separator();
                        ImGui.Checkbox("Esp Box", ref enableESP);
                        ImGui.Checkbox("Esp Visibility Check", ref enableVisibilityCheck);
                        ImGui.Checkbox("Name", ref enableName);
                        ImGui.Checkbox("Radar Hack(Ca bug active pas en public)", ref radar);
                        ImGui.Checkbox("Anti Flash", ref antiFlash);
                        ImGui.Separator();
                        ImGui.Text("FOV Changer");
                        ImGui.Separator();
                        ImGui.SliderInt("FOV", ref fov, 58, 140);
                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Couleur"))
                    {
                        ImGui.Text("Couleur de l'ennemi");
                        ImGui.Separator();
                        ImGui.ColorEdit4("##enemycolor", ref enemyColor, ImGuiColorEditFlags.NoInputs);
                        ImGui.Separator();
                        ImGui.Text("Couleur de l'equipe");
                        ImGui.Separator();
                        ImGui.ColorEdit4("##teamcolor", ref teamColor, ImGuiColorEditFlags.NoInputs);
                        ImGui.Separator();
                        ImGui.Text("Couleur de Visibility Check");
                        ImGui.Separator();
                        ImGui.ColorEdit4("##visibilitycolor", ref hiddenColor, ImGuiColorEditFlags.NoInputs);
                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Aim"))
                    {
                        ImGui.Text("Aim");
                        ImGui.Separator();
                        ImGui.Checkbox("Aimbot", ref aimbot);
                        ImGui.Checkbox("Aimbot On Team", ref aimOnTeam);
                        ImGui.Checkbox("Triger Bot", ref trigerBot);
                    }

                    

                    ImGui.EndTabBar();
                }

                ImGui.End();
            }

            DrawOverlay(screenSize);
            drawList = ImGui.GetWindowDrawList();

            if (enableESP)
            {
                foreach (var entity in entities)
                {
                    if (EntityOnScreen(entity))
                    {
                        DrawBox(entity);
                        DrawHealthBar(entity);
                        DrawLine(entity);
                        DrawName(entity, 20);
                    }
                }
            }


        }

        bool EntityOnScreen(Entity entity)
        {
            if (entity.position2D.X > 0 && entity.position2D.X < screenSize.X && entity.position2D.Y > 0 && entity.position2D.Y < screenSize.Y)
            {
                return true;
            }
            return false;
        }
        private void DrawHealthBar(Entity entity)
        {
            float entityHeight = entity.position2D.Y - entity.viewPosition2D.Y;
            float boxLeft = entity.viewPosition2D.X - entityHeight / 3;
            float boxRight = entity.position2D.X + entityHeight / 3;
            float barPercentWidth = 0.05f; 
            float barPixelWidth = barPercentWidth * (boxRight - boxLeft);
            float barHeight = entityHeight * (entity.health / 100f);
            Vector2 barTop = new Vector2(boxLeft - barPixelWidth, entity.position2D.Y - barHeight);
            Vector2 barBottom = new Vector2(boxLeft, entity.position2D.Y);
            Vector4 barColor = new Vector4(0, 1, 0, 1);

            drawList.AddRectFilled(barTop, barBottom, ImGui.ColorConvertFloat4ToU32(barColor));
        }

        private void DrawName(Entity entity, int yOffset)
        {
            if (enableName)
            {
                Vector2 textLocation = new Vector2(entity.viewPosition2D.X, entity.viewPosition2D.Y - yOffset); 
                drawList.AddText(textLocation, ImGui.ColorConvertFloat4ToU32(nameColor), $"{entity.name}"); 
            }
        }

        private void DrawBox(Entity entity)
        {
            float entityHeight = entity.position2D.Y - entity.viewPosition2D.Y;
            Vector2 rectTop = new Vector2(entity.viewPosition2D.X - entityHeight / 3, entity.viewPosition2D.Y);
            Vector2 rectBottom = new Vector2(entity.position2D.X + entityHeight / 3, entity.position2D.Y);
            Vector4 boxColor = localPlayer.team == entity.team ? teamColor : enemyColor;

            if (enableVisibilityCheck)
                boxColor = entity.spotted == true ? boxColor : hiddenColor; 

            drawList.AddRect(rectTop, rectBottom, ImGui.ColorConvertFloat4ToU32(boxColor));
        }
        private void DrawLine(Entity entity)
        {
            Vector4 lineColor = localPlayer.team == entity.team ? teamColor : enemyColor;
            drawList.AddLine(new Vector2(screenSize.X / 2, screenSize.Y), entity.position2D, ImGui.ColorConvertFloat4ToU32(lineColor));
        }

        public void UpdateEntities(IEnumerable<Entity> newEntities)
        {
            entities = new ConcurrentQueue<Entity>(newEntities);
        }
        public void UpdateLocalPlayer(Entity newEntity)
        {
            lock (entityLock)
            {
                localPlayer = newEntity;
            }
        }

        void DrawOverlay(Vector2 screenSize) 
        {
            ImGui.SetNextWindowSize(screenSize);
            ImGui.SetNextWindowPos(new Vector2(0, 0)); 
            ImGui.Begin("overlay", ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoBackground
                | ImGuiWindowFlags.NoBringToFrontOnFocus
                | ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoInputs
                | ImGuiWindowFlags.NoCollapse
                | ImGuiWindowFlags.NoScrollbar
                | ImGuiWindowFlags.NoScrollWithMouse
                );
        }


    }
}
