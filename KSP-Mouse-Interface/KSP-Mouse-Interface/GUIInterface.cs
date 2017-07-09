using System.Collections.Generic;
using UnityEngine;

namespace KSPMouseInterface
{
    public class GUIInterface
    {
        private bool mCollapsed = false;

        public Rect InterfacePos { get; set; }
        public Rect InterfacePosCollapsed { get; set; }
        public Rect InterfacePosExpended { get; set; }

        public GUIInterfaceState States { get; set; }

        private Texture2D mTextureMC_Interface = null;
        public Texture2D TextureMC_Interface
        {
            get
            {
                return mTextureMC_Interface;
            }
            set
            {
                mTextureMC_Interface = value;
                if (mTextureMC_Interface != null)
                    Size = new Size(mTextureMC_Interface.width, mTextureMC_Interface.height);
            }
        }
        public Texture2D TextureMC_Interface_HotSpots { get; set; }

        public bool MainInterface { get { return Parent == null;} }
        public string Name { get; set; }
        public MCAlignment Alignment { get; set; }
        public Margin Margin { get; set; }
        public Vector2 Position { get; set; } // the position as parsed from the settings file.
        public bool RelativeX { get; set; }
        public bool RelativeY { get; set; }
        public Size Size { get; set; }
        public int OpenCloseBtnHeight { get; set; }
        public string PicPath { get; set; }
        public string HotSpotPicPath { get; set; }

        public Color HotSpotColor
        {
            get
            {
                if (States.Color.r == 0 && States.Color.g == 0 && States.Color.b == 0 && States.Color.a == 0)
                {
                    foreach (var entry in Subinterfaces)
                    {
                        if (entry.Value.States.Color.r != 0 || entry.Value.States.Color.g != 0 || entry.Value.States.Color.b != 0 || entry.Value.States.Color.a != 0)
                            return entry.Value.States.Color;
                    }
                }
                
                return States.Color;
            }
        }

        public GUIInterface Parent { get; set; }
        public Dictionary<string, GUIInterface> Subinterfaces { get; set; }

        public Dictionary<string, ColorAction> Color2ColorActionMapping { get; set; }
        public Dictionary<MCActions, string> Action2ColorMapping { get; set; }

        public bool Collapsed {
            get
            {
                return mCollapsed;
            }
            set
            {
                mCollapsed = value;

                if (!mCollapsed)
                    foreach (var entry in Subinterfaces)
                        entry.Value.Collapsed = false;

                if (mCollapsed)
                    InterfacePos = InterfacePosCollapsed;
                else
                    InterfacePos = InterfacePosExpended;
            }
        }
        public bool ShowSettingsWindow { get; set; }

        public bool IsEmpty
        {
            get
            {
                return (Name == string.Empty &&
                        Alignment == MCAlignment.Bottom &&
                        Margin.IsEmpty &&
                        Position.x == 0 && Position.y == 0 &&
                        !RelativeX && !RelativeY &&
                        Size.IsEmpty &&
                        OpenCloseBtnHeight == 0 &&
                        PicPath == string.Empty &&
                        HotSpotPicPath == string.Empty &&
                        Parent == null &&
                        Subinterfaces.Count == 0 &&
                        Color2ColorActionMapping.Count == 0 &&
                        Action2ColorMapping.Count == 0 &&
                        Collapsed == true &&
                        ShowSettingsWindow == false);
            }
        }

        public bool HasValidTexCoords
        {
            get
            {
                return (States.TexCoords.x >= 0 && States.TexCoords.y >= 0 && States.TexCoords.x <= Size.Width && States.TexCoords.y <= Size.Height); 
            }
        }

