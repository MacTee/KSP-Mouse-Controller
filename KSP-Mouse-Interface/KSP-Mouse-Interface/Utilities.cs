using System.Collections.Generic;
using UnityEngine;

namespace KSPMouseInterface
{
    #region Enumeration

    public enum MCAlignment
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public enum MCDirections
    {
        None,
        Up,
        Down,
        Left,
        Right

    }

    public enum MCActions
    {
        None,
        Settings,
        OpenClose,
        NextVessel,
        PrevVessel,
        ThrottleMax,
        ThrottleUp,
        ThrottleDown,
        ThrottleOff,
        RCS,
        SAS,
        PitchUp,
        PitchDown,
        YawRight,
        YawLeft,
        RollRight,
        RollLeft,
        NextStage,
        Actiongroup0,
        Actiongroup1,
        Actiongroup2,
        Actiongroup3,
        Actiongroup4,
        Actiongroup5,
        Actiongroup6,
        Actiongroup7,
        Actiongroup8,
        Actiongroup9,
        AnalogCenter,
        AnalogUp,
        AnalogDown,
        AnalogLeft,
        AnalogRight,
        AnalogUpLeft,
        AnalogUpRight,
        AnalogDownLeft,
        AnalogDownRight,
        Func0,
        Func1,
        Func2,
        Func3,
        RCSPitchUp,
        RCSPitchDown,
        RCSYawRight,
        RCSYawLeft,
        RCSRollRight,
        RCSRollLeft,
        RCSAnalogCenter,
        RCSAnalogUp,
        RCSAnalogDown,
        RCSAnalogLeft,
        RCSAnalogRight,
        RCSAnalogUpLeft,
        RCSAnalogUpRight,
        RCSAnalogDownLeft,
        RCSAnalogDownRight,
    }

    public enum MCButtonAction
    {
        None,
        Click,
        Hover
    }

    #endregion

    public class Consts
    {
        #region Constants

        // XML Node names
        public const string cKSPMouseInterface = "KSPMouseInterface";
        //public const string cSettings = "Settings"; // see MC Actions
        public const string cSelectedInterface = "SelectedInterface";
        public const string cInterfaces = "Interfaces";
        public const string cInterface = "Interface";
        public const string cSubInterface = "SubInterface";
        public const string cColorActionMapping = "ColorActionMapping";
        public const string cColorAction = "ColorAction";
        public const string cButtonGraphic = "ButtonGraphic";

        // XML Attribute names
        public const string cName = "Name";
        public const string cAlignment = "Alignment";
        public const string cMargin = "Margin";
        public const string cX = "X";
        public const string cY = "Y";
        public const string cWidth = "Width";
        public const string cHeight = "Height";
        public const string cTextureX = "TextureX";
        public const string cTextureY = "TextureY";
        public const string cTextureWidth = "TextureWidth";
        public const string cTextureHeight = "TextureHeight";
        public const string cPicPath = "PicPath";
        public const string cHotSpotPicPath = "HotSpotPicPath";
        public const string cColor = "Color";
        public const string cAction = "Action";
        public const string cOpenCloseButtonHeight = "OpenCloseButtonHeight";

        // MC Alignments
        public const string cLeft = "Left";
        public const string cRight = "Right";
        public const string cTop = "Top";
        public const string cBottom = "Bottom";

        // MC Button Actions
        public const string cClick = "Click";
        public const string cHover = "Hover";

