using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KSP.UI.Screens;

namespace KSPMouseInterface
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class KSPMouseInterface : MonoBehaviour
    {
        #region Member

        float maxAnalogValueLength = 1.5f;

        // paths
        static string rootPath = KSPUtil.ApplicationRootPath.Replace("\\", "/");
        static string pluginFolderPath = rootPath + "GameData/KSPMouseInterface";
        static string settingsPath = pluginFolderPath + "/Data/KSPMouseInterface_Settings.xml";

        // last mouse position
        Vector3 currentMousePosition = new Vector3();

        GuiPanel mainPanel = null;
        ColorAction activeColorAction = null;

        // last window positions
        Rect debugWindowPosition = new Rect(50, 0, 350, 500);

        Dictionary<string, GuiPanel> guiPanels = new Dictionary<string, GuiPanel>();
        Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        #endregion

        #region public

        // called on scene start
        public void Start()
        {
            LoadSettings(settingsPath.Replace("/", "\\"));

            if (mainPanel != null)
            {
                textures.Clear();

                // Load textures 
                mainPanel.Texture = LoadTexture(pluginFolderPath + "/" + mainPanel.TexturePath);
                mainPanel.TextureHotSpots = LoadTexture(pluginFolderPath + "/" + mainPanel.HotSpotTexturePath);

                LoadButtonTextures(mainPanel);

                foreach (var subpanel in mainPanel.Subpanels.Values)
                {
                    subpanel.Texture = LoadTexture(pluginFolderPath + "/" + subpanel.TexturePath);
                    subpanel.TextureHotSpots = LoadTexture(pluginFolderPath + "/" + subpanel.HotSpotTexturePath);

                    LoadButtonTextures(subpanel);
                }

                // Calculate interface positions
                mainPanel.CalculateMainPanelPos();
                foreach (var inter in mainPanel.Subpanels)
                    inter.Value.CalculateChildPanelPos();

                // hook to vessel control
                FlightGlobals.ActiveVessel.OnFlyByWire += UpdateControl;
            }
        }

        // called every frame
        public void OnGUI()
        {
            // get/adjust mouse position (0, 0 = bottom left corner)
            currentMousePosition = Input.mousePosition;
            currentMousePosition.y = Screen.height - Input.mousePosition.y;

            if (mainPanel != null)
            {
                // draw subinterfaces
                foreach (var entry in mainPanel.Subpanels)
                    if (entry.Value.Texture != null && !mainPanel.Collapsed)
                    {
                        GUI.DrawTexture(entry.Value.PanelRect, entry.Value.Texture);
                        DrawButtons(entry.Value.ButtonsToDraw);
                    }

                // draw main interfaces
                if (mainPanel.Texture != null)
                {
                    GUI.DrawTexture(mainPanel.PanelRect, mainPanel.Texture);
                    DrawButtons(mainPanel.ButtonsToDraw);
                }

                // draw settings/debug window
                if (mainPanel.ShowSettingsWindow)
                {
                    GUI.skin = HighLogic.Skin;
                    debugWindowPosition = GUILayout.Window(44835, debugWindowPosition, DrawWindow, "KSPMouseInterface - Debug Window", GUILayout.MinWidth(200));
                }
            }
        }

        #endregion

        #region private

        private void LoadSettings(string path)
        {
            try
            {
                guiPanels = GuiPanel.CreateFromXml(path);

                if (guiPanels.ContainsKey("Default"))
                    mainPanel = guiPanels["Default"];

                else if (guiPanels.Count > 0)
                    mainPanel = guiPanels.Values.First();

                else
                    print("Error (KSPMouseInterface): No Interface found!");
            }
            catch (Exception ex)
            {
                print("ErrorMessage: " + ex.Message);
            }
        }
        
        private Texture2D LoadTexture(string path)
        {
            if (textures.ContainsKey(path))
                return textures[path];
            
            WWW www = new WWW("file://" + path);
            textures.Add(path, www.texture);

            return www.texture;
        }

        private void LoadButtonTextures(GuiPanel panel)
        {
            foreach (var entry in panel.Color2ColorActionMapping.Values)
            {
                foreach (var button in entry.ButtonGraphics.Values)
                    button.Texture = LoadTexture(pluginFolderPath + "/" + button.TexturePath);
            }
        }

        private void DrawWindow(int winID)
        {
            switch (winID)
            {
                case 44835: // Debug Window
                    GUILayout.BeginVertical();
                    //GUILayout.Label("PluginFolder = " + pluginFolderPath);
                    GUILayout.Label("MousePosition = (" + currentMousePosition.x + ", " + currentMousePosition.y + ")");
                    GUILayout.Label("Texture Pos = " + mainPanel.States.TexCoords.ToString());
                    GUILayout.Label("Color = " + mainPanel.States.Color.ToString());
                    GUILayout.Label("InterfaceStates:");
                    GUILayout.Label("Position = " + mainPanel.PanelRect.position.ToString());
                    GUILayout.Label("HasInput = " + mainPanel.States.HasInput.ToString());
                    GUILayout.Label("HasAnalogInput = " + mainPanel.States.AnalogInput.ToString());
                    GUILayout.Label("AnalogInputValue = " + mainPanel.States.AnalogInputValue.ToString()); 
                    GUILayout.Label("ActiveColorAction = " + (activeColorAction != null ? activeColorAction.Action.ToString() : "")); 
                    GUILayout.Label("HoverColorAction = " + (mainPanel.CurrentColorAction != null ? mainPanel.CurrentColorAction.Action.ToString() : "")); 
                    GUILayout.Label("OpenClose = " + mainPanel.States.OpenCloseDown.ToString());
                    GUILayout.Label("Yaw = " + mainPanel.States.Yaw.ToString());
                    GUILayout.Label("Pitch = " + mainPanel.States.Pitch.ToString());
                    GUILayout.Label("Roll = " + mainPanel. States.Roll.ToString());
                    GUILayout.Label("RCS = " + mainPanel.States.RCSDown.ToString());
                    GUILayout.Label("SAS = " + mainPanel.States.SASDown.ToString());
                    GUILayout.Label("NextStage = " + mainPanel.States.NextStageDown.ToString());
                    GUILayout.Label("ThrottleFast = " + mainPanel.States.ThrottleMax.ToString());
                    GUILayout.Label("Throttle = " + mainPanel.States.Throttle.ToString());
                    GUILayout.Label("ThrottleOff = " + mainPanel.States.ThrottleOff.ToString());
                    foreach (var entry in mainPanel.Subpanels)
                    {
                        GUILayout.Label("Sub InterfaceStates (" + entry.Value.Name + "):");
                        GUILayout.Label("Position = " + entry.Value.PanelRect.position.ToString());
                        GUILayout.Label("Texture Pos = " + entry.Value.States.TexCoords.ToString());
                        GUILayout.Label("Color = " + entry.Value.States.Color.ToString());
                        GUILayout.Label("HasInput = " + entry.Value.States.HasInput.ToString());
                    }

                    //GUILayout.Label("Func0 = " + mMCInterface.CurrentInterfaceStates.Func0Down.ToString());
                    //GUILayout.Label("Func1 = " + mMCInterface.CurrentInterfaceStates.Func1Down.ToString());
                    //GUILayout.Label("Func2 = " + mMCInterface.CurrentInterfaceStates.Func2Down.ToString());
                    //GUILayout.Label("Func3 = " + mMCInterface.CurrentInterfaceStates.Func3Down.ToString());

                    //GUILayout.Label("Actiongroup0 = " + mMCInterface.CurrentInterfaceStates.ActionGroup0Down.ToString());
                    //GUILayout.Label("Actiongroup1 = " + mMCInterface.CurrentInterfaceStates.ActionGroup1Down.ToString());
                    //GUILayout.Label("Actiongroup2 = " + mMCInterface.CurrentInterfaceStates.ActionGroup2Down.ToString());
                    //GUILayout.Label("Actiongroup3 = " + mMCInterface.CurrentInterfaceStates.ActionGroup3Down.ToString());
                    //GUILayout.Label("Actiongroup4 = " + mMCInterface.CurrentInterfaceStates.ActionGroup4Down.ToString());
                    //GUILayout.Label("Actiongroup5 = " + mMCInterface.CurrentInterfaceStates.ActionGroup5Down.ToString());
                    //GUILayout.Label("Actiongroup6 = " + mMCInterface.CurrentInterfaceStates.ActionGroup6Down.ToString());
                    //GUILayout.Label("Actiongroup7 = " + mMCInterface.CurrentInterfaceStates.ActionGroup7Down.ToString());
                    //GUILayout.Label("Actiongroup8 = " + mMCInterface.CurrentInterfaceStates.ActionGroup8Down.ToString());
                    //GUILayout.Label("Actiongroup9 = " + mMCInterface.CurrentInterfaceStates.ActionGroup9Down.ToString());
                    GUILayout.EndVertical();

                    GUI.DragWindow();

                    break;
            }
        }

        private void DrawButtons(List<ButtonGraphic> buttons)
        {
            foreach (ButtonGraphic button in buttons)
            {
                Rect pos = button.Position;
                pos.x += button.ColorAction.Interface.PanelRect.x;
                pos.y += button.ColorAction.Interface.PanelRect.y;

                if (button.Texture == null)
                {
                    //print("ColorAction:" + button.ColorAction.Action.ToString() + "; ButtonAction: " + button.Action.ToString() + "; Pos: " + pos.ToString() + " Texture: NULL");
                    return;
                }
                else if (button.HasTextureCoords)
                {
                    //print("ColorAction:" + button.ColorAction.Action.ToString() + "; ButtonAction: " + button.Action.ToString() + "; Pos: " + pos.ToString() + "; TexturePos: " + button.TexturePosition.ToString());
                    GUI.DrawTextureWithTexCoords(pos, button.Texture, button.TexturePosition);
                }
                else
                {
                    //print("ColorAction:" + button.ColorAction.Action.ToString() + "; ButtonAction: " + button.Action.ToString() + "; Pos: " + pos.ToString());
                    GUI.DrawTexture(pos, button.Texture);
                }
            }
        }

        // called every frame
        private void UpdateControl(FlightCtrlState flightCtrlState)
        {
            if (mainPanel != null)
            {
                UpdatePanelState();

                // expand/collapse interface
                mainPanel.Collapsed = !mainPanel.States.OpenCloseToggled;

                foreach (var subpanel in mainPanel.Subpanels.Values)
                    subpanel.Collapsed = !subpanel.States.OpenCloseToggled;

                // next stage
                if (mainPanel.States.NextStageToggled && StageManager.StageCount > 0)
                    StageManager.ActivateNextStage();

                // show/hide debug window.
                mainPanel.ShowSettingsWindow = mainPanel.States.SettingsToggled;

                var actionGroups = FlightGlobals.ActiveVessel.ActionGroups;

                // de/activate RCS
                actionGroups.SetGroup(KSPActionGroup.RCS, mainPanel.States.RCSToggled);

                // de/activate SAS
                actionGroups.SetGroup(KSPActionGroup.SAS, mainPanel.States.SASToggled);

                // set action group states.
                actionGroups.SetGroup(KSPActionGroup.Custom01, mainPanel.States.ActionGroup0Toggled);
                actionGroups.SetGroup(KSPActionGroup.Custom02, mainPanel.States.ActionGroup1Toggled);
                actionGroups.SetGroup(KSPActionGroup.Custom03, mainPanel.States.ActionGroup2Toggled);
                actionGroups.SetGroup(KSPActionGroup.Custom04, mainPanel.States.ActionGroup3Toggled);
                actionGroups.SetGroup(KSPActionGroup.Custom05, mainPanel.States.ActionGroup4Toggled);
                actionGroups.SetGroup(KSPActionGroup.Custom06, mainPanel.States.ActionGroup5Toggled);
                actionGroups.SetGroup(KSPActionGroup.Custom07, mainPanel.States.ActionGroup6Toggled);
                actionGroups.SetGroup(KSPActionGroup.Custom08, mainPanel.States.ActionGroup7Toggled);
                actionGroups.SetGroup(KSPActionGroup.Custom09, mainPanel.States.ActionGroup8Toggled);
                actionGroups.SetGroup(KSPActionGroup.Custom10, mainPanel.States.ActionGroup9Toggled);

                UpdateThrottle();

                UpdateYawPitchRoll(flightCtrlState);
                UpdateRCS(flightCtrlState);
            }
        }

        string debug1 = "";
        string debug2 = "";
        string debug3 = "";
        string debug4 = "";
        string debug5 = "";
        private void UpdatePanelState()
        {
            bool lButton = Input.GetMouseButton(0);

            // get mouse position on hot spot texture.
            if (!lButton)
            {
                mainPanel.States.AnalogInput = false;
                mainPanel.States.AnalogInputValue = Vector2.one;
                mainPanel.States.RCSAnalogInput = false;
                mainPanel.States.ThrottleMax = Directions.None;
                mainPanel.States.Throttle = Directions.None;
                mainPanel.States.ThrottleOff = false;
                mainPanel.States.Yaw = Directions.None;
                mainPanel.States.Pitch = Directions.None;
                mainPanel.States.Roll = Directions.None;
                mainPanel.States.RCSYaw = Directions.None;
                mainPanel.States.RCSPitch = Directions.None;
                mainPanel.States.RCSRoll = Directions.None;
            }

            mainPanel.ButtonsToDraw.Clear();
            mainPanel.UpdateTexCoords(currentMousePosition);

            if (lButton && activeColorAction == null)
                activeColorAction = mainPanel.CurrentColorAction;

            if (activeColorAction != null)
            {
                switch (activeColorAction.Action)
                {
                    #region Settings

                    case Actions.Settings:
                        mainPanel.States.SettingsDown = lButton;
                        break;

                    #endregion

                    #region RCS

                    case Actions.RCS:
                        mainPanel.States.RCSDown = lButton;
                        break;

                    #endregion

                    #region SAS

                    case Actions.SAS:
                        mainPanel.States.SASDown = lButton;
                        break;

                    #endregion

                    #region Next Stage

                    case Actions.NextStage:
                        mainPanel.States.NextStageDown = lButton;
                        break;

                    #endregion

                    #region Vessel change

                    case Actions.NextVessel:
                        break;

                    case Actions.PrevVessel:
                        break;

                    #endregion

                    #region Open/Close

                    case Actions.OpenClose:
                        if (activeColorAction.Name == mainPanel.Name)
                            mainPanel.States.OpenCloseDown = lButton;

                        else
                        {
                            foreach (GuiPanel subInterface in mainPanel.Subpanels.Values)
                            {
                                if (activeColorAction.Name == subInterface.Name)
                                    subInterface.States.OpenCloseDown = lButton;
                            }
                        }
                        break;

                    #endregion

                    #region Throttle

                    case Actions.ThrottleMax:
                        if (lButton)
                            mainPanel.States.ThrottleMax = Directions.Up;
                        break;

                    case Actions.ThrottleUp:
                        if (lButton)
                            mainPanel.States.Throttle = Directions.Up;
                        break;

                    case Actions.ThrottleDown:
                        if (lButton)
                            mainPanel.States.Throttle = Directions.Down;
                        break;

                    case Actions.ThrottleOff:
                        if (lButton)
                            mainPanel.States.ThrottleOff = true;
                        break;

                    #endregion

                    #region Pitch, Yaw, Roll

                    case Actions.PitchUp: // up
                        if (lButton)
                            mainPanel.States.Pitch = Directions.Up;
                        break;

                    case Actions.PitchDown: // down
                        if (lButton)
                            mainPanel.States.Pitch = Directions.Down;
                        break;

                    case Actions.YawRight: // right
                        if (lButton)
                            mainPanel.States.Yaw = Directions.Right;
                        break;

                    case Actions.YawLeft: // left
                        if (lButton)
                            mainPanel.States.Yaw = Directions.Left;
                        break;

                    case Actions.RollRight:
                        if (lButton)
                            mainPanel.States.Roll = Directions.Right;
                        break;

                    case Actions.RollLeft:
                        if (lButton)
                            mainPanel.States.Roll = Directions.Left;
                        break;

                    #endregion

                    #region Analog

                    case Actions.AnalogCenter:
                        if (lButton)
                        {
                            var localCenter = activeColorAction.Center;
                            var radius = activeColorAction.Radius;
                            var pos = mainPanel.PanelRect.position;
                            var center = pos + localCenter;
                            var centerToMouse = new Vector2(currentMousePosition.x, currentMousePosition.y) - center;

                            mainPanel.States.AnalogInput = true;
                            mainPanel.States.AnalogInputValue = new Vector2(Mathf.Abs(centerToMouse.x) / radius, Mathf.Abs(centerToMouse.y) / radius);
                            if (mainPanel.States.AnalogInputValue.magnitude > maxAnalogValueLength)
                            {
                                var newAnalogInputValue = mainPanel.States.AnalogInputValue;
                                newAnalogInputValue.Normalize();
                                newAnalogInputValue *= maxAnalogValueLength;
                                mainPanel.States.AnalogInputValue = newAnalogInputValue;
                            }

                            if (centerToMouse.x >= 0f && centerToMouse.y >= 0f)
                            {
                                mainPanel.States.Pitch = Directions.Down;
                                mainPanel.States.Yaw = Directions.Right;
                            }
                            else if (centerToMouse.x < 0f && centerToMouse.y >= 0f)
                            {
                                mainPanel.States.Pitch = Directions.Down;
                                mainPanel.States.Yaw = Directions.Left;
                            }
                            else if (centerToMouse.x >= 0f && centerToMouse.y < 0f)
                            {
                                mainPanel.States.Pitch = Directions.Up;
                                mainPanel.States.Yaw = Directions.Right;
                            }
                            else if (centerToMouse.x < 0f && centerToMouse.y < 0f)
                            {
                                mainPanel.States.Pitch = Directions.Up;
                                mainPanel.States.Yaw = Directions.Left;
                            }
                        }
                        break;

                    case Actions.AnalogUp:
                        if (lButton)
                        {
                            mainPanel.States.Pitch = Directions.Up;
                        }
                        break;

                    case Actions.AnalogDown:
                        if (lButton)
                        {
                            mainPanel.States.Pitch = Directions.Down;
                        }
                        break;

                    case Actions.AnalogLeft:
                        if (lButton)
                        {
                            mainPanel.States.Yaw = Directions.Left;
                        }
                        break;

                    case Actions.AnalogRight:
                        if (lButton)
                        {
                            mainPanel.States.Yaw = Directions.Right;
                        }
                        break;

                    case Actions.AnalogUpLeft:
                        if (lButton)
                        {
                            mainPanel.States.Pitch = Directions.Up;
                            mainPanel.States.Yaw = Directions.Left;
                        }
                        break;

                    case Actions.AnalogUpRight:
                        if (lButton)
                        {
                            mainPanel.States.Pitch = Directions.Up;
                            mainPanel.States.Yaw = Directions.Right;
                        }
                        break;

                    case Actions.AnalogDownLeft:
                        if (lButton)
                        {
                            mainPanel.States.Pitch = Directions.Down;
                            mainPanel.States.Yaw = Directions.Left;
                        }
                        break;

                    case Actions.AnalogDownRight:
                        if (lButton)
                        {
                            mainPanel.States.Pitch = Directions.Down;
                            mainPanel.States.Yaw = Directions.Right;
                        }
                        break;

                    #endregion

                    #region RCS Pitch, Yaw, Roll

                    case Actions.RCSPitchUp: // up
                        if (lButton)
                            mainPanel.States.RCSPitch = Directions.Up;
                        break;

                    case Actions.RCSPitchDown: // down
                        if (lButton)
                            mainPanel.States.RCSPitch = Directions.Down;
                        break;

                    case Actions.RCSYawRight: // right
                        if (lButton)
                            mainPanel.States.RCSYaw = Directions.Right;
                        break;

                    case Actions.RCSYawLeft: // left
                        if (lButton)
                            mainPanel.States.RCSYaw = Directions.Left;
                        break;

                    case Actions.RCSRollRight:
                        if (lButton)
                            mainPanel.States.RCSRoll = Directions.Right;
                        break;

                    case Actions.RCSRollLeft:
                        mainPanel.States.RCSRoll = Directions.Left;
                        break;

                    #endregion

                    #region RCS Analog

                    case Actions.RCSAnalogCenter:
                        break;

                    case Actions.RCSAnalogUp:
                        if (lButton)
                        {
                            mainPanel.States.RCSAnalogInput = true;
                            mainPanel.States.RCSPitch = Directions.Up;
                        }
                        break;

                    case Actions.RCSAnalogDown:
                        if (lButton)
                        {
                            mainPanel.States.RCSAnalogInput = true;
                            mainPanel.States.RCSPitch = Directions.Down;
                        }
                        break;

                    case Actions.RCSAnalogLeft:
                        if (lButton)
                        {
                            mainPanel.States.RCSAnalogInput = true;
                            mainPanel.States.RCSYaw = Directions.Left;
                        }
                        break;

                    case Actions.RCSAnalogRight:
                        if (lButton)
                        {
                            mainPanel.States.RCSAnalogInput = true;
                            mainPanel.States.RCSYaw = Directions.Right;
                        }
                        break;

                    case Actions.RCSAnalogUpLeft:
                        if (lButton)
                        {
                            mainPanel.States.RCSAnalogInput = true;
                            mainPanel.States.RCSPitch = Directions.Up;
                            mainPanel.States.RCSYaw = Directions.Left;
                        }
                        break;

                    case Actions.RCSAnalogUpRight:
                        if (lButton)
                        {
                            mainPanel.States.RCSAnalogInput = true;
                            mainPanel.States.RCSPitch = Directions.Up;
                            mainPanel.States.RCSYaw = Directions.Right;
                        }
                        break;

                    case Actions.RCSAnalogDownLeft:
                        if (lButton)
                        {
                            mainPanel.States.RCSAnalogInput = true;
                            mainPanel.States.RCSPitch = Directions.Down;
                            mainPanel.States.RCSYaw = Directions.Left;
                        }
                        break;

                    case Actions.RCSAnalogDownRight:
                        if (lButton)
                        {
                            mainPanel.States.RCSAnalogInput = true;
                            mainPanel.States.RCSPitch = Directions.Down;
                            mainPanel.States.RCSYaw = Directions.Right;
                        }
                        break;

                    #endregion

                    #region ActionGroups

                    case Actions.Actiongroup0:
                        mainPanel.States.ActionGroup0Down = lButton;
                        break;

                    case Actions.Actiongroup1:
                        mainPanel.States.ActionGroup1Down = lButton;
                        break;

                    case Actions.Actiongroup2:
                        mainPanel.States.ActionGroup2Down = lButton;
                        break;

                    case Actions.Actiongroup3:
                        mainPanel.States.ActionGroup3Down = lButton;
                        break;

                    case Actions.Actiongroup4:
                        mainPanel.States.ActionGroup4Down = lButton;
                        break;

                    case Actions.Actiongroup5:
                        mainPanel.States.ActionGroup5Down = lButton;
                        break;

                    case Actions.Actiongroup6:
                        mainPanel.States.ActionGroup6Down = lButton;
                        break;

                    case Actions.Actiongroup7:
                        mainPanel.States.ActionGroup7Down = lButton;
                        break;

                    case Actions.Actiongroup8:
                        mainPanel.States.ActionGroup8Down = lButton;
                        break;

                    case Actions.Actiongroup9:
                        mainPanel.States.ActionGroup9Down = lButton;
                        break;

                    #endregion

                    #region Func

                    case Actions.Func0:
                        mainPanel.States.Func0Down = lButton;
                        break;

                    case Actions.Func1:
                        mainPanel.States.Func1Down = lButton;
                        break;

                    case Actions.Func2:
                        mainPanel.States.Func2Down = lButton;
                        break;

                    case Actions.Func3:
                        mainPanel.States.Func3Down = lButton;
                        break;

                    #endregion
                }

                if (!lButton)
                    activeColorAction = null;
            }

            if (activeColorAction != null &&
                activeColorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
            {
                var btn = activeColorAction.ButtonGraphics[ButtonActions.Click];

                // for AnalogCenter we use click texture for normal state (no hover, not clicked)
                // the clicked state of the AnaloCenter uses the hover texture instead.
                // during dragging of the AnalogCenter the position of the hover texture will change,
                // but will be reset during hoverColorAction handling (see line ~717) if needed.
                if (activeColorAction.Action == Actions.AnalogCenter)
                    btn = CalcAnalogCenterPosition(btn);

                mainPanel.ButtonsToDraw.Add(btn);
            }

            // add analog stick
            var acColorAction = mainPanel.GetColorActionByAction(Actions.AnalogCenter);
            if (acColorAction != null && (activeColorAction == null || activeColorAction.Action != Actions.AnalogCenter))
                mainPanel.ButtonsToDraw.Add(acColorAction.ButtonGraphics[ButtonActions.Click]);

            var hoverColorAction = mainPanel.CurrentColorAction;
            if (hoverColorAction != null && hoverColorAction.ButtonGraphics.ContainsKey(ButtonActions.Hover))
            {
                if (activeColorAction == null || (hoverColorAction.Action == Actions.AnalogCenter && activeColorAction.Action != Actions.AnalogCenter))
                {
                    var btn = hoverColorAction.ButtonGraphics[ButtonActions.Hover];
                    if (hoverColorAction.Action == Actions.AnalogCenter)
                    {
                        // reset position to center (the position might have changed during last drag of AnalogCenter see CalcAnalogCenterPosition(..))
                        btn.X = Mathf.RoundToInt(hoverColorAction.Center.x - (btn.Width / 2f)) + 1;
                        btn.Y = Mathf.RoundToInt(hoverColorAction.Center.y - (btn.Height / 2f)) + 1;
                        btn.CalcPosition();
                    }
                    mainPanel.ButtonsToDraw.Add(btn);
                }
            }

            ResetToggleButtons(activeColorAction);
            UpdateToggleButtonsState();
        }

        private ButtonGraphic CalcAnalogCenterPosition(ButtonGraphic btn)
        {
            var localCenter = activeColorAction.Center;
            var radius = activeColorAction.Radius;
            var panelPos = mainPanel.PanelRect.position;
            var mousePos = new Vector2(currentMousePosition.x, currentMousePosition.y);

            var mousePosRel2PanelTopLeft = panelPos - mousePos;
            var mousePosRel2Center = mousePosRel2PanelTopLeft + localCenter;

            Vector2 newPos = Vector2.one;
            if (mousePosRel2Center.magnitude > radius)
            {
                var maxPosRel2Center = mousePosRel2Center;
                maxPosRel2Center.Normalize();
                maxPosRel2Center *= radius;
                var maxPosRel2PanelTopLeft1 = localCenter - maxPosRel2Center;
                var maxPosRel2PanelTopLeft = new Vector2(localCenter.x - maxPosRel2Center.x, localCenter.y + maxPosRel2Center.y);
                newPos = new Vector2(maxPosRel2PanelTopLeft.x - btn.Width / 2, mainPanel.PanelRect.height - maxPosRel2PanelTopLeft.y + btn.Height);
            }
            else
            {
                newPos = new Vector2(mainPanel.States.TexCoords.x - btn.Width / 2f, -(mainPanel.States.TexCoords.y - mainPanel.PanelRect.height) - btn.Height / 2f);
            }

            btn = activeColorAction.ButtonGraphics[ButtonActions.Hover];
            btn.X = Mathf.RoundToInt(newPos.x);
            btn.Y = Mathf.RoundToInt(newPos.y);
            btn.CalcPosition();
            return btn;
        }

        private void UpdateToggleButtonsState()
        {
            if (mainPanel.States.SettingsToggled && mainPanel.Action2ColorMapping.ContainsKey(Actions.Settings))
            {
                var colorAction = mainPanel.Color2ColorActionMapping[mainPanel.Action2ColorMapping[Actions.Settings]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    mainPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (mainPanel.States.RCSToggled && mainPanel.Action2ColorMapping.ContainsKey(Actions.RCS))
            {
                var colorAction = mainPanel.Color2ColorActionMapping[mainPanel.Action2ColorMapping[Actions.RCS]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    mainPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (mainPanel.States.SASToggled && mainPanel.Action2ColorMapping.ContainsKey(Actions.SAS))
            {
                var colorAction = mainPanel.Color2ColorActionMapping[mainPanel.Action2ColorMapping[Actions.SAS]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    mainPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (mainPanel.States.ActionGroup0Toggled && mainPanel.Action2ColorMapping.ContainsKey(Actions.Actiongroup0))
            {
                var colorAction = mainPanel.Color2ColorActionMapping[mainPanel.Action2ColorMapping[Actions.Actiongroup0]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    mainPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (mainPanel.States.ActionGroup1Toggled && mainPanel.Action2ColorMapping.ContainsKey(Actions.Actiongroup1))
            {
                var colorAction = mainPanel.Color2ColorActionMapping[mainPanel.Action2ColorMapping[Actions.Actiongroup1]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    mainPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (mainPanel.States.ActionGroup2Toggled && mainPanel.Action2ColorMapping.ContainsKey(Actions.Actiongroup2))
            {
                var colorAction = mainPanel.Color2ColorActionMapping[mainPanel.Action2ColorMapping[Actions.Actiongroup2]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    mainPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (mainPanel.States.ActionGroup3Toggled && mainPanel.Action2ColorMapping.ContainsKey(Actions.Actiongroup3))
            {
                var colorAction = mainPanel.Color2ColorActionMapping[mainPanel.Action2ColorMapping[Actions.Actiongroup3]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    mainPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (mainPanel.States.ActionGroup4Toggled && mainPanel.Action2ColorMapping.ContainsKey(Actions.Actiongroup4))
            {
                var colorAction = mainPanel.Color2ColorActionMapping[mainPanel.Action2ColorMapping[Actions.Actiongroup4]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    mainPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (mainPanel.States.ActionGroup5Toggled && mainPanel.Action2ColorMapping.ContainsKey(Actions.Actiongroup5))
            {
                var colorAction = mainPanel.Color2ColorActionMapping[mainPanel.Action2ColorMapping[Actions.Actiongroup5]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    mainPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (mainPanel.States.ActionGroup6Toggled && mainPanel.Action2ColorMapping.ContainsKey(Actions.Actiongroup6))
            {
                var colorAction = mainPanel.Color2ColorActionMapping[mainPanel.Action2ColorMapping[Actions.Actiongroup6]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    mainPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (mainPanel.States.ActionGroup7Toggled && mainPanel.Action2ColorMapping.ContainsKey(Actions.Actiongroup7))
            {
                var colorAction = mainPanel.Color2ColorActionMapping[mainPanel.Action2ColorMapping[Actions.Actiongroup7]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    mainPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (mainPanel.States.ActionGroup8Toggled && mainPanel.Action2ColorMapping.ContainsKey(Actions.Actiongroup8))
            {
                var colorAction = mainPanel.Color2ColorActionMapping[mainPanel.Action2ColorMapping[Actions.Actiongroup8]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    mainPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (mainPanel.States.ActionGroup9Toggled && mainPanel.Action2ColorMapping.ContainsKey(Actions.Actiongroup9))
            {
                var colorAction = mainPanel.Color2ColorActionMapping[mainPanel.Action2ColorMapping[Actions.Actiongroup9]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    mainPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }
            if (mainPanel.States.Func0Toggled && mainPanel.Action2ColorMapping.ContainsKey(Actions.Func0))
            {
                var colorAction = mainPanel.Color2ColorActionMapping[mainPanel.Action2ColorMapping[Actions.Func0]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    mainPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (mainPanel.States.Func1Toggled && mainPanel.Action2ColorMapping.ContainsKey(Actions.Func1))
            {
                var colorAction = mainPanel.Color2ColorActionMapping[mainPanel.Action2ColorMapping[Actions.Func1]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    mainPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (mainPanel.States.Func2Toggled && mainPanel.Action2ColorMapping.ContainsKey(Actions.Func2))
            {
                var colorAction = mainPanel.Color2ColorActionMapping[mainPanel.Action2ColorMapping[Actions.Func2]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    mainPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (mainPanel.States.Func3Toggled && mainPanel.Action2ColorMapping.ContainsKey(Actions.Func3))
            {
                var colorAction = mainPanel.Color2ColorActionMapping[mainPanel.Action2ColorMapping[Actions.Func3]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    mainPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }
        }
        
        private void UpdateThrottle()
        {
            // adjust throttle to input.
            if (mainPanel.States.ThrottleMax == Directions.Up)
                FlightInputHandler.state.mainThrottle = 1f;

            else if (mainPanel.States.Throttle == Directions.Up)
                FlightInputHandler.state.mainThrottle += 0.01f;

            else if (mainPanel.States.Throttle == Directions.Down)
                FlightInputHandler.state.mainThrottle -= 0.01f;

            else if (mainPanel.States.ThrottleOff)
                FlightInputHandler.state.mainThrottle = 0f;
        }

        private void UpdateYawPitchRoll(FlightCtrlState flightCtrlState)
        {
            // adjust controls to yaw/pitch/roll input
            if (mainPanel.States.Pitch == Directions.Up)
                flightCtrlState.pitch = mainPanel.States.AnalogInputValue.y; // 1f;
            else if (mainPanel.States.Pitch == Directions.Down)
                flightCtrlState.pitch = -mainPanel.States.AnalogInputValue.y; //-1f;

            if (mainPanel.States.Yaw == Directions.Right)
                flightCtrlState.yaw = mainPanel.States.AnalogInputValue.x; // 1f;
            else if (mainPanel.States.Yaw == Directions.Left)
                flightCtrlState.yaw = -mainPanel.States.AnalogInputValue.x; // -1f;

            if (mainPanel.States.Roll == Directions.Right)
                flightCtrlState.roll = 1f;
            else if (mainPanel.States.Roll == Directions.Left)
                flightCtrlState.roll = -1f;
        }

        private void UpdateRCS(FlightCtrlState flightCtrlState)
        {
            // adjust controls to RCS input
            if (mainPanel.States.RCSPitch == Directions.Up)
                flightCtrlState.Y = -1f;
            if (mainPanel.States.RCSPitch == Directions.Down)
                flightCtrlState.Y = 1f;

            if (mainPanel.States.RCSYaw == Directions.Right)
                flightCtrlState.X = -1f;
            if (mainPanel.States.RCSYaw == Directions.Left)
                flightCtrlState.X = 1f;

            if (mainPanel.States.RCSRoll == Directions.Right)
                flightCtrlState.Z = -1f;
            if (mainPanel.States.RCSRoll == Directions.Left)
                flightCtrlState.Z = 1f;
        }

        private void ResetToggleButtons(ColorAction colorAction)
        {
            if (colorAction == null || colorAction.Action != Actions.Settings)
                mainPanel.States.SettingsDown = false;
            if (colorAction == null || colorAction.Action != Actions.RCS)
                mainPanel.States.RCSDown = false;
            if (colorAction == null || colorAction.Action != Actions.SAS)
                mainPanel.States.SASDown = false;
            if (colorAction == null || colorAction.Action != Actions.Actiongroup0)
                mainPanel.States.ActionGroup0Down = false;
            if (colorAction == null || colorAction.Action != Actions.Actiongroup1)
                mainPanel.States.ActionGroup1Down = false;
            if (colorAction == null || colorAction.Action != Actions.Actiongroup2)
                mainPanel.States.ActionGroup2Down = false;
            if (colorAction == null || colorAction.Action != Actions.Actiongroup3)
                mainPanel.States.ActionGroup3Down = false;
            if (colorAction == null || colorAction.Action != Actions.Actiongroup4)
                mainPanel.States.ActionGroup4Down = false;
            if (colorAction == null || colorAction.Action != Actions.Actiongroup5)
                mainPanel.States.ActionGroup5Down = false;
            if (colorAction == null || colorAction.Action != Actions.Actiongroup6)
                mainPanel.States.ActionGroup6Down = false;
            if (colorAction == null || colorAction.Action != Actions.Actiongroup7)
                mainPanel.States.ActionGroup7Down = false;
            if (colorAction == null || colorAction.Action != Actions.Actiongroup8)
                mainPanel.States.ActionGroup8Down = false;
            if (colorAction == null || colorAction.Action != Actions.Actiongroup9)
                mainPanel.States.ActionGroup9Down = false;
            if (colorAction == null || colorAction.Action != Actions.Func0)
                mainPanel.States.Func0Down = false;
            if (colorAction == null || colorAction.Action != Actions.Func1)
                mainPanel.States.Func0Down = false;
            if (colorAction == null || colorAction.Action != Actions.Func2)
                mainPanel.States.Func0Down = false;
            if (colorAction == null || colorAction.Action != Actions.Func3)
                mainPanel.States.Func0Down = false;
            if (colorAction == null || colorAction.Action != Actions.OpenClose)
            {
                if (colorAction == null || colorAction.Name != mainPanel.Name)
                    mainPanel.States.OpenCloseDown = false;

                foreach (GuiPanel subInterface in mainPanel.Subpanels.Values)
                {
                    if (colorAction == null || colorAction.Name == subInterface.Name)
                        subInterface.States.OpenCloseDown = false;
                }
            }
        }

        #endregion
    }
}