        public List<string> PropertyAsStrings
        {
            get
            {
                List<string> propertyStrings = new List<string>();
                propertyStrings.Add("=== Interface (" + this.Name + ") ===");
                propertyStrings.Add("Name = " + this.Name.ToString());
                propertyStrings.Add("MainInterface = " + this.MainInterface.ToString());
                propertyStrings.Add("Alignment = " + this.Alignment.ToString());
                propertyStrings.Add("Collapsed = " + this.Collapsed.ToString());
                propertyStrings.Add("PicPath = " + this.PicPath.ToString());
                propertyStrings.Add("HotSpotPicPath = " + this.HotSpotPicPath.ToString());
                propertyStrings.Add("Margin = " + this.Margin.ToString());
                propertyStrings.Add("OpenCloseBtnHeight = " + this.OpenCloseBtnHeight.ToString());
                propertyStrings.Add("HasParent = " + ((this.Parent != null) ? true.ToString() : false.ToString()));
                propertyStrings.Add("Position = " + this.Position.ToString());
                propertyStrings.Add("RelativeX = " + this.RelativeX.ToString());
                propertyStrings.Add("RelativeY = " + this.RelativeY.ToString());
                propertyStrings.Add("ShowSettingsWindow = " + this.ShowSettingsWindow.ToString());
                propertyStrings.Add("Size = " + this.Size.ToString());

                propertyStrings.Add("InterfacePos = " + this.InterfacePos.ToString());
                propertyStrings.Add("InterfacePosCollapsed = " + this.InterfacePosCollapsed.ToString());
                propertyStrings.Add("InterfacePosExpended = " + this.InterfacePosExpended.ToString());

                propertyStrings.Add((TextureMC_Interface == null) ? "TextureMC_Interface = null" : "TextureMC_Interface = OK");
                propertyStrings.Add((TextureMC_Interface_HotSpots == null) ? "TextureMC_Interface_HotSpots = null" : "TextureMC_Interface_HotSpots = OK");

                int i = 0;
                propertyStrings.Add("== ColorActionMapping (" + Color2ColorActionMapping.Count + ") ==");
                foreach (var entry in Color2ColorActionMapping)
                    propertyStrings.Add(string.Format("ColorAction {0} = {1}", i++, entry.ToString())); //string.Format("ColorAction {0} = Name: {1}, Action: {2}, Color: {3}", i++, entry.Name, entry.Action, entry.Color));

                //propertyStrings.Add("States: " + this.States.ToString());

                foreach (var sub in Subinterfaces)
                {
                    propertyStrings.Add("=== Subinterface (" + sub.Value.Name + ") ===");
                    propertyStrings.AddRange(sub.Value.PropertyAsStrings);
                }

                return propertyStrings;
            }
        }

        public List<ButtonGraphic> ButtonsToDraw { get; set; }


        public GUIInterface()
        {
            InterfacePos = new Rect();
            InterfacePosCollapsed = new Rect();
            InterfacePosExpended = new Rect();

            States = new GUIInterfaceState();

            TextureMC_Interface = null;
            TextureMC_Interface_HotSpots = null;

            Name = string.Empty;
            Alignment = MCAlignment.Bottom;
            Margin = new Margin();
            Position = new Vector2();
            RelativeX = false;
            RelativeY = false;
            Size = new Size();
            OpenCloseBtnHeight = 0;
            PicPath = string.Empty;
            HotSpotPicPath = string.Empty;
            Parent = null;
            Subinterfaces = new Dictionary<string, GUIInterface>();
            Color2ColorActionMapping = new Dictionary<string, ColorAction>();
            Action2ColorMapping = new Dictionary<MCActions, string>();
            Collapsed = true;
            ShowSettingsWindow = false;
            ButtonsToDraw = new List<ButtonGraphic>();
        }

        
        public void CalculateMainInterfacePos()
        {
            Rect posCol = new Rect(0, 0, Size.Width, Size.Height);
            Rect posExp = new Rect(0, 0, Size.Width, Size.Height);
            //if (Alignment == MCAlignment.Top)
            //{ }
            //else if (Alignment == MCAlignment.Bottom)
            //{
                #region Calc bottom alignment

                if (RelativeX)
                    posCol.x = Screen.width * (Position.x / 100);
                else
                    posCol.x = Position.x;

                posCol.x += Margin.Left;
                posCol.x -= Margin.Right;

                posExp.x = posCol.x;

                // Collapsed
                if (RelativeY)
                    posCol.y = Screen.height * (Position.y / 100) - OpenCloseBtnHeight;
                else
                    posCol.y = Position.y - OpenCloseBtnHeight;

                posCol.y += Margin.Top;
                posCol.y -= Margin.Bottom;

                // Expanded
                if (RelativeY)
                    posExp.y = Screen.height * (Position.y / 100) - Size.Height;
                else
                    posExp.y = Position.y - Size.Height;

                posExp.y += Margin.Top;
                posExp.y -= Margin.Bottom;

                InterfacePosCollapsed = posCol;
                InterfacePosExpended = posExp;

                #endregion
            //}
            //else if (Alignment == MCAlignment.Left)
            //{ }
            //else if (Alignment == MCAlignment.Right)
            //{ }

            Collapsed = true;
        }