        // MC Actions
        public const string cSettings = "Settings";
        public const string cOpenClose = "OpenClose";
        public const string cNextVessel = "NextVessel";
        public const string cPrevVessel = "PrevVessel";
        public const string cThrottleMax = "ThrottleMax";
        public const string cThrottleUp = "ThrottleUp";
        public const string cThrottleDown = "ThrottleDown";
        public const string cThrottleOff = "ThrottleOff";
        public const string cRCS = "RCS";
        public const string cSAS = "SAS";
        public const string cPitchUp = "PitchUp";
        public const string cPitchDown = "PitchDown";
        public const string cYawRight = "YawRight";
        public const string cYawLeft = "YawLeft";
        public const string cRollRight = "RollRight";
        public const string cRollLeft = "RollLeft";
        public const string cNextStage = "NextStage";
        public const string cActiongroup0 = "Actiongroup0";
        public const string cActiongroup1 = "Actiongroup1";
        public const string cActiongroup2 = "Actiongroup2";
        public const string cActiongroup3 = "Actiongroup3";
        public const string cActiongroup4 = "Actiongroup4";
        public const string cActiongroup5 = "Actiongroup5";
        public const string cActiongroup6 = "Actiongroup6";
        public const string cActiongroup7 = "Actiongroup7";
        public const string cActiongroup8 = "Actiongroup8";
        public const string cActiongroup9 = "Actiongroup9";
        public const string cAnalogCenter = "AnalogCenter";
        public const string cAnalogUp = "AnalogUp";
        public const string cAnalogDown = "AnalogDown";
        public const string cAnalogLeft = "AnalogLeft";
        public const string cAnalogRight = "AnalogRight";
        public const string cAnalogUpLeft = "AnalogUpLeft";
        public const string cAnalogUpRight = "AnalogUpRight";
        public const string cAnalogDownLeft = "AnalogDownLeft";
        public const string cAnalogDownRight = "AnalogDownRight";
        public const string cRCSPitchUp = "RCSPitchUp";
        public const string cRCSPitchDown = "RCSPitchDown";
        public const string cRCSYawRight = "RCSYawRight";
        public const string cRCSYawLeft = "RCSYawLeft";
        public const string cRCSRollRight = "RCSRollRight";
        public const string cRCSRollLeft = "RCSRollLeft";
        public const string cRCSAnalogCenter = "RCSAnalogCenter";
        public const string cRCSAnalogUp = "RCSAnalogUp";
        public const string cRCSAnalogDown = "RCSAnalogDown";
        public const string cRCSAnalogLeft = "RCSAnalogLeft";
        public const string cRCSAnalogRight = "RCSAnalogRight";
        public const string cRCSAnalogUpLeft = "RCSAnalogUpLeft";
        public const string cRCSAnalogUpRight = "RCSAnalogUpRight";
        public const string cRCSAnalogDownLeft = "RCSAnalogDownLeft";
        public const string cRCSAnalogDownRight = "RCSAnalogDownRight";

        #endregion
    }

    // Maps a color to an action.
    public class ColorAction
    {
        // The parent Interface
        public GUIInterface Interface { get; set; }

        // Name of the interface the action is for.
        public string Name { get; set; }

        // The action that should be executed when this button is clicked.
        public MCActions Action { get; set; }

        // The color of the Button on the hot spot graphic.
        public Color Color { get; set; }

        // List of button states
        public Dictionary<MCButtonAction, ButtonGraphic> ButtonGraphics { get; set; }

        // Flag to determine whether this instance is empty or not.
        public bool IsEmpty
        {
            get
            {
                return (Interface == null && Name == string.Empty && Action == MCActions.None && Color.r == 0 && Color.g == 0 && Color.b == 0 && Color.a == 0 && ButtonGraphics.Count == 0);
            }
        }


        public ColorAction()
        {
            Interface = null;
            Name = string.Empty;
            Action = MCActions.None;
            Color = new Color();

            ButtonGraphics = new Dictionary<MCButtonAction, ButtonGraphic>();
        }


        public override string ToString()
        {
            int i = 0;
            string buttons = string.Empty;
            foreach (var entry in ButtonGraphics)
                buttons += ", ButtonGraphics (" + i++ +") = " + entry.Value.ToString();
            if (ButtonGraphics.Count == 0)
                buttons = ", ButtonGraphics = 0";
            return string.Format("(Name = {0}, Action = {1}, Color = {2}{3})", Name, Action, Color, buttons);
        }
    }

    public class Size
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public bool IsEmpty
        {
            get
            {
                return (Width == 0 && Height == 0);
            }
        }


        public Size(int width = 0, int height = 0)
        {
            Width = width;
            Height = height;
        }


