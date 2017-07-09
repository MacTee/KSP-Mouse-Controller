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

        GUIInterface mMCInterface = null;

        // paths
        static string mRoot = KSPUtil.ApplicationRootPath.Replace("\\", "/");
        static string mPluginFolder = mRoot + "GameData/KSPMouseInterface";
        static string mSettingsPath = mPluginFolder + "/Data/KSPMouseInterface_Settings.xml";

        // last mouse position
        Vector3 mMousePosition = new Vector3();

        // last window positions
        Rect mDebugWindowPosition = new Rect(50, 0, 350, 500);

        Dictionary<string, GUIInterface> mInterfaces = new Dictionary<string, GUIInterface>();

        #endregion

        #region public

        // called on scene start
        public void Start()
        {
            LoadSettings(mSettingsPath.Replace("/", "\\"));

            if (mMCInterface != null)
            {
                // Load textures 
                mTextures.Clear();
                mMCInterface.TextureMC_Interface = LoadTexture(mMCInterface.PicPath);
                mMCInterface.TextureMC_Interface_HotSpots = LoadTexture(mMCInterface.HotSpotPicPath);

                foreach (var entry in mMCInterface.Subinterfaces)
                {
                    entry.Value.TextureMC_Interface = LoadTexture(entry.Value.PicPath);
                    entry.Value.TextureMC_Interface_HotSpots = LoadTexture(entry.Value.HotSpotPicPath);
                }

                foreach (var entry in mMCInterface.Color2ColorActionMapping)
                {
                    foreach (var button in entry.Value.ButtonGraphics)
                        button.Value.Texture = LoadTexture(button.Value.PicPath);
                }

                // Calculate interface positions
                mMCInterface.CalculateMainInterfacePos();
                foreach (var inter in mMCInterface.Subinterfaces)
                    inter.Value.CalculateChildInterfacePos();

                // hook to vessel control
                FlightGlobals.ActiveVessel.OnFlyByWire += UpdateControl;
            }
        }

        // called every frame
        public void OnGUI()
        {
            // get/adjust mouse position (0, 0 = bottom left corner)
            mMousePosition = Input.mousePosition;
            mMousePosition.y = Screen.height - Input.mousePosition.y;

            if (mMCInterface != null)
            {
                // draw interface
                foreach (var entry in mMCInterface.Subinterfaces)
                    if (entry.Value.TextureMC_Interface != null && !mMCInterface.Collapsed)
                        GUI.DrawTexture(entry.Value.InterfacePos, entry.Value.TextureMC_Interface);

                if (mMCInterface.TextureMC_Interface != null)
                    GUI.DrawTexture(mMCInterface.InterfacePos, mMCInterface.TextureMC_Interface);

                foreach (ButtonGraphic button in mMCInterface.ButtonsToDraw)
                {
                    Rect pos = button.Position;
                    pos.x += button.ColorAction.Interface.InterfacePos.x;
                    pos.y += button.ColorAction.Interface.InterfacePos.y;

                    if (button.HasTextureCoords)
                    {
                        print(pos.ToString() + button.TexturePosition.ToString());
                        GUI.DrawTextureWithTexCoords(pos, button.Texture, button.TexturePosition);
                    }
                    else
                        GUI.DrawTexture(pos, button.Texture);
                }

                // draw settings/debug window
                if (mMCInterface.ShowSettingsWindow)
                {
                    GUI.skin = HighLogic.Skin;
                    mDebugWindowPosition = GUILayout.Window(44835, mDebugWindowPosition, DrawWindow, "KSPMouseInterface - Debug Window", GUILayout.MinWidth(200));
                }
            }
        }

        #endregion

        #region private

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
                    GUILayout.Label("Texture Pos = " + mMCInterface.States.TexCoords.ToString());
                    GUILayout.Label("Color = " + mMCInterface.States.Color.ToString());
                    GUILayout.Label("InterfaceStates:");
                    GUILayout.Label("HasInput = " + mMCInterface.States.HasInput.ToString());
                    GUILayout.Label("OpenClose = " + mMCInterface.States.OpenCloseDown.ToString());
                    //GUILayout.Label("Func0 = " + mMCInterface.CurrentInterfaceStates.Func0Down.ToString());
                    //GUILayout.Label("Func1 = " + mMCInterface.CurrentInterfaceStates.Func1Down.ToString());
                    //GUILayout.Label("Func2 = " + mMCInterface.CurrentInterfaceStates.Func2Down.ToString());
                    //GUILayout.Label("Func3 = " + mMCInterface.CurrentInterfaceStates.Func3Down.ToString());
                    GUILayout.Label("Yaw = " + mMCInterface.States.Yaw.ToString());
                    GUILayout.Label("Pitch = " + mMCInterface.States.Pitch.ToString());
                    GUILayout.Label("Roll = " + mMCInterface. States.Roll.ToString());
                    GUILayout.Label("RCS = " + mMCInterface.States.RCSDown.ToString());
                    GUILayout.Label("SAS = " + mMCInterface.States.SASDown.ToString());
                    GUILayout.Label("NextStage = " + mMCInterface.States.NextStageDown.ToString());
                    GUILayout.Label("ThrottleFast = " + mMCInterface.States.ThrottleMax.ToString());
                    GUILayout.Label("Throttle = " + mMCInterface.States.Throttle.ToString());
                    GUILayout.Label("ThrottleOff = " + mMCInterface.States.ThrottleOff.ToString());
                    foreach (var entry in mMCInterface.Subinterfaces)
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

        private void UpdateControl(FlightCtrlState flightCtrlState)
        {
            if (mMCInterface != null)
            {
                UpdateInterfaceState();

                // expand/collapse interface
                mMCInterface.Collapsed = !mMCInterface.States.OpenCloseToggled;

                foreach (var entry in mMCInterface.Subinterfaces)
                    entry.Value.Collapsed = !entry.Value.States.OpenCloseToggled;

                // next stage
                if (mMCInterface.States.NextStageToggled && StageManager.StageCount > 0)
                    StageManager.ActivateNextStage();

                // show/hide debug window.
                mMCInterface.ShowSettingsWindow = mMCInterface.States.SettingsToggled;

                // de/activate RCS
                FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.RCS, mMCInterface.States.RCSToggled);

                // de/activate SAS
                FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.SAS, mMCInterface.States.SASToggled);

                // set action group states.
                FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.Custom01, mMCInterface.States.ActionGroup0Toggled);
                FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.Custom02, mMCInterface.States.ActionGroup1Toggled);
                FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.Custom03, mMCInterface.States.ActionGroup2Toggled);
                FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.Custom04, mMCInterface.States.ActionGroup3Toggled);
                FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.Custom05, mMCInterface.States.ActionGroup4Toggled);
                FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.Custom06, mMCInterface.States.ActionGroup5Toggled);
                FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.Custom07, mMCInterface.States.ActionGroup6Toggled);
                FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.Custom08, mMCInterface.States.ActionGroup7Toggled);
                FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.Custom09, mMCInterface.States.ActionGroup8Toggled);
                FlightGlobals.ActiveVessel.ActionGroups.SetGroup(KSPActionGroup.Custom10, mMCInterface.States.ActionGroup9Toggled);

                UpdateThrottle();

                UpdateYawPitchRoll(flightCtrlState);
                UpdateRCS(flightCtrlState);
            }
        }

        private void UpdateInterfaceState()
        {
            // get mouse position on hot spot texture.
            mMCInterface.UpdateTexCoords(mMousePosition);

            foreach (var entry in mMCInterface.Subinterfaces)
                entry.Value.UpdateTexCoords(mMousePosition);

            mMCInterface.States.AnalogInput = false;
            mMCInterface.States.RCSAnalogInput = false;
            mMCInterface.States.ThrottleMax = MCDirections.None;
            mMCInterface.States.Throttle = MCDirections.None;
            mMCInterface.States.ThrottleOff = false;
            mMCInterface.States.Yaw = MCDirections.None;
            mMCInterface.States.Pitch = MCDirections.None;
            mMCInterface.States.Roll = MCDirections.None;
            mMCInterface.States.RCSYaw = MCDirections.None;
            mMCInterface.States.RCSPitch = MCDirections.None;
            mMCInterface.States.RCSRoll = MCDirections.None;

            mMCInterface.ButtonsToDraw.Clear();

            if (mMCInterface.Color2ColorActionMapping.ContainsKey(mMCInterface.HotSpotColor.ToString()))
            {
                bool lButton = Input.GetMouseButton(0);
                var colorAction = mMCInterface.Color2ColorActionMapping[mMCInterface.HotSpotColor.ToString()];

                if (lButton && colorAction.ButtonGraphics.ContainsKey(MCButtonAction.Click))
                    mMCInterface.ButtonsToDraw.Add(colorAction.ButtonGraphics[MCButtonAction.Click]);

                switch (colorAction.Action)
                {
                    #region Settings

                    case MCActions.Settings:
                        mMCInterface.States.SettingsDown = lButton;
                        break;

                    #endregion

                    #region RCS

                    case MCActions.RCS:
                        mMCInterface.States.RCSDown = lButton;
                        break;

                    #endregion

                    #region SAS

                    case MCActions.SAS:
                        mMCInterface.States.SASDown = lButton;
                        break;

                    #endregion

                    #region Next Stage

                    case MCActions.NextStage:
                        mMCInterface.States.NextStageDown = lButton;
                        break;

                    #endregion

                    #region Vessel change

                    case MCActions.NextVessel:
                        break;

                    case MCActions.PrevVessel:
                        break;

                    #endregion

                    #region Open/Close

                    case MCActions.OpenClose:
                        if (colorAction.Name == mMCInterface.Name)
                            mMCInterface.States.OpenCloseDown = lButton;

                        else
                        {
                            foreach (GUIInterface subInterface in mMCInterface.Subinterfaces.Values)
                            {
                                if (colorAction.Name == subInterface.Name)
                                    subInterface.States.OpenCloseDown = lButton;
                            }
                        }
                        break;

                    #endregion

                    #region Throttle

                    case MCActions.ThrottleMax:
                        if (lButton)
                            mMCInterface.States.ThrottleMax = MCDirections.Up;
                        break;

                    case MCActions.ThrottleUp:
                        if (lButton)
                            mMCInterface.States.Throttle = MCDirections.Up;
                        break;

                    case MCActions.ThrottleDown:
                        if (lButton)
                            mMCInterface.States.Throttle = MCDirections.Down;
                        break;

                    case MCActions.ThrottleOff:
                        if (lButton)
                            mMCInterface.States.ThrottleOff = true;
                        break;

                    #endregion

                    #region Pitch, Yaw, Roll

                    case MCActions.PitchUp: // up
                        if (lButton)
                            mMCInterface.States.Pitch = MCDirections.Up;
                        break;

                    case MCActions.PitchDown: // down
                        if (lButton)
                            mMCInterface.States.Pitch = MCDirections.Down;
                        break;

                    case MCActions.YawRight: // right
                        if (lButton)
                            mMCInterface.States.Yaw = MCDirections.Right;
                        break;

                    case MCActions.YawLeft: // left
                        if (lButton)
                            mMCInterface.States.Yaw = MCDirections.Left;
                        break;

                    case MCActions.RollRight:
                        if (lButton)
                            mMCInterface.States.Roll = MCDirections.Right;
                        break;

                    case MCActions.RollLeft:
                        if (lButton)
                            mMCInterface.States.Roll = MCDirections.Left;
                        break;

                    #endregion

                    #region Analog

                    case MCActions.AnalogCenter:
                        break;

                    case MCActions.AnalogUp:
                        if (lButton)
                        {
                            mMCInterface.States.AnalogInput = true;
                            mMCInterface.States.Pitch = MCDirections.Up;
                        }
                        break;

                    case MCActions.AnalogDown:
                        if (lButton)
                        {
                            mMCInterface.States.AnalogInput = true;
                            mMCInterface.States.Pitch = MCDirections.Down;
                        }
                        break;

                    case MCActions.AnalogLeft:
                        if (lButton)
                        {
                            mMCInterface.States.AnalogInput = true;
                            mMCInterface.States.Yaw = MCDirections.Left;
                        }
                        break;

                    case MCActions.AnalogRight:
                        if (lButton)
                        {
                            mMCInterface.States.AnalogInput = true;
                            mMCInterface.States.Yaw = MCDirections.Right;
                        }
                        break;

                    case MCActions.AnalogUpLeft:
                        if (lButton)
                        {
                            mMCInterface.States.AnalogInput = true;
                            mMCInterface.States.Pitch = MCDirections.Up;
                            mMCInterface.States.Yaw = MCDirections.Left;
                        }
                        break;

                    case MCActions.AnalogUpRight:
                        if (lButton)
                        {
                            mMCInterface.States.AnalogInput = true;
                            mMCInterface.States.Pitch = MCDirections.Up;
                            mMCInterface.States.Yaw = MCDirections.Right;
                        }
                        break;

                    case MCActions.AnalogDownLeft:
                        if (lButton)
                        {
                            mMCInterface.States.AnalogInput = true;
                            mMCInterface.States.Pitch = MCDirections.Down;
                            mMCInterface.States.Yaw = MCDirections.Left;
                        }
                        break;

                    case MCActions.AnalogDownRight:
                        if (lButton)
                        {
                            mMCInterface.States.AnalogInput = true;
                            mMCInterface.States.Pitch = MCDirections.Down;
                            mMCInterface.States.Yaw = MCDirections.Right;
                        }
                        break;

                    #endregion

                    #region RCS Pitch, Yaw, Roll

                    case MCActions.RCSPitchUp: // up
                        if (lButton)
                            mMCInterface.States.RCSPitch = MCDirections.Up;
                        break;

                    case MCActions.RCSPitchDown: // down
                        if (lButton)
                            mMCInterface.States.RCSPitch = MCDirections.Down;
                        break;

                    case MCActions.RCSYawRight: // right
                        if (lButton)
                            mMCInterface.States.RCSYaw = MCDirections.Right;
                        break;

                    case MCActions.RCSYawLeft: // left
                        if (lButton)
                            mMCInterface.States.RCSYaw = MCDirections.Left;
                        break;

                    case MCActions.RCSRollRight:
                        if (lButton)
                            mMCInterface.States.RCSRoll = MCDirections.Right;
                        break;

                    case MCActions.RCSRollLeft:
                        mMCInterface.States.RCSRoll = MCDirections.Left;
                        break;

                    #endregion

                    #region RCS Analog

                    case MCActions.RCSAnalogCenter:
                        break;

                    case MCActions.RCSAnalogUp:
                        if (lButton)
                        {
                            mMCInterface.States.RCSAnalogInput = true;
                            mMCInterface.States.RCSPitch = MCDirections.Up;
                        }
                        break;

                    case MCActions.RCSAnalogDown:
                        if (lButton)
                        {
                            mMCInterface.States.RCSAnalogInput = true;
                            mMCInterface.States.RCSPitch = MCDirections.Down;
                        }
                        break;

                    case MCActions.RCSAnalogLeft:
                        if (lButton)
                        {
                            mMCInterface.States.RCSAnalogInput = true;
                            mMCInterface.States.RCSYaw = MCDirections.Left;
                        }
                        break;

                    case MCActions.RCSAnalogRight:
                        if (lButton)
                        {
                            mMCInterface.States.RCSAnalogInput = true;
                            mMCInterface.States.RCSYaw = MCDirections.Right;
                        }
                        break;

                    case MCActions.RCSAnalogUpLeft:
                        if (lButton)
                        {
                            mMCInterface.States.RCSAnalogInput = true;
                            mMCInterface.States.RCSPitch = MCDirections.Up;
                            mMCInterface.States.RCSYaw = MCDirections.Left;
                        }
                        break;

                    case MCActions.RCSAnalogUpRight:
                        if (lButton)
                        {
                            mMCInterface.States.RCSAnalogInput = true;
                            mMCInterface.States.RCSPitch = MCDirections.Up;
                            mMCInterface.States.RCSYaw = MCDirections.Right;
                        }
                        break;

                    case MCActions.RCSAnalogDownLeft:
                        if (lButton)
                        {
                            mMCInterface.States.RCSAnalogInput = true;
                            mMCInterface.States.RCSPitch = MCDirections.Down;
                            mMCInterface.States.RCSYaw = MCDirections.Left;
                        }
                        break;

                    case MCActions.RCSAnalogDownRight:
                        if (lButton)
                        {
                            mMCInterface.States.RCSAnalogInput = true;
                            mMCInterface.States.RCSPitch = MCDirections.Down;
                            mMCInterface.States.RCSYaw = MCDirections.Right;
                        }
                        break;

                    #endregion

                    #region ActionGroups

                    case MCActions.Actiongroup0:
                        mMCInterface.States.ActionGroup0Down = lButton;
                        break;

                    case MCActions.Actiongroup1:
                        mMCInterface.States.ActionGroup1Down = lButton;
                        break;

                    case MCActions.Actiongroup2:
                        mMCInterface.States.ActionGroup2Down = lButton;
                        break;

                    case MCActions.Actiongroup3:
                        mMCInterface.States.ActionGroup3Down = lButton;
                        break;

                    case MCActions.Actiongroup4:
                        mMCInterface.States.ActionGroup4Down = lButton;
                        break;

                    case MCActions.Actiongroup5:
                        mMCInterface.States.ActionGroup5Down = lButton;
                        break;

                    case MCActions.Actiongroup6:
                        mMCInterface.States.ActionGroup6Down = lButton;
                        break;

                    case MCActions.Actiongroup7:
                        mMCInterface.States.ActionGroup7Down = lButton;
                        break;

                    case MCActions.Actiongroup8:
                        mMCInterface.States.ActionGroup8Down = lButton;
                        break;

                    case MCActions.Actiongroup9:
                        mMCInterface.States.ActionGroup9Down = lButton;
                        break;

                    #endregion

                    #region Func

                    case MCActions.Func0:
                        mMCInterface.States.Func0Down = lButton;
                        break;

                    case MCActions.Func1:
                        mMCInterface.States.Func1Down = lButton;
                        break;

                    case MCActions.Func2:
                        mMCInterface.States.Func2Down = lButton;
                        break;

                    case MCActions.Func3:
                        mMCInterface.States.Func3Down = lButton;
                        break;

                    #endregion
                }

                ResetToggleButtons(colorAction);
                UpdateToggleButtonsState();

                if (colorAction.ButtonGraphics.ContainsKey(MCButtonAction.Hover))
                    mMCInterface.ButtonsToDraw.Add(colorAction.ButtonGraphics[MCButtonAction.Hover]);
            }
            else
            {
                ResetToggleButtons(null);
                UpdateToggleButtonsState();
            }
        }

        private void UpdateToggleButtonsState()
        {
            if (mMCInterface.States.SettingsToggled && mMCInterface.Action2ColorMapping.ContainsKey(MCActions.Settings))
            {
                var colorAction = mMCInterface.Color2ColorActionMapping[mMCInterface.Action2ColorMapping[MCActions.Settings]];
                if (colorAction.ButtonGraphics.ContainsKey(MCButtonAction.Click))
                    mMCInterface.ButtonsToDraw.Add(colorAction.ButtonGraphics[MCButtonAction.Click]);
            }

            if (mMCInterface.States.RCSToggled && mMCInterface.Action2ColorMapping.ContainsKey(MCActions.RCS))
            {
                var colorAction = mMCInterface.Color2ColorActionMapping[mMCInterface.Action2ColorMapping[MCActions.RCS]];
                if (colorAction.ButtonGraphics.ContainsKey(MCButtonAction.Click))
                    mMCInterface.ButtonsToDraw.Add(colorAction.ButtonGraphics[MCButtonAction.Click]);
            }

            if (mMCInterface.States.SASToggled && mMCInterface.Action2ColorMapping.ContainsKey(MCActions.SAS))
            {
                var colorAction = mMCInterface.Color2ColorActionMapping[mMCInterface.Action2ColorMapping[MCActions.SAS]];
                if (colorAction.ButtonGraphics.ContainsKey(MCButtonAction.Click))
                    mMCInterface.ButtonsToDraw.Add(colorAction.ButtonGraphics[MCButtonAction.Click]);
            }

            if (mMCInterface.States.ActionGroup0Toggled && mMCInterface.Action2ColorMapping.ContainsKey(MCActions.Actiongroup0))
            {
                var colorAction = mMCInterface.Color2ColorActionMapping[mMCInterface.Action2ColorMapping[MCActions.Actiongroup0]];
                if (colorAction.ButtonGraphics.ContainsKey(MCButtonAction.Click))
                    mMCInterface.ButtonsToDraw.Add(colorAction.ButtonGraphics[MCButtonAction.Click]);
            }

            if (mMCInterface.States.ActionGroup1Toggled && mMCInterface.Action2ColorMapping.ContainsKey(MCActions.Actiongroup1))
            {
                var colorAction = mMCInterface.Color2ColorActionMapping[mMCInterface.Action2ColorMapping[MCActions.Actiongroup1]];
                if (colorAction.ButtonGraphics.ContainsKey(MCButtonAction.Click))
                    mMCInterface.ButtonsToDraw.Add(colorAction.ButtonGraphics[MCButtonAction.Click]);
            }

            if (mMCInterface.States.ActionGroup2Toggled && mMCInterface.Action2ColorMapping.ContainsKey(MCActions.Actiongroup2))
            {
                var colorAction = mMCInterface.Color2ColorActionMapping[mMCInterface.Action2ColorMapping[MCActions.Actiongroup2]];
                if (colorAction.ButtonGraphics.ContainsKey(MCButtonAction.Click))
                    mMCInterface.ButtonsToDraw.Add(colorAction.ButtonGraphics[MCButtonAction.Click]);
            }

            if (mMCInterface.States.ActionGroup3Toggled && mMCInterface.Action2ColorMapping.ContainsKey(MCActions.Actiongroup3))
            {
                var colorAction = mMCInterface.Color2ColorActionMapping[mMCInterface.Action2ColorMapping[MCActions.Actiongroup3]];
                if (colorAction.ButtonGraphics.ContainsKey(MCButtonAction.Click))
                    mMCInterface.ButtonsToDraw.Add(colorAction.ButtonGraphics[MCButtonAction.Click]);
            }

            if (mMCInterface.States.ActionGroup4Toggled && mMCInterface.Action2ColorMapping.ContainsKey(MCActions.Actiongroup4))
            {
                var colorAction = mMCInterface.Color2ColorActionMapping[mMCInterface.Action2ColorMapping[MCActions.Actiongroup4]];
                if (colorAction.ButtonGraphics.ContainsKey(MCButtonAction.Click))
                    mMCInterface.ButtonsToDraw.Add(colorAction.ButtonGraphics[MCButtonAction.Click]);
            }

            if (mMCInterface.States.ActionGroup5Toggled && mMCInterface.Action2ColorMapping.ContainsKey(MCActions.Actiongroup5))
            {
                var colorAction = mMCInterface.Color2ColorActionMapping[mMCInterface.Action2ColorMapping[MCActions.Actiongroup5]];
                if (colorAction.ButtonGraphics.ContainsKey(MCButtonAction.Click))
                    mMCInterface.ButtonsToDraw.Add(colorAction.ButtonGraphics[MCButtonAction.Click]);
            }

            if (mMCInterface.States.ActionGroup6Toggled && mMCInterface.Action2ColorMapping.ContainsKey(MCActions.Actiongroup6))
            {
                var colorAction = mMCInterface.Color2ColorActionMapping[mMCInterface.Action2ColorMapping[MCActions.Actiongroup6]];
                if (colorAction.ButtonGraphics.ContainsKey(MCButtonAction.Click))
                    mMCInterface.ButtonsToDraw.Add(colorAction.ButtonGraphics[MCButtonAction.Click]);
            }

            if (mMCInterface.States.ActionGroup7Toggled && mMCInterface.Action2ColorMapping.ContainsKey(MCActions.Actiongroup7))
            {
                var colorAction = mMCInterface.Color2ColorActionMapping[mMCInterface.Action2ColorMapping[MCActions.Actiongroup7]];
                if (colorAction.ButtonGraphics.ContainsKey(MCButtonAction.Click))
                    mMCInterface.ButtonsToDraw.Add(colorAction.ButtonGraphics[MCButtonAction.Click]);
            }

            if (mMCInterface.States.ActionGroup8Toggled && mMCInterface.Action2ColorMapping.ContainsKey(MCActions.Actiongroup8))
            {
                var colorAction = mMCInterface.Color2ColorActionMapping[mMCInterface.Action2ColorMapping[MCActions.Actiongroup8]];
                if (colorAction.ButtonGraphics.ContainsKey(MCButtonAction.Click))
                    mMCInterface.ButtonsToDraw.Add(colorAction.ButtonGraphics[MCButtonAction.Click]);
            }

            if (mMCInterface.States.ActionGroup9Toggled && mMCInterface.Action2ColorMapping.ContainsKey(MCActions.Actiongroup9))
            {
                var colorAction = mMCInterface.Color2ColorActionMapping[mMCInterface.Action2ColorMapping[MCActions.Actiongroup9]];
                if (colorAction.ButtonGraphics.ContainsKey(MCButtonAction.Click))
                    mMCInterface.ButtonsToDraw.Add(colorAction.ButtonGraphics[MCButtonAction.Click]);
            }
            if (mMCInterface.States.Func0Toggled && mMCInterface.Action2ColorMapping.ContainsKey(MCActions.Func0))
            {
                var colorAction = mMCInterface.Color2ColorActionMapping[mMCInterface.Action2ColorMapping[MCActions.Func0]];
                if (colorAction.ButtonGraphics.ContainsKey(MCButtonAction.Click))
                    mMCInterface.ButtonsToDraw.Add(colorAction.ButtonGraphics[MCButtonAction.Click]);
            }

            if (mMCInterface.States.Func1Toggled && mMCInterface.Action2ColorMapping.ContainsKey(MCActions.Func1))
            {
                var colorAction = mMCInterface.Color2ColorActionMapping[mMCInterface.Action2ColorMapping[MCActions.Func1]];
                if (colorAction.ButtonGraphics.ContainsKey(MCButtonAction.Click))
                    mMCInterface.ButtonsToDraw.Add(colorAction.ButtonGraphics[MCButtonAction.Click]);
            }

            if (mMCInterface.States.Func2Toggled && mMCInterface.Action2ColorMapping.ContainsKey(MCActions.Func2))
            {
                var colorAction = mMCInterface.Color2ColorActionMapping[mMCInterface.Action2ColorMapping[MCActions.Func2]];
                if (colorAction.ButtonGraphics.ContainsKey(MCButtonAction.Click))
                    mMCInterface.ButtonsToDraw.Add(colorAction.ButtonGraphics[MCButtonAction.Click]);
            }

            if (mMCInterface.States.Func3Toggled && mMCInterface.Action2ColorMapping.ContainsKey(MCActions.Func3))
            {
                var colorAction = mMCInterface.Color2ColorActionMapping[mMCInterface.Action2ColorMapping[MCActions.Func3]];
                if (colorAction.ButtonGraphics.ContainsKey(MCButtonAction.Click))
                    mMCInterface.ButtonsToDraw.Add(colorAction.ButtonGraphics[MCButtonAction.Click]);
            }
        }

        private void ResetToggleButtons(ColorAction colorAction)
        {
            if (colorAction == null || colorAction.Action != MCActions.Settings)
                mMCInterface.States.SettingsDown = false;
            if (colorAction == null || colorAction.Action != MCActions.RCS)
                mMCInterface.States.RCSDown = false;
            if (colorAction == null || colorAction.Action != MCActions.SAS)
                mMCInterface.States.SASDown = false;
            if (colorAction == null || colorAction.Action != MCActions.Actiongroup0)
                mMCInterface.States.ActionGroup0Down = false;
            if (colorAction == null || colorAction.Action != MCActions.Actiongroup1)
                mMCInterface.States.ActionGroup1Down = false;
            if (colorAction == null || colorAction.Action != MCActions.Actiongroup2)
                mMCInterface.States.ActionGroup2Down = false;
            if (colorAction == null || colorAction.Action != MCActions.Actiongroup3)
                mMCInterface.States.ActionGroup3Down = false;
            if (colorAction == null || colorAction.Action != MCActions.Actiongroup4)
                mMCInterface.States.ActionGroup4Down = false;
            if (colorAction == null || colorAction.Action != MCActions.Actiongroup5)
                mMCInterface.States.ActionGroup5Down = false;
            if (colorAction == null || colorAction.Action != MCActions.Actiongroup6)
                mMCInterface.States.ActionGroup6Down = false;
            if (colorAction == null || colorAction.Action != MCActions.Actiongroup7)
                mMCInterface.States.ActionGroup7Down = false;
            if (colorAction == null || colorAction.Action != MCActions.Actiongroup8)
                mMCInterface.States.ActionGroup8Down = false;
            if (colorAction == null || colorAction.Action != MCActions.Actiongroup9)
                mMCInterface.States.ActionGroup9Down = false;
            if (colorAction == null || colorAction.Action != MCActions.Func0)
                mMCInterface.States.Func0Down = false;
            if (colorAction == null || colorAction.Action != MCActions.Func1)
                mMCInterface.States.Func0Down = false;
            if (colorAction == null || colorAction.Action != MCActions.Func2)
                mMCInterface.States.Func0Down = false;
            if (colorAction == null || colorAction.Action != MCActions.Func3)
                mMCInterface.States.Func0Down = false;
            if (colorAction == null || colorAction.Action != MCActions.OpenClose)
            {
                if (colorAction == null || colorAction.Name != mMCInterface.Name)
                    mMCInterface.States.OpenCloseDown = false;

                foreach (GUIInterface subInterface in mMCInterface.Subinterfaces.Values)
                {
                    if (colorAction == null || colorAction.Name == subInterface.Name)
                        subInterface.States.OpenCloseDown = false;
                }
            }
        }
        
        private void UpdateThrottle()
        {
            // adjust throttle to input.
            if (mMCInterface.States.ThrottleMax == MCDirections.Up)
                FlightInputHandler.state.mainThrottle = 1f;

            else if (mMCInterface.States.Throttle == MCDirections.Up)
                FlightInputHandler.state.mainThrottle += 0.01f;

            else if (mMCInterface.States.Throttle == MCDirections.Down)
                FlightInputHandler.state.mainThrottle -= 0.01f;

            else if (mMCInterface.States.ThrottleOff)
                FlightInputHandler.state.mainThrottle = 0f;
        }

        private void UpdateYawPitchRoll(FlightCtrlState flightCtrlState)
        {
            // adjust controls to yaw/pitch/roll input
            if (mMCInterface.States.Pitch == MCDirections.Up)
                flightCtrlState.pitch = 1f;
            else if (mMCInterface.States.Pitch == MCDirections.Down)
                flightCtrlState.pitch = -1f;

            if (mMCInterface.States.Yaw == MCDirections.Right)
                flightCtrlState.yaw = 1f;
            else if (mMCInterface.States.Yaw == MCDirections.Left)
                flightCtrlState.yaw = -1f;

            if (mMCInterface.States.Roll == MCDirections.Right)
                flightCtrlState.roll = 1f;
            else if (mMCInterface.States.Roll == MCDirections.Left)
                flightCtrlState.roll = -1f;
        }

        private void UpdateRCS(FlightCtrlState flightCtrlState)
        {
            // adjust controls to RCS input
            if (mMCInterface.States.RCSPitch == MCDirections.Up)
                flightCtrlState.Y = -1f;
            if (mMCInterface.States.RCSPitch == MCDirections.Down)
                flightCtrlState.Y = 1f;

            if (mMCInterface.States.RCSYaw == MCDirections.Right)
                flightCtrlState.X = -1f;
            if (mMCInterface.States.RCSYaw == MCDirections.Left)
                flightCtrlState.X = 1f;

            if (mMCInterface.States.RCSRoll == MCDirections.Right)
                flightCtrlState.Z = -1f;
            if (mMCInterface.States.RCSRoll == MCDirections.Left)
                flightCtrlState.Z = 1f;
        }
        

        private Dictionary<string, Texture2D> mTextures = new Dictionary<string, Texture2D>();
        private Texture2D LoadTexture(string path)
        {
            if (mTextures.ContainsKey(path))
                return mTextures[path];
            
            WWW www = new WWW("file://" + path);
            mTextures.Add(path, www.texture);

            return www.texture;
        }


        #region LoadSettings

        public void LoadSettings(string path)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(path.Replace("/", "\\"));
                var i = doc.GetElementsByTagName(Consts.cInterface);
                foreach (XmlNode node in i)
                {
                    GUIInterface mcInterface = null;

                    if (node.Name == Consts.cInterface)
                    {
                        // read interface
                        try
                        {
                            mcInterface = GetInterface(node);
                            if (mcInterface == null)
                                continue;

                            // read sub interfaces
                            foreach (XmlNode child in node.ChildNodes)
                            {
                                if (child.Name == Consts.cSubInterface)
                                {
                                    GUIInterface newSubinterface = GetInterface(child);
                                    if (!newSubinterface.IsEmpty)
                                    {
                                        newSubinterface.Parent = mcInterface;
                                        mcInterface.Subinterfaces.Add(newSubinterface.Name, newSubinterface);
                                    }
                                }
                                else if (child.Name == Consts.cColorActionMapping)
                                {
                                    // read color action mapping
                                    foreach (XmlNode colorActionNode in child.ChildNodes)
                                    {
                                        if (colorActionNode.Name == Consts.cColorAction)
                                        {
                                            ColorAction colorAction = GetColorAction(colorActionNode);
                                            try
                                            {
                                                if (!colorAction.IsEmpty)
                                                {
                                                    colorAction.Interface = mcInterface;
                                                    mcInterface.Color2ColorActionMapping.Add(colorAction.Color.ToString(), colorAction);
                                                    if (!mcInterface.Action2ColorMapping.ContainsKey(colorAction.Action))
                                                        mcInterface.Action2ColorMapping.Add(colorAction.Action, colorAction.Color.ToString());
                                                }
                                            }
                                            catch
                                            {
                                                print(string.Format("Can't add ColorAction! Color ({0}) already exists.", colorAction.Color));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            print("ErrorMessage: " + ex.Message);
                        }
                    }

                    mInterfaces.Add(mcInterface.Name, mcInterface);
                }

                if (mInterfaces.ContainsKey("Default"))
                    mMCInterface = mInterfaces["Default"];

                else if (mInterfaces.Count > 0)
                {
                    foreach (var entry in mInterfaces)
                    {
                        mMCInterface = entry.Value;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                print("ErrorMessage: " + ex.Message);
            }
        }

        private GUIInterface GetInterface(XmlNode node)
        {
            GUIInterface newInterface = new GUIInterface();

            string posX = string.Empty;
            string posY = string.Empty;
            //MCSize size = new MCSize();

            foreach (XmlAttribute att in node.Attributes)
            {
                if (att.Name == Consts.cName)
                    newInterface.Name = att.Value.ToString();

                else if (att.Name == Consts.cAlignment)
                    newInterface.Alignment = GetAlignment(att.Value.ToString().Trim());

                else if (att.Name == Consts.cMargin)
                    newInterface.Margin = GetMargin(att.Value.ToString().Trim());

                else if (att.Name == Consts.cX)
                    posX = att.Value;

                else if (att.Name == Consts.cY)
                    posY = att.Value;

                else if (att.Name == Consts.cOpenCloseButtonHeight)
                    newInterface.OpenCloseBtnHeight = ParseInt(att.Value);

                //else if (att.Name == Consts.cWidth)
                //    size.Width = ParseInt(att.Value);

                //else if (att.Name == Consts.cHeight)
                //    size.Height = ParseInt(att.Value);

                else if (att.Name == Consts.cPicPath)
                    newInterface.PicPath = mPluginFolder + "/" + att.Value.ToString();

                else if (att.Name == Consts.cHotSpotPicPath)
                    newInterface.HotSpotPicPath = mPluginFolder + "/" + att.Value.ToString();
            }

            if (posX.Contains("%"))
                newInterface.RelativeX = true;
            if (posY.Contains("%"))
                newInterface.RelativeY = true;

            Vector2 pos = new Vector2();
            if (!string.IsNullOrEmpty(posX))
                pos.x = ParseInt(posX);
            if (!string.IsNullOrEmpty(posY))
                pos.y = ParseInt(posY);

            newInterface.Position = pos;
            //newInterface.Size = size;

            return newInterface;
        }

        private ColorAction GetColorAction(XmlNode colorActionNode)
        {
            ColorAction colorAction = new ColorAction();
            foreach (XmlAttribute att in colorActionNode.Attributes)
            {
                if (att.Name == Consts.cName)
                    colorAction.Name = att.Value.ToString();

                else if (att.Name == Consts.cColor)
                    colorAction.Color = GetColor(att.Value.ToString().Trim());

                else if (att.Name == Consts.cAction)
                    colorAction.Action = GetAction(att.Value.ToString().Trim());
            }

            foreach (XmlNode buttonNode in colorActionNode.ChildNodes)
            {
                if (buttonNode.Name == Consts.cButtonGraphic)
                {
                    var button = GetButtonGraphic(buttonNode);
                    if (!button.IsEmpty)
                    {
                        button.ColorAction = colorAction;
                        colorAction.ButtonGraphics.Add(button.Action, button);
                    }
                }
            }

            return colorAction;
        }

        private ButtonGraphic GetButtonGraphic(XmlNode buttonNode)
        {
            ButtonGraphic button = new ButtonGraphic();
            foreach (XmlAttribute att in buttonNode.Attributes)
            {
                if (att.Name == Consts.cAction)
                    button.Action = GetButtonAction(att.Value.ToString().Trim());

                else if (att.Name == Consts.cPicPath)
                    button.PicPath = mPluginFolder + "/" + att.Value.ToString();

                else if (att.Name == Consts.cX)
                    button.X = ParseInt(att.Value.ToString().Trim());

                else if (att.Name == Consts.cY)
                    button.Y = ParseInt(att.Value.ToString().Trim());

                //else if (att.Name == Consts.cWidth)
                //    button.Width = ParseInt(att.Value.ToString().Trim());

                //else if (att.Name == Consts.cHeight)
                //    button.Height = ParseInt(att.Value.ToString().Trim());

                else if (att.Name == Consts.cTextureX)
                    button.TextureX = ParseInt(att.Value.ToString().Trim());

                else if (att.Name == Consts.cTextureY)
                    button.TextureY = ParseInt(att.Value.ToString().Trim());

                else if (att.Name == Consts.cTextureWidth)
                    button.TextureWidth = ParseInt(att.Value.ToString().Trim());

                else if (att.Name == Consts.cTextureHeight)
                    button.TextureHeight = ParseInt(att.Value.ToString().Trim());
            }

            return button;
        }

        private Margin GetMargin(string marginAsString)
        {
            int l = 0;
            int r = 0;
            int t = 0;
            int b = 0;

            string[] temp = marginAsString.Split(',');

            if (temp == null || temp.Length != 4)
            {
                //print("KSPMouseInterface Error: Can't parse Margin");
            }
            else
            {
                try
                {
                    l = int.Parse(temp[0].Trim());
                    r = int.Parse(temp[1].Trim());
                    t = int.Parse(temp[2].Trim());
                    b = int.Parse(temp[3].Trim());
                }
                catch
                {
                    //print("KSPMouseInterface Error: Can't parse Margin");
                }
            }

            return new Margin(l, r, t, b);
        }

        private MCAlignment GetAlignment(string alignmentAsString)
        {
            MCAlignment alignment = MCAlignment.Bottom;

            switch (alignmentAsString)
            {
                case Consts.cLeft:
                    alignment = MCAlignment.Left;
                    break;
                case Consts.cRight:
                    alignment = MCAlignment.Right;
                    break;
                case Consts.cTop:
                    alignment = MCAlignment.Top;
                    break;
                case Consts.cBottom:
                    alignment = MCAlignment.Bottom;
                    break;
            }

            return alignment;
        }

        private Color GetColor(string colorAsString)
        {
            int r = 0;
            int g = 0;
            int b = 0;

            string[] temp = colorAsString.Split(',');

            if (temp == null || temp.Length != 3)
            {
                //print("KSPMouseInterface Error: Can't parse Color");
            }
            else
            {
                try
                {
                    r = int.Parse(temp[0].Trim());
                    g = int.Parse(temp[1].Trim());
                    b = int.Parse(temp[2].Trim());
                }
                catch
                {
                    //print("KSPMouseInterface Error: Can't parse Color");
                }
            }

            return new Color(r, g, b, 255);
        }

        private MCActions GetAction(string actionAsString)
        {
            MCActions action = MCActions.None;

            switch (actionAsString)
            {
                case Consts.cSettings:
                    action = MCActions.Settings;
                    break;
                case Consts.cOpenClose:
                    action = MCActions.OpenClose;
                    break;
                case Consts.cNextVessel:
                    action = MCActions.NextVessel;
                    break;
                case Consts.cPrevVessel:
                    action = MCActions.PrevVessel;
                    break;
                case Consts.cThrottleMax:
                    action = MCActions.ThrottleMax;
                    break;
                case Consts.cThrottleUp:
                    action = MCActions.ThrottleUp;
                    break;
                case Consts.cThrottleDown:
                    action = MCActions.ThrottleDown;
                    break;
                case Consts.cThrottleOff:
                    action = MCActions.ThrottleOff;
                    break;
                case Consts.cRCS:
                    action = MCActions.RCS;
                    break;
                case Consts.cSAS:
                    action = MCActions.SAS;
                    break;
                case Consts.cPitchUp:
                    action = MCActions.PitchUp;
                    break;
                case Consts.cPitchDown:
                    action = MCActions.PitchDown;
                    break;
                case Consts.cYawRight:
                    action = MCActions.YawRight;
                    break;
                case Consts.cYawLeft:
                    action = MCActions.YawLeft;
                    break;
                case Consts.cRollRight:
                    action = MCActions.RollRight;
                    break;
                case Consts.cRollLeft:
                    action = MCActions.RollLeft;
                    break;
                case Consts.cNextStage:
                    action = MCActions.NextStage;
                    break;
                case Consts.cActiongroup0:
                    action = MCActions.Actiongroup0;
                    break;
                case Consts.cActiongroup1:
                    action = MCActions.Actiongroup1;
                    break;
                case Consts.cActiongroup2:
                    action = MCActions.Actiongroup2;
                    break;
                case Consts.cActiongroup3:
                    action = MCActions.Actiongroup3;
                    break;
                case Consts.cActiongroup4:
                    action = MCActions.Actiongroup4;
                    break;
                case Consts.cActiongroup5:
                    action = MCActions.Actiongroup5;
                    break;
                case Consts.cActiongroup6:
                    action = MCActions.Actiongroup6;
                    break;
                case Consts.cActiongroup7:
                    action = MCActions.Actiongroup7;
                    break;
                case Consts.cActiongroup8:
                    action = MCActions.Actiongroup8;
                    break;
                case Consts.cActiongroup9:
                    action = MCActions.Actiongroup9;
                    break;
                case Consts.cAnalogCenter:
                    action = MCActions.AnalogCenter;
                    break;
                case Consts.cAnalogUp:
                    action = MCActions.AnalogUp;
                    break;
                case Consts.cAnalogDown:
                    action = MCActions.AnalogDown;
                    break;
                case Consts.cAnalogLeft:
                    action = MCActions.AnalogLeft;
                    break;
                case Consts.cAnalogRight:
                    action = MCActions.AnalogRight;
                    break;
                case Consts.cAnalogUpLeft:
                    action = MCActions.AnalogUpLeft;
                    break;
                case Consts.cAnalogUpRight:
                    action = MCActions.AnalogUpRight;
                    break;
                case Consts.cAnalogDownLeft:
                    action = MCActions.AnalogDownLeft;
                    break;
                case Consts.cAnalogDownRight:
                    action = MCActions.AnalogDownRight;
                    break;
                case Consts.cRCSPitchUp:
                    action = MCActions.RCSPitchUp;
                    break;
                case Consts.cRCSPitchDown:
                    action = MCActions.RCSPitchDown;
                    break;
                case Consts.cRCSYawRight:
                    action = MCActions.RCSYawRight;
                    break;
                case Consts.cRCSYawLeft:
                    action = MCActions.RCSYawLeft;
                    break;
                case Consts.cRCSRollRight:
                    action = MCActions.RCSRollRight;
                    break;
                case Consts.cRCSRollLeft:
                    action = MCActions.RCSRollLeft;
                    break;
                case Consts.cRCSAnalogCenter:
                    action = MCActions.RCSAnalogCenter;
                    break;
                case Consts.cRCSAnalogUp:
                    action = MCActions.RCSAnalogUp;
                    break;
                case Consts.cRCSAnalogDown:
                    action = MCActions.RCSAnalogDown;
                    break;
                case Consts.cRCSAnalogLeft:
                    action = MCActions.RCSAnalogLeft;
                    break;
                case Consts.cRCSAnalogRight:
                    action = MCActions.RCSAnalogRight;
                    break;
                case Consts.cRCSAnalogUpLeft:
                    action = MCActions.RCSAnalogUpLeft;
                    break;
                case Consts.cRCSAnalogUpRight:
                    action = MCActions.RCSAnalogUpRight;
                    break;
                case Consts.cRCSAnalogDownLeft:
                    action = MCActions.RCSAnalogDownLeft;
                    break;
                case Consts.cRCSAnalogDownRight:
                    action = MCActions.RCSAnalogDownRight;
                    break;
                default:
                    action = MCActions.None;
                    break;
            }

            return action;
        }

        private MCButtonAction GetButtonAction(string buttonActionAsString)
        {
            MCButtonAction action = MCButtonAction.None;

            switch (buttonActionAsString)
            {
                case Consts.cClick:
                    action = MCButtonAction.Click;
                    break;
                case Consts.cHover:
                    action = MCButtonAction.Hover;
                    break;
            }

            return action;
        }

        private int ParseInt(string integer)
        {
            int pos = 0;
            try
            {
                if (integer.Contains("%"))
                    pos = int.Parse(integer.Replace("%", "").Trim());
                else
                    pos = int.Parse(integer);
            }
            catch
            {
                //print("KSPMouseInterface error: Error during int parsing!");
            }

            return pos;
        }

        #endregion

        #endregion
    }
}