        public void CalculateChildInterfacePos()
        {
            InterfacePosCollapsed = new Rect(Parent.InterfacePosExpended.x + Position.x, Parent.InterfacePosExpended.y + Position.y, Size.Width, Size.Height);
            InterfacePosExpended = new Rect(Parent.InterfacePosExpended.x + Position.x, Parent.InterfacePosExpended.y + Position.y, Size.Width, Size.Height);

            if (Alignment == MCAlignment.Top)
            {
                InterfacePosCollapsed = new Rect(InterfacePosExpended.x, InterfacePosExpended.y, InterfacePosCollapsed.width, InterfacePosCollapsed.height);
                InterfacePosExpended = new Rect(InterfacePosExpended.x, InterfacePosExpended.y - Size.Height, InterfacePosExpended.width, InterfacePosExpended.height);
            }
            else if (Alignment == MCAlignment.Bottom)
            {
                InterfacePosCollapsed = new Rect(InterfacePosExpended.x, InterfacePosExpended.y, InterfacePosCollapsed.width, InterfacePosCollapsed.height);
                InterfacePosExpended = new Rect(InterfacePosExpended.x, InterfacePosExpended.y + Size.Height, InterfacePosExpended.width, InterfacePosExpended.height);
            }
            else if (Alignment == MCAlignment.Left)
            {
                InterfacePosCollapsed = new Rect(InterfacePosExpended.x, InterfacePosExpended.y, InterfacePosCollapsed.width, InterfacePosCollapsed.height);
                InterfacePosExpended = new Rect(InterfacePosExpended.x - Size.Width, InterfacePosExpended.y, InterfacePosExpended.width, InterfacePosExpended.height);
            }
            else if (Alignment == MCAlignment.Right)
            {
                InterfacePosCollapsed = new Rect(InterfacePosExpended.x, InterfacePosExpended.y, InterfacePosCollapsed.width, InterfacePosCollapsed.height);
                InterfacePosExpended = new Rect(InterfacePosExpended.x + Size.Width, InterfacePosExpended.y, InterfacePosExpended.width, InterfacePosExpended.height);
            }

            Collapsed = true;
        }
        
        public void UpdateTexCoords(Vector2 mousePosition)
        {
            int x = (int)mousePosition.x - (int)InterfacePos.x;
            int y = Size.Height - ((int)mousePosition.y - (int)InterfacePos.y);
            States.TexCoords = new Vector2(x, y);

            Color color = new Color(0, 0, 0, 0);
            if (HasValidTexCoords && (MainInterface || (!MainInterface && !Collapsed)))
            {
                color = TextureMC_Interface_HotSpots.GetPixel((int)States.TexCoords.x,
                                                              (int)States.TexCoords.y);
                color = new Color(color.r * 255, color.g * 255, color.b * 255, color.a * 255);
            }
            States.Color = color;
        }
    }
}