        public override string ToString()
        {
            return string.Format("(Width = {0}, Height = {1})", Width, Height);
        }
    }

    public class Margin
    {
        public int Left { get; set; }
        public int Right { get; set; }
        public int Top { get; set; }
        public int Bottom { get; set; }
        public bool IsEmpty
        {
            get
            {
                return (Left == 0 && Right == 0 && Top == 0 && Bottom == 0);
            }
        }


        public Margin(int left = 0, int right = 0, int top = 0, int bottom = 0)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }


        public override string ToString()
        {
            return string.Format("(Left = {0}, Right = {1}, Top = {2}, Right = {3})", Left, Right, Top, Right);
        }
    }

    // Wraps a Button graphic for a certain MCButtonAction.
    public class ButtonGraphic
    {
        #region Member

        private Rect mPosition = new Rect();
        private Rect mTexturePosition = new Rect();
        private Texture2D mTexture = null;

        #endregion

        // The parent ColorAction.
        public ColorAction ColorAction { get; set; }

        // Action of the Button (Click, hover)
        public MCButtonAction Action { get; set; }

        // path to the texture.
        public string PicPath { get; set; }

        // Top of the OnScreen position rect (in pixel).
        public int X { get; set; }
        // Left of the OnScreen position rect (in pixel).
        public int Y { get; set; }
        // Width of the OnScreen position rect (in pixel).
        public int Width 
        {
            get
            {
                if (Texture == null)
                    return 0; 
                else
                    return Texture.width; 
            }
        }
        // Height of the OnScreen position rect (in pixel).
        public int Height
        {
            get 
            {
                if (Texture == null)
                    return 0; 
                else
                    return Texture.height;
            }
        }

        // OnScreen render position rect (in pixel).
        public Rect Position
        {
            get
            {
                if (mPosition.x == 0 && mPosition.x == 0 && mPosition.y == 0 && mPosition.width == 0 && mPosition.height == 0)
                    CalcPosition();

                return mPosition;
            }
        }

        // Top of the rect that envelop the texture pert to render (in pixel).
        public int TextureX { get; set; }
        // Left of the rect that envelop the texture pert to render (in pixel).
        public int TextureY { get; set; }
        // Width of the rect that envelop the texture pert to render (in pixel).
        public int TextureWidth { get; set; }
        // Height of the rect that envelop the texture pert to render (in pixel).
        public int TextureHeight { get; set; }

        // The rect that envelop the texture part to render (in normalized format).
        public Rect TexturePosition
        {
            get
            {
                if (HasTextureCoords && mTexturePosition.x == 0 && mTexturePosition.x == 0 && mTexturePosition.y == 0 && mTexturePosition.width == 0 && mTexturePosition.height == 0)
                    CalcPosition();

                return mTexturePosition;
            }
        }

        // Flag to determine whether there are valid texture coords or not.
        public bool HasTextureCoords
        {
            get
            {
                return (TextureX != 0 || TextureY != 0 || TextureWidth != 0 || TextureHeight != 0);
            }
        }

        // The texture to render from.
        public Texture2D Texture
        {
            get
            {
                return mTexture;
            }
            set
            {
                mTexture = value;

                if (mTexture != null)
                    CalcPosition();
            }
        }

        // Flag to determine whether this instance is empty or not.
        public bool IsEmpty
        {
            get
            {
                return (ColorAction == null &&
                        Action == MCButtonAction.None &&
                        PicPath == string.Empty &&
                        X == 0 &&
                        Y == 0 &&
                        TextureX == 0 &&
                        TextureY == 0 &&
                        TextureWidth == 0 &&
                        TextureHeight == 0 &&
                        Texture == null);
            }
        }


        public ButtonGraphic()
        {
            ColorAction = null;
            Action = MCButtonAction.None;
            PicPath = string.Empty;
            X = 0;
            Y = 0;
            TextureX = 0;
            TextureY = 0;
            TextureWidth = 0;
            TextureHeight = 0;
            Texture  = null;
        }


        public override string ToString()
        {
            return string.Format("(Action = {0}, PicPath = {1}, Position = {2}, TexturePosition = {3}, Texture = {4})", 
                                 Action, PicPath, Position, TexturePosition, (Texture == null) ? "null" : "OK");
        }


        // Calculates the on screen rect and the texture area rect.
        private void CalcPosition()
        {
            if (HasTextureCoords)
            {
                mPosition = new Rect(X, Y, TextureWidth, TextureHeight);
                if (Texture.width > 0 && Texture.height > 0)
                    mTexturePosition = new Rect((float)TextureX / (float)Texture.width, (float)TextureY / (float)Texture.height,
                                                (float)TextureWidth / (float)Texture.width, (float)TextureHeight / (float)Texture.height);
            }
            else
                mPosition = new Rect(X, Y, Width, Height);
        }
    }
}
