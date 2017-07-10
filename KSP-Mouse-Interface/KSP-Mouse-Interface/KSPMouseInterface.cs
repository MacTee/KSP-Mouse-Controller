using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using KSP.UI.Screens;

namespace KSPMouseInterface
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class KSPMouseInterface : MonoBehaviour
    {
        #region Member

        GuiPanel currentPanel = null;

        // paths
        static string rootPath = KSPUtil.ApplicationRootPath.Replace("\\", "/");
        static string pluginFolderPath = rootPath + "GameData/KSPMouseInterface";
        static string settingsPath = pluginFolderPath + "/Data/KSPMouseInterface_Settings.xml";

        // last mouse position
        Vector3 currentMousePosition = new Vector3();

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

            if (currentPanel != null)
            {
                // Load textures 
                textures.Clear();
                currentPanel.Texture = LoadTexture(pluginFolderPath + "/" + currentPanel.TexturePath);
                currentPanel.TextureHotSpots = LoadTexture(pluginFolderPath + "/" + currentPanel.HotSpotTexturePath);

                foreach (var entry in currentPanel.Subpanels)
                {
                    entry.Value.Texture = LoadTexture(pluginFolderPath + "/" + entry.Value.TexturePath);
                    entry.Value.TextureHotSpots = LoadTexture(pluginFolderPath + "/" + entry.Value.HotSpotTexturePath);
                }

                foreach (var entry in currentPanel.Color2ColorActionMapping)
                {
                    foreach (var button in entry.Value.ButtonGraphics)
                        button.Value.Texture = LoadTexture(pluginFolderPath + "/" + button.Value.TexturePath);
                }

                // Calculate interface positions
                currentPanel.CalculateMainPanelPos();
                foreach (var inter in currentPanel.Subpanels)
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

            if (currentPanel != null)
            {
                // draw interface
                foreach (var entry in currentPanel.Subpanels)
                    if (entry.Value.Texture != null && !currentPanel.Collapsed)
                        GUI.DrawTexture(entry.Value.PanelRect, entry.Value.Texture);

                if (currentPanel.Texture != null)
                    GUI.DrawTexture(currentPanel.PanelRect, currentPanel.Texture);

                foreach (ButtonGraphic button in currentPanel.ButtonsToDraw)
                {
                    Rect pos = button.Position;
                    pos.x += button.ColorAction.Interface.PanelRect.x;
                    pos.y += button.ColorAction.Interface.PanelRect.y;

                    if (button.HasTextureCoords)
                    {
                        print(pos.ToString() + button.TexturePosition.ToString());
                        GUI.DrawTextureWithTexCoords(pos, button.Texture, button.TexturePosition);
                    }
                    else
                        GUI.DrawTexture(pos, button.Texture);
                }

                // draw settings/debug window
                if (currentPanel.ShowSettingsWindow)
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
                    currentPanel = guiPanels["Default"];

                else if (guiPanels.Count > 0)
                {
                    foreach (var entry in guiPanels)
                    {
                        currentPanel = entry.Value;
                        break;
                    }
                }
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

        private void DrawWindow(int winID)
        {
            switch (winID)
            {
                case 44835: // Debug Window
                    GUILayout.BeginVertical();
                    //GUILayout.Label("Root = " + Root);
                    //GUILayout.Label("PluginFolder = " + PluginFolder);
                    //GUILayout.Label("Screen size = (" + mScreenWidth + ", " + mScreenHeight + ")");
                    //GUILayout.Label("MousePosition = (" + mMousePosition.x + ", " + mMousePosition.y + ")");
                    //GUILayout.Label("WindowPosition = (" + mWindowPosition.x + ", " + mWindowPosition.y + ", " + mWindowPosition.width + ", " + mWindowPosition.height + ")");
                    //GUILayout.Label("WindowPosition.Contains = (" + mWindowPosition.Contains(mMousePosition).ToString() + ")");
                    //GUILayout.Label("L Mouse Button = (" + Input.GetMouseButton(0).ToString() + ")");
                    //GUILayout.Label("Rel. Pic Pos = (" + ((int)mMousePosition.x - (int)mWindowPosition.x).ToString() + ", " + ((int)mMousePosition.y - (int)mWindowPosition.y).ToString() + ")");
                    GUILayout.Label("Texture Pos = " + currentPanel.States.TexCoords.ToString());
                    GUILayout.Label("Color = " + currentPanel.States.Color.ToString());
                    GUILayout.Label("InterfaceStates:");
                    GUILayout.Label("HasInput = " + currentPanel.States.HasInput.ToString());
                    GUILayout.Label("OpenClose = " + currentPanel.States.OpenCloseDown.ToString());
                    //GUILayout.Label("Func0 = " + mMCInterface.CurrentInterfaceStates.Func0Down.ToString());
                    //GUILayout.Label("Func1 = " + mMCInterface.CurrentInterfaceStates.Func1Down.ToString());
                    //GUILayout.Label("Func2 = " + mMCInterface.CurrentInterfaceStates.Func2Down.ToString());
                    //GUILayout.Label("Func3 = " + mMCInterface.CurrentInterfaceStates.Func3Down.ToString());
                    GUILayout.Label("Yaw = " + currentPanel.States.Yaw.ToString());
                    GUILayout.Label("Pitch = " + currentPanel.States.Pitch.ToString());
                    GUILayout.Label("Roll = " + currentPanel. States.Roll.ToString());
                    GUILayout.Label("RCS = " + currentPanel.States.RCSDown.ToString());
                    GUILayout.Label("SAS = " + currentPanel.States.SASDown.ToString());
                    GUILayout.Label("NextStage = " + currentPanel.States.NextStageDown.ToString());
                    GUILayout.Label("ThrottleFast = " + currentPanel.States.ThrottleMax.ToString());
                    GUILayout.Label("Throttle = " + currentPanel.States.Throttle.ToString());
                    GUILayout.Label("ThrottleOff = " + currentPanel.States.ThrottleOff.ToString());
                    foreach (var entry in currentPanel.Subpanels)
                    {
                        GUILayout.Label("Sub InterfaceStates (" + entry.Value.Name + "):");
                        GUILayout.Label("Texture Pos = " + entry.Value.States.TexCoords.ToString());
                        GUILayout.Label("Color = " + entry.Value.States.Color.ToString());
                        GUILayout.Label("Sub Arrow = " + entry.Value.States.OpenCloseDown.ToString());
                    }
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

        // called every frame
        private void UpdateControl(FlightCtrlState flightCtrlState)
        {
            if (currentPanel != null)
            {
                UpdatePanelState();

                // expand/collapse interface
                currentPanel.Collapsed = !currentPanel.States.OpenCloseToggled;

                foreach (var entry in currentPanel.Subpanels)
                    entry.Value.Collapsed = !entry.Value.States.OpenCloseToggled;

                // next stage
                if (currentPanel.States.NextStageToggled && StageManager.StageCount > 0)
                    StageManager.ActivateNextStage();

                // show/hide debug window.
                currentPanel.ShowSettingsWindow = currentPanel.States.SettingsToggled;

                var actionGroups = FlightGlobals.ActiveVessel.ActionGroups;

                // de/activate RCS
                actionGroups.SetGroup(KSPActionGroup.RCS, currentPanel.States.RCSToggled);

                // de/activate SAS
                actionGroups.SetGroup(KSPActionGroup.SAS, currentPanel.States.SASToggled);

                // set action group states.
                actionGroups.SetGroup(KSPActionGroup.Custom01, currentPanel.States.ActionGroup0Toggled);
                actionGroups.SetGroup(KSPActionGroup.Custom02, currentPanel.States.ActionGroup1Toggled);
                actionGroups.SetGroup(KSPActionGroup.Custom03, currentPanel.States.ActionGroup2Toggled);
                actionGroups.SetGroup(KSPActionGroup.Custom04, currentPanel.States.ActionGroup3Toggled);
                actionGroups.SetGroup(KSPActionGroup.Custom05, currentPanel.States.ActionGroup4Toggled);
                actionGroups.SetGroup(KSPActionGroup.Custom06, currentPanel.States.ActionGroup5Toggled);
                actionGroups.SetGroup(KSPActionGroup.Custom07, currentPanel.States.ActionGroup6Toggled);
                actionGroups.SetGroup(KSPActionGroup.Custom08, currentPanel.States.ActionGroup7Toggled);
                actionGroups.SetGroup(KSPActionGroup.Custom09, currentPanel.States.ActionGroup8Toggled);
                actionGroups.SetGroup(KSPActionGroup.Custom10, currentPanel.States.ActionGroup9Toggled);

                UpdateThrottle();

                UpdateYawPitchRoll(flightCtrlState);
                UpdateRCS(flightCtrlState);
            }
        }

        private void UpdatePanelState()
        {
            // get mouse position on hot spot texture.
            currentPanel.UpdateTexCoords(currentMousePosition);

            foreach (var entry in currentPanel.Subpanels)
                entry.Value.UpdateTexCoords(currentMousePosition);

            currentPanel.States.AnalogInput = false;
            currentPanel.States.RCSAnalogInput = false;
            currentPanel.States.ThrottleMax = Directions.None;
            currentPanel.States.Throttle = Directions.None;
            currentPanel.States.ThrottleOff = false;
            currentPanel.States.Yaw = Directions.None;
            currentPanel.States.Pitch = Directions.None;
            currentPanel.States.Roll = Directions.None;
            currentPanel.States.RCSYaw = Directions.None;
            currentPanel.States.RCSPitch = Directions.None;
            currentPanel.States.RCSRoll = Directions.None;

            currentPanel.ButtonsToDraw.Clear();

            if (currentPanel.Color2ColorActionMapping.ContainsKey(currentPanel.HotSpotColor.ToString()))
            {
                bool lButton = Input.GetMouseButton(0);
                var colorAction = currentPanel.Color2ColorActionMapping[currentPanel.HotSpotColor.ToString()];

                if (lButton && colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    currentPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);

                switch (colorAction.Action)
                {
                    #region Settings

                    case Actions.Settings:
                        currentPanel.States.SettingsDown = lButton;
                        break;

                    #endregion

                    #region RCS

                    case Actions.RCS:
                        currentPanel.States.RCSDown = lButton;
                        break;

                    #endregion

                    #region SAS

                    case Actions.SAS:
                        currentPanel.States.SASDown = lButton;
                        break;

                    #endregion

                    #region Next Stage

                    case Actions.NextStage:
                        currentPanel.States.NextStageDown = lButton;
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
                        if (colorAction.Name == currentPanel.Name)
                            currentPanel.States.OpenCloseDown = lButton;

                        else
                        {
                            foreach (GuiPanel subInterface in currentPanel.Subpanels.Values)
                            {
                                if (colorAction.Name == subInterface.Name)
                                    subInterface.States.OpenCloseDown = lButton;
                            }
                        }
                        break;

                    #endregion

                    #region Throttle

                    case Actions.ThrottleMax:
                        if (lButton)
                            currentPanel.States.ThrottleMax = Directions.Up;
                        break;

                    case Actions.ThrottleUp:
                        if (lButton)
                            currentPanel.States.Throttle = Directions.Up;
                        break;

                    case Actions.ThrottleDown:
                        if (lButton)
                            currentPanel.States.Throttle = Directions.Down;
                        break;

                    case Actions.ThrottleOff:
                        if (lButton)
                            currentPanel.States.ThrottleOff = true;
                        break;

                    #endregion

                    #region Pitch, Yaw, Roll

                    case Actions.PitchUp: // up
                        if (lButton)
                            currentPanel.States.Pitch = Directions.Up;
                        break;

                    case Actions.PitchDown: // down
                        if (lButton)
                            currentPanel.States.Pitch = Directions.Down;
                        break;

                    case Actions.YawRight: // right
                        if (lButton)
                            currentPanel.States.Yaw = Directions.Right;
                        break;

                    case Actions.YawLeft: // left
                        if (lButton)
                            currentPanel.States.Yaw = Directions.Left;
                        break;

                    case Actions.RollRight:
                        if (lButton)
                            currentPanel.States.Roll = Directions.Right;
                        break;

                    case Actions.RollLeft:
                        if (lButton)
                            currentPanel.States.Roll = Directions.Left;
                        break;

                    #endregion

                    #region Analog

                    case Actions.AnalogCenter:
                        break;

                    case Actions.AnalogUp:
                        if (lButton)
                        {
                            currentPanel.States.AnalogInput = true;
                            currentPanel.States.Pitch = Directions.Up;
                        }
                        break;

                    case Actions.AnalogDown:
                        if (lButton)
                        {
                            currentPanel.States.AnalogInput = true;
                            currentPanel.States.Pitch = Directions.Down;
                        }
                        break;

                    case Actions.AnalogLeft:
                        if (lButton)
                        {
                            currentPanel.States.AnalogInput = true;
                            currentPanel.States.Yaw = Directions.Left;
                        }
                        break;

                    case Actions.AnalogRight:
                        if (lButton)
                        {
                            currentPanel.States.AnalogInput = true;
                            currentPanel.States.Yaw = Directions.Right;
                        }
                        break;

                    case Actions.AnalogUpLeft:
                        if (lButton)
                        {
                            currentPanel.States.AnalogInput = true;
                            currentPanel.States.Pitch = Directions.Up;
                            currentPanel.States.Yaw = Directions.Left;
                        }
                        break;

                    case Actions.AnalogUpRight:
                        if (lButton)
                        {
                            currentPanel.States.AnalogInput = true;
                            currentPanel.States.Pitch = Directions.Up;
                            currentPanel.States.Yaw = Directions.Right;
                        }
                        break;

                    case Actions.AnalogDownLeft:
                        if (lButton)
                        {
                            currentPanel.States.AnalogInput = true;
                            currentPanel.States.Pitch = Directions.Down;
                            currentPanel.States.Yaw = Directions.Left;
                        }
                        break;

                    case Actions.AnalogDownRight:
                        if (lButton)
                        {
                            currentPanel.States.AnalogInput = true;
                            currentPanel.States.Pitch = Directions.Down;
                            currentPanel.States.Yaw = Directions.Right;
                        }
                        break;

                    #endregion

                    #region RCS Pitch, Yaw, Roll

                    case Actions.RCSPitchUp: // up
                        if (lButton)
                            currentPanel.States.RCSPitch = Directions.Up;
                        break;

                    case Actions.RCSPitchDown: // down
                        if (lButton)
                            currentPanel.States.RCSPitch = Directions.Down;
                        break;

                    case Actions.RCSYawRight: // right
                        if (lButton)
                            currentPanel.States.RCSYaw = Directions.Right;
                        break;

                    case Actions.RCSYawLeft: // left
                        if (lButton)
                            currentPanel.States.RCSYaw = Directions.Left;
                        break;

                    case Actions.RCSRollRight:
                        if (lButton)
                            currentPanel.States.RCSRoll = Directions.Right;
                        break;

                    case Actions.RCSRollLeft:
                        currentPanel.States.RCSRoll = Directions.Left;
                        break;

                    #endregion

                    #region RCS Analog

                    case Actions.RCSAnalogCenter:
                        break;

                    case Actions.RCSAnalogUp:
                        if (lButton)
                        {
                            currentPanel.States.RCSAnalogInput = true;
                            currentPanel.States.RCSPitch = Directions.Up;
                        }
                        break;

                    case Actions.RCSAnalogDown:
                        if (lButton)
                        {
                            currentPanel.States.RCSAnalogInput = true;
                            currentPanel.States.RCSPitch = Directions.Down;
                        }
                        break;

                    case Actions.RCSAnalogLeft:
                        if (lButton)
                        {
                            currentPanel.States.RCSAnalogInput = true;
                            currentPanel.States.RCSYaw = Directions.Left;
                        }
                        break;

                    case Actions.RCSAnalogRight:
                        if (lButton)
                        {
                            currentPanel.States.RCSAnalogInput = true;
                            currentPanel.States.RCSYaw = Directions.Right;
                        }
                        break;

                    case Actions.RCSAnalogUpLeft:
                        if (lButton)
                        {
                            currentPanel.States.RCSAnalogInput = true;
                            currentPanel.States.RCSPitch = Directions.Up;
                            currentPanel.States.RCSYaw = Directions.Left;
                        }
                        break;

                    case Actions.RCSAnalogUpRight:
                        if (lButton)
                        {
                            currentPanel.States.RCSAnalogInput = true;
                            currentPanel.States.RCSPitch = Directions.Up;
                            currentPanel.States.RCSYaw = Directions.Right;
                        }
                        break;

                    case Actions.RCSAnalogDownLeft:
                        if (lButton)
                        {
                            currentPanel.States.RCSAnalogInput = true;
                            currentPanel.States.RCSPitch = Directions.Down;
                            currentPanel.States.RCSYaw = Directions.Left;
                        }
                        break;

                    case Actions.RCSAnalogDownRight:
                        if (lButton)
                        {
                            currentPanel.States.RCSAnalogInput = true;
                            currentPanel.States.RCSPitch = Directions.Down;
                            currentPanel.States.RCSYaw = Directions.Right;
                        }
                        break;

                    #endregion

                    #region ActionGroups

                    case Actions.Actiongroup0:
                        currentPanel.States.ActionGroup0Down = lButton;
                        break;

                    case Actions.Actiongroup1:
                        currentPanel.States.ActionGroup1Down = lButton;
                        break;

                    case Actions.Actiongroup2:
                        currentPanel.States.ActionGroup2Down = lButton;
                        break;

                    case Actions.Actiongroup3:
                        currentPanel.States.ActionGroup3Down = lButton;
                        break;

                    case Actions.Actiongroup4:
                        currentPanel.States.ActionGroup4Down = lButton;
                        break;

                    case Actions.Actiongroup5:
                        currentPanel.States.ActionGroup5Down = lButton;
                        break;

                    case Actions.Actiongroup6:
                        currentPanel.States.ActionGroup6Down = lButton;
                        break;

                    case Actions.Actiongroup7:
                        currentPanel.States.ActionGroup7Down = lButton;
                        break;

                    case Actions.Actiongroup8:
                        currentPanel.States.ActionGroup8Down = lButton;
                        break;

                    case Actions.Actiongroup9:
                        currentPanel.States.ActionGroup9Down = lButton;
                        break;

                    #endregion

                    #region Func

                    case Actions.Func0:
                        currentPanel.States.Func0Down = lButton;
                        break;

                    case Actions.Func1:
                        currentPanel.States.Func1Down = lButton;
                        break;

                    case Actions.Func2:
                        currentPanel.States.Func2Down = lButton;
                        break;

                    case Actions.Func3:
                        currentPanel.States.Func3Down = lButton;
                        break;

                    #endregion
                }

                ResetToggleButtons(colorAction);
                UpdateToggleButtonsState();

                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Hover))
                    currentPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Hover]);
            }
            else
            {
                ResetToggleButtons(null);
                UpdateToggleButtonsState();
            }
        }

        private void UpdateToggleButtonsState()
        {
            if (currentPanel.States.SettingsToggled && currentPanel.Action2ColorMapping.ContainsKey(Actions.Settings))
            {
                var colorAction = currentPanel.Color2ColorActionMapping[currentPanel.Action2ColorMapping[Actions.Settings]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    currentPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (currentPanel.States.RCSToggled && currentPanel.Action2ColorMapping.ContainsKey(Actions.RCS))
            {
                var colorAction = currentPanel.Color2ColorActionMapping[currentPanel.Action2ColorMapping[Actions.RCS]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    currentPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (currentPanel.States.SASToggled && currentPanel.Action2ColorMapping.ContainsKey(Actions.SAS))
            {
                var colorAction = currentPanel.Color2ColorActionMapping[currentPanel.Action2ColorMapping[Actions.SAS]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    currentPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (currentPanel.States.ActionGroup0Toggled && currentPanel.Action2ColorMapping.ContainsKey(Actions.Actiongroup0))
            {
                var colorAction = currentPanel.Color2ColorActionMapping[currentPanel.Action2ColorMapping[Actions.Actiongroup0]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    currentPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (currentPanel.States.ActionGroup1Toggled && currentPanel.Action2ColorMapping.ContainsKey(Actions.Actiongroup1))
            {
                var colorAction = currentPanel.Color2ColorActionMapping[currentPanel.Action2ColorMapping[Actions.Actiongroup1]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    currentPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (currentPanel.States.ActionGroup2Toggled && currentPanel.Action2ColorMapping.ContainsKey(Actions.Actiongroup2))
            {
                var colorAction = currentPanel.Color2ColorActionMapping[currentPanel.Action2ColorMapping[Actions.Actiongroup2]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    currentPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (currentPanel.States.ActionGroup3Toggled && currentPanel.Action2ColorMapping.ContainsKey(Actions.Actiongroup3))
            {
                var colorAction = currentPanel.Color2ColorActionMapping[currentPanel.Action2ColorMapping[Actions.Actiongroup3]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    currentPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (currentPanel.States.ActionGroup4Toggled && currentPanel.Action2ColorMapping.ContainsKey(Actions.Actiongroup4))
            {
                var colorAction = currentPanel.Color2ColorActionMapping[currentPanel.Action2ColorMapping[Actions.Actiongroup4]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    currentPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (currentPanel.States.ActionGroup5Toggled && currentPanel.Action2ColorMapping.ContainsKey(Actions.Actiongroup5))
            {
                var colorAction = currentPanel.Color2ColorActionMapping[currentPanel.Action2ColorMapping[Actions.Actiongroup5]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    currentPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (currentPanel.States.ActionGroup6Toggled && currentPanel.Action2ColorMapping.ContainsKey(Actions.Actiongroup6))
            {
                var colorAction = currentPanel.Color2ColorActionMapping[currentPanel.Action2ColorMapping[Actions.Actiongroup6]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    currentPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (currentPanel.States.ActionGroup7Toggled && currentPanel.Action2ColorMapping.ContainsKey(Actions.Actiongroup7))
            {
                var colorAction = currentPanel.Color2ColorActionMapping[currentPanel.Action2ColorMapping[Actions.Actiongroup7]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    currentPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (currentPanel.States.ActionGroup8Toggled && currentPanel.Action2ColorMapping.ContainsKey(Actions.Actiongroup8))
            {
                var colorAction = currentPanel.Color2ColorActionMapping[currentPanel.Action2ColorMapping[Actions.Actiongroup8]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    currentPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (currentPanel.States.ActionGroup9Toggled && currentPanel.Action2ColorMapping.ContainsKey(Actions.Actiongroup9))
            {
                var colorAction = currentPanel.Color2ColorActionMapping[currentPanel.Action2ColorMapping[Actions.Actiongroup9]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    currentPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }
            if (currentPanel.States.Func0Toggled && currentPanel.Action2ColorMapping.ContainsKey(Actions.Func0))
            {
                var colorAction = currentPanel.Color2ColorActionMapping[currentPanel.Action2ColorMapping[Actions.Func0]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    currentPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (currentPanel.States.Func1Toggled && currentPanel.Action2ColorMapping.ContainsKey(Actions.Func1))
            {
                var colorAction = currentPanel.Color2ColorActionMapping[currentPanel.Action2ColorMapping[Actions.Func1]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    currentPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (currentPanel.States.Func2Toggled && currentPanel.Action2ColorMapping.ContainsKey(Actions.Func2))
            {
                var colorAction = currentPanel.Color2ColorActionMapping[currentPanel.Action2ColorMapping[Actions.Func2]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    currentPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }

            if (currentPanel.States.Func3Toggled && currentPanel.Action2ColorMapping.ContainsKey(Actions.Func3))
            {
                var colorAction = currentPanel.Color2ColorActionMapping[currentPanel.Action2ColorMapping[Actions.Func3]];
                if (colorAction.ButtonGraphics.ContainsKey(ButtonActions.Click))
                    currentPanel.ButtonsToDraw.Add(colorAction.ButtonGraphics[ButtonActions.Click]);
            }
        }
        
        private void UpdateThrottle()
        {
            // adjust throttle to input.
            if (currentPanel.States.ThrottleMax == Directions.Up)
                FlightInputHandler.state.mainThrottle = 1f;

            else if (currentPanel.States.Throttle == Directions.Up)
                FlightInputHandler.state.mainThrottle += 0.01f;

            else if (currentPanel.States.Throttle == Directions.Down)
                FlightInputHandler.state.mainThrottle -= 0.01f;

            else if (currentPanel.States.ThrottleOff)
                FlightInputHandler.state.mainThrottle = 0f;
        }

        private void UpdateYawPitchRoll(FlightCtrlState flightCtrlState)
        {
            // adjust controls to yaw/pitch/roll input
            if (currentPanel.States.Pitch == Directions.Up)
                flightCtrlState.pitch = 1f;
            else if (currentPanel.States.Pitch == Directions.Down)
                flightCtrlState.pitch = -1f;

            if (currentPanel.States.Yaw == Directions.Right)
                flightCtrlState.yaw = 1f;
            else if (currentPanel.States.Yaw == Directions.Left)
                flightCtrlState.yaw = -1f;

            if (currentPanel.States.Roll == Directions.Right)
                flightCtrlState.roll = 1f;
            else if (currentPanel.States.Roll == Directions.Left)
                flightCtrlState.roll = -1f;
        }

        private void UpdateRCS(FlightCtrlState flightCtrlState)
        {
            // adjust controls to RCS input
            if (currentPanel.States.RCSPitch == Directions.Up)
                flightCtrlState.Y = -1f;
            if (currentPanel.States.RCSPitch == Directions.Down)
                flightCtrlState.Y = 1f;

            if (currentPanel.States.RCSYaw == Directions.Right)
                flightCtrlState.X = -1f;
            if (currentPanel.States.RCSYaw == Directions.Left)
                flightCtrlState.X = 1f;

            if (currentPanel.States.RCSRoll == Directions.Right)
                flightCtrlState.Z = -1f;
            if (currentPanel.States.RCSRoll == Directions.Left)
                flightCtrlState.Z = 1f;
        }

        private void ResetToggleButtons(ColorAction colorAction)
        {
            if (colorAction == null || colorAction.Action != Actions.Settings)
                currentPanel.States.SettingsDown = false;
            if (colorAction == null || colorAction.Action != Actions.RCS)
                currentPanel.States.RCSDown = false;
            if (colorAction == null || colorAction.Action != Actions.SAS)
                currentPanel.States.SASDown = false;
            if (colorAction == null || colorAction.Action != Actions.Actiongroup0)
                currentPanel.States.ActionGroup0Down = false;
            if (colorAction == null || colorAction.Action != Actions.Actiongroup1)
                currentPanel.States.ActionGroup1Down = false;
            if (colorAction == null || colorAction.Action != Actions.Actiongroup2)
                currentPanel.States.ActionGroup2Down = false;
            if (colorAction == null || colorAction.Action != Actions.Actiongroup3)
                currentPanel.States.ActionGroup3Down = false;
            if (colorAction == null || colorAction.Action != Actions.Actiongroup4)
                currentPanel.States.ActionGroup4Down = false;
            if (colorAction == null || colorAction.Action != Actions.Actiongroup5)
                currentPanel.States.ActionGroup5Down = false;
            if (colorAction == null || colorAction.Action != Actions.Actiongroup6)
                currentPanel.States.ActionGroup6Down = false;
            if (colorAction == null || colorAction.Action != Actions.Actiongroup7)
                currentPanel.States.ActionGroup7Down = false;
            if (colorAction == null || colorAction.Action != Actions.Actiongroup8)
                currentPanel.States.ActionGroup8Down = false;
            if (colorAction == null || colorAction.Action != Actions.Actiongroup9)
                currentPanel.States.ActionGroup9Down = false;
            if (colorAction == null || colorAction.Action != Actions.Func0)
                currentPanel.States.Func0Down = false;
            if (colorAction == null || colorAction.Action != Actions.Func1)
                currentPanel.States.Func0Down = false;
            if (colorAction == null || colorAction.Action != Actions.Func2)
                currentPanel.States.Func0Down = false;
            if (colorAction == null || colorAction.Action != Actions.Func3)
                currentPanel.States.Func0Down = false;
            if (colorAction == null || colorAction.Action != Actions.OpenClose)
            {
                if (colorAction == null || colorAction.Name != currentPanel.Name)
                    currentPanel.States.OpenCloseDown = false;

                foreach (GuiPanel subInterface in currentPanel.Subpanels.Values)
                {
                    if (colorAction == null || colorAction.Name == subInterface.Name)
                        subInterface.States.OpenCloseDown = false;
                }
            }
        }

        #endregion
    }
}
